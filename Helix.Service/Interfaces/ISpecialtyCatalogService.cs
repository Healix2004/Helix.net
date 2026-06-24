using Helix.Data.Entities;

namespace Helix.Service.Interfaces
{
    public interface ISpecialtyCatalogService
    {
        Task<IEnumerable<SpecialtyCatalog>> SearchSpecialtiesAsync(string query, int count = 40);
        Task<IEnumerable<SpecialtyCatalog>> GetTopSpecialtiesAsync(int count = 10);
    }
}

