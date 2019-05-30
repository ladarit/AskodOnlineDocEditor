using System.Collections.Generic;
using System.Threading.Tasks;
using AskodOnline.AdminDAL.Objects;
using AskodOnline.Editor.Business.Attributes;
using AskodOnline.Editor.Business.DataProvider;
using AskodOnline.Editor.Business.Interfaces;

namespace AskodOnline.Editor.Business.AdminStore
{
	public class AdminStore<T> : IAdminStore<T> where T : AdminEntity, new()
	{
		private IDataProvider<T> DataProvider { get; }

		public AdminStore()
		{
			DataProvider = new DataProvider<T>();
		}

		[RequiredSqlLiteDbConnection]
		public async Task<T> GetUserAsync(string login)
		{
			return await DataProvider.GetRecordByPropertyAsync("login", login);
		}

		[RequiredSqlLiteDbConnection]
		public async Task<T> GetRecordByIdAsync(string id)
		{
			var res = await DataProvider.GetRecordByIdAsync(id);
			return res;
		}

		[RequiredSqlLiteDbConnection]
		public async Task<IList<T>> GetAll()
		{
			var res = await DataProvider.GetRecordsAsync();
			return res;
		}

		[RequiredSqlLiteDbConnection]
		public async Task<bool> UpdateRecordAsync(T userEntity) //todo refactor to return TransactionResult
		{
			return await DataProvider.UpdateRecordAsync(userEntity);
		}
		
	}
}
