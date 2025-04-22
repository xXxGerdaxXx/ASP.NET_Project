using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Hubs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class NotificationService(AppDbContext context, IHubContext<NotificationHub> notificationHub) : INotificationService
    {
        private readonly AppDbContext _context = context;
        private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

        public async Task AddNotificationAsync(NotificationEntity notificationEntity, string userId = "anonymous")
        {
            if (string.IsNullOrEmpty(notificationEntity.Icon))
            {
                switch (notificationEntity.NotificationTypeId)
                {
                    case 1:
                        notificationEntity.Icon = "/images/user-template.svg";
                        break;
                    case 2:
                        notificationEntity.Icon = "~/images/project-template.svg";
                        break;
                    case 3:
                        notificationEntity.Icon = "~/images/user-template.svg";
                        break;
                }
            }

            _context.Add(notificationEntity);
            await _context.SaveChangesAsync();

            var notifications = await GetNotificationsAsync(userId);
            var newNotification = notifications.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            if (newNotification != null)
            {
                await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
            }

        }

        public async Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 10)
        {
            var dismissedIds = await _context.DismissedNotifications
                .Where(x => x.UserId == userId)
                .Select(x => x.NotificationId)
                .ToListAsync();

            var notifications = await _context.Notifications
                .Where(x => !dismissedIds.Contains(x.Id))
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync();

            return notifications;
        }


        public async Task AddNotificationAsync(int notificationTypeId, string message, string? image = null, int notificationTargetGroup = 1)
        {
            var notification = new NotificationEntity
            {
                NotificationTypeId = notificationTypeId,
                Message = message,
                Icon = image, 
                NotificationTargetGroupId = notificationTargetGroup,
                CreatedAt = DateTime.UtcNow
            };

            await AddNotificationAsync(notification);
        }

        public async Task<bool> DismissNotificationAsync(string notificationId, string userId)
        {
            var notification = await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
                return false;

            var alreadyDismissed = await _context.DismissedNotifications
                .AnyAsync(x => x.NotificationId == notificationId && x.UserId == userId);

            if (alreadyDismissed)
                return true; 

            var dismissed = new NotificationDismissedEntity
            {
                NotificationId = notificationId,
                UserId = userId,
                DismissedAt = DateTime.UtcNow
            };

            _context.DismissedNotifications.Add(dismissed);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
