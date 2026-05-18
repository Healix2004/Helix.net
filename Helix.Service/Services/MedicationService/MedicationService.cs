using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.MedicationDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.MedicationService
{
    public class MedicationService : IMedicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MedicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<MedicationDto> CreateMedicationAsync(CreateMedicationDto createMedicationDto)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == createMedicationDto.PatientId).Result.FirstOrDefault();
            if (patient == null) throw new Exception("Patient not found");

            var terminology = _unitOfWork.Repository<TerminologyCodeLookup>().Find(t => t.Id == createMedicationDto.TerminologyCodeId).Result.FirstOrDefault();
            if (terminology == null) throw new Exception("Terminology Code not found");

            var medication = _mapper.Map<Medication>(createMedicationDto);
            medication.Patient = patient;
            medication.TerminologyCode = terminology;

            _unitOfWork.Repository<Medication>().Add(medication);
            _unitOfWork.Complete();

            var result = _mapper.Map<MedicationDto>(medication);
            result.PatientId = patient.Id;
            result.TerminologyCodeId = terminology.Id;
            return Task.FromResult(result);
        }

        public Task<bool> DeleteMedicationAsync(Guid id)
        {
            var medication = _unitOfWork.Repository<Medication>().Find(m => m.Id == id).Result.FirstOrDefault();
            if (medication == null) return Task.FromResult(false);

            _unitOfWork.Repository<Medication>().Delete(medication);
            _unitOfWork.Complete();
            return Task.FromResult(true);
        }
        public Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync()
        {
            var medications = _unitOfWork.Repository<Medication>().GetALL();
            return Task.FromResult(_mapper.Map<IEnumerable<MedicationDto>>(medications));
        }
        public Task<IEnumerable<MedicationDto>> GetPatientMedicationsAsync(Guid patientId)
        {
            var medications = _unitOfWork.Repository<Medication>().Find(m => m.Id == patientId).Result.ToList();
            return Task.FromResult(_mapper.Map<IEnumerable<MedicationDto>>(medications));
        }

        public Task<MedicationDto> GetMedicationByIdAsync(Guid id)
        {
            var medication = _unitOfWork.Repository<Medication>().Find(m => m.Id == id).Result.FirstOrDefault();
            return Task.FromResult(_mapper.Map<MedicationDto>(medication));
        }
        public Task<MedicationDto> UpdateMedicationAsync(Guid id, UpdateMedicationDto updateMedicationDto)
        {
            var medication = _unitOfWork.Repository<Medication>().Find(m => m.Id == id).Result.FirstOrDefault();
            if (medication == null) throw new Exception("Medication not found");

            _mapper.Map(updateMedicationDto, medication);
            _unitOfWork.Repository<Medication>().Update(medication);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<MedicationDto>(medication));
        }
    }
}
