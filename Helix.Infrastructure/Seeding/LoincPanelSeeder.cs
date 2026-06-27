using CsvHelper;
using CsvHelper.Configuration;
using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Infrastructure.Seeding
{
    public class LoincPanelSeeder(ApplicationDbContext dbContext, ILogger<LoincPanelSeeder> logger)
    {
        public async Task SeedPanelsAndFormsDataAsync(string panelsAndFormsCsvFilePath)
        {
            // Only seed if the table is completely empty
            if (await dbContext.Set<LoincPanelComponent>().AnyAsync())
            {
                return;
            }
            logger.LogInformation("Starting LOINC PanelsAndForms data seeding process...");

            if (!File.Exists(panelsAndFormsCsvFilePath))
            {
                logger.LogError($"PanelsAndForms CSV file not found at: {panelsAndFormsCsvFilePath}");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                BadDataFound = context =>
                {
                    logger.LogWarning($"Bad data found in PanelsAndForms CSV: {context.RawRecord}");
                }
            };

            using (var reader = new StreamReader(panelsAndFormsCsvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<PanelsAndFormsCsvMap>();

                var records = csv.GetRecords<PanelsAndFormsCsvRecord>().ToList();
                logger.LogInformation($"Read {records.Count} records from PanelsAndForms CSV.");

                var batchSize = 1000;
                for (int i = 0; i < records.Count; i += batchSize)
                {
                    var batch = records.Skip(i).Take(batchSize).ToList();

                    // 1. Get all unique LOINC codes needed for this batch
                    var neededCodes = batch.Select(r => r.ParentLoinc)
                                           .Concat(batch.Select(r => r.Loinc))
                                           .Where(code => !string.IsNullOrWhiteSpace(code))
                                           .Distinct()
                                           .ToList();

                    // 2. Fetch all required Medical Concepts in ONE query and store in a Dictionary
                    var conceptDict = await dbContext.MedicalConceptCatalogs
                        .AsNoTracking()
                        .Where(mc => mc.SystemUri == "http://loinc.org" && neededCodes.Contains(mc.Code))
                        .ToDictionaryAsync(mc => mc.Code, mc => mc.Id);

                    var panelComponents = new List<LoincPanelComponent>();

                    foreach (var record in batch)
                    {
                        if (string.IsNullOrWhiteSpace(record.ParentLoinc) || string.IsNullOrWhiteSpace(record.Loinc))
                            continue;

                        // 3. Instant memory lookup instead of database querying!
                        if (!conceptDict.TryGetValue(record.ParentLoinc, out var parentId))
                        {
                            logger.LogWarning($"Parent LOINC concept not found for code: {record.ParentLoinc}. Skipping.");
                            continue;
                        }

                        if (!conceptDict.TryGetValue(record.Loinc, out var childId))
                        {
                            logger.LogWarning($"Child LOINC concept not found for code: {record.Loinc}. Skipping.");
                            continue;
                        }

                        // TRUNCATE HERE: Safely size the string for both Inserts and Updates!
                        var safeConditionality = !string.IsNullOrEmpty(record.Conditionality) && record.Conditionality.Length > 10
                            ? record.Conditionality.Substring(0, 10)
                            : record.Conditionality ?? string.Empty;

                        var panelComponent = new LoincPanelComponent
                        {
                            ParentLoincConceptId = parentId,
                            ChildLoincConceptId = childId,
                            Sequence = int.TryParse(record.Sequence, out int seq) ? seq : 0,
                            Conditionality = safeConditionality,
                            LastUpdated = DateTime.UtcNow
                        };
                        panelComponents.Add(panelComponent);
                    }

                    await UpsertLoincPanelComponents(panelComponents);
                    logger.LogInformation($"Processed panel batch {i / batchSize + 1}/{(records.Count + batchSize - 1) / batchSize}");
                }

                logger.LogInformation("LOINC PanelsAndForms data seeding process completed.");
            }
        }

        private async Task UpsertLoincPanelComponents(List<LoincPanelComponent> components)
        {
            if (!components.Any()) return;

            // Extract all Parent IDs in this batch to limit our database query
            var parentIds = components.Select(c => c.ParentLoincConceptId).Distinct().ToList();

            // Fetch existing mappings in ONE query
            var existingComponentsList = await dbContext.LoincPanelComponents
                .Where(lpc => parentIds.Contains(lpc.ParentLoincConceptId))
                .ToListAsync();

            // Create a dictionary using a composite string key: "ParentId_ChildId"
            var existingDict = existingComponentsList
                .ToDictionary(lpc => $"{lpc.ParentLoincConceptId}_{lpc.ChildLoincConceptId}");

            var newComponents = new List<LoincPanelComponent>();

            foreach (var component in components)
            {
                var compositeKey = $"{component.ParentLoincConceptId}_{component.ChildLoincConceptId}";

                if (existingDict.TryGetValue(compositeKey, out var existingComponent))
                {
                    // Update properties of the existing component
                    existingComponent.Sequence = component.Sequence;

                    // Directly assign, it is already safely truncated from Step 1!
                    existingComponent.Conditionality = component.Conditionality;

                    existingComponent.LastUpdated = component.LastUpdated;
                    dbContext.LoincPanelComponents.Update(existingComponent);
                }
                else
                {
                    // It's new, add to our list
                    newComponents.Add(component);

                    // Add to dictionary immediately to prevent duplicate inserts within the same batch!
                    existingDict[compositeKey] = component;
                }
            }

            if (newComponents.Any())
            {
                await dbContext.LoincPanelComponents.AddRangeAsync(newComponents);
            }

            await dbContext.SaveChangesAsync();

            // CRITICAL: Clear the Change Tracker so RAM doesn't overflow
            dbContext.ChangeTracker.Clear();
        }
    }

    public class PanelsAndFormsCsvRecord
    {
        public string ParentLoinc { get; set; }
        public string Loinc { get; set; }
        public string Sequence { get; set; }
        public string Conditionality { get; set; }
    }

    public sealed class PanelsAndFormsCsvMap : ClassMap<PanelsAndFormsCsvRecord>
    {
        public PanelsAndFormsCsvMap()
        {
            Map(m => m.ParentLoinc).Name("ParentLoinc");
            Map(m => m.Loinc).Name("Loinc");

            // Fix 1: Match the exact ALL-CAPS header in the CSV
            Map(m => m.Sequence).Name("SEQUENCE").Optional();

            // Fix 2: Map to the correct column name in LOINC 2.82 and make it optional
            Map(m => m.Conditionality).Name("ObservationRequiredInPanel").Optional();
        }
    }
}