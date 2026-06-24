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
using Helix.Service.Helper;

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

            CreateMap<AppUser, UserDto>();

            CreateMap<RegisterUserDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.FullName, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
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

            // Read (Entity -> Dto)
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.AppUser.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AppUser.Address))
                .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType))
                .ForMember(dest => dest.PatientCategory, opt => opt.MapFrom(src => src.PatientCategory))
                .ForMember(dest => dest.EgyptianNationalId, opt => opt.MapFrom(src => src.AppUser.NationalId));

            // Write (Create/Update Dto -> Entity)
            CreateMap<CreatePatientDto, Patient>()
                .ForMember(dest => dest.ChronicDiseases, opt => opt.MapFrom(src => src.ChronicDiseases))
                .ForMember(dest => dest.Surgeries, opt => opt.MapFrom(src => src.Surgeries))
                .ForMember(dest => dest.EmergencyContacts, opt => opt.MapFrom(src => src.EmergencyContacts))
                .ForMember(dest => dest.Allergies, opt => opt.MapFrom(src => src.Allergies))
                .ForMember(dest => dest.Medications, opt => opt.MapFrom(src => src.CurrentMedications));

            CreateMap<UpdatePatientDto, Patient>();

            // --- Nested Patient Object Mappings (Read) ---
            CreateMap<EmergencyContact, EmergencyContactDto>();
            CreateMap<Insurance, InsuranceDto>();
            CreateMap<ChronicDisease, ChronicDiseaseDto>();

            // Flattens the SNOMED procedure name
            CreateMap<Surgery, SurgeryDto>()
                .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.ProcedureCatalog.DisplayName));

            // --- Nested Patient Object Mappings (Write - Fixes the 500 error!) ---
            CreateMap<CreateInsuranceDto, Insurance>();
            CreateMap<CreateEmergencyContactDto, EmergencyContact>();
            CreateMap<CreatePatientChronicDiseaseDto, ChronicDisease>();
            CreateMap<CreateSurgeryDto, Surgery>();

            // ==========================================
            // Doctor Mappings
            // ==========================================

            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AppUser.Address));

            CreateMap<CreateDoctorDto, Doctor>();
            CreateMap<UpdateDoctorDto, Doctor>();

            // ==========================================
            // Medication Mappings (Consolidated)
            // ==========================================

            CreateMap<Medication, MedicationDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                // Flattens the RxNorm Drug Name for the frontend
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.medicationCatalog.DrugName));

            CreateMap<CreatePatientMedicationDto, Medication>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.medicationCatalog, opt => opt.Ignore());

            CreateMap<CreateMedicationDto, Medication>()
                .ForMember(dest => dest.medicationCatalog, opt => opt.Ignore());

            CreateMap<UpdateMedicationDto, Medication>();

            // ==========================================
            // Clinical Domain Mappings
            // ==========================================

            // Consent Mappings
            CreateMap<Consent, ConsentDto>();
            CreateMap<CreateConsentDto, Consent>()
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<UpdateConsentDto, Consent>();

            // Allergy Mappings
            CreateMap<Allergy, AllergyDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.AllergenName, opt => opt.MapFrom(src => src.AllergenCatalog.DisplayName));

            CreateMap<CreateAllergyDto, Allergy>()
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.AllergenCatalog, opt => opt.Ignore());
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


            CreateMap<CreateEncounterDto, Encounter>()
                .ForMember(dest => dest.patient, opt => opt.Ignore());
            CreateMap<UpdateEncounterDto, Encounter>();

            // Facility Mappings
            CreateMap<Facilitie, FacilitieDto>();
            CreateMap<CreateFacilitieDto, Facilitie>();
            CreateMap<UpdateFacilitieDto, Facilitie>();

            // LabTestResult Mappings
            CreateMap<LabTestResult, LabTestResultDto>()
                .ForMember(dest => dest.TerminologyName, opt => opt.MapFrom(src => src.TerminologyCode.Display))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.AppUser.FullName))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.ResultDate))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateLabTestResultDto, LabTestResult>()
                .ForMember(dest => dest.TerminologyCode, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Encounter, opt => opt.Ignore());
            CreateMap<UpdateLabTestResultDto, LabTestResult>();

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