using AutoMapper;
using Helix.Data.Entities;
using Helix.Service.DTOs.NotificationDtos;
using Helix.Service.Interfaces;

namespace Helix.Service.Services.NotificationService
{
    public class NotificationService(IUnitOfWork unitOfWork,IMapper mapper,INotificationDispatcher dispatcher) : INotificationService
    {
        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                AppUserId = dto.AppUserId,
                Title = dto.Title,
                Message = dto.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Repository<Notification>().AddAsync(notification);
            await unitOfWork.CompleteAsync();

            var resultDto = mapper.Map<NotificationDto>(notification);

            // Trigger the interface methods! The service doesn't care HOW this happens.
            await dispatcher.SendNotificationToUserAsync(dto.AppUserId, resultDto);

            var unreadCount = await GetUnreadCountAsync(dto.AppUserId);
            await dispatcher.SendUnreadCountToUserAsync(dto.AppUserId, unreadCount);

            return resultDto;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false)
        {
            var query = await unitOfWork.Repository<Notification>().FindAsQueryable(n => n.AppUserId == userId);
            if (unreadOnly)
            {
                query = query.Where(n => n.IsRead == false);
            }
            var notifications = query
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return mapper.Map<List<NotificationDto>>(notifications);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var query = await unitOfWork.Repository<Notification>().FindAsync(n => n.AppUserId == userId && n.IsRead == false);
            return query.Count();
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId)
        {
            var notification = await unitOfWork.Repository<Notification>().GetByIdAsync(notificationId);
            if (notification == null || notification.IsRead)
                return false;
            notification.IsRead = true;
            await unitOfWork.Repository<Notification>().UpdateAsync(notification);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var query = await unitOfWork.Repository<Notification>().FindAsync(n => n.AppUserId == userId && n.IsRead == false);
            var unreadNotifications = query.ToList();
            if (!unreadNotifications.Any())
                return true;

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                await unitOfWork.Repository<Notification>().UpdateAsync(notification);
            }
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteOldNotificationsAsync(int daysOld = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            var query = await unitOfWork.Repository<Notification>().FindAsync(n => n.CreatedAt < cutoffDate);
            var oldNotifications = query.ToList();
            if (!oldNotifications.Any())
                return true;

            foreach (var notification in oldNotifications)
            {
                await unitOfWork.Repository<Notification>().DeleteAsync(notification);
            }
            return await unitOfWork.CompleteAsync() > 0;
        }
    }
}
