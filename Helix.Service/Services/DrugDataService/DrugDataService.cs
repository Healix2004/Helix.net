using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Helix.Service.Services.DrugDataService
{
    public class DrugDataService(IWebHostEnvironment env,IHttpClientFactory httpClientFactory,
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
            // 1. Validate Drugs
            var drugs = await GetDrugsFromCacheAsync();
            var drug1 = drugs.FirstOrDefault(d => d.Id == dto.IdDrug1);
            var drug2 = drugs.FirstOrDefault(d => d.Id == dto.IdDrug2);

            if (drug1 == null || drug2 == null)
                throw new ArgumentException("Invalid Drug ID provided.");

            // 2. Prepare API Call
            var serverIp = await GetServerIP();
            var pythonApiUrl = $"http://{serverIp}:8000/predict";
            var payload = new { drug_a = drug1.Name, drug_b = drug2.Name };
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
                // Check for interaction status
                bool isInteraction = !rawResult.Result.Contains("No interaction detected", StringComparison.OrdinalIgnoreCase);

                // Extract confidence using Regex
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

        public async Task<List<DrugDTO>> ImportDrugDataAsync()
        {
            return await GetDrugsFromCacheAsync();
        }

        public async Task<bool> IsValidDrug(int drugId)
        {
            var drugs = await GetDrugsFromCacheAsync();
            return drugs.Any(d => d.Id == drugId);
        }

        public async Task<string?> GetDrugName(int drugId)
        {
            var drugs = await GetDrugsFromCacheAsync();
            return drugs.FirstOrDefault(d => d.Id == drugId)?.Name;
        }

        public Task<string> GetServerIP()
        {
            // Safely fetch the IP from cache, falling back to the default if it hasn't been changed
            var ip = cache.Get<string>(IpCacheKey) ?? DefaultServerIp;
            return Task.FromResult(ip);
        }

        public Task SetServerIP(string ip)
        {
            // Saving the IP in the global memory cache means the change applies instantly
            // to all future requests, regardless of the DI lifecycle!
            cache.Set(IpCacheKey, ip);
            return Task.CompletedTask;
        }
    }
}