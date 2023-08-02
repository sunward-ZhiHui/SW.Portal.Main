using Application.Constant;
using Microsoft.AspNetCore.SignalR;

namespace SW.Portal.Solutions.Hubs
{
    public class NotificationHub :Hub
    {
        public async Task OnConnectAsync(string userId)
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.ConnectUser, userId);
        }

        public async Task OnDisconnectAsync(string userId)
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.DisconnectUser, userId);
        }
        public async Task UpdateEmailInboxAsync()
        {
            await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveEmail);
        }
    }
}
