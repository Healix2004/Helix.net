using CsvHelper;
using CsvHelper.Configuration;
using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Hl7.Fhir.Model.CdsHooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Helix.Infrastructure.Seeding
{
    public class LoincSeeder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<LoincSeeder> _logger;

        public LoincSeeder(ApplicationDbContext dbContext, ILogger<LoincSeeder> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task SeedLoincDataAsync(string loincCsvFilePath)
        {
            // Only seed if the table is completely empty
            if (await _dbContext.Set<MedicalConcept>().AnyAsync())
            { 
                return;
            }
                _logger.LogInformation("Starting LOINC data seeding process...");

            if (!File.Exists(loincCsvFilePath))
            {
                _logger.LogError($"LOINC CSV file not found at: {loincCsvFilePath}");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                BadDataFound = context =>
                {
                    _logger.LogWarning($"Bad data found in LOINC CSV: {context.RawRecord}");
                }
            };

            using (var reader = new StreamReader(loincCsvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<LoincCsvMap>();

                var records = csv.GetRecords<LoincCsvRecord>().ToList();
                _logger.LogInformation($"Read {records.Count} records from LOINC CSV.");

                var batchSize = 5000;
                for (int i = 0; i < records.Count; i += batchSize)
                {
                    var batch = records.Skip(i).Take(batchSize).ToList();
                    var medicalConcepts = new List<MedicalConcept>();

                    foreach (var record in batch)
                    {
                        if (string.IsNullOrWhiteSpace(record.LOINC_NUM) || string.IsNullOrWhiteSpace(record.LONG_COMMON_NAME))
                        {
                            _logger.LogWarning($"Skipping record due to missing LOINC_NUM or LONG_COMMON_NAME: {record.LOINC_NUM} - {record.LONG_COMMON_NAME}");
                            continue;
                        }

                        var medicalConcept = new MedicalConcept
                        {
                            Code = record.LOINC_NUM,
                            SystemUri = "http://loinc.org",
                            Component = record.COMPONENT ?? string.Empty,
                            Property = record.PROPERTY ?? string.Empty,
                            TimeAspect = record.TIME_ASPCT ?? string.Empty,
                            System = record.SYSTEM ?? string.Empty,
                            ScaleType = record.SCALE_TYP ?? string.Empty,
                            MethodType = record.METHOD_TYP ?? string.Empty,
                            Class = record.CLASS ?? string.Empty,
                            Display = record.LONG_COMMON_NAME,
                            IsActive = record.STATUS == "ACTIVE",
                            IsRadiology = record.CLASS != null && record.CLASS.StartsWith("RAD", StringComparison.OrdinalIgnoreCase),
                            LastUpdated = DateTime.UtcNow
                        };
                        medicalConcepts.Add(medicalConcept);
                    }

                    await UpsertMedicalConcepts(medicalConcepts);
                    _logger.LogInformation($"Processed batch {i / batchSize + 1}/{(records.Count + batchSize - 1) / batchSize}");
                }

                _logger.LogInformation("LOINC data seeding process completed.");
            }
        }

        private async Task UpsertMedicalConcepts(List<MedicalConcept> concepts)
        {
            // 1. Extract all the LOINC codes from this specific batch of 5000
            var incomingCodes = concepts.Select(c => c.Code).ToList();

            // 2. Fetch all existing records that match these codes in ONE single query!
            // We put them in a Dictionary so we can look them up instantly in memory.
            var existingConcepts = await _dbContext.MedicalConceptCatalogs
                .Where(mc => mc.SystemUri == "http://loinc.org" && incomingCodes.Contains(mc.Code))
                .ToDictionaryAsync(mc => mc.Code);

            var newConcepts = new List<MedicalConcept>();

            // 3. Process the batch
            foreach (var concept in concepts)
            {
                // Instant memory lookup instead of a database query
                if (existingConcepts.TryGetValue(concept.Code, out var existingConcept))
                {
                    // Update properties of the existing concept
                    existingConcept.Component = concept.Component;
                    existingConcept.Property = concept.Property;
                    existingConcept.TimeAspect = concept.TimeAspect;
                    existingConcept.System = concept.System;
                    existingConcept.ScaleType = concept.ScaleType;
                    existingConcept.MethodType = concept.MethodType;
                    existingConcept.Class = concept.Class;
                    existingConcept.Display = concept.Display;
                    existingConcept.IsActive = concept.IsActive;
                    existingConcept.IsRadiology = concept.IsRadiology;
                    existingConcept.LastUpdated = concept.LastUpdated;

                    _dbContext.MedicalConceptCatalogs.Update(existingConcept);
                }
                else
                {
                    // It's new, add it to our list
                    newConcepts.Add(concept);
                }
            }

            // 4. Add all new items to EF Core at once
            if (newConcepts.Any())
            {
                await _dbContext.MedicalConceptCatalogs.AddRangeAsync(newConcepts);
            }

            // 5. Send the batch to the database
            await _dbContext.SaveChangesAsync();

            // 6. CRITICAL FIX: Clear the memory! 
            // This stops EF Core from slowing down as it processes the 85MB file.
            _dbContext.ChangeTracker.Clear();
        }
    }

    public class LoincCsvRecord
    {
        public string LOINC_NUM { get; set; }
        public string COMPONENT { get; set; }
        public string PROPERTY { get; set; }
        public string TIME_ASPCT { get; set; }
        public string SYSTEM { get; set; }
        public string SCALE_TYP { get; set; }
        public string METHOD_TYP { get; set; }
        public string CLASS { get; set; }
        public string LONG_COMMON_NAME { get; set; }
        public string STATUS { get; set; }
    }

    public sealed class LoincCsvMap : ClassMap<LoincCsvRecord>
    {
        public LoincCsvMap()
        {
            Map(m => m.LOINC_NUM).Name("LOINC_NUM");
            Map(m => m.COMPONENT).Name("COMPONENT");
            Map(m => m.PROPERTY).Name("PROPERTY");
            Map(m => m.TIME_ASPCT).Name("TIME_ASPCT");
            Map(m => m.SYSTEM).Name("SYSTEM");
            Map(m => m.SCALE_TYP).Name("SCALE_TYP");
            Map(m => m.METHOD_TYP).Name("METHOD_TYP");
            Map(m => m.CLASS).Name("CLASS");
            Map(m => m.LONG_COMMON_NAME).Name("LONG_COMMON_NAME");
            Map(m => m.STATUS).Name("STATUS");
        }
    }
}
