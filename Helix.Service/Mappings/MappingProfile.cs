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
using System.Linq;

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
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
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
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType.ToString()))
                .ForMember(dest => dest.PatientCategory, opt => opt.MapFrom(src => src.PatientCategory.ToString()));
            // THE FIX: Removed the .Select() chains. AutoMapper will automatically map the lists
            // of Surgeries, Allergies, Medications, and ChronicDiseases using the child mappings below!

            // Write (Create/Update Dto -> Entity)
            CreateMap<CreatePatientDto, Patient>();
            CreateMap<UpdatePatientDto, Patient>();

            // --- Nested Patient Object Mappings (Read) ---
            CreateMap<EmergencyContact, EmergencyContactDto>();
            CreateMap<Insurance, InsuranceDto>();

            // THE FIX: Move the catalog display name extraction here!
            // Note: Change 'DiseaseName' to match whatever string property is inside your ChronicDiseaseDto
            CreateMap<ChronicDisease, ChronicDiseaseDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ChronicDiseaseCatalog.DisplayName));

            // Flattens the SNOMED procedure name
            CreateMap<Surgery, SurgeryDto>()
                .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.ProcedureCatalog.DisplayName));

            // --- Nested Patient Object Mappings (Write) ---
            CreateMap<CreateInsuranceDto, Insurance>();
            CreateMap<CreateEmergencyContactDto, EmergencyContact>();
            CreateMap<CreatePatientChronicDiseaseDto, ChronicDisease>();
            CreateMap<CreateSurgeryDto, Surgery>();

            // ==========================================
            // Doctor Mappings
            // ==========================================

            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUser.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.SpecialtyCatalog.DisplayName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber));

            CreateMap<CreateDoctorDto, Doctor>()
                .ForMember(dest => dest.AvailabeDays, opt => opt.MapFrom(src => src.AvailabeDays))
                .ForMember(dest => dest.AvailableTimeSlots, opt => opt.MapFrom(src => src.AvailableTimeSlots));
            CreateMap<UpdateDoctorDto, Doctor>();

            CreateMap<CreateAvailableTimeSlotDto, AvailableTimeSlot>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));

            // ==========================================
            // Medication Mappings
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
                .ForMember(dest => dest.ResultId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.MedicalConcept.Display))
                .ForMember(dest => dest.TestCode, opt => opt.MapFrom(src => src.MedicalConcept.Code));

            CreateMap<CreateLabTestResultDto, LabTestResult>()
                .ForMember(dest => dest.MedicalConcept, opt => opt.Ignore())
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