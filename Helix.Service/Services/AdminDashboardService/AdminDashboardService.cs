using Helix.Data.Entities;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Helix.Service.Helper;
using Helix.Data.Enums;
using Helix.Infrastructure.Context;

namespace Helix.Service.Services.AdminDashboardService
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<AdminDashboardDto> GetSystemOverviewAsync()
        {
            // 1. Execute highly efficient COUNT(*) queries on the database
            var patientsQuery = await _unitOfWork.Repository<Patient>().FindAsQueryable(p => true);
            var totalPatients = await patientsQuery.CountAsync();

            var doctorsQuery = await _unitOfWork.Repository<Doctor>().FindAsQueryable(d => true);
            var totalDoctors = await doctorsQuery.CountAsync();

            var facilitiesQuery = await _unitOfWork.Repository<Pharmacy>().FindAsQueryable(f => true);
            var totalFacilities = await facilitiesQuery.CountAsync();

            var labOrdersQuery = await _unitOfWork.Repository<LabOrder>().FindAsQueryable(l => true);
            var totalLabOrders = await labOrdersQuery.CountAsync();

            var radOrdersQuery = await _unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(r => true);
            var totalRadOrders = await radOrdersQuery.CountAsync();

            var drugsQuery = await _unitOfWork.Repository<Medication>().FindAsQueryable(m => true);
            var totalDrugs = await drugsQuery.CountAsync();

            // 2. Map data and mock the UI trends until historical analytics are implemented
            return new AdminDashboardDto
            {
                Patients = new DashboardMetricDto
                {
                    TotalCount = totalPatients,
                    TrendText = "+12% from last month"
                },
                Doctors = new DashboardMetricDto
                {
                    TotalCount = totalDoctors,
                    TrendText = "+8% from last month"
                },
                Facilities = new DashboardMetricDto
                {
                    TotalCount = totalFacilities,
                    TrendText = "+5% from last month"
                },
                LabOrders = new DashboardMetricDto
                {
                    TotalCount = totalLabOrders,
                    TrendText = "+18% from last month"
                },
                RadiologyOrders = new DashboardMetricDto
                {
                    TotalCount = totalRadOrders,
                    TrendText = "+6% from last month"
                },
                Drugs = new DashboardMetricDto
                {
                    TotalCount = totalDrugs,
                    TrendText = "+3% from last month"
                }
            };
        }

        public async Task<PatientsManagementDashboardDto> GetPatientsManagementAsync(string searchTerm = null)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // 1. Base Query for metrics (Global, unaffected by search)
            // Provide a predicate as the repository API requires a filter parameter
            var baseQuery = await _unitOfWork.Repository<Patient>().FindAsQueryable(p => true);

            var totalPatients = await baseQuery.CountAsync();
            // Assuming your Patient entity has an IsActive boolean flag. Adjust if it uses an Enum status.
            var activePatients = await baseQuery.CountAsync();
            var newThisMonth = await baseQuery.CountAsync();

            var dashboard = new PatientsManagementDashboardDto
            {
                TotalPatients = totalPatients,
                ActivePatients = activePatients,
                InactivePatients = totalPatients - activePatients,
                NewThisMonth = newThisMonth
            };

            // 2. Query for the Table Data (Includes AppUser for name/email/phone)
            var tableQuery = baseQuery.Include(p => p.AppUser).AsQueryable();

            // 3. Apply Search Filter if provided by the frontend
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                tableQuery = tableQuery.Where(p =>
                    (p.FullName != null && p.FullName.ToLower().Contains(searchTerm)) ||
                    (p.AppUser.Email != null && p.AppUser.Email.ToLower().Contains(searchTerm)) ||
                    (p.AppUser.PhoneNumber != null && p.AppUser.PhoneNumber.Contains(searchTerm))
                );
            }

            // 4. Fetch the records (Note: In a production environment with thousands of patients, 
            // you should add .Skip() and .Take() here for pagination!)
            var patients = await tableQuery
                .Take(50) // Limiting to top 50 for the initial view
                .ToListAsync();

            // 5. Map to DTO
            dashboard.PatientsList = patients.Select((p, index) => new PatientListItemDto
            {
                Id = p.Id,
                // Mocking a sequential ID format like P-1001. If you have a real sequential ID in the DB, map it here.
                PatientIdDisplay = $"P-{1001 + index}",
                Initials = GetInitials(p.FullName),
                Name = p.FullName ?? "Unknown Patient",
                Email = p.AppUser?.Email ?? "No Email",
                Phone = p.AppUser?.PhoneNumber ?? "No Phone",
                Gender = p.NationalId.ParseEgyptianId().gender.ToString(),
                BloodType = p.BloodType.ToString() ?? "-", // Assuming you have a BloodType string property
                Status ="Active" ,
                RegisteredDate = DateTime.Now.AddMonths(-1).ToString("MMM d, yyyy")
            }).ToList();

            return dashboard;
        }

        public async Task<DoctorsManagementDashboardDto> GetDoctorsManagementAsync(string searchTerm = null)
        {
            // 1. Base Query for metrics (Global, unaffected by search)
            var baseQuery = await _unitOfWork.Repository<Doctor>().FindAsQueryable(d => true);

            // Calculate metrics based on a theoretical VerificationStatus enum or property
            // Adjust the property names based on your actual Doctor entity structure
            var totalDoctors = await baseQuery.CountAsync();
            var verifiedDoctors = await baseQuery.CountAsync(d => d.IsVerified == true);
            var pendingDoctors = await baseQuery.CountAsync(d => d.IsVerified == false);
            var rejectedDoctors = 0;

            var dashboard = new DoctorsManagementDashboardDto
            {
                TotalDoctors = totalDoctors,
                VerifiedDoctors = verifiedDoctors,
                PendingVerification = pendingDoctors,
                RejectedDoctors = rejectedDoctors
            };

            // 2. Query for the Table Data (Includes AppUser for name/contact and SpecialtyCatalog for the specialty name)
            var tableQuery = baseQuery
                .Include(d => d.AppUser)
                .Include(d => d.SpecialtyCatalog)
                .AsQueryable();

            // 3. Apply Search Filter if provided by the frontend
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                tableQuery = tableQuery.Where(d =>
                    (d.FullName != null && d.FullName.ToLower().Contains(searchTerm)) ||
                    (d.AppUser.Email != null && d.AppUser.Email.ToLower().Contains(searchTerm)) ||
                    (d.AppUser.PhoneNumber != null && d.AppUser.PhoneNumber.Contains(searchTerm))
                );
            }

            // 4. Fetch the records (Limit to 50 for performance without pagination)
            var doctors = await tableQuery
                .Take(50)
                .ToListAsync();

            // 5. Map to DTO
            dashboard.DoctorsList = doctors.Select((d, index) => new DoctorListItemDto
            {
                Id = d.Id,
                // Mocking sequential ID format like D-2001
                DoctorIdDisplay = $"D-{2001 + index}",
                Initials = GetInitials(d.FullName),
                Name = d.FullName != null ? $"Dr. {d.FullName}" : "Unknown Doctor",
                Email = d.AppUser?.Email ?? "No Email",
                Phone = d.AppUser?.PhoneNumber ?? "No Phone",
                Specialty = d.SpecialtyCatalog?.DisplayName ?? "General Practice",
                Experience = $"{d.YearsOfExperience} years", // Assuming you track this as an int
                Consultation = DetermineConsultationType(d), // Helper method to map clinic/online flags
                Status = d.IsVerified.ToString(),
                RegisteredDate = DateTime.Now.AddMonths(-1).ToString("MMM d, yyyy")
            }).ToList();

            return dashboard;
        }

        public async Task<FacilitiesManagementDashboardDto> GetFacilitiesManagementAsync(string searchTerm = null)
        {
            // 1. Base Query for metrics (Global, unaffected by search)
            // Note: If you split hospitals, labs, and pharmacies into different tables, 
            // you might need to query them separately. I'm assuming a unified Facility entity here.
            var baseQuery = await _unitOfWork.Repository<Facilitie>().FindAsQueryable(f => true);

            var totalFacilities = await baseQuery.CountAsync();
            var activeFacilities = await baseQuery.CountAsync(); // Adjust if using an Enum for Status

            // Assuming you have an Enum for Facility Types
            var hospitalsCount = await baseQuery.CountAsync();

            var dashboard = new FacilitiesManagementDashboardDto
            {
                TotalFacilities = totalFacilities,
                ActiveFacilities = activeFacilities,
                InactiveFacilities = totalFacilities - activeFacilities,
                HospitalsCount = hospitalsCount
            };

            // 2. Query for the Table Data
            var tableQuery = baseQuery.AsQueryable();

            // 3. Apply Search Filter if provided by the frontend
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                tableQuery = tableQuery.Where(f =>
                    (f.Name != null && f.Name.ToLower().Contains(searchTerm)));
            }

            // 4. Fetch the records (Limit to 50 for performance)
            var facilities = await tableQuery
                .Take(50)
                .ToListAsync();

            // 5. Map to DTO
            dashboard.FacilitiesList = facilities.Select((f, index) => new FacilityListItemDto
            {
                Id = f.Id,
                // Mocking sequential ID format like F-3001
                FacilityIdDisplay = $"F-{3001 + index}",
                Name = f.Name ?? "Unknown Facility",

                // Combining address parts for the UI subtitle (e.g., "Nasr City, Cairo")
                LocationSubtitle = f.Address,

                Type = "Hospital", // e.g., converts EnFacilityType.RadiologyCenter to "Radiology Center"
                Email = "No Email",
                Phone ="No Phone",
                City = "-",
                Status = "Active",
                CreatedDate = DateTime.Now.AddMonths(-1).ToString("MMM d, yyyy")
            }).ToList();

            return dashboard;
        }

        public async Task<DrugsManagementDashboardDto> GetDrugsManagementAsync(string searchTerm = null)
        {
            // 1. Base Query with AsNoTracking() for maximum read performance
            var baseQuery = _context.MedicationCatalogs.AsNoTracking();

            // 2. Calculate Top Metrics
            var totalDrugs = await baseQuery.CountAsync();

            // Assuming your MedicationCatalog entity has an IsAvailable boolean
            var availableDrugs = await baseQuery.CountAsync();

            // Efficiently count unique categories directly in the SQL database
            var categoriesCount = await baseQuery
                .Where(d => !string.IsNullOrEmpty(d.TermType))
                .Select(d => d.TermType)
                .Distinct()
                .CountAsync();

            var dashboard = new DrugsManagementDashboardDto
            {
                TotalDrugs = totalDrugs,
                AvailableDrugs = availableDrugs,
                UnavailableDrugs = totalDrugs - availableDrugs,
                CategoriesCount = categoriesCount
            };

            // 3. Setup Table Query
            var tableQuery = baseQuery;

            // 4. Apply Search Filter if provided by the frontend
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                tableQuery = tableQuery.Where(d =>
                    (d.Rxcui != null && d.Rxcui.ToLower().Contains(searchTerm)) ||
                    (d.DrugName != null && d.DrugName.ToLower().Contains(searchTerm)) ||
                    (d.AiModelName != null && d.AiModelName.ToLower().Contains(searchTerm)) ||
                    (d.TermType != null && d.TermType.ToLower().Contains(searchTerm))
                );
            }

            // 5. Fetch the records (Limit to 50 for performance without pagination)
            var drugs = await tableQuery
                .Take(50)
                .ToListAsync();

            // 6. Map to DTO
            dashboard.DrugsList = drugs.Select((d, index) => new DrugListItemDto
            {
                Id = d.Rxcui,
                // Mocking sequential ID format like DR-4001
                DrugIdDisplay = $"DR-{4001 + index}",
                DrugName = d.DrugName ?? "Unknown Drug",
                CreatedSubtitle = $"Created: {DateTime.Now:MMM d, yyyy}",
                GenericName = d.AiModelName ?? "-",
                Category = d.TermType ?? "Uncategorized",
                Form =  "Tablet", // e.g., Tablet, Capsule, Syrup
                Strength = "-", // e.g., "500 mg"
                Manufacturer ="Unknown",
                Status = "Available" 
            }).ToList();

            return dashboard;
        }
        // Helper method to format consultation types based on boolean flags in your DB
        private string DetermineConsultationType(Doctor doctor)
        {
            var types = new List<string>();
            if (doctor.ConsultationType==EnConsultationType.Video) types.Add("Online");
            if (doctor.ConsultationType == EnConsultationType.InPerson) types.Add("Clinic");

            return types.Any() ? string.Join(" / ", types) : "Not Specified";
        }
        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
    }
}
