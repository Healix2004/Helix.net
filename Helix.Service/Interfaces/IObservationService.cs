using Helix.Service.DTOs.ObservationDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IObservationService
    {
        Task<ObservationDto> GetObservationByIdAsync(Guid id);
        Task<IEnumerable<ObservationDto>> GetAllObservationsAsync();
        Task<ObservationDto> CreateObservationAsync(CreateObservationDto createObservationDto);
        Task<ObservationDto> UpdateObservationAsync(Guid id, UpdateObservationDto updateObservationDto);
        Task<bool> DeleteObservationAsync(Guid id);
    }
}
