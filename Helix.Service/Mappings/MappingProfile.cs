using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.ConsentDTOs;
using Helix.Service.DTOs.DiagnoseDTOs;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.DTOs.EncounterDTOs;
using Helix.Service.DTOs.FacilitieDTOs;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.DTOs.MedicationDTOs;
using Helix.Service.DTOs.ObservationDTOs;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;

namespace Helix.Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ==========================================
            // Auth & Identity Mappings
            // ==========================================

            CreateMap<RegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                // Note: FirstName, LastName, Email, etc., are mapped automatically by AutoMapper by name convention!
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

            // AutoMapper perfectly maps matching properties here without explicit ForMember calls
            CreateMap<AppUser, UserDto>();

            CreateMap<RegisterUserDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
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

            // ==========================================
            // Patient Mappings
            // ==========================================

            CreateMap<Patient, PatientDto>()
                // AutoMapper is null-safe natively! No need for: src.AppUser != null ? ... : null
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.AppUser.MiddleName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AppUser.Address));

            CreateMap<CreatePatientDto, Patient>();
            CreateMap<UpdatePatientDto, Patient>();

            // ==========================================
            // Doctor Mappings
            // ==========================================

            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.AppUser.MiddleName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AppUser.Address));
            // Bio maps automatically by name convention

            CreateMap<UpdateDoctorDto, Doctor>().ReverseMap();

            // ==========================================
            // Clinical Domain Mappings
            // ==========================================

            // Consent Mappings
            CreateMap<Consent, ConsentDto>(); // PatientId and DoctorId map automatically
            CreateMap<CreateConsentDto, Consent>()
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<UpdateConsentDto, Consent>();

            // Allergy Mappings
            CreateMap<Allergy, AllergyDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id));
            CreateMap<CreateAllergyDto, Allergy>()
                .ForMember(dest => dest.Patient, opt => opt.Ignore());
            CreateMap<UpdateAllergyDto, Allergy>();

            // Diagnose Mappings
            CreateMap<Diagnose, DiagnoseDto>()
                .ForMember(dest => dest.TerminologyCodeId, opt => opt.MapFrom(src => src.TerminologyCode.Id))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.doctor.Id));

            CreateMap<CreateDiagnoseDto, Diagnose>()
                .ForMember(dest => dest.TerminologyCode, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.doctor, opt => opt.Ignore());
            CreateMap<UpdateDiagnoseDto, Diagnose>();

            // Encounter Mappings
            CreateMap<Encounter, EncounterDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.patient.Id))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor.Id));

            CreateMap<CreateEncounterDto, Encounter>()
                .ForMember(dest => dest.patient, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<UpdateEncounterDto, Encounter>();

            // Facility Mappings
            CreateMap<Facilitie, FacilitieDto>();
            CreateMap<CreateFacilitieDto, Facilitie>();
            CreateMap<UpdateFacilitieDto, Facilitie>();

            // LabTestResult Mappings
            CreateMap<LabTestResult, LabTestResultDto>()
                .ForMember(dest => dest.TerminologyName, opt => opt.MapFrom(src => src.TerminologyCode.Display))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.AppUser.FirstName} {src.Patient.AppUser.LastName}"))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.ResultDate))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateLabTestResultDto, LabTestResult>()
                .ForMember(dest => dest.TerminologyCode, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Encounter, opt => opt.Ignore());
            CreateMap<UpdateLabTestResultDto, LabTestResult>();

            // Medication Mappings
            CreateMap<Medication, MedicationDto>()
                .ForMember(dest => dest.TerminologyCodeId, opt => opt.MapFrom(src => src.TerminologyCode.Id))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id));

            CreateMap<CreateMedicationDto, Medication>()
                .ForMember(dest => dest.TerminologyCode, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore());
            CreateMap<UpdateMedicationDto, Medication>();

            // Observation Mappings
            CreateMap<Observation, ObservationDto>()
                .ForMember(dest => dest.EncounterId, opt => opt.MapFrom(src => src.Encounter.Id));

            CreateMap<CreateObservationDto, Observation>()
                .ForMember(dest => dest.Encounter, opt => opt.Ignore());
            CreateMap<UpdateObservationDto, Observation>();

            // TerminologyCodeLookup Mappings
            CreateMap<TerminologyCodeLookup, TerminologyCodeLookupDto>().ReverseMap();

            CreateMap<CreateTerminologyCodeLookupDto, TerminologyCodeLookup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateTerminologyCodeLookupDto, TerminologyCodeLookup>();
        }
    }
}