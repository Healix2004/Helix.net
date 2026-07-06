using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Helix.Service.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.PatientService
{
    public class PatientDashboardService(IUnitOfWork unitOfWork) : IPatientDashboardService
    {
        public async Task<PatientPortalDashboardDto> GetDashboardDataAsync(Guid patientId)
        {
            var now = DateTime.UtcNow;
            var dashboard = new PatientPortalDashboardDto();

            // 1. Calculate Top Metric Cards
            var labsCount = await (await unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(l => l.PatientId == patientId && l.Status == EnLabOrderStatus.Completed)).CountAsync();

            var radiologyCount = await (await unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(r => r.PatientId == patientId && r.Status == EnRadiologyOrderStatus.Completed)).CountAsync();
            dashboard.TotalRecords = labsCount + radiologyCount; // Or include uploaded documents if applicable

            dashboard.ActivePrescriptions = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.PatientId == patientId )).CountAsync();

            dashboard.UpcomingAppointments = await (await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.PatientId == patientId && a.StartTime > now && a.Status == EnAppointmentStatus.Booked)).CountAsync();
            // 2. Fetch Recent Activity (Mixed feed)
            var activities = new List<RecentActivityItemDto>();

            // Get latest 2 labs
            var recentLabs = await(await unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(l => l.PatientId == patientId && l.Status == EnLabOrderStatus.Completed))
                .Include(l => l.MedicalConcept)
                .OrderByDescending(l => l.CreatedAt)
                .Take(2).ToListAsync();

            activities.AddRange(recentLabs.Select(l => new RecentActivityItemDto
            {
                ActivityType = "Lab",
                Title = "Lab Results Available",
                Description = $"Blood test results for {l.MedicalConcept?.Display ?? "General Panel"}",
                Date = l.CreatedAt,
                TimeAgo = GetTimeAgo(l.CreatedAt)
            }));

            // Get latest 2 upcoming appointments
            var upcomingAppts = await (await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.PatientId == patientId && a.StartTime > now))
                .Include(a => a.Doctor)
                .OrderBy(a => a.StartTime)
                .Take(2).ToListAsync();

            activities.AddRange(upcomingAppts.Select(a => new RecentActivityItemDto
            {
                ActivityType = "Appointment",
                Title = "Appointment Confirmed",
                Description = $"Dr. {a.Doctor?.FullName} - {a.StartTime:MMM dd, yyyy}",
                Date = a.StartTime, // Note: For upcoming, TimeAgo logic might need adjusting or hiding
                TimeAgo = $"In {(a.StartTime - now).Days} days"
            }));

            //Get latest 2 radiology 
            var recentRadiology = await (await unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(r => r.PatientId == patientId && r.Status == EnRadiologyOrderStatus.Completed))
                .Include(r => r.MedicalConcept)
                .OrderByDescending(r => r.CreatedAt)
                .Take(2).ToListAsync();
            
            activities.AddRange(recentRadiology.Select(r => new RecentActivityItemDto
            {
                ActivityType = "Radiology",
                Title = "Radiology Results Available",
                Description = $"Radiology results for {r.MedicalConcept?.Display ?? "General Scan"}",
                Date = r.CreatedAt,
                TimeAgo = GetTimeAgo(r.CreatedAt)
            }));

            // Get latest 2 prescriptions
            var recentPrescriptions = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.PatientId == patientId))
                .Include(p => p.Items).ThenInclude(i=> i.MedicationCatalog)
                .OrderByDescending(p => p.CreatedAt)
                .Take(2).ToListAsync();
            
            activities.AddRange(recentPrescriptions.Select(p => new RecentActivityItemDto
            {
                ActivityType = "Prescription",
                Title = "Prescription Updated",
                Description = $"Medication: {p.Items?.FirstOrDefault()?.MedicationCatalog?.DrugName ?? "General Medication"}",
                Date = p.CreatedAt,
                TimeAgo = GetTimeAgo(p.CreatedAt)
            }));

            // Sort mixed activities by date and take the top 4 for the UI
            dashboard.RecentActivity = activities.OrderByDescending(a => a.Date).Take(4).ToList();

            // 3. Mock Health Score & Tips (Can be dynamic later based on actual patient vitals)
            dashboard.HealthScore = 85;
            dashboard.HealthScoreMessage = "Your health metrics are looking good!";

            dashboard.HealthTips = new List<HealthTipDto>
            {
                new HealthTipDto { IconType = "Heart", Title = "Stay Hydrated", Description = "Drink at least 8 glasses of water daily" },
                new HealthTipDto { IconType = "Activity", Title = "Daily Exercise", Description = "30 minutes of moderate activity" },
                new HealthTipDto { IconType = "Drop", Title = "Monitor Blood Sugar", Description = "Check levels before meals" }
            };

            return dashboard;
        }

        public async Task<PatientLabDashboardDto> GetPatientLabDashboardAsync(Guid patientId)
        {
            // 1. Fetch all Lab Orders for the patient
            var labs = await (await unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(l => l.PatientId == patientId))
                .Include(l => l.Doctor)
                .Include(l => l.MedicalConcept)
                .Include(l => l.Results)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            var dashboard = new PatientLabDashboardDto
            {
                TotalTests = labs.Count
            };

            // 2. Process each lab and determine its UI Status
            foreach (var lab in labs)
            {
                string uiStatus = "Pending";

                if (lab.Status == EnLabOrderStatus.Completed)
                {
                    // Check if ANY of the results in this panel have an abnormal flag
                    bool isAbnormal = lab.Results != null && lab.Results.Any(r =>
                        !string.IsNullOrWhiteSpace(r.InterpretationFlag) &&
                        (r.InterpretationFlag.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                         r.InterpretationFlag.Equals("Low", StringComparison.OrdinalIgnoreCase) ||
                         r.InterpretationFlag.Equals("Abnormal", StringComparison.OrdinalIgnoreCase)));

                    uiStatus = isAbnormal ? "Abnormal" : "Completed";
                }

                // Update top-level counters
                if (uiStatus == "Pending") dashboard.PendingResults++;
                if (uiStatus == "Abnormal") dashboard.AbnormalResults++;

                // Add to the main table list
                dashboard.LabTests.Add(new LabTestItemDto
                {
                    Id = lab.Id,
                    TestName = lab.MedicalConcept?.Display ?? lab.MedicalConcept?.Code ?? "Unknown Test",
                    Date = lab.CreatedAt,
                    RequestedBy = lab.Doctor != null ? $"Dr. {lab.Doctor.FullName}" : "Unknown Provider",
                    Status = uiStatus
                });
            }

            // 3. Generate the Active Alert (Grab the most recent Abnormal test, if any)
            var latestAbnormal = dashboard.LabTests.FirstOrDefault(l => l.Status == "Abnormal");
            if (latestAbnormal != null)
            {
                dashboard.ActiveAlert = new LabAlertDto
                {
                    Title = "Abnormal Result Alert",
                    Description = $"Your {latestAbnormal.TestName} shows results outside normal range. Please contact your provider.",
                    TimeAgo = GetTimeAgo(latestAbnormal.Date)
                };
            }

            // 4. Generate the Recent Activity sidebar (Take the 3 most recent events)
            dashboard.RecentActivity = dashboard.LabTests.Take(3).Select(l => new LabRecentActivityDto
            {
                Title = l.Status == "Pending" ? $"{l.TestName} ordered" :
                        l.Status == "Abnormal" ? $"{l.TestName} results uploaded" :
                        $"{l.TestName} completed",
                TimeAgo = GetTimeAgo(l.Date),
                StatusColor = l.Status == "Abnormal" ? "red" : l.Status == "Pending" ? "yellow" : "green"
            }).ToList();

            return dashboard;
        }
        public async Task<LabTestDetailsDto> GetLabTestDetailsAsync(Guid orderId, Guid requestingPatientId)
        {
            // 1. Fetch the specific order, ensuring it belongs to the logged-in patient for security
            var labOrder = await (await unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(l => l.Id == orderId && l.PatientId == requestingPatientId))
                .Include(l => l.Patient)
                    .ThenInclude(p => p.AppUser)
                .Include(l => l.Doctor)
                    .ThenInclude(d => d.SpecialtyCatalog) // Ensure the catalog is loaded to access DisplayName
                .Include(l => l.MedicalConcept)
                .Include(l => l.Results)
                    .ThenInclude(r => r.MedicalConcept)
                .FirstOrDefaultAsync();

            if (labOrder == null)
            {
                return null; // Will trigger a 404/Error in the controller
            }

            // 2. Map the Patient Info Card (Bulletproofed)
            var patientInfo = new LabPatientInfoDto
            {
                PatientName = labOrder.Patient?.FullName ?? "Unknown",
                Email = labOrder.Patient?.AppUser?.Email ?? "No email provided",
                Contact = labOrder.Patient?.AppUser?.PhoneNumber ?? "No contact provided",

                // Safely check if Patient and NationalId exist before attempting to parse
                DateOfBirth = (labOrder.Patient != null && !string.IsNullOrWhiteSpace(labOrder.Patient.NationalId))
                    ? FormatDateOfBirth(labOrder.Patient.NationalId.ParseEgyptianId().dateOfBirth)
                    : "Unknown",

                TestDate = labOrder.CreatedAt.ToString("MMM d, yyyy"),

                // Safely evaluate the ID before calling Substring
                PatientIdDisplay = labOrder.Patient != null
                    ? $"PT-{labOrder.Patient.Id.ToString().Substring(0, 8).ToUpper()}"
                    : "PT-UNKNOWN"
            };

            // 3. Map the Doctor's Comments Sidebar (Bulletproofed)
            var doctorComment = new LabDoctorCommentDto
            {
                DoctorName = labOrder.Doctor != null ? $"Dr. {labOrder.Doctor.FullName}" : "Unknown Provider",

                // Safely navigate through SpecialtyCatalog
                Specialty = labOrder.Doctor?.SpecialtyCatalog?.DisplayName ?? "General Medicine",

                // Safely check if Results collection is null before getting FirstOrDefault
                CommentDate = labOrder.Results?.FirstOrDefault()?.ResultDate?.ToString("MMM d, yyyy 'at' h:mm tt")
                              ?? labOrder.CreatedAt.ToString("MMM d, yyyy 'at' h:mm tt"),

                OverallComment = "No additional comments provided.",
                Recommendations = new List<string>()
            };

            // 4. Map the Results Table
            var resultsList = labOrder.Results?.Select(r => new LabParameterResultDto
            {
                ParameterName = r.MedicalConcept?.Display ?? r.MedicalConcept?.Code ?? "Unknown Parameter",
                ResultValue = r.NumericValue.HasValue ? r.NumericValue.Value.ToString("G") : (r.StringValue ?? "-"),
                NormalRange = r.ReferenceRange ?? "-",
                Unit = r.Unit ?? "",

                // Default to "Normal" if no flag is set. The UI uses this for the red/green formatting.
                Status = string.IsNullOrWhiteSpace(r.InterpretationFlag) ? "Normal" : r.InterpretationFlag
            }).ToList() ?? new List<LabParameterResultDto>();

            // 5. Assemble the final response
            return new LabTestDetailsDto
            {
                OrderId = labOrder.Id,
                PanelName = labOrder.MedicalConcept?.Display ?? "General Lab Panel",
                PatientInfo = patientInfo,
                DoctorComment = doctorComment,
                Results = resultsList
            };
        }


        public async Task<RadiologyDashboardDto> GetPatientDashboardAsync(Guid patientId)
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);

            // 1. Fetch radiology orders ONLY for the logged-in patient
            // CRITICAL SECURITY FIX: Filter by r.PatientId == patientId
            var orders = await (await unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(r => r.PatientId == patientId))
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .Include(r => r.MedicalConcept)
                .Include(r => r.Report)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var dashboard = new RadiologyDashboardDto
            {
                TotalScans = orders.Count,
                RecentUploads = orders.Count(r => r.CreatedAt >= yesterday)
            };

            // 2. Process each scan for the table and metrics
            foreach (var order in orders)
            {
                string uiStatus = order.Status.ToString();
                string category = DetermineCategory(order.MedicalConcept?.Display ?? "");

                // Increment Counters
                if (uiStatus == "Pending") dashboard.PendingReview++;
                if (uiStatus == "Abnormal") dashboard.AbnormalFindings++;

                // Add to table list
                dashboard.Scans.Add(new RadiologyScanItemDto
                {
                    Id = order.Id,
                    ScanType = order.MedicalConcept?.Display ?? "Unknown Scan",
                    Category = category,
                    Date = order.CreatedAt,
                    RequestedBy = order.Doctor != null ? $"Dr. {order.Doctor.FullName}" : "Unknown Provider",
                    Status = uiStatus
                });
            }

            // 3. Populate Right Sidebar: Latest Scan Preview
            var latestScan = orders.FirstOrDefault(r => r.Status == EnRadiologyOrderStatus.Completed);
            if (latestScan != null)
            {
                dashboard.LatestScanPreview = new LatestScanPreviewDto
                {
                    ScanId = latestScan.Id,
                    ScanName = latestScan.MedicalConcept?.Display ?? "Recent Scan",
                    PatientName = latestScan.Patient?.FullName ?? "Unknown Patient"
                };
            }

            // 4. Populate Right Sidebar: Latest Radiologist Note
            var latestNoteScan = orders.FirstOrDefault(r => r.Report != null && !string.IsNullOrWhiteSpace(r.Report.Conclusion));

            if (latestNoteScan != null)
            {
                dashboard.LatestNote = new LatestRadiologistNoteDto
                {
                    DoctorName = latestNoteScan.Doctor != null ? $"Dr. {latestNoteScan.Doctor.FullName}" : "Radiologist",
                    TimeAgo = GetTimeAgo(latestNoteScan.Report.ReportDate),
                    NotePreview = latestNoteScan.Report.Conclusion
                };
            }

            return dashboard;
        }
        public async Task<RadiologyStudyDetailsDto> GetRadiologyStudyDetailsAsync(Guid orderId, Guid requestingPatientId)
        {
            // 1. Fetch the specific order, locked down to the requesting patient!
            var order = await (await unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(r => r.Id == orderId && r.PatientId == requestingPatientId))
                .Include(r => r.Patient).ThenInclude(p => p.AppUser)
                .Include(r => r.Doctor) // The Referring/Attending Physician
                .Include(r => r.MedicalConcept) // Holds Modality and Body Part data
                .Include(r => r.Report) // Holds the actual radiologist findings
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return null; // Will trigger a 404 Not Found in the controller
            }

            // 2. Map Metadata (Bulletproofed)
            var metadata = new RadiologyMetadataDto
            {
                PatientName = order.Patient?.FullName ?? "Unknown",
                Gender = order.Patient?.NationalId.ParseEgyptianId().gender.ToString() ?? "Unknown",
                Email = order.Patient?.AppUser?.Email ?? "No email provided",
                Contact = order.Patient?.AppUser?.PhoneNumber ?? "No contact provided",

                DateOfBirth = (order.Patient != null && !string.IsNullOrWhiteSpace(order.Patient.NationalId))
                    ? FormatDateOfBirth(order.Patient.NationalId.ParseEgyptianId().dateOfBirth)
                    : "Unknown",

                PatientIdDisplay = order.Patient != null
                    ? $"PT-{order.Patient.Id.ToString().Substring(0, 8).ToUpper()}"
                    : "PT-UNKNOWN",

                StudyDate = order.CreatedAt.ToString("MMM d, yyyy"),
                StudyTime = order.CreatedAt.ToString("HH:mm tt"),
                ReferringPhysician = order.Doctor != null ? $"Dr. {order.Doctor.FullName}" : "Unknown Provider",

                // Map the correct concept fields instead of the UI's dummy data
                Modality = order.MedicalConcept?.Display ?? "Unknown Modality",
                BodyPart = order.MedicalConcept?.Code ?? "Unknown Region",
                Institution = "Helix Memorial Hospital" // Replace with actual Clinic/Hospital entity if multi-tenant
            };

            // 3. Map Radiologist Findings (Bulletproofed)
            var findings = new RadiologistFindingsDto
            {
                Status = order.Status.ToString(),

                // Grab the Radiologist's name from the Report, fallback to "Radiology Dept"
                RadiologistName = order.Report?.ExternalRadiologistName ?? "Helix Radiology Dept",

                ClinicalIndication = order.Report.Conclusion ?? "Follow-up examination.", // Or map from order reason

                FindingsText = order.Report?.Findings ?? "No specific findings recorded.",
                Impression = order.Report?.Conclusion ?? "No impression recorded.",

                ReportDate = order.Report?.ReportDate != null
                    ? $"Reported on {order.Report.ReportDate.ToString("MMM d, yyyy 'at' h:mm tt")}"
                    : "Pending Report"
            };

            // 4. Map Attending Physician Notes
            var notes = new PhysicianNotesDto
            {
                PhysicianName = order.Doctor != null ? $"Dr. {order.Doctor.FullName}, MD" : "Attending Physician",

                // If your schema has a separate field for the referring doctor's discussion notes, map it here.
                // Otherwise, it can remain a generic fallback or empty until the doctor adds a comment.
                Notes = "Discussed findings with patient. Will proceed with current treatment plan."
            };

            // 5. Assemble Final Response
            return new RadiologyStudyDetailsDto
            {
                OrderId = order.Id,
                ImageUrls = order.Report?.ImageUrls,
                Metadata = metadata,
                Findings = findings,
                PhysicianNotes = notes
            };
        }


        public async Task<PatientPrescriptionDashboardDto> GetPatientPrescriptionDashboardAsync(Guid patientId)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // 1. Fetch all prescriptions for the logged-in patient
            // Assuming your Prescription entity contains a collection of Medications
            var prescriptions = await( await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.PatientId == patientId))
                .Include(p => p.Doctor)
                .Include(p => p.Items).ThenInclude(i=> i.MedicationCatalog) // The individual drugs prescribed
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var dashboard = new PatientPrescriptionDashboardDto();

            // Track unique medication names to prevent duplicate tips
            var uniqueMedNames = new HashSet<string>();

            // 2. Process each medication across all prescriptions
            foreach (var prescription in prescriptions)
            {
                // Skip if the prescription has no medications attached
                if (prescription.Items == null) continue;

                foreach (var med in prescription.Items)
                {
                    dashboard.TotalMedications++;

                    // Assuming your Medication entity has Name or MedicalConcept attached
                    string medName = med.MedicationCatalog.DrugName ?? "Unknown Medication";
                    uniqueMedNames.Add(medName);

                    // Check if it was prescribed this month
                    if (prescription.CreatedAt >= startOfMonth)
                    {
                        dashboard.RenewedThisMonth++;
                    }

                    // Calculate the expiration date
                    DateTime endDate = CalculateEndDate(prescription.CreatedAt, med.Duration);
                    var daysRemaining = (endDate - now).TotalDays;

                    // Determine UI Status
                    string uiStatus;
                    if (daysRemaining < 0)
                    {
                        uiStatus = "Completed";
                    }
                    else if (daysRemaining <= 14) // Consider it "Expiring" if 14 days or less remain
                    {
                        uiStatus = "Expiring";
                        dashboard.ExpiringSoon++;
                        dashboard.ActivePrescriptions++; // Expiring is still technically active

                        // Add to Renewal Reminders sidebar
                        dashboard.RenewalReminders.Add(new RenewalReminderDto
                        {
                            MedicationId = med.Id,
                            MedicationName = medName,
                            ExpiresInText = daysRemaining < 1 ? "Expires today" : $"Expires in {(int)daysRemaining} days"
                        });
                    }
                    else
                    {
                        uiStatus = "Active";
                        dashboard.ActivePrescriptions++;
                    }

                    // Add to the main table list
                    dashboard.Medications.Add(new MedicationItemDto
                    {
                        Id = med.Id,
                        PrescriptionId = prescription.Id,
                        MedicationName = medName,
                        PrescribedBy = prescription.Doctor != null ? $"Dr. {prescription.Doctor.FullName}" : "Unknown Provider",
                        Date = prescription.CreatedAt,
                        Duration = med.Duration ?? "Unknown",
                        Status = uiStatus
                    });
                }
            }

            // 3. Sort Reminders by urgency (fewest days remaining first)
            dashboard.RenewalReminders = dashboard.RenewalReminders
                .OrderBy(r => int.Parse(System.Text.RegularExpressions.Regex.Match(r.ExpiresInText, @"\d+").Value))
                .Take(4) // Show top 4 in UI
                .ToList();

            // 4. Generate Medication Tips (Mocked based on active medications)
            // In a full implementation, you could map these to warning labels in your drug database
            dashboard.MedicationTips = GenerateMockTips(uniqueMedNames);

            return dashboard;
        }
        public async Task<PrescriptionDetailsDto> GetPrescriptionDetailsAsync(Guid prescriptionId, Guid requestingPatientId)
        {
            // 1. Fetch the specific prescription securely
            var prescription = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.Id == prescriptionId && p.PatientId == requestingPatientId))
                .Include(p => p.Patient).ThenInclude(p => p.AppUser)
                .Include(p => p.Doctor)
                .Include(p => p.Items).ThenInclude(i=>i.MedicationCatalog) // The list of prescribed drugs
                .FirstOrDefaultAsync();

            if (prescription == null)
            {
                return null;
            }

            // 2. Map the Summary Card (Bulletproofed)
            var summary = new PrescriptionSummaryDto
            {
                PatientName = prescription.Patient?.FullName ?? "Unknown",
                Gender = prescription.Patient?.NationalId.ParseEgyptianId().gender.ToString() ?? "Unknown",
                Email = prescription.Patient?.AppUser?.Email ?? "No email provided",
                Contact = prescription.Patient?.AppUser?.PhoneNumber ?? "No contact provided",

                DateOfBirth = (prescription.Patient != null && !string.IsNullOrWhiteSpace(prescription.Patient.NationalId))
                    ? FormatDateOfBirth(prescription.Patient.NationalId.ParseEgyptianId().dateOfBirth)
                    : "Unknown",

                PatientIdDisplay = prescription.Patient != null
                    ? $"PT-{prescription.Patient.Id.ToString().Substring(0, 8).ToUpper()}"
                    : "PT-UNKNOWN",

                IssueDate = prescription.CreatedAt.ToString("MMM d, yyyy"),
                PrescribingDoctor = prescription.Doctor != null ? $"Dr. {prescription.Doctor.FullName}" : "Unknown Provider",

                // Calculate ValidUntil (Default to 90 days if we can't parse the medications)
                ValidUntil = CalculateValidUntil(prescription.CreatedAt, prescription.Items).ToString("MMM d, yyyy")
            };

            // 3. Map the Medications Table
            var medications = prescription.Items?.Select(m => new PrescribedMedicationDto
            {
                MedicationName = m.MedicationCatalog.DrugName?.ToUpper() ?? "UNKNOWN MEDICATION",
                Dosage = m.Dosage ?? "-",
                Frequency = m.Frequency ?? "-",
                Duration = m.Duration ?? "-"
            }).ToList() ?? new List<PrescribedMedicationDto>();

            // 4. Assemble Final Response
            return new PrescriptionDetailsDto
            {
                PrescriptionId = prescription.Id,
                Summary = summary,
                DoctorInstructions = prescription.DoctorNotes ?? "Take all medications exactly as prescribed. Contact your doctor if you experience unusual side effects.",
                Medications = medications,

                // If you have a Refill entity, query it here. Otherwise, leave empty for now.
                RefillHistory = new List<RefillHistoryItemDto>(),

                // If you integrate with a drug database (like RxNorm), map the warnings here.
                Warnings = GenerateMockWarnings()
            };
        }

        // --- Helper Methods ---
        private DateTime CalculateValidUntil(DateTime issueDate, IEnumerable<PrescriptionItem> meds)
        {
            if (meds == null || !meds.Any()) return issueDate.AddDays(90);

            int maxDays = 30; // Minimum default validity

            foreach (var med in meds)
            {
                if (string.IsNullOrWhiteSpace(med.Duration)) continue;

                var match = System.Text.RegularExpressions.Regex.Match(med.Duration.ToLower(), @"\d+");
                if (!match.Success) continue;

                int amount = int.Parse(match.Value);
                int days = med.Duration.ToLower().Contains("month") ? amount * 30 :
                           med.Duration.ToLower().Contains("week") ? amount * 7 : amount;

                if (days > maxDays) maxDays = days;
            }

            return issueDate.AddDays(maxDays);
        }
        private List<MedicationWarningDto> GenerateMockWarnings()
        {
            // Placeholder to match your UI design until you wire up a real drug interaction API
            return new List<MedicationWarningDto>
            {
                new MedicationWarningDto { WarningType = "Drug Interaction", Description = "Avoid grapefruit and grapefruit juice while taking Atorvastatin as it may increase the risk of side effects." },
                new MedicationWarningDto { WarningType = "Side Effects", Description = "May cause dizziness or lightheadedness. Use caution when driving or operating machinery." },
                new MedicationWarningDto { WarningType = "Monitoring", Description = "Regular blood glucose monitoring recommended for patients taking Metformin." }
            };
        }
        private DateTime CalculateEndDate(DateTime startDate, string durationText)
        {
            if (string.IsNullOrWhiteSpace(durationText)) return startDate.AddDays(30); // Default fallback

            durationText = durationText.ToLower();

            // Extract the number from the string (e.g., "90" from "90 Days")
            var match = System.Text.RegularExpressions.Regex.Match(durationText, @"\d+");
            if (!match.Success) return startDate.AddDays(30); // Default fallback

            int amount = int.Parse(match.Value);

            if (durationText.Contains("week")) return startDate.AddDays(amount * 7);
            if (durationText.Contains("month")) return startDate.AddMonths(amount);

            return startDate.AddDays(amount); // Default to days
        }
        private List<MedicationTipDto> GenerateMockTips(HashSet<string> activeMeds)
        {
            var tips = new List<MedicationTipDto>();

            // Generic tips that always apply
            tips.Add(new MedicationTipDto
            {
                Title = "Take with Food",
                Description = "Certain medications should be taken with meals to reduce side effects. Check your bottle labels."
            });

            tips.Add(new MedicationTipDto
            {
                Title = "Morning Dose",
                Description = "Try taking your once-daily pills at the same time each morning to build a routine."
            });

            return tips;
        }
        private string DetermineCategory(string scanName)
        {
            if (string.IsNullOrWhiteSpace(scanName)) return "Other";

            var lowerName = scanName.ToLower();
            if (lowerName.Contains("mri")) return "MRI";
            if (lowerName.Contains("ct") || lowerName.Contains("computed tomography")) return "CT";
            if (lowerName.Contains("ultrasound") || lowerName.Contains("us ")) return "Ultrasound";
            if (lowerName.Contains("x-ray") || lowerName.Contains("xray")) return "X-Ray";

            return "Other";
        }
        private string FormatDateOfBirth(DateOnly? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return "Unknown";
            var dob = dateOfBirth.Value;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dob.Year;

            if (dob.AddYears(age) > today)
            {
                age--;
            }

            return $"{dob:MMMM d, yyyy} ({age} years)";
        }
        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;

            if (timeSpan.TotalMinutes < 60) return timeSpan.TotalMinutes < 2 ? "Just now" : $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} days ago";

            return $"{(int)(timeSpan.TotalDays / 30)} months ago";
        }
    }
}