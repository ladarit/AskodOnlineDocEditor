using System.Threading.Tasks;
using AskodOnline.Editor.Models;
using AskodOnline.Editor.SignalRManagment.Hubs;
using Microsoft.AspNet.SignalR;

namespace AskodOnline.Editor.SignalRManagment
{
    public class NotificationStore
    {
        private static IHubContext HubContext => GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

        public static async Task SendMessage(UserModel user, string groupId, string command = null)
        {
            await HubContext.Clients.Group(groupId, user.Id).updateSpreadsheetPartialView(user.Name, command);
        }

        public static async Task SendTypingNotification(UserModel user, string groupId, string command = null)
        {
            if (command == "showTypingAnimation")
            {
                await HubContext.Clients.Group(groupId, user.Id).showTypingAnimation(user.Id, command);
            }
            else if (command == "hideTypingAnimation")
            {
                await HubContext.Clients.Group(groupId, user.Id).hideTypingAnimation(user.Id, command);
            }
        }
    }
}