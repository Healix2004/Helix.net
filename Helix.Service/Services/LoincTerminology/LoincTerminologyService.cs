using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Helix.Service.Interfaces;
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

        public LoincTerminologyService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Look up a specific LOINC code using the FHIR CodeSystem $lookup endpoint
        /// </summary>
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

        /// <summary>
        /// Search for LOINC codes using ValueSet $expand operation
        /// </summary>
        public async Task<IEnumerable<CodeableConcept>> SearchLoincCodesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<CodeableConcept>();

            try
            {
                // Correct FHIR expansion operation for searching concepts inside a terminology
                var requestUri = $"ValueSet/$expand?url=http://loinc.org/vs&filter={Uri.EscapeDataString(searchTerm)}&count=50";

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    return Enumerable.Empty<CodeableConcept>();
                }

                var content = await response.Content.ReadAsStringAsync();
                return ParseSearchResults(content);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to search LOINC codes for '{searchTerm}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create a FHIR Observation resource from LOINC code details
        /// </summary>
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

        /// <summary>
        /// Parse FHIR Parameters resource response from lookup operation
        /// </summary>
        private Parameters ParseParametersResponse(string jsonContent)
        {
            try
            {
                var parser = new FhirJsonParser();
                return parser.Parse<Parameters>(jsonContent);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extract a parameter value from a FHIR Parameters resource
        /// </summary>
        private string ExtractParameterValue(Parameters parameters, string name)
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

        /// <summary>
        /// Parse search results natively using the HL7 FHIR ValueSet Expansion
        /// </summary>
        private IEnumerable<CodeableConcept> ParseSearchResults(string jsonContent)
        {
            try
            {
                var parser = new FhirJsonParser();
                var valueSet = parser.Parse<ValueSet>(jsonContent);

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