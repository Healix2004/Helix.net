using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

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
            var drugs = await GetDrugsFromCacheAsync();

            var drug1 = drugs.FirstOrDefault(d => d.Id == dto.IdDrug1);
            var drug2 = drugs.FirstOrDefault(d => d.Id == dto.IdDrug2);

            if (drug1 == null || drug2 == null)
            {
                return null; // Or throw a specific custom exception
            }

            var serverIp = await GetServerIP();
            var pythonApiUrl = $"http://{serverIp}:8000/predict";

            var payload = new
            {
                drug_a = drug1.Name,
                drug_b = drug2.Name
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync(pythonApiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<InteractionResponseDTO>(responseContent, options);
            }
            catch (HttpRequestException ex)
            {
                // Better error logging indicating a network issue
                throw new Exception($"Network error connecting to Python AI Server at {pythonApiUrl}: {ex.Message}");
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