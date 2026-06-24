using Helix.Data.Entities;
using Helix.Infrastructure.Context; // Update to match your DbContext namespace if different
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.SpecialtyCatalogServices
{
    public class SpecialtyCatalogService(ApplicationDbContext context) : ISpecialtyCatalogService
    {
        public async Task<IEnumerable<SpecialtyCatalog>> SearchSpecialtiesAsync(string query, int count = 40)
        {
            // If the search query is empty, return the top default specialties
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetTopSpecialtiesAsync(count);
            }
            return await context.Set<SpecialtyCatalog>()
                .Where(s => s.DisplayName.Contains(query))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpecialtyCatalog>> GetTopSpecialtiesAsync(int count = 10)
        {
            return await context.Set<SpecialtyCatalog>()
                .Take(count)
                .ToListAsync();
        }
    }
}