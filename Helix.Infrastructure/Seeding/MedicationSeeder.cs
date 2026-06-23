using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Helix.Infrastructure.Seeding
{
    public static class MedicationSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Set<MedicationCatalog>().AnyAsync())
            {
                Console.WriteLine("Seeding RxNorm Medication dataset from local RRF file...");

                try
                {
                    // Looks for the RXNCONSO.RRF file in your project root
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "RXNCONSO.RRF");

                    if (File.Exists(filePath))
                    {
                        var medications = new Dictionary<string, MedicationCatalog>();
                        int batchSize = 10000;
                        int count = 0;

                        // File.ReadLines streams the file one line at a time to save RAM
                        foreach (var line in File.ReadLines(filePath))
                        {
                            // RRF files use the pipe character | as a separator
                            var columns = line.Split('|');

                            // Ensure the line has enough columns 
                            // In RXNCONSO: Index 0 is RXCUI, Index 1 is Language, Index 12 is Term Type, Index 14 is Drug Name
                            if (columns.Length > 14)
                            {
                                var rxcui = columns[0].Trim();
                                var language = columns[1].Trim();
                                var termType = columns[12].Trim();
                                var drugName = columns[14].Trim();

                                // Only grab English terms, and ensure we don't add duplicate RXCUIs
                                if (language == "ENG" && !medications.ContainsKey(rxcui))
                                {
                                    medications[rxcui] = new MedicationCatalog
                                    {
                                        Rxcui = rxcui,
                                        TermType = termType,
                                        DrugName = drugName
                                    };

                                    count++;

                                    // Save to database in chunks of 10,000 to prevent memory overload
                                    if (count % batchSize == 0)
                                    {
                                        await context.Set<MedicationCatalog>().AddRangeAsync(medications.Values);
                                        await context.SaveChangesAsync();

                                        Console.WriteLine($" -> Saved {count} medications...");
                                        medications.Clear(); // Clear dictionary to free up RAM
                                    }
                                }
                            }
                        }

                        // Save any remaining medications
                        if (medications.Count > 0)
                        {
                            await context.Set<MedicationCatalog>().AddRangeAsync(medications.Values);
                            await context.SaveChangesAsync();
                        }

                        Console.WriteLine($"\nSUCCESS! Extracted and seeded a total of {count} prescribable medications!");
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Could not find RXNCONSO.RRF in the project root.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to seed medications: {ex.Message}");
                }
            }
        }
    }
}