using Microsoft.AspNetCore.SignalR;


namespace Bislerium.Infrastructure.SignalHubs
{
    public class PushNotification : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

