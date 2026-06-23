using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface ISnowstormTerminologyService
    {
        Task<CodeSystem.ConceptDefinitionComponent?> LookupSnomedCodeAsync(string snomedCode, CancellationToken cancellationToken = default);
        Task<IEnumerable<CodeableConcept>> SearchSnomedCodesAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> ValidateSnomedCodeAsync(string snomedCode, CancellationToken cancellationToken = default);
        CodeableConcept CreateSnomedCodeableConcept(string snomedCode, string display);
    }
}