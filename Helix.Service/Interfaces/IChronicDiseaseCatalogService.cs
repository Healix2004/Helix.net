using Helix.Data.Entities;

namespace Helix.Service.Interfaces
{
    public interface IChronicDiseaseCatalogService
    {
        Task<IEnumerable<ChronicDiseaseCatalog>> SearchChronicDiseasesAsync(string query, int count = 40);
        Task<IEnumerable<ChronicDiseaseCatalog>> GetTopChronicDiseasesAsync(int count = 10);
    }
}
