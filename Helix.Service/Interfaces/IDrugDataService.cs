using Helix.Service.DTOs.DrugDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Interfaces
{
    public interface IDrugDataService
    {
        Task<List<DrugDTO>> ImportDrugDataAsync();
        Task<InteractionResponseDTO> CheckDrugInteractionAsync(InteractionRequestDTO dto);
        Task<bool> IsValidDrug(int drugId);
        Task<string> GetDrugName(int drugId);
        Task<int?> GetIdAsync(string drugName);
    }
}
