using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.FacilitieDTOs;
using Helix.Service.Interfaces;

namespace Helix.Service.Services.FacilitieService
{
    public class FacilitieService(IUnitOfWork unitOfWork, IMapper mapper) : IFacilitieService
    {
        public async Task<FacilitieDto> CreateFacilitieAsync(CreateFacilitieDto createFacilitieDto)
        {
            var facilitie = mapper.Map<Facilitie>(createFacilitieDto);

            await unitOfWork.Repository<Facilitie>().AddAsync(facilitie);
            await unitOfWork.CompleteAsync(); // Using the async commit method!

            return mapper.Map<FacilitieDto>(facilitie);
        }

        public async Task<bool> DeleteFacilitieAsync(Guid id)
        {
            // 1. Memory-optimized lookup using GetByIdAsync instead of Find().FirstOrDefault()
            var facilitie = await unitOfWork.Repository<Facilitie>().GetByIdAsync(id);

            if (facilitie == null)
                return false;

            await unitOfWork.Repository<Facilitie>().DeleteAsync(facilitie);
            await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<FacilitieDto>> GetAllFacilitiesAsync()
        {
            // 2. True asynchronous execution instead of .Result
            var facilities = await unitOfWork.Repository<Facilitie>().GetAllAsync();
            return mapper.Map<IEnumerable<FacilitieDto>>(facilities);
        }

        public async Task<FacilitieDto?> GetFacilitieByIdAsync(Guid id)
        {
            var facilitie = await unitOfWork.Repository<Facilitie>().GetByIdAsync(id);

            return facilitie == null ? null : mapper.Map<FacilitieDto>(facilitie);
        }

        public async Task<FacilitieDto> UpdateFacilitieAsync(Guid id, UpdateFacilitieDto updateFacilitieDto)
        {
            var facilitie = await unitOfWork.Repository<Facilitie>().GetByIdAsync(id);

            if (facilitie == null)
                throw new Exception($"Facility with ID {id} not found.");

            // Maps the new values from the DTO directly onto the tracked DB entity
            mapper.Map(updateFacilitieDto, facilitie);

            await unitOfWork.Repository<Facilitie>().UpdateAsync(facilitie);
            await unitOfWork.CompleteAsync();

            return mapper.Map<FacilitieDto>(facilitie);
        }
    }
}