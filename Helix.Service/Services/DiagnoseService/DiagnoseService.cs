using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.DiagnoseDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.Service.Services.DiagnoseService
{
    public class DiagnoseService(IUnitOfWork unitOfWork, IMapper mapper) : IDiagnoseService
    {
        public async Task<DiagnoseDto> CreateDiagnoseAsync(CreateDiagnoseDto createDiagnoseDto)
        {
            // 1. Optimize: Use GetByIdAsync for faster validation
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(createDiagnoseDto.PatientId);
            if (patient == null)
                throw new Exception($"Patient with ID {createDiagnoseDto.PatientId} not found.");

            var doctor = await unitOfWork.Repository<Doctor>().GetByIdAsync(createDiagnoseDto.DoctorId);
            if (doctor == null)
                throw new Exception($"Doctor with ID {createDiagnoseDto.DoctorId} not found.");

            // Assuming TerminologyCode uses an 'int' ID based on previous controllers, adjust if it uses Guid
            var terminology = await unitOfWork.Repository<TerminologyCodeLookup>().GetByIdAsync(createDiagnoseDto.TerminologyCodeId);
            if (terminology == null)
                throw new Exception($"Terminology Code with ID {createDiagnoseDto.TerminologyCodeId} not found.");

            var diagnose = mapper.Map<Diagnose>(createDiagnoseDto);

            diagnose.PatientId = patient.Id;
            diagnose.DoctorId = doctor.Id;
            diagnose.TerminologyCodeLookupId = terminology.Id;

            await unitOfWork.Repository<Diagnose>().AddAsync(diagnose);
            await unitOfWork.CompleteAsync(); // Ensure we use the async Complete method we built!

            return mapper.Map<DiagnoseDto>(diagnose);
        }

        public async Task<bool> DeleteDiagnoseAsync(Guid id)
        {
            var diagnose = await unitOfWork.Repository<Diagnose>().GetByIdAsync(id);

            if (diagnose == null)
                return false;

            await unitOfWork.Repository<Diagnose>().DeleteAsync(diagnose);
            await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<DiagnoseDto>> GetAllDiagnosesAsync()
        {
            // 3. True async await instead of .Result and Task.FromResult
            var diagnoses = await unitOfWork.Repository<Diagnose>().GetAllAsync();
            return mapper.Map<IEnumerable<DiagnoseDto>>(diagnoses);
        }

        public async Task<DiagnoseDto?> GetDiagnoseByIdAsync(Guid id)
        {
            var diagnose = await unitOfWork.Repository<Diagnose>().GetByIdAsync(id);

            return diagnose == null ? null : mapper.Map<DiagnoseDto>(diagnose);
        }

        public async Task<IEnumerable<DiagnoseDto>> GetPatientDiagnosesAsync(Guid patientId)
        {
            var diagnoses = await unitOfWork.Repository<Diagnose>().FindAsync(d => d.PatientId == patientId);
            return mapper.Map<IEnumerable<DiagnoseDto>>(diagnoses);
        }

        public async Task<DiagnoseDto> UpdateDiagnoseAsync(Guid id, UpdateDiagnoseDto updateDiagnoseDto)
        {
            var diagnose = await unitOfWork.Repository<Diagnose>().GetByIdAsync(id);

            if (diagnose == null)
                throw new Exception($"Diagnose with ID {id} not found.");

            // Maps the new values from the DTO directly onto the tracked DB entity
            mapper.Map(updateDiagnoseDto, diagnose);

            await unitOfWork.Repository<Diagnose>().UpdateAsync(diagnose);
            await unitOfWork.CompleteAsync();

            return mapper.Map<DiagnoseDto>(diagnose);
        }
    }
}