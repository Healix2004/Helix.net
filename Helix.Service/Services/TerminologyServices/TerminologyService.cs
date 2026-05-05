using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Service.DTOs.Terminology;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.TerminologyServices
{
    public class TerminologyService(ApplicationDbContext dbContext) : ITerminologyService
    {
        // 1. Get the official display name for a specific code
        public async Task<string> GetDisplayForCodeAsync(string systemUri, string code)
        {
            if (string.IsNullOrWhiteSpace(systemUri) || string.IsNullOrWhiteSpace(code))
                return null;

            //// We use AsNoTracking for read-only operations to boost performance
            //var result = await dbContext.TerminologyCodes
            //    .AsNoTracking()
            //    .Where(x => x.SystemUri == systemUri && x.Code == code && x.IsActive)
            //    .Select(x => x.Display)
            //    .FirstOrDefaultAsync();

            return "Unknown Code";
        }

        // 2. Search/Autocomplete functionality
        public async Task<List<CodingDto>> LookupCodesAsync(string filterText, string systemUri)
        {
            //var query = dbContext.TerminologyCodes.AsNoTracking().AsQueryable();
            var query = dbContext.Set<TerminologyCodeLookup>().AsNoTracking().AsQueryable();

            // Filter by System if provided (e.g., only search SNOMED codes)
            if (!string.IsNullOrEmpty(systemUri))
            {
                query = query.Where(x => x.SystemUrl == systemUri);
            }

            // Filter by Text (Search inside the Code itself OR the Display text)
            if (!string.IsNullOrWhiteSpace(filterText))
            {
                // Note: EF.Functions.Like is used for SQL 'LIKE' behavior
                // For high-volume production, consider Full-Text Search or Elasticsearch here
                query = query.Where(x =>
                    x.Display.Contains(filterText) ||
                    x.Code.StartsWith(filterText));
            }

            // Project to CodingDto and take top 20 to prevent huge data loads
            return await query
                .OrderBy(x => x.Display) // Optional: Sort alphabetically
                .Take(20)
                .Select(x => new CodingDto
                {
                    System = x.SystemUrl,
                    Code = x.Code,
                    Display = x.Display
                })
                .ToListAsync();
        }

        // 3. Validation - Check if a code strictly exists
        public async Task<bool> ValidateCodeAsync(string systemUri, string code)
        {
            if (string.IsNullOrWhiteSpace(systemUri) || string.IsNullOrWhiteSpace(code))
                return false;

            return await dbContext.Set<TerminologyCodeLookup>()
                .AsNoTracking()
                .AnyAsync(x => x.SystemUrl == systemUri && x.Code == code );
        }
    }
    
}