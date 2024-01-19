using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Vilau_Paula_Proiect.Models;

namespace ToyStore.Hubs
{
    [Authorize]
    public class ForumHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }
    }
}
