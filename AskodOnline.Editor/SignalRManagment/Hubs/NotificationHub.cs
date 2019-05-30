using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AskodOnline.Editor.Business.Attributes;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Helpers;
using AskodOnline.Editor.Models;
using AskodOnline.Security;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace AskodOnline.Editor.SignalRManagment.Hubs
{
    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {
        protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IUserStore UserStore { get; }

        private IUsersGroupsStore UsersGroupsStore { get; }

        public NotificationHub(IUserStore userStore, IUsersGroupsStore usersGroupsStore)
        {
            UserStore = userStore;
            UsersGroupsStore = usersGroupsStore;
        }

        [RequiredDbConnection]
        public override async Task OnConnected()
        {
            try
            {
                //fill custom collections for tracking users and groups
                var authCookie = Context.Request.Cookies["AskodOnlineDocEditor"]?.Value;
                if (!string.IsNullOrEmpty(authCookie))
                {
                    var decyptedMetaData = CryptManager.Decrypt(authCookie);
                    var metadata = JsonConvert.DeserializeObject<MetaDataModel>(decyptedMetaData);

                    //getting user
                    var counter = metadata.UserCounter;
                    var userEntity = await UserStore.GetUserByCounterAsync(counter);
                    if (userEntity != null)
                    {
                        var userModel = UserStore.CreateUserModel(userEntity, Context);

                        var roomId = Context.QueryString["group"];
                        var existingRoom = UsersGroupsStore.Find(roomId).FirstOrDefault();
                        if (existingRoom != null)
                        {
                            var existingUser = UsersGroupsStore.FindUserInRoomByCounter(existingRoom, userModel.Counter);
                            if (existingUser != null)
                            {
                                UsersGroupsStore.RemoveUserFromRoom(existingRoom, existingUser);
                            }
                            UsersGroupsStore.AddUserToRoom(existingRoom, userModel);
                        }
                        else
                        {
                            UsersGroupsStore.CreateRoom(new UserGroupModel
                            {
                                Id = roomId,
                                DocumentCounter = long.Parse(roomId.Split('_').Last()),
                                Users = new SynchronizedCollection<UserModel> { userModel }
                            });
                        }
                        DeleteEmptyRooms();

                        var usersInGroup = UsersGroupsStore.GetUsersViewModelInGroup(roomId);
                        bool.TryParse(Context.QueryString["isReconnecting"], out var isUserReconnecting);
                        if (isUserReconnecting)
                        {
                            Clients.Caller.onReconnected(usersInGroup);
                        }
                        else
                        {
                            Clients.Caller.onConnected(usersInGroup);
                        }
                        Clients.OthersInGroup(roomId).onNewUserConnected(new UserViewModel { Id = userModel.Id, Name = userModel.Name, Avatar = userModel.AvatarPath });

                        //join signalR group
                        await JoinGroup(Context.ConnectionId, Context.QueryString["group"]);
                        await base.OnConnected();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            //remove user from custom collections
            var roomId = Context.QueryString["group"];
            var existingRoom = UsersGroupsStore.Find(roomId).FirstOrDefault();
            if (existingRoom != null)
            {
                var existingUser = UsersGroupsStore.FindUserInRoomByConnectionId(existingRoom, Context.ConnectionId);
                if (existingUser != null)
                {
                    UsersGroupsStore.RemoveUserFromRoom(existingRoom, existingUser);
                }
                DeleteEmptyRooms();
            }
            Clients.OthersInGroup(roomId).onUserDisconnected(Context.ConnectionId);

            await base.OnDisconnected(stopCalled);
        }

        private void DeleteEmptyRooms()
        {
            var emptyGroups = UsersGroupsStore.GetEmptyRecords();
            if (emptyGroups.Any())
            {
                for (int i = 0; i < emptyGroups.Count; i++)
                {
                    UsersGroupsStore.RemoveRecord(emptyGroups[i]);
                }
            }
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }

        public async Task JoinGroup(string userId, string roomName)
        {
            try
            {
                await Groups.Add(userId, roomName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public async Task LeaveGroup(string userId, string roomName)
        {
            try
            {
                await Groups.Remove(userId, roomName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public async Task SendMessageToClients(string command)
        {
            await RunIfUsersInGroupExists(command, NotificationStore.SendMessage);
        }

        public async Task SendTypingNotificationToClients(string command)
        {
            await RunIfUsersInGroupExists(command, NotificationStore.SendTypingNotification);
        }

        private async Task RunIfUsersInGroupExists(string command, Func<UserModel, string, string, Task> method)
        {
            var roomId = Context.QueryString["group"];
            var existingRoom = UsersGroupsStore.Find(roomId).FirstOrDefault();
            if (existingRoom != null)
            {
                var existingUser = UsersGroupsStore.FindUserInRoomByConnectionId(existingRoom, Context.ConnectionId);
                if (existingUser != null && !string.IsNullOrEmpty(roomId))
                {
                    await method(existingUser, roomId, command);
                }
            }
        }
    }
}