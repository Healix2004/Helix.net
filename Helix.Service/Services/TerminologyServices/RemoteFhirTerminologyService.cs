using Helix.Service.DTOs.Terminology;
using Helix.Service.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Infrastructure.ExternalServices
{
    public class RemoteFhirTerminologyService : ITerminologyService
    {
        private readonly FhirClient _client;

        public RemoteFhirTerminologyService(IConfiguration config)
        {
            var serverUrl = config["Fhir:ServerUrl"];
            var username = config["Fhir:Username"];
            var password = config["Fhir:Password"];

            var settings = new FhirClientSettings
            {
                Timeout = 30000,
                PreferredFormat = ResourceFormat.Json
            };

            // Ensure the server URL ends with a slash to allow clean URI combining
            if (!serverUrl.EndsWith("/")) serverUrl += "/";

            _client = new FhirClient(serverUrl, settings);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                _client.RequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
            }
        }

        public async Task<List<CodingDto>> LookupCodesAsync(string filterText, string systemUri)
        {
            var parameters = new Parameters();
            parameters.Add("filter", new FhirString(filterText));

            if (!string.IsNullOrEmpty(systemUri))
            {
                // SNOMED Fix: Append ?fhir_vs if missing
                if (systemUri.Contains("snomed.info/sct") && !systemUri.Contains("fhir_vs"))
                {
                    systemUri += "?fhir_vs";
                }
                parameters.Add("url", new FhirUri(systemUri));
            }

            try
            {
                // FIX: Combine Base Endpoint with Relative Path to create an ABSOLUTE URI.
                // Resolves "Must be an absolute url" error.
                var absoluteUri = new Uri(_client.Endpoint, "ValueSet/$expand");

                var resource = await _client.OperationAsync(
                    absoluteUri,
                    parameters,
                    useGet: true
                );

                if (resource is ValueSet vs && vs.Expansion?.Contains != null)
                {
                    return vs.Expansion.Contains.Select(c => new CodingDto
                    {
                        System = c.System,
                        Code = c.Code,
                        Display = c.Display
                    }).ToList();
                }
            }
            catch (FhirOperationException)
            {
                return new List<CodingDto>();
            }

            return new List<CodingDto>();
        }

        public async Task<bool> ValidateCodeAsync(string systemUri, string code)
        {
            var parameters = new Parameters();
            parameters.Add("url", new FhirUri(systemUri));
            parameters.Add("code", new FhirString(code));

            try
            {
                // FIX: Create Absolute URI for CodeSystem/$validate-code
                var absoluteUri = new Uri(_client.Endpoint, "CodeSystem/$validate-code");

                var resource = await _client.OperationAsync(
                    absoluteUri,
                    parameters,
                    useGet: true
                );

                if (resource is Parameters resultParams)
                {
                    var resultParam = resultParams.Parameter.FirstOrDefault(p => p.Name == "result");
                    if (resultParam?.Value is FhirBoolean boolValue)
                    {
                        return boolValue.Value ?? false;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetDisplayForCodeAsync(string systemUri, string code)
        {
            var parameters = new Parameters();
            parameters.Add("system", new FhirUri(systemUri));
            parameters.Add("code", new FhirString(code));

            try
            {
                // FIX: Create Absolute URI for CodeSystem/$lookup
                var absoluteUri = new Uri(_client.Endpoint, "CodeSystem/$lookup");

                var resource = await _client.OperationAsync(
                    absoluteUri,
                    parameters,
                    useGet: true
                );

                if (resource is Parameters resultParams)
                {
                    var displayParam = resultParams.Parameter.FirstOrDefault(p => p.Name == "display");
                    if (displayParam?.Value is FhirString stringValue)
                    {
                        return stringValue.Value;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}