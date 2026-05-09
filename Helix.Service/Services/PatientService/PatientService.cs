using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.PatientService
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            var user = await _userManager.FindByIdAsync(createPatientDto.AppUserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var patient = _mapper.Map<Patient>(createPatientDto);
            // patient.AppUser = user; // Depending on how EF handles it, sometimes just AppUserId is enough if exposed, but Patient only has virtual AppUser
            patient.AppUser = user;
            
            _unitOfWork.Repository<Patient>().Add(patient);
            _unitOfWork.Complete();

            return _mapper.Map<PatientDto>(patient);
        }

        public Task<bool> DeletePatientAsync(Guid id)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == id).FirstOrDefault();
            if (patient == null)
            {
                return Task.FromResult(false);
            }

            _unitOfWork.Repository<Patient>().Delete(patient);
            _unitOfWork.Complete();

            return Task.FromResult(true);
        }

        public Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = _unitOfWork.Repository<Patient>().GetALL();
            // Try to map. If lazy loading is enabled, AppUser will be loaded.
            var patientsDto = _mapper.Map<IEnumerable<PatientDto>>(patients);
            return Task.FromResult(patientsDto);
        }

        public Task<PatientDto> GetPatientByIdAsync(Guid id)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == id).FirstOrDefault();
            if (patient == null)
            {
                return Task.FromResult<PatientDto>(null);
            }

            var patientDto = _mapper.Map<PatientDto>(patient);
            return Task.FromResult(patientDto);
        }

        public Task<PatientDto> UpdatePatientAsync(Guid id, UpdatePatientDto updatePatientDto)
        {
            var patient = _unitOfWork.Repository<Patient>().Find(p => p.Id == id).FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient not found");
            }

            _mapper.Map(updatePatientDto, patient);
            
            _unitOfWork.Repository<Patient>().Update(patient);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<PatientDto>(patient));
        }
    }
}
