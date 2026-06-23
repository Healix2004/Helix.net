using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Helix.Infrastructure.Seeding
{
    public static class ProcedureSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Set<ProcedureCatalog>().AnyAsync())
            {
                Console.WriteLine("Seeding SNOMED procedure dataset from local CSV file...");
                var procedures = new List<ProcedureCatalog>();

                try
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "snomed_procedures_dataset.csv");

                    if (File.Exists(filePath))
                    {
                        var lines = await File.ReadAllLinesAsync(filePath);

                        for (int i = 1; i < lines.Length; i++)
                        {
                            var columns = lines[i].Split(',');

                            if (columns.Length >= 2)
                            {
                                procedures.Add(new ProcedureCatalog
                                {
                                    Code = columns[0].Trim(),
                                    DisplayName = columns[1].Trim(),
                                    CodeSystem = "SNOMED"
                                });
                            }
                        }

                        await context.Set<ProcedureCatalog>().AddRangeAsync(procedures);
                        await context.SaveChangesAsync();

                        Console.WriteLine($"Successfully seeded {procedures.Count} procedures into the database!");
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Could not find snomed_procedures_dataset.csv in the project root.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to seed procedures: {ex.Message}");
                }
            }
        }
    }
}