using Helix.Data.Entities;

namespace Helix.Service.Interfaces
{
    public interface IMedicationCatalogService
    {
        Task<IEnumerable<MedicationCatalog>> SearchMedicationsAsync(string query, int count = 40);
        Task<IEnumerable<MedicationCatalog>> GetTopMedicationsAsync(int count = 10);
    }
}
