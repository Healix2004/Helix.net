using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
