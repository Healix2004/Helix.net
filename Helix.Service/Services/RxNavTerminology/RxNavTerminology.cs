using Helix.Service.DTOs.RxNavDTOS;
using Helix.Service.Interfaces;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Helix.Service.Services.RxNavTerminology
{
    public class RxNavTerminologyService(HttpClient httpClient) : IRxNavTerminologyService
    {
        private const string BaseUrl = "https://rxnav.nlm.nih.gov/REST";

        public async Task<string> GetRxcuiByDrugNameAsync(string drugName)
        {
            var url = $"{BaseUrl}/rxcui.json?name={Uri.EscapeDataString(drugName)}";
            var response = await httpClient.GetFromJsonAsync<RxNavRxcuiResponse>(url);
            return response?.IdGroup?.Rxcui?.FirstOrDefault();
        }

        public async Task<Medication> GetFhirMedicationByRxcuiAsync(string rxcui)
        {
            var url = $"{BaseUrl}/rxcui/{rxcui}/properties.json";
            var response = await httpClient.GetFromJsonAsync<RxNavPropertiesResponse>(url);

            if (response?.Properties == null) return null;

            return MapToFhirMedication(response.Properties);
        }

        public async Task<IEnumerable<Medication>> SearchFhirMedicationsAsync(string searchQuery)
        {
            var url = $"{BaseUrl}/drugs.json?name={Uri.EscapeDataString(searchQuery)}";
            var response = await httpClient.GetFromJsonAsync<RxNavDrugsResponse>(url);

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