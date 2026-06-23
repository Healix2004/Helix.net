using Helix.Service.DTOs.Terminology;
using Helix.Service.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RemoteFhirTerminologyService> _logger;

        public RemoteFhirTerminologyService(IConfiguration config, ILogger<RemoteFhirTerminologyService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var serverUrl = config["Fhir:ServerUrl"];

            // IMPROVEMENT 1: Configuration Safety. 
            // Fails immediately with a clear message if the URL is missing, rather than throwing a NullReferenceException later.
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new InvalidOperationException("FHIR Server URL is missing in the configuration (Fhir:ServerUrl).");
            }

            var username = config["Fhir:Username"];
            var password = config["Fhir:Password"];

            var settings = new FhirClientSettings
            {
                // IMPROVEMENT 2: Fail Fast. 
                // Reduced from 30s to 10s. A UI search dropdown should never make a user wait 30 seconds.
                Timeout = 10000,
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

            // IMPROVEMENT 3: Added a result limit.
            // Just like the issue you faced with Snowstorm, querying a FHIR server without a limit 
            // will cause it to crash or timeout on broad searches.
            parameters.Add("count", new Integer(20));

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
            catch (FhirOperationException ex)
            {
                // IMPROVEMENT 4: Structured Logging.
                // Silently swallowing exceptions returns an empty list and hides the problem. Now you can check your logs.
                _logger.LogWarning(ex, "FHIR Operation rejected by server during LookupCodesAsync for '{FilterText}'", filterText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network or unexpected error during LookupCodesAsync for '{FilterText}'", filterText);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating code '{Code}' against system '{SystemUri}'", code, systemUri);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving display text for code '{Code}' in system '{SystemUri}'", code, systemUri);
                return null;
            }
        }
    }
}