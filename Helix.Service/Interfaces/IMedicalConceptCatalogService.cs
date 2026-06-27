using Helix.Service.DTOs.CatalogDTOs;

namespace Helix.Service.Interfaces
{
    public interface IMedicalConceptCatalogService
    {
        Task<IEnumerable<ConceptSearchDto>> SearchConceptsAsync(string searchTerm, bool? isRadiology = null);
    }
}