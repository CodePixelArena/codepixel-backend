using Microsoft.AspNetCore.SignalR;

namespace codepixel_backend.Hubs
{
    public class PixelHub : Hub
    {
        public async Task SendPixelUpdate(int x, int y, string color, string userId)
        {
            await Clients.All.SendAsync("ReceivePixelUpdate", x, y, color, userId);
        }

        public async Task JoinCanvas()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "canvas");
        }

        public async Task LeaveCanvas()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "canvas");
        }
    }
}