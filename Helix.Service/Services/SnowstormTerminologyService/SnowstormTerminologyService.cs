using Helix.Service.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Helix.Service.Services.SnowstormTerminology
{
    public class SnowstormTerminologyService : ISnowstormTerminologyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SnowstormTerminologyService> _logger;
        private readonly FhirJsonDeserializer _fhirJsonDeserializer;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        // IMPROVEMENT 1: Switched to a much more reliable, enterprise-grade public terminology server 
        // to prevent the timeout issues you were previously experiencing.
        private const string DefaultFhirBaseUrl = "https://r4.ontoserver.csiro.au/fhir/";
        private const string SnomedSystemUri = "http://snomed.info/sct";
        private const string CacheKeyPrefix = "SNOMED_";

        public SnowstormTerminologyService(
            HttpClient httpClient,
            IMemoryCache cache,
            ILogger<SnowstormTerminologyService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _fhirJsonDeserializer = new FhirJsonDeserializer();

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(DefaultFhirBaseUrl);
            }

            // IMPROVEMENT 2: Structured logging in the retry policy and safer exception handling
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests || (int)r.StatusCode >= 500)
                .Or<HttpRequestException>()
                .Or<SocketException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    // Using structured logging (best practice for Serilog/AppInsights) instead of string interpolation
                    _logger.LogWarning("Terminology API request failed. Retrying in {Delay}s (Attempt {RetryAttempt}/3)...", delay.TotalSeconds, retryAttempt);
                    return delay;
                });
        }

        // IMPROVEMENT 3: Added CancellationToken support across all methods to prevent hanging threads
        public async Task<CodeSystem.ConceptDefinitionComponent?> LookupSnomedCodeAsync(string snomedCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(snomedCode)) return null;

            string cacheKey = $"{CacheKeyPrefix}Lookup_{snomedCode}";
            if (_cache.TryGetValue(cacheKey, out CodeSystem.ConceptDefinitionComponent? cachedResult))
            {
                return cachedResult;
            }

            var requestUrl = $"CodeSystem/$lookup?system={Uri.EscapeDataString(SnomedSystemUri)}&code={Uri.EscapeDataString(snomedCode)}";

            try
            {
                var response = await _retryPolicy.ExecuteAsync(ct => _httpClient.GetAsync(requestUrl, ct), cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var parameters = _fhirJsonDeserializer.Deserialize<Parameters>(json);

                var conceptProperty = parameters.Parameter.FirstOrDefault(p => p.Name == "concept");
                if (conceptProperty?.Part != null)
                {
                    var codeElement = conceptProperty.Part.FirstOrDefault(p => p.Name == "code")?.Value as FhirString;
                    var displayElement = conceptProperty.Part.FirstOrDefault(p => p.Name == "display")?.Value as FhirString;

                    if (codeElement != null && displayElement != null)
                    {
                        var result = new CodeSystem.ConceptDefinitionComponent
                        {
                            Code = codeElement.Value,
                            Display = displayElement.Value
                        };

                        _cache.Set(cacheKey, result, TimeSpan.FromHours(24));
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up SNOMED code {SnomedCode}", snomedCode);
            }

            return null;
        }

        public async Task<IEnumerable<CodeableConcept>> SearchSnomedCodesAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<CodeableConcept>();

            string cacheKey = $"{CacheKeyPrefix}Search_{searchTerm}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<CodeableConcept>? cachedResult))
            {
                return cachedResult!;
            }

            // IMPROVEMENT 4: Fixed malformed URL encoding. 
            // The ?fhir_vs parameter belongs INSIDE the escaped URL parameter, not floating outside.
            var valueSetUrl = $"{SnomedSystemUri}?fhir_vs";
            var requestUrl = $"ValueSet/$expand?url={Uri.EscapeDataString(valueSetUrl)}&filter={Uri.EscapeDataString(searchTerm)}&count=50";
            try
            {
                var response = await _retryPolicy.ExecuteAsync(ct => _httpClient.GetAsync(requestUrl, ct), cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var valueSet = _fhirJsonDeserializer.Deserialize<ValueSet>(json);

                var conceptCodes = new List<CodeableConcept>();

                // Simplified null checking using modern C#
                if (valueSet.Expansion?.Contains != null)
                {
                    foreach (var concept in valueSet.Expansion.Contains)
                    {
                        if (!string.IsNullOrEmpty(concept.Code) && !string.IsNullOrEmpty(concept.Display))
                        {
                            conceptCodes.Add(CreateSnomedCodeableConcept(concept.Code, concept.Display));
                        }
                    }
                }

                _cache.Set(cacheKey, conceptCodes, TimeSpan.FromHours(1));
                return conceptCodes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching SNOMED codes for term '{SearchTerm}'", searchTerm);
                return Enumerable.Empty<CodeableConcept>();
            }
        }

        public async Task<bool> ValidateSnomedCodeAsync(string snomedCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(snomedCode)) return false;

            string cacheKey = $"{CacheKeyPrefix}Validate_{snomedCode}";
            if (_cache.TryGetValue(cacheKey, out bool cachedResult))
            {
                return cachedResult;
            }

            var requestUrl = $"CodeSystem/$validate-code?system={Uri.EscapeDataString(SnomedSystemUri)}&code={Uri.EscapeDataString(snomedCode)}";

            try
            {
                var response = await _retryPolicy.ExecuteAsync(ct => _httpClient.GetAsync(requestUrl, ct), cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var parameters = _fhirJsonDeserializer.Deserialize<Parameters>(json);

                var resultProperty = parameters.Parameter.FirstOrDefault(p => p.Name == "result");
                if (resultProperty?.Value is FhirBoolean resultBoolean)
                {
                    bool isValid = resultBoolean.Value ?? false;
                    _cache.Set(cacheKey, isValid, TimeSpan.FromHours(24));
                    return isValid;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating SNOMED code {SnomedCode}", snomedCode);
            }

            return false;
        }

        public CodeableConcept CreateSnomedCodeableConcept(string snomedCode, string display)
        {
            return new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = SnomedSystemUri,
                        Code = snomedCode,
                        Display = display
                    }
                },
                Text = display
            };
        }
    }
}