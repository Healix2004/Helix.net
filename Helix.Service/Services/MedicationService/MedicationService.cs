using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.MedicationDTOs;
using Helix.Service.Interfaces;

namespace Helix.Service.Services.MedicationService
{
    public class MedicationService(IUnitOfWork unitOfWork, IMapper mapper) : IMedicationService
    {
        public async Task<MedicationDto> CreateMedicationAsync(CreateMedicationDto createMedicationDto)
        {
            // 1. Optimize: Use GetByIdAsync for fast memory-cache lookups
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(createMedicationDto.PatientId);
            if (patient == null)
                throw new Exception($"Patient with ID {createMedicationDto.PatientId} not found.");

            var terminology = await unitOfWork.Repository<TerminologyCodeLookup>().GetByIdAsync(createMedicationDto.TerminologyCodeId);
            if (terminology == null)
                throw new Exception($"Terminology Code with ID {createMedicationDto.TerminologyCodeId} not found.");

            var medication = mapper.Map<Medication>(createMedicationDto);

            // 2. Optimize: Set Foreign Keys directly to avoid unnecessary EF Core object tracking
            medication.PatientId = patient.Id;

            await unitOfWork.Repository<Medication>().AddAsync(medication);
            await unitOfWork.CompleteAsync(); // Asynchronous database commit

            return mapper.Map<MedicationDto>(medication);
        }

        public async Task<bool> DeleteMedicationAsync(Guid id)
        {
            var medication = await unitOfWork.Repository<Medication>().GetByIdAsync(id);
            if (medication == null) return false;

            await unitOfWork.Repository<Medication>().DeleteAsync(medication);
            await unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync()
        {
            // 3. True async execution replacing .Result
            var medications = await unitOfWork.Repository<Medication>().GetAllAsync();
            return mapper.Map<IEnumerable<MedicationDto>>(medications);
        }

        public async Task<IEnumerable<MedicationDto>> GetPatientMedicationsAsync(Guid patientId)
        {
            // 4. THE FIX: Changed 'm => m.Id' to 'm => m.PatientId'
            var medications = await unitOfWork.Repository<Medication>().FindAsync(m => m.PatientId == patientId);
            return mapper.Map<IEnumerable<MedicationDto>>(medications);
        }

        public async Task<MedicationDto?> GetMedicationByIdAsync(Guid id)
        {
            var medication = await unitOfWork.Repository<Medication>().GetByIdAsync(id);
            return medication == null ? null : mapper.Map<MedicationDto>(medication);
        }

        public async Task<MedicationDto> UpdateMedicationAsync(Guid id, UpdateMedicationDto updateMedicationDto)
        {
            var medication = await unitOfWork.Repository<Medication>().GetByIdAsync(id);
            if (medication == null)
                throw new Exception($"Medication with ID {id} not found.");

            mapper.Map(updateMedicationDto, medication);

            await unitOfWork.Repository<Medication>().UpdateAsync(medication);
            await unitOfWork.CompleteAsync();

            return mapper.Map<MedicationDto>(medication);
        }
    }
}