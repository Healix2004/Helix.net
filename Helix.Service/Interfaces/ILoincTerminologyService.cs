using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface ILoincTerminologyService
    {
        /// <summary>
        /// Look up a specific LOINC code and retrieve its details
        /// </summary>
        /// <param name="loincCode">The LOINC code to lookup (e.g., "2345-7")</param>
        /// <returns>CodeableConcept containing the LOINC code information, or null if not found</returns>
        Task<CodeableConcept> LookupLoincCodeAsync(string loincCode);

        /// <summary>
        /// Search for LOINC codes based on a search term
        /// </summary>
        /// <param name="searchTerm">Search term (e.g., "glucose", "hemoglobin")</param>
        /// <returns>Collection of CodeableConcepts matching the search term</returns>
        Task<IEnumerable<CodeableConcept>> SearchLoincCodesAsync(string searchTerm);

        /// <summary>
        /// Create a FHIR Observation resource from LOINC code details
        /// </summary>
        /// <param name="loincCode">The LOINC code (e.g., "2345-7")</param>
        /// <param name="display">Display name for the code</param>
        /// <returns>FHIR Observation resource</returns>
        Observation CreateFhirObservation(string loincCode, string display);
    }
}
