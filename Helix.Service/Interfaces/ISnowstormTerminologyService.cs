using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Interfaces
{
    public interface ISnowstormTerminologyService
    {
        /// <summary>
        /// Looks up a specific SNOMED CT code and returns its details as a FHIR CodeSystem concept.
        /// </summary>
        /// <param name="snomedCode">The SNOMED CT code (e.g., "22298006").</param>
        /// <returns>A FHIR CodeSystem.ConceptDefinitionComponent if found, otherwise null.</returns>
        Task<CodeSystem.ConceptDefinitionComponent?> LookupSnomedCodeAsync(string snomedCode);

        /// <summary>
        /// Searches for SNOMED CT codes by a search term and returns them as FHIR CodeableConcepts.
        /// This uses the $expand operation on an implicit ValueSet.
        /// </summary>
        /// <param name="searchTerm">The term to search for (e.g., "diabetes").</param>
        /// <returns>A list of FHIR CodeableConcepts representing matching SNOMED CT codes.</returns>
        Task<IEnumerable<CodeableConcept>> SearchSnomedCodesAsync(string searchTerm);

        /// <summary>
        /// Validates if a given SNOMED CT code exists and is active.
        /// </summary>
        /// <param name="snomedCode">The SNOMED CT code to validate.</param>
        /// <returns>True if the code is valid, false otherwise.</returns>
        Task<bool> ValidateSnomedCodeAsync(string snomedCode);

        /// <summary>
        /// Creates a basic FHIR CodeableConcept for a given SNOMED CT code and display text.
        /// </summary>
        /// <param name="snomedCode">The SNOMED CT code.</param>
        /// <param name="display">The display text for the SNOMED CT code.</param>
        /// <returns>A FHIR CodeableConcept.</returns>
        CodeableConcept CreateSnomedCodeableConcept(string snomedCode, string display);
    }
}
