using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IRxNavTerminologyService
    {
        Task<string> GetRxcuiByDrugNameAsync(string drugName, CancellationToken cancellationToken = default);
        Task<Medication> GetFhirMedicationByRxcuiAsync(string rxcui, CancellationToken cancellationToken = default);
        // Strict exact search
        Task<IEnumerable<Medication>> SearchFhirMedicationsAsync(string searchQuery, CancellationToken cancellationToken = default);
        // NEW: Autocomplete "Search-as-you-type" approximate search
        Task<IEnumerable<Medication>> SearchApproximateMedicationsAsync(string term, int maxEntries = 5, CancellationToken cancellationToken = default);
    }
}