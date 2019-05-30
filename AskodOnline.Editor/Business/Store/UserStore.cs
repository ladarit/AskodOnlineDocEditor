using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using AskodOnline.Data.Objects;
using AskodOnline.Editor.Business.DataProvider;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Helpers;
using AskodOnline.Editor.Models;
using Microsoft.AspNet.SignalR.Hubs;

namespace AskodOnline.Editor.Business.Store
{
    public class UserStore : BaseStore<UserModel>, IUserStore
    {
        protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static SynchronizedCollection<UserModel> UsersCollection = new SynchronizedCollection<UserModel>();

        public override SynchronizedCollection<UserModel> GetCollection => UsersCollection;

        private IDataProvider<UserEntity> DataProvider { get; }

        private const string AvatarDirectory = "Content\\Images\\Avatars\\";

        private const string NoAvatarDirectory = "../Content/Images/Avatars/noAvatar.jpg";

        private const string AvatarName = "avatar_{0}.jpeg";

        private readonly string _applicationFolderPath;

        public UserStore()
        {
            _applicationFolderPath = HttpRuntime.AppDomainAppPath;
            DataProvider = new DataProvider<UserEntity>();
        }

        public override IList<UserModel> Find(object id)
        {
            return Find(UsersCollection, id);
        }

        public override UserModel CreateRecord(object obj)
        {
            return (UserModel)obj;
        }

        public override UserModel CreateRecordAndAddToCollection(object obj)
        {
            return CreateRecordAndAddToCollection(UsersCollection, obj);
        }

        public override void RemoveRecord(UserModel obj)
        {
            RemoveRecord(UsersCollection, obj);
        }

        public async Task<UserEntity> GetUserByRowIdAsync(string rowId)
        {
            return await DataProvider.GetRecordByRowIdAsync(rowId);
        }

        public async Task<UserEntity> GetUserByCounterAsync(long counter)
        {
            return await DataProvider.GetRecordByCounterAsync(counter);
        }

        public UserModel CreateUserModel(UserEntity userEntity, HubCallerContext context)
        {
            var browser = HttpContext.Current.Request.Browser;
            return new UserModel
            {
                AvatarPath = GetPicturePath(userEntity.Avatar.Avatar, userEntity.Counter),
                Name = userEntity.Name,
                Counter = userEntity.Counter,
                Id = context.ConnectionId,
                Agent = new BrowserModel(browser.Type, browser.Version)
            };
        }

        private string GetPicturePath(byte[] photo, long userCounter)
        {
            try
            {
                if (photo == null)
                    return NoAvatarDirectory;

                var fileName = string.Format(AvatarName, userCounter);
                var pictureRelativePath = AvatarDirectory + fileName;
                var filePath = Path.Combine(_applicationFolderPath, pictureRelativePath);

                if (!File.Exists(filePath) || new FileInfo(filePath).Length != photo.Length)
                {
                    File.WriteAllBytes(filePath, photo);
                }

                return "../Content/Images/Avatars/" + fileName;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return string.Empty;
            }

        }

        public UserEntity GetUserEntity(UserModel user)
        {
            return new UserEntity
            {
                Counter = user.Counter
            };
        }
    }
}