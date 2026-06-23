using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.MedicationCatalogServices
{
    public class MedicationCatalogService : IMedicationCatalogService
    {
        private readonly ApplicationDbContext _context;

        public MedicationCatalogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicationCatalog>> SearchMedicationsAsync(string query, int count = 40)
        {
            // If the search query is empty, return the top default medications
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetTopMedicationsAsync(count);
            }

            // Convert query to lowercase to ensure case-insensitive searches
            var lowerQuery = query.ToLower();

            // Search the local database for matches
            // Limits to the specified count to ensure high UI performance on the frontend dropdown
            return await _context.Set<MedicationCatalog>()
                .Where(m => m.DrugName.ToLower().Contains(lowerQuery))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationCatalog>> GetTopMedicationsAsync(int count = 10)
        {
            return await _context.Set<MedicationCatalog>()
                .Take(count)
                .ToListAsync();
        }
    }
}