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

        public async Task AddNotificationAsync(NotificationEntity notificationEntity)
        {
            if (string.IsNullOrEmpty(notificationEntity.Icon))
            {
                switch (notificationEntity.NotificationTypeId)
                {
                    case 1: // UserLogin
                    case 2: // UserSignup
                        notificationEntity.Icon = "~/images/user-template.svg";
                        break;
                    case 3: // ProjectAdded
                        notificationEntity.Icon = "~/images/project-template.svg";
                        break;
                }
            }


            _context.Add(notificationEntity);
            await _context.SaveChangesAsync();

            if (notificationEntity.NotificationTargetGroupId == 1) 
            {
                await _notificationHub.Clients.Group("Admins").SendAsync("ReceiveNotification", notificationEntity);
            }
            else if (notificationEntity.NotificationTargetGroupId == 2) 
            {
                await _notificationHub.Clients.Group("Users").SendAsync("ReceiveNotification", notificationEntity);
            }
        }


        public async Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int targetGroupId, int take = 10)
        {
            var dismissedIds = await _context.DismissedNotifications
                .Where(x => x.UserId == userId)
                .Select(x => x.NotificationId)
                .ToListAsync();

            var notifications = await _context.Notifications
                .Where(x => !dismissedIds.Contains(x.Id) && x.NotificationTargetGroupId == targetGroupId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync();

            return notifications;
        }



        //public async Task<bool> DismissNotificationAsync(string notificationId, string userId)
        //{
        //    var notification = await _context.Notifications
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(n => n.Id == notificationId);

        //    if (notification == null)
        //        return false;

        //    var alreadyDismissed = await _context.DismissedNotifications
        //        .AnyAsync(x => x.NotificationId == notificationId && x.UserId == userId);

        //    if (alreadyDismissed)
        //        return true; 

        //    var dismissed = new NotificationDismissedEntity
        //    {
        //        NotificationId = notificationId,
        //        UserId = userId,
        //        DismissedAt = DateTime.UtcNow
        //    };

        //    _context.DismissedNotifications.Add(dismissed);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task DismissNotificationAsync(string notificationId, string userId)
        {
            var alreadyDismissed = await _context.DismissedNotifications
                .AnyAsync(x => x.NotificationId == notificationId && x.UserId == userId);

            if (!alreadyDismissed)
            {
                var dismissed = new NotificationDismissedEntity
                {
                    NotificationId = notificationId,
                    UserId = userId,
                };
                _context.DismissedNotifications.Add(dismissed);
                await _context.SaveChangesAsync();
            }
        }
    }
}
