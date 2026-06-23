using Helix.Data.Entities;

namespace Helix.Service.Interfaces
{
    public interface IProcedureCatalogService
    {
        Task<IEnumerable<ProcedureCatalog>> SearchProceduresAsync(string query, int count = 40);
        Task<IEnumerable<ProcedureCatalog>> GetTopProceduresAsync(int count = 10);
    }
}
