using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.DoctorService
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            var user = await _userManager.FindByIdAsync(createDoctorDto.AppUserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var doctor = _mapper.Map<Doctor>(createDoctorDto);
            doctor.AppUser = user;
            
            _unitOfWork.Repository<Doctor>().Add(doctor);
            _unitOfWork.Complete();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public Task<bool> DeleteDoctorAsync(Guid id)
        {
            var doctor = _unitOfWork.Repository<Doctor>().Find(d => d.Id == id).FirstOrDefault();
            if (doctor == null)
            {
                return Task.FromResult(false);
            }

            _unitOfWork.Repository<Doctor>().Delete(doctor);
            _unitOfWork.Complete();

            return Task.FromResult(true);
        }

        public Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = _unitOfWork.Repository<Doctor>().GetALL();
            var doctorsDto = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return Task.FromResult(doctorsDto);
        }

        public Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            var doctor = _unitOfWork.Repository<Doctor>().Find(d => d.Id == id).FirstOrDefault();
            if (doctor == null)
            {
                return Task.FromResult<DoctorDto>(null);
            }

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return Task.FromResult(doctorDto);
        }

        public Task<DoctorDto> UpdateDoctorAsync(Guid id, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = _unitOfWork.Repository<Doctor>().Find(d => d.Id == id).FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor not found");
            }

            _mapper.Map(updateDoctorDto, doctor);
            
            _unitOfWork.Repository<Doctor>().Update(doctor);
            _unitOfWork.Complete();

            return Task.FromResult(_mapper.Map<DoctorDto>(doctor));
        }
    }
}
