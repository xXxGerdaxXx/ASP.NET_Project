using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 10);

    Task AddNotificationAsync(NotificationEntity notificationEntity, string userId = "anonymous");

    Task AddNotificationAsync(int notificationTypeId, string message, string? image = null, int notificationTargetGroup = 1);

    Task DismissNotificationAsync(string notificationId, string userId);
}
