using Helix.Service.DTOs.DoctorDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorDto> GetDoctorByIdAsync(Guid id);
        Task<DoctorDto> GetDoctorByUserIdAsync(string userId);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto);
        Task<DoctorDto> UpdateDoctorAsync(Guid id, UpdateDoctorDto updateDoctorDto);
        Task<bool> DeleteDoctorAsync(Guid id);
    }
}
