using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.AppointmentService
{
    public class AppointmentService(IUnitOfWork unitOfWork) : IAppointmentService
    {
        public async Task<Guid> ScheduleAppointmentAsync(CreateAppointmentDto dto)
        {
            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
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
                .Include(a => a.Patient)
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

        public async Task<AvailableDaysDto> GetAvailableDaysInMonthAsync(Guid doctorId, int year, int month)
        {
            var result = new AvailableDaysDto { Year = year, Month = month, AvailableDays = new List<int>() };

            // 1. Fetch Doctor with their shifts (Use Include if AvailableTimeSlots is a separate table)
            var query = await unitOfWork.Repository<Doctor>()
                .FindAsQueryable(d => d.Id == doctorId);

            // Make sure to eagerly load the shifts if they are in a related table!
            // query = query.Include(d => d.AvailableTimeSlots); 

            var doctor = await query.FirstOrDefaultAsync();

            if (doctor == null || doctor.AvailabeDays == null || !doctor.AvailableTimeSlots.Any())
                return result; // Doctor not found or hasn't set up a schedule

            var firstDayOfMonth = new DateTime(year, month, 1);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var endOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);
            var today = DateTime.Today;

            // 2. Fetch all appointments for the ENTIRE MONTH in one fast query
            var appointmentsQuery = await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.DoctorId == doctorId &&
                                      a.StartTime >= firstDayOfMonth &&
                                      a.StartTime <= endOfMonth &&
                                      a.Status != EnAppointmentStatus.Cancelled);

            var monthlyAppointments = await appointmentsQuery.ToListAsync();

            // 3. Calculate how many total slots the doctor can take in a single day
            int totalPossibleSlotsPerDay = 0;
            var slotDuration = TimeSpan.FromMinutes(30);
            foreach (var shift in doctor.AvailableTimeSlots)
            {
                totalPossibleSlotsPerDay += (int)((shift.EndTime - shift.StartTime).TotalMinutes / slotDuration.TotalMinutes);
            }

            // 4. Loop through the days of the month to check availability
            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateTime(year, month, day);

                // Rule A: Skip days in the past
                if (currentDate < today) continue;

                // Rule B: Skip days the doctor doesn't work (e.g., Weekends)
                string currentDayName = currentDate.DayOfWeek.ToString(); // e.g., "Monday"
                if (!doctor.AvailabeDays.Contains(currentDayName)) continue;

                // Rule C: Count booked appointments for this specific day
                var bookedCount = monthlyAppointments.Count(a => a.StartTime.Date == currentDate);

                // If the doctor has fewer bookings than their maximum capacity, the day is available!
                if (bookedCount < totalPossibleSlotsPerDay)
                {
                    result.AvailableDays.Add(day);
                }
            }

            return result;
        }


        public async Task<DayTimeSlotsDto> GetTimeSlotsForDayAsync(Guid doctorId, DateTime date)
        {
            var result = new DayTimeSlotsDto { Date = date.Date, Slots = new List<TimeSlotDto>() };

            // 1. Fetch Doctor configuration
            var doctorQuery = await unitOfWork.Repository<Doctor>().FindAsQueryable(d => d.Id == doctorId);
            var doctor = await doctorQuery.FirstOrDefaultAsync();

            if (doctor == null) throw new KeyNotFoundException("Doctor not found.");

            // Check if the doctor actually works on this day before querying appointments
            if (!doctor.AvailabeDays.Contains(date.DayOfWeek.ToString()))
            {
                return result; // Returns empty list of slots because doctor is off
            }

            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            // 2. Fetch appointments for ONLY THIS SINGLE DAY (Massive performance boost)
            var query = await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.DoctorId == doctorId &&
                                      a.StartTime >= startOfDay &&
                                      a.StartTime < endOfDay &&
                                      a.Status != EnAppointmentStatus.Cancelled);

            var dailyAppointments = await query.ToListAsync();

            var slotDuration = TimeSpan.FromMinutes(30);

            // Note: If your system uses local time (Egypt UTC+2/+3), ensure this is DateTime.Now
            var now = DateTime.UtcNow;

            // 3. Loop through the specific shifts (e.g., Morning Shift, Evening Shift)
            foreach (var shift in doctor.AvailableTimeSlots)
            {
                var currentSlot = startOfDay.Add(shift.StartTime);
                var endOfShift = startOfDay.Add(shift.EndTime);

                // Generate the 30-minute chunks for this specific shift
                while (currentSlot < endOfShift)
                {
                    var slotEndTime = currentSlot.Add(slotDuration);

                    // Check if slot overlaps with any booked appointment
                    bool isBooked = dailyAppointments.Any(a =>
                        (currentSlot >= a.StartTime && currentSlot < a.EndTime) ||
                        (a.StartTime >= currentSlot && a.StartTime < slotEndTime));

                    // Block slots that have already passed if viewing today's date
                    if (currentSlot < now)
                    {
                        isBooked = true;
                    }


                    result.Slots.Add(new TimeSlotDto
                    {
                        StartTime = currentSlot,
                        EndTime = slotEndTime,
                        DisplayTime = currentSlot.ToString("hh:mm tt"), // e.g., "08:30 AM"
                        IsAvailable = !isBooked
                    });

                    currentSlot = slotEndTime; // Move to next slot
                }
            }

            return result;
        }
    }
}
