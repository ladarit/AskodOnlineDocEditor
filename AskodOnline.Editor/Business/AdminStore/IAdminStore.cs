using System.Collections.Generic;
using System.Threading.Tasks;
using AskodOnline.AdminDAL.Objects;

namespace AskodOnline.Editor.Business.AdminStore
{
	public interface IAdminStore<T> where T : AdminEntity
	{
		Task<IList<T>> GetAll();

		Task<T> GetUserAsync(string login);

		Task<bool> UpdateRecordAsync(T userEntity);
	}
}
