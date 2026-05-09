using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.AuthDTOs;

namespace Helix.Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth Mappings: RegisterDto -> AppUser
            CreateMap<RegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address , opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Identity will set this
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore());

            // Auth Mappings: AppUser -> UserDto (for safe data transfer)
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<RegisterUserDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Identity will set this
                .ForMember(dest => dest.MiddleName, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore());

            // Patient Mappings
            CreateMap<Patient, Helix.Service.DTOs.PatientDTOs.PatientDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Id : null))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.FirstName : null))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.MiddleName : null))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.LastName : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Email : null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.PhoneNumber : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Gender : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Address : null))
                .ForMember(dest => dest.DataOfBrith, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.DataOfBrith : default));

            CreateMap<Helix.Service.DTOs.PatientDTOs.CreatePatientDto, Patient>();
            CreateMap<Helix.Service.DTOs.PatientDTOs.UpdatePatientDto, Patient>();

            // Doctor Mappings
            CreateMap<Doctor, Helix.Service.DTOs.DoctorDTOs.DoctorDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Id : null))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.FirstName : null))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.MiddleName : null))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.LastName : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Email : null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.PhoneNumber : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Gender : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Address : null))
                .ForMember(dest => dest.DataOfBrith, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.DataOfBrith : default));

            CreateMap<Helix.Service.DTOs.DoctorDTOs.CreateDoctorDto, Doctor>();
            CreateMap<Helix.Service.DTOs.DoctorDTOs.UpdateDoctorDto, Doctor>();

            // Consent Mappings
            CreateMap<Consent, Helix.Service.DTOs.ConsentDTOs.ConsentDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Id : default(System.Guid)))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Id : default(System.Guid)));

            CreateMap<Helix.Service.DTOs.ConsentDTOs.CreateConsentDto, Consent>()
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.ConsentDTOs.UpdateConsentDto, Consent>();
        }
    }
}

