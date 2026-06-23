using Helix.Data.Entities;

namespace Helix.Service.Interfaces
{
    public interface IAllergenCatalogService
    {
        Task<IEnumerable<AllergenCatalog>> SearchAllergiesAsync(string query, int count = 40);
        Task<IEnumerable<AllergenCatalog>> GetTopAllergiesAsync(int count = 10);
    }
}
