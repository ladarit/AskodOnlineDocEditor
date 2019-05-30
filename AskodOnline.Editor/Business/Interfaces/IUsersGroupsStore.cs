using System.Collections.Generic;
using AskodOnline.Editor.Models;

namespace AskodOnline.Editor.Business.Interfaces
{
    public interface IUsersGroupsStore : IBaseStore<UserGroupModel>
    {
        IList<UserViewModel> GetUsersViewModelInGroup(string groupId);

        IList<UserGroupModel> GetEmptyRecords();

        UserModel FindUserInRoomByCounter(UserGroupModel room, long userCounter);

        UserModel FindUserInRoomByConnectionId(UserGroupModel room, string connectionId);

        void RemoveUserFromRoom(UserGroupModel room, UserModel user);

        void AddUserToRoom(UserGroupModel room, UserModel user);

        void CreateRoom(UserGroupModel obj);
    }
}