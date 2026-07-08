using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Infrastructure.Context;
using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using Helix.Service.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Helix.Service.Services.DrugDataService
{
    public class DrugDataService(IWebHostEnvironment env,IHttpClientFactory httpClientFactory,IUnitOfWork unitOfWork,
        IMemoryCache cache) : IDrugDataService
    {
        private const string DrugCacheKey = "DrugListCache";
        private const string IpCacheKey = "PythonApiIpCache";
        private const string DefaultServerIp = "helix.ai.ddi";

        // ==========================================
        // CACHE MANAGEMENT (The Fix)
        // ==========================================

        private async Task<List<DrugDTO>> GetDrugsFromCacheAsync()
        {
            // This guarantees the file is read from the hard drive EXACTLY ONCE.
            // After the first read, it serves the list instantly from RAM.
            return await cache.GetOrCreateAsync(DrugCacheKey, async entry =>
            {
                // Keep it in memory forever (or until the app restarts)
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365);

                var drugs = new List<DrugDTO>();
                var filePath = Path.Combine(env.ContentRootPath, "wwwroot", "DrugList.txt");

                if (File.Exists(filePath))
                {
                    // Asynchronous file reading! Doesn't block the thread.
                    var lines = await File.ReadAllLinesAsync(filePath);
                    int count = 0;
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            drugs.Add(new DrugDTO { Id = count, Name = line.Trim() });
                            count++;
                        }
                    }
                }
                return drugs;
            });
        }

        // ==========================================
        // AI INTEGRATION
        // ==========================================
        public async Task<InteractionResponseDTO?> CheckDrugInteractionAsync(InteractionRequestDTO dto)
        {
            // 1. Fetch from cache
            var drugs = await GetDrugsFromCacheAsync();
            var drug1 = drugs.FirstOrDefault(d => d.Id == dto.IdDrug1);
            var drug2 = drugs.FirstOrDefault(d => d.Id == dto.IdDrug2);

            // 2. Validate IDs specifically before passing to the name method
            if (drug1 == null || drug2 == null)
                throw new ArgumentException("One or both Drug IDs are invalid or not found in the catalog.");

            // 3. Delegate to the core logic
            return await CheckDrugInteractionByNameAsync(drug1.Name, drug2.Name);
        }

        public async Task<InteractionResponseDTO?> CheckDrugInteractionByNameAsync(string drug1, string drug2)
        {
            // 1. Use IsNullOrWhiteSpace to catch empty strings ("") as well as nulls
            if (string.IsNullOrWhiteSpace(drug1) || string.IsNullOrWhiteSpace(drug2))
                throw new ArgumentException("Invalid Drug Name provided.");

            // 2. Prepare API Call
            var serverIp = DefaultServerIp;
            var pythonApiUrl = $"http://{serverIp}:8000/predict";
            var payload = new { drug_a = drug1, drug_b = drug2 };

            // Assuming httpClientFactory is injected at the class level
            var client = httpClientFactory.CreateClient();

            try
            {
                // 3. Post to AI Service
                var response = await client.PostAsJsonAsync(pythonApiUrl, payload);
                response.EnsureSuccessStatusCode();

                var rawResult = await response.Content.ReadFromJsonAsync<RawAiResponseDTO>();
                if (rawResult == null || rawResult.Status != "success")
                    return null;

                // 4. Parse AI Logic (Result string: "The metabolism decrease (Confidence: 87.2%)")
                bool isInteraction = !rawResult.Result.Contains("No interaction detected", StringComparison.OrdinalIgnoreCase);

                double confidence = 0.0;
                var match = Regex.Match(rawResult.Result, @"Confidence:\s*(\d+(\.\d+)?)%");
                if (match.Success)
                {
                    double.TryParse(match.Groups[1].Value, out confidence);
                }

                // 5. Return Clean DTO
                return new InteractionResponseDTO
                {
                    DrugA = rawResult.DrugA,
                    DrugB = rawResult.DrugB,
                    IsInteraction = isInteraction,
                    Confidence = confidence,
                    Message = rawResult.Result
                };
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"AI Server unavailable at {pythonApiUrl}: {ex.Message}");
            }
        }
        // ==========================================
        // INTERFACE IMPLEMENTATIONS
        // ==========================================
        public async Task<List<InteractionResponseDTO>?> CheckPatientDrugInteractionsAsync(Guid patientId, List<string> newDrugs)
        {
            // 1. Clean the input to avoid duplicate checks
            var distinctNewDrugs = newDrugs.Where(d => !string.IsNullOrWhiteSpace(d)).Distinct().ToList();
            var interactionTasks = new List<Task<InteractionResponseDTO?>>();

            // 2. Fetch the patient's current active medications from the database
            var currentDrugs = await GetCurrentPatientMedicationsAsync(patientId);

            // 3. COMBINATION A: Check interactions BETWEEN the NEW drugs
            for (int i = 0; i < distinctNewDrugs.Count; i++)
            {
                for (int j = i + 1; j < distinctNewDrugs.Count; j++)
                {
                    interactionTasks.Add(CheckDrugInteractionByNameAsync(distinctNewDrugs[i], distinctNewDrugs[j]));
                }
            }

            // 4. COMBINATION B: Check interactions between NEW drugs and CURRENT drugs
            foreach (var newDrug in distinctNewDrugs)
            {
                foreach (var currentDrug in currentDrugs)
                {
                    if (newDrug.Equals(currentDrug, StringComparison.OrdinalIgnoreCase))
                        continue;

                    interactionTasks.Add(CheckDrugInteractionByNameAsync(newDrug, currentDrug));
                }
            }

            // Guard clause: If no interaction checks are needed, exit early
            if (!interactionTasks.Any()) return null;

            try
            {
                // 5. Execute all AI checks concurrently
                // If the Python server is offline, the internal tasks will throw,
                // and Task.WhenAll will immediately intercept the failure.
                var allResults = await Task.WhenAll(interactionTasks);

                // 6. Filter out the safe combinations (nulls) and keep ONLY the active interactions
                var activeInteractions = allResults
                    .Where(result => result != null && result.IsInteraction)
                    .ToList();

                if (!activeInteractions.Any())
                {
                    return null;
                }

                return activeInteractions!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The drug-drug interaction verification service is temporarily unavailable. Safety checks could not be completed.", ex);
            }
        }
        public async Task<List<DrugDTO>> ImportDrugDataAsync()
        {
            return await GetDrugsFromCacheAsync();
        }

        public async Task<bool> IsValidDrug(int drugId)
        {
            var drugs = await GetDrugsFromCacheAsync();
            return drugs.Any(d => d.Id == drugId);
        }
        public async Task<int?> GetIdAsync(string drugName)
        {
            var drugs = await GetDrugsFromCacheAsync();
            return drugs.FirstOrDefault(d => d.Name == drugName)?.Id;
        }

        public async Task<string?> GetDrugName(int drugId)
        {
            var drugs = await GetDrugsFromCacheAsync();
            return drugs.FirstOrDefault(d => d.Id == drugId)?.Name;
        }

        private async Task<List<string>> GetCurrentPatientMedicationsAsync(Guid patientId)
        {
            var currentMeds = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.PatientId == patientId && p.Status == EnPrescriptionStatus.Active))
                .Include(p => p.Items)
                .ThenInclude(i => i.MedicationCatalog)
                .SelectMany(p => p.Items.Select(m => m.MedicationCatalog.AiModelName))
                .Distinct()
                .ToListAsync();
            var patient = await(await unitOfWork.Repository<Patient>().FindAsQueryable(p => p.Id == patientId))
                .Include(p => p.Medications)
                .ThenInclude(m => m.medicationCatalog)
                .FirstOrDefaultAsync();
            var patientMeds = patient?.Medications.Select(m => m.medicationCatalog.AiModelName).ToList() ?? new List<string>();
            currentMeds.AddRange(patientMeds);
            return currentMeds;
        }
    }
}