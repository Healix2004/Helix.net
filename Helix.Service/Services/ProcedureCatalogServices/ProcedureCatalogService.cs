using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.ProcedureCatalogServices
{
    public class ProcedureCatalogService : Interfaces.IProcedureCatalogService
    {
        private readonly ApplicationDbContext _context;

        public ProcedureCatalogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProcedureCatalog>> SearchProceduresAsync(string query, int count = 40)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetTopProceduresAsync(count);

            return await _context.Set<ProcedureCatalog>()
                .Where(p => p.DisplayName.Contains(query))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcedureCatalog>> GetTopProceduresAsync(int count = 10)
        {
            return await _context.Set<ProcedureCatalog>()
                .Take(count)
                .ToListAsync();
        }
    }
}
