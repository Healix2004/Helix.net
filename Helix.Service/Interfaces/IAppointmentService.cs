using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.DTOs.PatientDTOs;

namespace Helix.Service.Interfaces
{
    public interface IAppointmentService
    {
        Task<Guid> ScheduleAppointmentAsync(CreateAppointmentDto dto);
        Task<bool> UpdateAppointmentStatusAsync(Guid id, EnAppointmentStatus newStatus);
        // Dashboard Endpoints
        Task<List<AppointmentListDto>> GetDoctorAppointmentsForTodayAsync(Guid doctorId);
        Task<DailyScheduleSummaryDto> GetDailyScheduleSummaryAsync(Guid doctorId);
        Task<DoctorAvailabilityDto> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime selectedDate);

        Task<IEnumerable<DoctorSearchResultDto>> SearchDoctorsAsync(string query, int count = 20);
        Task<DoctorDetailsDto?> GetDoctorDetailsAsync(Guid id);

        Task<AvailableDaysDto> GetAvailableDaysInMonthAsync(Guid doctorId, int year, int month, Guid? patientId = null);
        Task<DayTimeSlotsDto> GetTimeSlotsForDayAsync(Guid doctorId, DateTime date, Guid? patientId = null);
        Task<List<AppointmentListDto>> GetDoctorAppointmentsAsync(Guid doctorId, string filter);
        Task<List<AppointmentListDto>> GetHighPriorityPatientsTodayAsync(Guid doctorId);

        Task<List<AppointmentListDto>> GetDoctorDayAppointmentAsync(Guid doctorId, DateTime date);
        Task<PatientDashboardDto> GetPatientDashboardSummaryAsync(Guid appointmentId, Guid doctorId);
    }
}