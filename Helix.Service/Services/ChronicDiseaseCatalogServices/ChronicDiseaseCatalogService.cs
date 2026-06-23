using Helix.Data.Entities;
using Helix.Infrastructure.Context; // Update to match your DbContext namespace
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.ChronicDiseaseCatalogServices
{
    public class ChronicDiseaseCatalogService : IChronicDiseaseCatalogService
    {
        private readonly ApplicationDbContext _context;

        public ChronicDiseaseCatalogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChronicDiseaseCatalog>> SearchChronicDiseasesAsync(string query, int count = 40)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetTopChronicDiseasesAsync(count);
            }
            return await _context.Set<ChronicDiseaseCatalog>()
                .Where(c => c.DisplayName.Contains(query))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChronicDiseaseCatalog>> GetTopChronicDiseasesAsync(int count = 10)
        {
            return await _context.Set<ChronicDiseaseCatalog>()
                .Take(count)
                .ToListAsync();
        }
    }
}