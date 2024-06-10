using Microsoft.AspNetCore.SignalR;

namespace ThisPresentationDoesNotExist.Hubs;

public class PresentationHub : Hub
{
    public async Task Ping(string user, string message)
    {
        await Clients.All.SendAsync("Pong", user, message);
    }
    
    public async Task SyncSlide(int slide)
    {
        await Clients.Others.SendAsync("GotoSlide", slide);
    }
}