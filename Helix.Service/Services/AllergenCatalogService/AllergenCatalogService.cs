using Helix.Data.Entities;
using Helix.Infrastructure.Context; // Update to match your DbContext namespace
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.AllergenCatalogServices
{
    public class AllergenCatalogService(ApplicationDbContext context) : IAllergenCatalogService
    {
        public async Task<IEnumerable<AllergenCatalog>> SearchAllergiesAsync(string query, int count = 40)
        {
            // If the search query is empty, return the top default allergies
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetTopAllergiesAsync(count);
            }

            // Search the local database for matches
            // Limits to the specified count to ensure high UI performance on the frontend dropdown
            return await context.AllergenCatalogs
                .Where(a => a.DisplayName.Contains(query))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<AllergenCatalog>> GetTopAllergiesAsync(int count = 10)
        {
            return await context.AllergenCatalogs
                .Take(count)
                .ToListAsync();
        }
    }
}