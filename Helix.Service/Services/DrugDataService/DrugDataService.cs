using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO; // Needed for Path and File
using System.Linq; // Needed for FirstOrDefault
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helix.Service.Services.DrugDataService
{
    public class DrugDataService : IDrugDataService
    {
        private readonly List<DrugDTO> _drugs = new List<DrugDTO>();
        private readonly IHttpClientFactory _httpClientFactory;

        // Default IP to localhost if not set, preventing crashes
        private string _serverIP = "localhost";

        public DrugDataService(IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            // --- FIX 1: LOAD DATA IMMEDIATELY ---
            var filePath = Path.Combine(env.ContentRootPath, "wwwroot", "DrugList.txt");
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                int count = 0;
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        _drugs.Add(new DrugDTO
                        {
                            Id = count,
                            Name = line.Trim()
                        });
                        count++;
                    }
                }
            }
        }

        public async Task<InteractionResponseDTO> CheckDrugInteractionAsync(InteractionRequestDTO dto)
        {
            // --- FIX 2: AWAIT THE TASKS ---
            // Since we already have the data in memory, we don't strictly need async for validity checks,
            // but we keep it to match your Interface signature.
            bool isValid1 = await IsValidDrug(dto.IdDrug1);
            bool isValid2 = await IsValidDrug(dto.IdDrug2);

            if (isValid1 && isValid2)
            {
                // Retrieve names (Since data is in memory, we can grab it synchronously effectively)
                string drugName1 = _drugs.FirstOrDefault(d => d.Id == dto.IdDrug1)?.Name;
                string drugName2 = _drugs.FirstOrDefault(d => d.Id == dto.IdDrug2)?.Name;

                var pythonApiUrl = $"http://{_serverIP}:8000/predict";

                var payload = new
                {
                    drug_a = drugName1,
                    drug_b = drugName2
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient();

                try
                {
                    var response = await client.PostAsync(pythonApiUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Ensure case-insensitive deserialization usually helps with Python APIs
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<InteractionResponseDTO>(responseContent, options);

                    return result;
                }
                catch (Exception ex)
                {
                    // Log error here
                    throw new Exception($"Error connecting to AI Server at {pythonApiUrl}: {ex.Message}");
                }
            }

            return null; // Or throw an exception saying "Invalid Drug ID"
        }

        // We can keep this method if the Interface requires it, but it just returns the already loaded list.
        public Task<List<DrugDTO>> ImportDrugDataAsync()
        {
            return Task.FromResult(_drugs);
        }

        public Task<bool> IsValidDrug(int drugId)
        {
            return Task.FromResult(_drugs.Any(d => d.Id == drugId));
        }

        public Task<string> GetDrugName(int drugId)
        {
            var drug = _drugs.FirstOrDefault(d => d.Id == drugId);
            return Task.FromResult(drug?.Name);
        }

        public Task<string> GetServerIP()
        {
            return Task.FromResult(_serverIP);
        }

        public Task SetServerIP(string ip)
        {
            _serverIP = ip;
            return Task.CompletedTask;
        }
    }
}