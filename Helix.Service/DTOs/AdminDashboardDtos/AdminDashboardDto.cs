namespace Helix.Service.DTOs.AllergyDTOs
{
    public class AdminDashboardDto
    {
        public DashboardMetricDto Patients { get; set; }
        public DashboardMetricDto Doctors { get; set; }
        public DashboardMetricDto Facilities { get; set; }
        public DashboardMetricDto LabOrders { get; set; }
        public DashboardMetricDto RadiologyOrders { get; set; }
        public DashboardMetricDto Drugs { get; set; }
    }

    public class DashboardMetricDto
    {
        public int TotalCount { get; set; }
        public string TrendText { get; set; }
    }

    public class PatientsManagementDashboardDto
    {
        // Top Metric Cards
        public int TotalPatients { get; set; }
        public int ActivePatients { get; set; }
        public int InactivePatients { get; set; }
        public int NewThisMonth { get; set; }

        // Main Table List
        public List<PatientListItemDto> PatientsList { get; set; } = new();
    }

    public class PatientListItemDto
    {
        public Guid Id { get; set; }
        public string PatientIdDisplay { get; set; } // e.g., "P-1001"
        public string Initials { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public string Status { get; set; } // "Active" or "Inactive"
        public string RegisteredDate { get; set; }
    }

    public class DoctorsManagementDashboardDto
    {
        // Top Metric Cards
        public int TotalDoctors { get; set; }
        public int VerifiedDoctors { get; set; }
        public int PendingVerification { get; set; }
        public int RejectedDoctors { get; set; }

        // Main Table List
        public List<DoctorListItemDto> DoctorsList { get; set; } = new();
    }

    public class DoctorListItemDto
    {
        public Guid Id { get; set; }
        public string DoctorIdDisplay { get; set; } // e.g., "D-2001"
        public string Initials { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialty { get; set; }
        public string Experience { get; set; } // e.g., "12 years"
        public string Consultation { get; set; } // e.g., "Online / Clinic"
        public string Status { get; set; } // "Verified", "Pending", "Rejected"
        public string RegisteredDate { get; set; }
    }

    public class FacilitiesManagementDashboardDto
    {
        // Top Metric Cards
        public int TotalFacilities { get; set; }
        public int ActiveFacilities { get; set; }
        public int InactiveFacilities { get; set; }
        public int HospitalsCount { get; set; } // The specific "Hospitals" card

        // Main Table List
        public List<FacilityListItemDto> FacilitiesList { get; set; } = new();
    }

    public class FacilityListItemDto
    {
        public Guid Id { get; set; }
        public string FacilityIdDisplay { get; set; } // e.g., "F-3001"
        public string Name { get; set; }
        public string LocationSubtitle { get; set; } // e.g., "Nasr City, Cairo"
        public string Type { get; set; } // e.g., "Hospital", "Radiology Center"
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Status { get; set; } // e.g., "Active"
        public string CreatedDate { get; set; }
    }
    public class DrugsManagementDashboardDto
    {
        // Top Metric Cards
        public int TotalDrugs { get; set; }
        public int AvailableDrugs { get; set; }
        public int UnavailableDrugs { get; set; }
        public int CategoriesCount { get; set; }

        // Main Table List
        public List<DrugListItemDto> DrugsList { get; set; } = new();
    }

    public class DrugListItemDto
    {
        public string Id { get; set; }
        public string DrugIdDisplay { get; set; } // e.g., "DR-4001"
        public string DrugName { get; set; }
        public string CreatedSubtitle { get; set; } // e.g., "Created: May 15, 2026"
        public string GenericName { get; set; }
        public string Category { get; set; } // e.g., "Analgesic", "Antibiotic"
        public string Form { get; set; } // e.g., "Tablet", "Capsule"
        public string Strength { get; set; } // e.g., "500 mg"
        public string Manufacturer { get; set; }
        public string Status { get; set; } // "Available" or "Unavailable"
    }
}
