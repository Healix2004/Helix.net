using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;

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
    }
}