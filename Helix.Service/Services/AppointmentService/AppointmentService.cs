using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.DTOs.MedicationDTOs;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Services.AppointmentService
{
    public class AppointmentService(IUnitOfWork unitOfWork) : IAppointmentService
    {
        public async Task<Guid> ScheduleAppointmentAsync(CreateAppointmentDto dto)
        {
            // 1. Fetch the exact availability for that day using the logic we already built
            // We pass the PatientId so the "one appointment per day" rule is checked!
            var dailyAvailability = await GetTimeSlotsForDayAsync(dto.DoctorId, dto.StartTime.Date, dto.PatientId);

            // 2. Find the specific slot the user is trying to book
            var requestedSlot = dailyAvailability.Slots.FirstOrDefault(s => s.StartTime == dto.StartTime);

            // 3. Validation: Does the slot exist in the schedule?
            if (requestedSlot == null)
            {
                throw new InvalidOperationException("The requested time is outside the doctor's working hours or is not a valid 30-minute slot.");
            }

            // 4. Validation: Is the slot actually available?
            if (!requestedSlot.IsAvailable)
            {
                throw new InvalidOperationException("This time slot is already booked, has passed, or you already have an appointment with this doctor today.");
            }

            // 5. Proceed with booking
            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                StartTime = dto.StartTime,

                // Safety feature: Force the EndTime to match the official 30-minute slot duration
                // This prevents bad data from the frontend where EndTime might be completely wrong
                EndTime = requestedSlot.EndTime,

                AppointmentType = dto.AppointmentType,
                Priority = dto.Priority,
                PatientInstruction = dto.PatientInstruction,
                Status = EnAppointmentStatus.Booked // Auto-confirm for now
            };

            await unitOfWork.Repository<Appointment>().AddAsync(appointment);
            await unitOfWork.CompleteAsync();

            return appointment.Id;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(Guid id, EnAppointmentStatus newStatus)
        {
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdAsync(id);
            if (appointment == null) throw new KeyNotFoundException($"Appointment {id} not found.");

            appointment.Status = newStatus;

            await unitOfWork.Repository<Appointment>().UpdateAsync(appointment);
            return await unitOfWork.CompleteAsync() > 0;
        }

        // Powers the main list in the center of your UI
        public async Task<List<AppointmentListDto>> GetDoctorAppointmentsForTodayAsync(Guid doctorId)
        {
            var today = DateTime.UtcNow.Date;

            // 1. Fetch raw data from the database
            var query = await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.DoctorId == doctorId && a.StartTime.Date == today);

            var rawAppointments = await query
                .Include<Appointment, Patient>(a => a.Patient)
                .OrderBy(a => a.StartTime)
                .ToListAsync(); // Execute query here to bring data into memory

            // 2. Format specifically for the UI safely in memory
            return rawAppointments.Select(a => new AppointmentListDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                PatientInitials = GetInitials(a.Patient.FullName),
                AppointmentType = a.AppointmentType,
                DisplayTime = a.StartTime.ToString("hh:mm tt"), // e.g., "09:00 AM"
                Status = a.Status,
                Priority = a.Priority
            }).ToList();
        }

        // Powers the "Daily Schedule Summary" card on the right
        public async Task<DailyScheduleSummaryDto> GetDailyScheduleSummaryAsync(Guid doctorId)
        {
            var today = DateTime.UtcNow.Date;

            var query = await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.DoctorId == doctorId && a.StartTime.Date == today);

            // We can do counts directly in the database for maximum performance!
            var total = await query.CountAsync();
            var confirmed = await query.CountAsync(a => a.Status == EnAppointmentStatus.Booked);
            var waiting = await query.CountAsync(a => a.Status == EnAppointmentStatus.Arrived);
            var urgent = await query.CountAsync(a => a.Priority == EnAppointmentPriority.Urgent || a.Priority == EnAppointmentPriority.Stat);

            return new DailyScheduleSummaryDto
            {
                TotalAppointments = total,
                Confirmed = confirmed,
                Waiting = waiting,
                Urgent = urgent
            };
        }

        // Helper method to extract "SJ" from "Sarah Johnson"
        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "UK"; // Unknown

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0][..1].ToUpper();

            return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
        }
        public async Task<DoctorAvailabilityDto> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime selectedDate)
        {
            // 1. Fetch the doctor to get their exact schedule rules
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdAsync(doctorId);
            if (doctor == null) throw new KeyNotFoundException("Doctor not found.");

            var startOfWeek = selectedDate.Date.AddDays(-(int)selectedDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            // 2. Fetch all existing appointments for the ENTIRE WEEK
            var query = await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.DoctorId == doctorId &&
                                      a.StartTime >= startOfWeek &&
                                      a.StartTime < endOfWeek &&
                                      a.Status != EnAppointmentStatus.Cancelled);

            var weeklyAppointments = await query.ToListAsync();

            var result = new DoctorAvailabilityDto();
            var slotDuration = TimeSpan.FromMinutes(30);

            // 3. Loop through all 7 days of the calendar week
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startOfWeek.AddDays(i);
                string currentDayName = currentDate.DayOfWeek.ToString(); // e.g., "Monday"

                // Check if the doctor actually works on this day!
                if (!doctor.AvailabeDays.Contains(currentDayName))
                {
                    result.WeeklyAvailability.Add(new DayAvailabilityDto { Date = currentDate.Date, HasAvailableSlots = false });
                    continue; // Skip to the next day
                }

                var dailyAppointments = weeklyAppointments.Where(a => a.StartTime.Date == currentDate.Date).ToList();
                bool hasAvailableSlot = false;
                var dailySlots = new List<TimeSlotDto>();

                // 4. Loop through the specific shifts the doctor defined (e.g., Morning Shift, Evening Shift)
                foreach (var shift in doctor.AvailableTimeSlots)
                {
                    var currentSlot = currentDate.Date.Add(shift.StartTime);
                    var endOfShift = currentDate.Date.Add(shift.EndTime);

                    // Generate the 30-minute chunks for this specific shift
                    while (currentSlot < endOfShift)
                    {
                        var slotEndTime = currentSlot.Add(slotDuration);

                        bool isBooked = dailyAppointments.Any(a =>
                            (currentSlot >= a.StartTime && currentSlot < a.EndTime) ||
                            (a.StartTime >= currentSlot && a.StartTime < slotEndTime));

                        if (currentSlot < DateTime.UtcNow) isBooked = true;

                        if (!isBooked) hasAvailableSlot = true;

                        if (currentDate.Date == selectedDate.Date)
                        {
                            dailySlots.Add(new TimeSlotDto
                            {
                                DisplayTime = currentSlot.ToString("hh:mm tt"),
                                StartTime = currentSlot,
                                EndTime = slotEndTime,
                                IsAvailable = !isBooked
                            });
                        }

                        currentSlot = slotEndTime;
                    }
                }

                result.WeeklyAvailability.Add(new DayAvailabilityDto
                {
                    Date = currentDate.Date,
                    HasAvailableSlots = hasAvailableSlot
                });

                if (currentDate.Date == selectedDate.Date)
                {
                    result.SelectedDaySlots = dailySlots;
                }
            }

            return result;
        }

        public async Task<IEnumerable<DoctorSearchResultDto>> SearchDoctorsAsync(string query, int count = 20)
        {
            // 1. Type the variable as IQueryable<Doctor> and REMOVE the .Include()
            IQueryable<Doctor> dbQuery = await unitOfWork.Repository<Doctor>().FindAsQueryable(d => true);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim();

                // 2. The explicit cast is gone. This now works perfectly.
                dbQuery = dbQuery.Where(d =>
                    d.FullName.Contains(term) ||
                    d.SpecialtyCatalog.DisplayName.Contains(term));
            }

            // 3. Map to the lightweight search DTO 
            return await dbQuery
                .Take(count)
                .Select(d => new DoctorSearchResultDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Specialty = d.SpecialtyCatalog.DisplayName,
                    ProfilePhotoUrl = d.ProfileImageUrl ?? ""
                })
                .ToListAsync();
        }

        public async Task<DoctorDetailsDto?> GetDoctorDetailsAsync(Guid id)
        {
            var dbQuery = await unitOfWork.Repository<Doctor>().FindAsQueryable(d => d.Id == id);

            var doctor = await dbQuery
                .Include(d => d.SpecialtyCatalog)
                .FirstOrDefaultAsync();

            if (doctor == null) return null;

            // Map the entity to the DTO
            return new DoctorDetailsDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Specialty = doctor.SpecialtyCatalog.DisplayName,
                ClinicAddress = doctor.ClinicAddress ?? "",
                ConsultationFees = doctor.ConsultationFee,
                YearsOfExperience = doctor.YearsOfExperience,
                ProfilePhotoUrl = doctor.ProfileImageUrl ?? ""
            };
        }

        // 1. Update the signature to accept the optional patientId
        public async Task<AvailableDaysDto> GetAvailableDaysInMonthAsync(Guid doctorId, int year, int month, Guid? patientId = null)
        {
            var result = new AvailableDaysDto { Year = year, Month = month, AvailableDays = new List<int>() };

            var query = await unitOfWork.Repository<Doctor>()
                .FindAsQueryable(d => d.Id == doctorId);

            var doctor = await query.FirstOrDefaultAsync();

            if (doctor == null || doctor.AvailabeDays == null || !doctor.AvailableTimeSlots.Any())
                return result;

            var firstDayOfMonth = new DateTime(year, month, 1);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var endOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);
            var today = DateTime.Today;

            var appointmentsQuery = await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.DoctorId == doctorId &&
                                      a.StartTime >= firstDayOfMonth &&
                                      a.StartTime <= endOfMonth &&
                                      a.Status != EnAppointmentStatus.Cancelled);

            var monthlyAppointments = await appointmentsQuery.ToListAsync();

            int totalPossibleSlotsPerDay = 0;
            var slotDuration = TimeSpan.FromMinutes(30);
            foreach (var shift in doctor.AvailableTimeSlots)
            {
                totalPossibleSlotsPerDay += (int)((shift.EndTime - shift.StartTime).TotalMinutes / slotDuration.TotalMinutes);
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateTime(year, month, day);

                // Rule A: Skip days in the past
                if (currentDate < today) continue;

                // Rule B: Skip days the doctor doesn't work
                string currentDayName = currentDate.DayOfWeek.ToString();
                if (!doctor.AvailabeDays.Contains(currentDayName)) continue;

                // Rule C: (NEW) Check if the patient already has a booking on this specific day
                if (patientId.HasValue && patientId.Value != Guid.Empty)
                {
                    bool hasPatientAlreadyBooked = monthlyAppointments.Any(a =>
                        a.StartTime.Date == currentDate.Date &&
                        a.PatientId == patientId.Value);

                    // If they already have an appointment today, skip this day entirely
                    if (hasPatientAlreadyBooked)
                    {
                        continue;
                    }
                }

                // Rule D: Count overall booked appointments to see if the doctor is fully booked
                var bookedCount = monthlyAppointments.Count(a => a.StartTime.Date == currentDate.Date);

                if (bookedCount < totalPossibleSlotsPerDay)
                {
                    result.AvailableDays.Add(day);
                }
            }

            return result;
        }

        // 1. Update the signature to accept an optional patientId
        public async Task<DayTimeSlotsDto> GetTimeSlotsForDayAsync(Guid doctorId, DateTime date, Guid? patientId = null)
        {
            var result = new DayTimeSlotsDto { Date = date.Date, Slots = new List<TimeSlotDto>() };

            var doctorQuery = await unitOfWork.Repository<Doctor>().FindAsQueryable(d => d.Id == doctorId);
            var doctor = await doctorQuery.FirstOrDefaultAsync();

            if (doctor == null) throw new KeyNotFoundException("Doctor not found.");

            if (!doctor.AvailabeDays.Contains(date.DayOfWeek.ToString()))
            {
                return result;
            }

            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            var query = await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.DoctorId == doctorId &&
                                      a.StartTime >= startOfDay &&
                                      a.StartTime < endOfDay &&
                                      a.Status != EnAppointmentStatus.Cancelled);

            var dailyAppointments = await query.ToListAsync();

            // 2. NEW: Check if the patient already has a booking today with this doctor
            bool hasPatientAlreadyBookedToday = false;
            if (patientId.HasValue && patientId.Value != Guid.Empty)
            {
                hasPatientAlreadyBookedToday = dailyAppointments.Any(a => a.PatientId == patientId.Value);
            }

            var slotDuration = TimeSpan.FromMinutes(30);
            var now = DateTime.UtcNow; // Ensure this matches your server's timezone configuration

            foreach (var shift in doctor.AvailableTimeSlots)
            {
                var currentSlot = startOfDay.Add(shift.StartTime);
                var endOfShift = startOfDay.Add(shift.EndTime);

                while (currentSlot < endOfShift)
                {
                    var slotEndTime = currentSlot.Add(slotDuration);

                    // Check if slot overlaps with ANY booked appointment
                    bool isBooked = dailyAppointments.Any(a =>
                        (currentSlot >= a.StartTime && currentSlot < a.EndTime) ||
                        (a.StartTime >= currentSlot && a.StartTime < slotEndTime));

                    // Block slots that have already passed
                    if (currentSlot < now)
                    {
                        isBooked = true;
                    }

                    // 3. NEW: If the patient already booked today, block all remaining slots to prevent double-booking
                    if (hasPatientAlreadyBookedToday)
                    {
                        isBooked = true;
                    }

                    result.Slots.Add(new TimeSlotDto
                    {
                        StartTime = currentSlot,
                        EndTime = slotEndTime,
                        DisplayTime = currentSlot.ToString("hh:mm tt"),
                        IsAvailable = !isBooked
                    });

                    currentSlot = slotEndTime;
                }
            }

            return result;
        }
        public async Task<List<AppointmentListDto>> GetDoctorAppointmentsAsync(Guid doctorId, string filter)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            DateTime startDate = today;
            DateTime endDate = today;
            bool urgentOnly = false;

            // 1. Determine date ranges
            switch (filter.ToLower())
            {
                case "tomorrow":
                    startDate = today.AddDays(1);
                    endDate = startDate;
                    break;
                case "past":
                    startDate = today.AddDays(-365);
                    endDate = today;
                    break;
                case "week":
                    endDate = today.AddDays(7);
                    break;
                case "urgent":
                    urgentOnly = true;
                    break;
            }

            // 2. Query the database
            // Optimized the Date comparison for better SQL indexing
            var query = await unitOfWork.Repository<Appointment>().FindAsQueryable(a =>
                a.DoctorId == doctorId &&
                a.Status != EnAppointmentStatus.Cancelled &&
                (urgentOnly
                    ? (a.Priority == EnAppointmentPriority.Urgent || a.Priority == EnAppointmentPriority.Stat)
                    : (a.StartTime >= startDate && a.StartTime < endDate.AddDays(1))));

            var rawAppointments = await query.Include(a => a.Patient).ToListAsync();

            // 3. Auto-Update Past Appointments
            bool requiresDatabaseSave = false;

            foreach (var apt in rawAppointments)
            {
                // If the appointment time has passed and it is still marked as Scheduled/Active
                if (apt.StartTime < now && apt.Status == EnAppointmentStatus.Pending)
                {
                    apt.Status = EnAppointmentStatus.Fulfilled;

                    await unitOfWork.Repository<Appointment>().UpdateAsync(apt);
                    requiresDatabaseSave = true;
                }
            }

            // Only hit the database with a commit if we actually changed something
            if (requiresDatabaseSave)
            {
                await unitOfWork.CompleteAsync();
            }

            // 4. Map to DTOs and return
            return rawAppointments
                .OrderBy(a => a.StartTime)
                .Select(a => new AppointmentListDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.FullName ?? "Unknown",
                    PatientInitials = GetInitials(a.Patient?.FullName),
                    AppointmentType = a.AppointmentType ?? "General Checkup",
                    DisplayTime = a.StartTime.ToString("hh:mm tt"),
                    Status = a.Status
                })
                .ToList();
        }

        public async Task<List<AppointmentListDto>> GetHighPriorityPatientsTodayAsync(Guid doctorId)
        {
            var today = DateTime.UtcNow.Date;

            var query = await unitOfWork.Repository<Appointment>().FindAsQueryable(a =>
                a.DoctorId == doctorId &&
                a.StartTime.Date == today &&
                (a.Priority == EnAppointmentPriority.Urgent || a.Priority == EnAppointmentPriority.Stat) &&
                a.Status != EnAppointmentStatus.Fulfilled &&
                a.Status != EnAppointmentStatus.Cancelled);

            var rawAppointments = await query.Include(a => a.Patient).ToListAsync();

            return rawAppointments
                .OrderBy(a => a.StartTime)
                .Take(5)
                .Select(a => new AppointmentListDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.FullName ?? "Unknown",
                    PatientInitials = GetInitials(a.Patient?.FullName),
                    AppointmentType = $"Urgent - {a.AppointmentType ?? "Assessment"}",
                    DisplayTime = a.StartTime.ToString("hh:mm tt"),
                    Status = a.Status
                })
                .ToList();
        }

        public async Task<PatientDashboardDto> GetPatientDashboardSummaryAsync(Guid appointmentId, Guid doctorId)
        {
            // 1. Fetch Current Appointment & Patient
            var appointments = await unitOfWork.Repository<Appointment>().FindAsync(a => a.Id == appointmentId);
            var appointment = appointments.FirstOrDefault();

            if (appointment == null) throw new KeyNotFoundException("Appointment not found.");
            if (appointment.DoctorId != doctorId) throw new UnauthorizedAccessException("Not allowed to view this data.");

            var patientId = appointment.PatientId;

            var patientQuery = await unitOfWork.Repository<Patient>().FindAsQueryable(p => p.Id == patientId);
            var patient = await patientQuery
                .Include(p => p.AppUser)
                .Include(p => p.Insurance) // CRITICAL: Added missing Include so patient.Insurance doesn't crash
                .Include(p => p.Allergies).ThenInclude(a => a.AllergenCatalog)
                .Include(p => p.ChronicDiseases).ThenInclude(ch => ch.ChronicDiseaseCatalog)
                .Include(p => p.Medications).ThenInclude(m => m.medicationCatalog)
                .AsNoTracking() // Performance boost
                .FirstOrDefaultAsync();

            if (patient == null) throw new KeyNotFoundException("Patient not found.");

            // Safe null-coalescing in case lists are empty
            var allergies = patient.Allergies ?? new List<Allergy>();
            var chronicList = patient.ChronicDiseases;

            // FIX: Show ACTIVE medications (EndDate is null OR in the future) instead of expired ones
            var todayDate = DateOnly.FromDateTime(DateTime.Now);
            var medications = patient.Medications?
                .Where(m => m.EndDate == null || m.EndDate >= todayDate)
                .ToList() ?? new List<Medication>();

            // 4. Fetch Last Visit (The most recent 'Fulfilled' appointment)
            var pastAppointments = await unitOfWork.Repository<Appointment>()
                .FindAsync(a => a.PatientId == patientId && a.Status == EnAppointmentStatus.Fulfilled);

            var lastAppointment = pastAppointments.OrderByDescending(a => a.StartTime).FirstOrDefault();

            LastVisitSummaryDto? lastVisitDto = null;
            if (lastAppointment != null)
            {
                // If a past visit exists, map its details
                lastVisitDto = new LastVisitSummaryDto
                {
                    AppointmentTitle = lastAppointment.AppointmentType ?? "General Checkup",
                    AppointmentDate = lastAppointment.StartTime.ToString("MM-dd-yyyy"),
                    PrescriptionSummary = "View active medications list",
                    ClinicalNoteSummary = "Routine visit completed.",
                    ClinicalNoteDate = GetRelativeDateString(lastAppointment.StartTime),
                    LabResultSummary = "No recent labs",
                    LabResultDate = ""
                };
            }

            // Safely parse National ID to prevent hard crashes on invalid data
            int patientAge = 0;
            string patientGender = "Unknown";
            if (!string.IsNullOrWhiteSpace(patient.NationalId))
            {
                try
                {
                    var idData = patient.NationalId.ParseEgyptianId();
                    patientAge = CalculateAge(idData.dateOfBirth);
                    patientGender = idData.gender.ToString();
                }
                catch
                {
                    // Fallback if NationalId format is invalid
                }
            }

            // 5. Construct the final Payload
            return new PatientDashboardDto
            {
                PatientId = patientId,
                FullName = patient.FullName ?? "Unknown",
                Initials = GetInitials(patient.FullName),
                DisplayId = patient.Id.ToString()[..7].ToUpper(), // Fallback if no custom ID
                Age = patientAge,
                Gender = patientGender,
                Phone = patient.AppUser?.PhoneNumber ?? "No Phone",
                Email = patient.AppUser?.Email ?? "No Email",
                InsuranceProvider = patient.Insurance?.InsuranceProvider ?? "Self-Pay",
                PatientStatus = "Active Patient",

                KnownAllergies = allergies.Select(a => a.AllergenCatalog?.DisplayName ?? "Unknown").ToList(),
                ChronicConditions = chronicList.Select(ch => ch.ChronicDiseaseCatalog?.DisplayName ?? "Unknown").ToList(),
                CurrentMedications = medications.Select(m => new MedicationDto
                {
                    Rxcui = m.medicationCatalogRxcui,
                    Name = m.medicationCatalog?.DrugName ?? "Unknown",
                    Dosage = m.Dosage,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate ?? default // Safe fallback
                }).ToList(),

                LastVisit = lastVisitDto,
                IsMedicalHistoryLocked = true // Keeps the bottom timeline blurred
            };
        }

        // Helper to calculate accurate age
        private int CalculateAge(DateOnly dateOfBirth)
        {
            // Convert today's DateTime into a DateOnly for safe comparison
            var today = DateOnly.FromDateTime(DateTime.Today);

            var age = today.Year - dateOfBirth.Year;

            // DateOnly supports AddYears, so this comparison now works perfectly
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        private string GetRelativeDateString(DateTime date)
        {
            // We use .Date to strip the time. This prevents a bug where an event at 
            // 11:00 PM yesterday evaluates to "0 days ago" if checked at 8:00 AM today.
            var today = DateTime.UtcNow.Date;
            var targetDate = date.Date;

            var span = today - targetDate;

            if (span.Days == 0) return "Today";
            if (span.Days == 1) return "Yesterday";
            if (span.Days < 7) return $"{span.Days} days ago";

            if (span.Days < 30)
            {
                int weeks = span.Days / 7;
                return weeks == 1 ? "1 week ago" : $"{weeks} weeks ago";
            }

            if (span.Days < 365)
            {
                int months = span.Days / 30;
                return months == 1 ? "1 month ago" : $"{months} months ago";
            }

            // Fallback for events older than a year
            return date.ToString("MMM d, yyyy"); // e.g., "Jan 15, 2025"
        }
    }
}
