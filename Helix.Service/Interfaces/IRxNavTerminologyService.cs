using Hl7.Fhir.Model;

namespace Helix.Service.Interfaces
{
    public interface IRxNavTerminologyService
    {
        Task<string> GetRxcuiByDrugNameAsync(string drugName);
        Task<Medication> GetFhirMedicationByRxcuiAsync(string rxcui);
        Task<IEnumerable<Medication>> SearchFhirMedicationsAsync(string searchQuery);
    }
}
