using Helix.Service.DTOs.RxNavDTOS;
using Helix.Service.Interfaces;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Helix.Service.Services.RxNavTerminology
{
    public class RxNavTerminologyService : IRxNavTerminologyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RxNavTerminologyService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        private const string BaseUrl = "https://rxnav.nlm.nih.gov/REST";

        public RxNavTerminologyService(HttpClient httpClient, ILogger<RxNavTerminologyService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(BaseUrl + "/");
            }

            if (_httpClient.Timeout == Timeout.InfiniteTimeSpan || _httpClient.Timeout.TotalSeconds > 10)
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(10);
            }

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(2),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "RxNav API request failed. Retrying in {Delay}s (Attempt {RetryAttempt}/1)...", timeSpan.TotalSeconds, retryCount);
                });
        }

        public async Task<string> GetRxcuiByDrugNameAsync(string drugName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(drugName)) return null;

            var url = $"rxcui.json?name={Uri.EscapeDataString(drugName)}";

            try
            {
                var response = await _retryPolicy.ExecuteAsync(ct => _httpClient.GetFromJsonAsync<RxNavRxcuiResponse>(url, ct), cancellationToken);
                return response?.IdGroup?.Rxcui?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get RXCUI for drug '{DrugName}'", drugName);
                return null;
            }
        }

        public async Task<Medication> GetFhirMedicationByRxcuiAsync(string rxcui, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rxcui)) return null;

            var url = $"rxcui/{Uri.EscapeDataString(rxcui)}/properties.json";

            try
            {
                var response = await _retryPolicy.ExecuteAsync(ct => _httpClient.GetFromJsonAsync<RxNavPropertiesResponse>(url, ct), cancellationToken);

                if (response?.Properties == null) return null;

                return MapToFhirMedication(response.Properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get FHIR Medication for RXCUI '{Rxcui}'", rxcui);
                return null;
            }
        }

        public async Task<IEnumerable<Medication>> SearchFhirMedicationsAsync(string searchQuery, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return Enumerable.Empty<Medication>();

            var url = $"drugs.json?name={Uri.EscapeDataString(searchQuery)}";

            try
            {
                var response = await _retryPolicy.ExecuteAsync(ct => _httpClient.GetFromJsonAsync<RxNavDrugsResponse>(url, ct), cancellationToken);

                var medications = new List<Medication>();

                if (response?.DrugGroup?.ConceptGroup != null)
                {
                    foreach (var group in response.DrugGroup.ConceptGroup)
                    {
                        if (group.ConceptProperties != null)
                        {
                            foreach (var prop in group.ConceptProperties)
                            {
                                medications.Add(MapToFhirMedication(prop));
                            }
                        }
                    }
                }

                return medications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search FHIR Medications for query '{SearchQuery}'", searchQuery);
                return Enumerable.Empty<Medication>();
            }
        }

        // ====================================================================
        // NEW: Approximate Search for Autocomplete (e.g. "aspi" -> "Aspirin")
        // ====================================================================
        public async Task<IEnumerable<Medication>> SearchApproximateMedicationsAsync(string term, int maxEntries = 5, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(term)) return Enumerable.Empty<Medication>();

            var url = $"approximateTerm.json?term={Uri.EscapeDataString(term)}&maxEntries={maxEntries}";

            try
            {
                var response = await _retryPolicy.ExecuteAsync(ct => _httpClient.GetFromJsonAsync<RxNavApproximateTermResponse>(url, ct), cancellationToken);

                var candidates = response?.ApproximateGroup?.Candidate;
                if (candidates == null || !candidates.Any())
                    return Enumerable.Empty<Medication>();

                var medications = new List<Medication>();

                // RxNav's approximateTerm only gives us the RXCUI codes, not the drug names.
                // We use Task.WhenAll to fetch the full FHIR medication properties for all candidates in parallel!
                var fetchTasks = candidates
                    .Where(c => !string.IsNullOrEmpty(c.Rxcui))
                    .Take(maxEntries) // Ensure we don't overload the API
                    .Select(c => GetFhirMedicationByRxcuiAsync(c.Rxcui, cancellationToken));

                // Await all parallel API calls
                var results = await Task.WhenAll(fetchTasks);

                foreach (var med in results)
                {
                    if (med != null)
                    {
                        medications.Add(med);
                    }
                }

                return medications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search approximate FHIR Medications for term '{Term}'", term);
                return Enumerable.Empty<Medication>();
            }
        }

        private Medication MapToFhirMedication(Properties props)
        {
            return new Medication
            {
                Status = Medication.MedicationStatusCodes.Active,
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://www.nlm.nih.gov/research/umls/rxnorm",
                            Code = props.Rxcui,
                            Display = props.Name
                        }
                    },
                    Text = props.Name
                }
            };
        }

        private Medication MapToFhirMedication(ConceptProperties props)
        {
            return new Medication
            {
                Status = Medication.MedicationStatusCodes.Active,
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://www.nlm.nih.gov/research/umls/rxnorm",
                            Code = props.Rxcui,
                            Display = props.Name
                        }
                    },
                    Text = props.Name
                }
            };
        }
    }
}