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
using System.Threading.Tasks;

namespace Helix.Service.Services.SnowstormTerminology
{
    public class SnowstormTerminologyService : ISnowstormTerminologyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SnowstormTerminologyService> _logger;
        private readonly FhirJsonParser _fhirParser;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        private const string SnomedFhirBaseUrl = "https://snowstorm.snomedtools.org/fhir/";
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
            _fhirParser = new FhirJsonParser();

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(SnomedFhirBaseUrl);
            }

            // Enhanced retry policy to handle 429, 5xx, and network-level exceptions/timeouts
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == (HttpStatusCode)429 || (int)r.StatusCode >= 500)
                .Or<HttpRequestException>()
                .Or<SocketException>()
                .Or<TaskCanceledException>() // Handles HttpClient timeouts
                .WaitAndRetryAsync(3, retryAttempt =>
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    _logger.LogWarning($"Snowstorm API request failed. Retrying in {delay.TotalSeconds}s (Attempt {retryAttempt}/3)...");
                    return delay;
                });
        }

        public async Task<CodeSystem.ConceptDefinitionComponent?> LookupSnomedCodeAsync(string snomedCode)
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
                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(requestUrl));
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var parameters = _fhirParser.Parse<Parameters>(json);

                var conceptProperty = parameters.Parameter.FirstOrDefault(p => p.Name == "concept");
                if (conceptProperty != null && conceptProperty.Part != null)
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
                _logger.LogError(ex, $"Error looking up SNOMED code {snomedCode}");
            }

            return null;
        }

        public async Task<IEnumerable<CodeableConcept>> SearchSnomedCodesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<CodeableConcept>();

            string cacheKey = $"{CacheKeyPrefix}Search_{searchTerm}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<CodeableConcept>? cachedResult))
            {
                return cachedResult!;
            }

            var requestUrl = $"ValueSet/$expand?url={Uri.EscapeDataString(SnomedSystemUri)}?fhir_vs&filter={Uri.EscapeDataString(searchTerm)}";

            try
            {
                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(requestUrl));
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var valueSet = _fhirParser.Parse<ValueSet>(json);

                var conceptCodes = new List<CodeableConcept>();

                if (valueSet.Expansion?.Contains != null)
                {
                    foreach (var concept in valueSet.Expansion.Contains)
                    {
                        if (!string.IsNullOrEmpty(concept.Code) && !string.IsNullOrEmpty(concept.Display))
                        {
                            conceptCodes.Add(new CodeableConcept
                            {
                                Coding = new List<Coding>
                                {
                                    new Coding
                                    {
                                        System = SnomedSystemUri,
                                        Code = concept.Code,
                                        Display = concept.Display
                                    }
                                },
                                Text = concept.Display
                            });
                        }
                    }
                }

                _cache.Set(cacheKey, conceptCodes, TimeSpan.FromHours(1));
                return conceptCodes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching SNOMED codes for term '{searchTerm}'");
                return Enumerable.Empty<CodeableConcept>();
            }
        }

        public async Task<bool> ValidateSnomedCodeAsync(string snomedCode)
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
                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(requestUrl));
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var parameters = _fhirParser.Parse<Parameters>(json);

                var resultProperty = parameters.Parameter.FirstOrDefault(p => p.Name == "result");
                if (resultProperty != null && resultProperty.Value is FhirBoolean resultBoolean)
                {
                    bool isValid = resultBoolean.Value ?? false;
                    _cache.Set(cacheKey, isValid, TimeSpan.FromHours(24));
                    return isValid;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating SNOMED code {snomedCode}");
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
