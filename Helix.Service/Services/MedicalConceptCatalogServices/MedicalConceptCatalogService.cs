using Helix.Infrastructure.Context;
using Helix.Service.DTOs.CatalogDTOs;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Services.MedicalConceptCatalogServices
{
    public class MedicalConceptCatalogService : IMedicalConceptCatalogService
    {
        private readonly ApplicationDbContext _dbContext;

        public MedicalConceptCatalogService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ConceptSearchDto>> SearchConceptsAsync(string searchTerm, bool? isRadiology = null)
        {
            // Protect against empty or massive queries
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                return new List<ConceptSearchDto>();

            var searchLower = searchTerm.ToLower();

            // 1. Start building the base query
            var query = _dbContext.MedicalConceptCatalogs
                .AsNoTracking()
                .Where(mc => mc.SystemUri == "http://loinc.org" && mc.Display.ToLower().Contains(searchLower));

            // 2. Apply the department filter ONLY if the frontend requested it
            if (isRadiology.HasValue)
            {
                query = query.Where(mc => mc.IsRadiology == isRadiology.Value);
            }

            // 3. Execute the final query and map to the DTO
            var results = await query
                .Select(mc => new ConceptSearchDto
                {
                    ConceptId = mc.Id,
                    Code = mc.Code,
                    DisplayName = mc.Display,
                    IsRadiology = mc.IsRadiology,
                    // MAGIC CHECK: Radiology tests are never panels, so skip the DB check if it's imaging!
                    IsPanel = !mc.IsRadiology && _dbContext.LoincPanelComponents.Any(lpc => lpc.ParentLoincConceptId == mc.Id)
                })
                .Take(50) // Limit to top 50 results to keep the UI snappy
                .ToListAsync();

            return results;
        }
    }
}
