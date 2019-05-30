using System.Collections.Generic;
using System.Linq;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Models;

namespace AskodOnline.Editor.Business.Store
{
    public class UsersGroupsStore : BaseStore<UserGroupModel>, IUsersGroupsStore
    {
        public static SynchronizedCollection<UserGroupModel> UsersGroups = new SynchronizedCollection<UserGroupModel>();

        public override SynchronizedCollection<UserGroupModel> GetCollection => UsersGroups;

        #region IBaseStore methods
        public override UserGroupModel CreateRecord(object obj)
        {
            return obj as UserGroupModel;
        }

        public override UserGroupModel CreateRecordAndAddToCollection(object obj)
        {
            return CreateRecordAndAddToCollection(UsersGroups, obj);
        }

        public override IList<UserGroupModel> Find(object id)
        {
            return UsersGroups.Where(t => t.Id.Equals((string)id)).ToList();
        }

        public override void RemoveRecord(UserGroupModel obj)
        {
            RemoveRecord(UsersGroups, obj);
        }
        #endregion

        #region IUsersGroupsStore methods
        public virtual void AddToCollection(SynchronizedCollection<UserModel> collection, UserModel obj)
        {
            collection?.Add(obj);
        }

        public IList<UserViewModel> GetUsersViewModelInGroup(string groupId)
        {
            return UsersGroups.FirstOrDefault(r => r.Id == groupId)?.Users.Select(t => new UserViewModel { Id = t.Id, Name = t.Name, Avatar = t.AvatarPath }).ToList();
        }

        public virtual IList<UserGroupModel> GetEmptyRecords()
        {
            return UsersGroups.Where(r => r.Users.Count == 0).Select(t => t).ToList();
        }

        public UserModel FindUserInRoomByCounter(UserGroupModel room, long userCounter)
        {
            return room.Users.FirstOrDefault(u => u.Counter == userCounter);
        }

        public UserModel FindUserInRoomByConnectionId(UserGroupModel room, string connectionId)
        {
            return room.Users.FirstOrDefault(u => u.Id == connectionId);
        }

        public void RemoveUserFromRoom(UserGroupModel room, UserModel user)
        {
            room.Users.Remove(user);
        }

        /// <summary>
        /// method add user to existing group
        /// </summary>
        /// <param name="room">existing group</param>
        /// <param name="user"></param>
        public void AddUserToRoom(UserGroupModel room, UserModel user)
        {
            AddToCollection(room.Users, user);
        }

        public void CreateRoom(UserGroupModel obj)
        {
            CreateRecordAndAddToCollection(UsersGroups, obj);
        }
        #endregion
    }
}