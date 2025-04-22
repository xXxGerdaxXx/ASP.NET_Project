using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

//public class NotificationHub : Hub
//{
//    public async Task SendNotification(object notification)
//    {
//        await Clients.All.SendAsync("ReceiveNotification", notification);
//    }
//}
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        if (user != null)
        {
            if (user.IsInRole("Admin"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

            if (user.IsInRole("User"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
        }

        await base.OnConnectedAsync();
    }



}

