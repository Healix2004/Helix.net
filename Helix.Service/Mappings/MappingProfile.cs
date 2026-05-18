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
            CreateMap<Doctor, DTOs.DoctorDTOs.DoctorDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Id : null))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.FirstName : null))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.MiddleName : null))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.LastName : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Email : null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.PhoneNumber : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Gender : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Address : null))
                .ForMember(dest => dest.Bio , opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.DataOfBrith, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.DataOfBrith : default));

            CreateMap<DTOs.DoctorDTOs.UpdateDoctorDto, Doctor>().ReverseMap();

            // Consent Mappings
            CreateMap<Consent, Helix.Service.DTOs.ConsentDTOs.ConsentDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId));

            CreateMap<Helix.Service.DTOs.ConsentDTOs.CreateConsentDto, Consent>()
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.ConsentDTOs.UpdateConsentDto, Consent>();

            // Allergy Mappings
            CreateMap<Allergy, Helix.Service.DTOs.AllergyDTOs.AllergyDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Id : default(System.Guid)));
            CreateMap<Helix.Service.DTOs.AllergyDTOs.CreateAllergyDto, Allergy>()
                .ForMember(dest => dest.Patient, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.AllergyDTOs.UpdateAllergyDto, Allergy>();

            // Diagnose Mappings
            CreateMap<Diagnose, Helix.Service.DTOs.DiagnoseDTOs.DiagnoseDto>()
                .ForMember(dest => dest.TerminologyCodeId, opt => opt.MapFrom(src => src.TerminologyCode != null ? src.TerminologyCode.Id : default(System.Guid)))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Id : default(System.Guid)))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.doctor != null ? src.doctor.Id : default(System.Guid)));
            CreateMap<Helix.Service.DTOs.DiagnoseDTOs.CreateDiagnoseDto, Diagnose>()
                .ForMember(dest => dest.TerminologyCode, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.doctor, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.DiagnoseDTOs.UpdateDiagnoseDto, Diagnose>();

            // Encounter Mappings
            CreateMap<Encounter, Helix.Service.DTOs.EncounterDTOs.EncounterDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.patient != null ? src.patient.Id : default(System.Guid)))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Id : default(System.Guid)));
            CreateMap<Helix.Service.DTOs.EncounterDTOs.CreateEncounterDto, Encounter>()
                .ForMember(dest => dest.patient, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.EncounterDTOs.UpdateEncounterDto, Encounter>();

            // Facilitie Mappings
            CreateMap<Facilitie, Helix.Service.DTOs.FacilitieDTOs.FacilitieDto>();
            CreateMap<Helix.Service.DTOs.FacilitieDTOs.CreateFacilitieDto, Facilitie>();
            CreateMap<Helix.Service.DTOs.FacilitieDTOs.UpdateFacilitieDto, Facilitie>();

            // LabTestResult Mappings
            CreateMap<LabTestResult, Helix.Service.DTOs.LabTestResultDTOs.LabTestResultDto>()
                .ForMember(dest => dest.TerminologyName, opt => opt.MapFrom(src => src.TerminologyCode != null ? src.TerminologyCode.Display : ""))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? $"{src.Patient.AppUser.FirstName} {src.Patient.AppUser.LastName}" : ""))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.ResultDate))
                .ForMember(dest => dest.status,opt => opt.MapFrom(src => src.Status.ToString()) );

            CreateMap<Helix.Service.DTOs.LabTestResultDTOs.CreateLabTestResultDto, LabTestResult>()
                .ForMember(dest => dest.TerminologyCode, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Encounter, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.LabTestResultDTOs.UpdateLabTestResultDto, LabTestResult>();

            // Medication Mappings
            CreateMap<Medication, Helix.Service.DTOs.MedicationDTOs.MedicationDto>()
                .ForMember(dest => dest.TerminologyCodeId, opt => opt.MapFrom(src => src.TerminologyCode != null ? src.TerminologyCode.Id : default(System.Guid)))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Id : default(System.Guid)));
            CreateMap<Helix.Service.DTOs.MedicationDTOs.CreateMedicationDto, Medication>()
                .ForMember(dest => dest.TerminologyCode, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.MedicationDTOs.UpdateMedicationDto, Medication>();

            // Observation Mappings
            CreateMap<Observation, Helix.Service.DTOs.ObservationDTOs.ObservationDto>()
                .ForMember(dest => dest.EncounterId, opt => opt.MapFrom(src => src.Encounter != null ? src.Encounter.Id : default(System.Guid)));
            CreateMap<Helix.Service.DTOs.ObservationDTOs.CreateObservationDto, Observation>()
                .ForMember(dest => dest.Encounter, opt => opt.Ignore());
            CreateMap<Helix.Service.DTOs.ObservationDTOs.UpdateObservationDto, Observation>();

            // TerminologyCodeLookup Mappings
            CreateMap<TerminologyCodeLookup, Helix.Service.DTOs.TerminologyCodeLookupDTOs.TerminologyCodeLookupDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Display, opt => opt.MapFrom(src => src.Display))
                .ForMember(dest => dest.SystemUrl, opt => opt.MapFrom(src => src.SystemUrl))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code)).ReverseMap();

            CreateMap<Helix.Service.DTOs.TerminologyCodeLookupDTOs.CreateTerminologyCodeLookupDto, TerminologyCodeLookup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Display, opt => opt.MapFrom(src => src.Display))
                .ForMember(dest => dest.SystemUrl, opt => opt.MapFrom(src => src.SystemUrl))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.TerminologyType, opt => opt.MapFrom(src => src.TerminologyType));
            CreateMap<Helix.Service.DTOs.TerminologyCodeLookupDTOs.UpdateTerminologyCodeLookupDto, TerminologyCodeLookup>();
        }
    }
}

