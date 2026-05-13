using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using Helix.Service.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Helix.Service.Services.LoincTerminology
{
    public class LoincTerminologyService : ILoincTerminologyService
    {
        private readonly HttpClient _httpClient;
        private readonly ITerminologyCodeLookupService _terminologyCode;

        public LoincTerminologyService(HttpClient httpClient,ITerminologyCodeLookupService terminologyCode)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _terminologyCode = terminologyCode ?? throw new ArgumentNullException(nameof(terminologyCode));
        }

        public async Task<CodeableConcept> LookupLoincCodeAsync(string loincCode)
        {
            if (string.IsNullOrWhiteSpace(loincCode))
                return null;

            try
            {
                // Correct FHIR lookup operation with the system parameter
                var requestUri = $"CodeSystem/$lookup?system=http://loinc.org&code={Uri.EscapeDataString(loincCode)}";

                // Ensure we request JSON format
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var parameters = ParseParametersResponse(content);

                if (parameters == null)
                    return null;

                // Extract code and display from the parameters response
                var code = ExtractParameterValue(parameters, "name") ?? loincCode;
                var display = ExtractParameterValue(parameters, "display") ?? loincCode;

                return new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = loincCode,
                            Display = display
                        }
                    },
                    Text = display
                };
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to lookup LOINC code {loincCode}: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<CodeableConcept>> SearchLoincCodesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<CodeableConcept>();
            // 1. Try to find in the Database first
            var cachedEntry= await _terminologyCode.GetOrFetchLoincCodeAsync(searchTerm);
            if (cachedEntry.Count > 0)
            {
                var concepts = cachedEntry.Select(entry => new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = entry.SystemUrl,
                            Code = entry.Code,
                            Display = entry.Display
                        }
                    },
                    Text = entry.Display
                });
                return concepts;
            }
            try
            {
                var requestUri = $"ValueSet/$expand?url=http://loinc.org/vs&filter={Uri.EscapeDataString(searchTerm)}&count=50";

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    return Enumerable.Empty<CodeableConcept>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var res = ParseSearchResults(content);
                foreach (var result in res)
                {
                    // Ensure non-nullable DTO properties receive non-null values to avoid CS8601
                    var first = result.Coding?.FirstOrDefault();
                    _ = _terminologyCode.CreateTerminologyCodeLookupAsync(new CreateTerminologyCodeLookupDto
                    {
                        Code = first?.Code ?? string.Empty,
                        Display = first?.Display ?? string.Empty,
                        SystemUrl = first?.System ?? "http://loinc.org",
                        TerminologyType = Data.Enums.EnTerminologyType.LabTest
                    });
                }
                return res;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to search LOINC codes for '{searchTerm}': {ex.Message}", ex);
            }
        }
        public Observation CreateFhirObservation(string loincCode, string display)
        {
            if (string.IsNullOrWhiteSpace(loincCode))
                throw new ArgumentException("LOINC code is required.", nameof(loincCode));

            var observation = new Observation
            {
                Status = ObservationStatus.Final,
                Category = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "laboratory",
                                Display = "Laboratory"
                            }
                        },
                        Text = "Laboratory"
                    }
                },
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = loincCode,
                            Display = display ?? loincCode
                        }
                    },
                    Text = display ?? loincCode
                },
                Effective = new FhirDateTime(DateTime.UtcNow),
                Issued = DateTimeOffset.UtcNow
            };

            return observation;
        }

        #region Private Helpers
        private Parameters? ParseParametersResponse(string jsonContent)
        {
            try
            {
                var parser = new FhirJsonDeserializer();
                return parser.Deserialize<Parameters>(jsonContent);
            }
            catch
            {
                return null;
            }
        }
        private string? ExtractParameterValue(Parameters? parameters, string name)
        {
            var param = parameters?.Parameter?.FirstOrDefault(p => p.Name == name);
            if (param == null)
                return null;

            if (param.Value is FhirString fhirString)
                return fhirString.Value;

            if (param.Value is Coding coding)
                return coding.Display ?? coding.Code;

            return param.Value?.ToString();
        }
        private IEnumerable<CodeableConcept> ParseSearchResults(string jsonContent)
        {
            try
            {
                var parser = new FhirJsonDeserializer();
                var valueSet = parser.Deserialize<ValueSet>(jsonContent);

                if (valueSet?.Expansion?.Contains == null)
                    return Enumerable.Empty<CodeableConcept>();

                return valueSet.Expansion.Contains.Select(concept => new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = concept.System ?? "http://loinc.org",
                            Code = concept.Code,
                            Display = concept.Display
                        }
                    },
                    Text = concept.Display ?? concept.Code
                });
            }
            catch
            {
                return Enumerable.Empty<CodeableConcept>();
            }
        }

        #endregion
    }
}