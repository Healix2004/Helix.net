using Helix.Data.Enums;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface ILoincTerminologyService
    {
        Task<CodeableConcept> LookupLoincCodeAsync(string loincCode);
        Task<IEnumerable<CodeableConcept>> SearchLoincCodesAsync(string searchTerm, EnTerminologyType category = EnTerminologyType.LabTest);
        Observation CreateFhirObservation(string loincCode, string display);
    }
}
