using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.Terminology;
using Helix.Service.Services.NotificationService;

namespace Helix.Service.Interfaces
{
    public interface ITerminologyService
    {
        // Checks if a code is valid within a specific system (e.g., is 'E11.9' valid in ICD-10?)
        Task<bool> ValidateCodeAsync(string systemUri, string code);

        // Searches for codes based on text (e.g., user types "diab")
        Task<List<CodingDto>> LookupCodesAsync(string filterText, string systemUri);

        // Returns the official display name for a raw code
        Task<string> GetDisplayForCodeAsync(string systemUri, string code);
    }
}
