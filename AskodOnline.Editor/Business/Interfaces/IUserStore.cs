using System.Threading.Tasks;
using AskodOnline.Data.Objects;
using AskodOnline.Editor.Models;
using Microsoft.AspNet.SignalR.Hubs;

namespace AskodOnline.Editor.Business.Interfaces
{
    public interface IUserStore : IBaseStore<UserModel>
    {
        Task<UserEntity> GetUserByRowIdAsync(string rowId);

        Task<UserEntity> GetUserByCounterAsync(long counter);

        UserModel CreateUserModel(UserEntity userEntity, HubCallerContext context);

        UserEntity GetUserEntity(UserModel user);
    }
}