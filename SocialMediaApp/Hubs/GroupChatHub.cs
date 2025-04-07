using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SocialMediaApp.Hubs
{
    public class GroupChatHub : Hub
    {
        public async Task SendMessage(string groupId, string user, string message)
        {
            await Clients.Group(groupId).SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var groupId = httpContext?.Request.Query["groupId"];

            if (!string.IsNullOrEmpty(groupId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var groupId = httpContext?.Request.Query["groupId"];

            if (!string.IsNullOrEmpty(groupId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}


