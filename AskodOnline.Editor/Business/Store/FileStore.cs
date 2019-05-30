using System;
using System.Linq;
using System.Threading.Tasks;
using AskodOnline.Data.Objects;
using AskodOnline.Editor.Business.Attributes;
using AskodOnline.Editor.Business.DataProvider;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Models;
using NHibernate;

namespace AskodOnline.Editor.Business.Store
{
	public class FileStore<T> : IFileStore<T> where T: FileEntity, new()
	{
		private IDataProvider<T> DataProvider { get; }

		private IUserStore UserStore { get; }

		protected ISession Session => DbConnectionUnit.Current.Session;

		public FileStore(IUserStore userStore) 
		{
			DataProvider = new DataProvider<T>();
			UserStore = userStore;
		}

		[RequiredDbConnection]
		public async Task<FileModel> GetFileByCounterAsync(long counter)
		{
			var teamworkId = await GetFileFistVersionCounter(counter);
			var fileEnity = await DataProvider.GetRecordByCounterAsync(counter);
			return GetFileModel(fileEnity, teamworkId);
		}

		[RequiredDbConnection]
		public async Task<string> GetFileExtensionByCounterAsync(long counter)
		{
			Session.Clear();
			return await Task.FromResult(Session.Query<FileEntity>().Where(t => t.Counter == counter).Select(t => t.FileName).FirstOrDefault());
		}

		[RequiredDbConnection]
		public async Task<TransactionResult> UpdateFileAsync(FileModel file, UserModel user)
		{
			var fileEntity = GetFileEntity(file);
			var userEntity = UserStore.GetUserEntity(user);
			return await new DbProcedures().UpdateFile(fileEntity, userEntity);
		}

		public async Task<TransactionResult> AddFileAsNewVersionAsync(FileModel file, UserModel user)
		{
			var fileEntity = GetFileEntity(file);
			var userEntity = UserStore.GetUserEntity(user);
			return await new DbProcedures().AddFileAsNewVersion(fileEntity, userEntity);
		}

		[RequiredDbConnection]
		public async Task<TransactionResult> PingFileEditingAsync(FileModel file, UserModel user, int interval)
		{
			var fileEntity = GetFileEntity(file);
			var userEntity = UserStore.GetUserEntity(user);
			return await new DbProcedures().PingFileEditing(fileEntity, userEntity, interval);
		}

		[RequiredDbConnection]
		public async Task<long> GetFileFistVersionCounter(long counter)
		{
			var sql = $"select PKG_FILES.GetFileFirstVersionID({counter}) from dual";
			return (long)await DataProvider.RunSqlQuery<decimal>(sql);
		}

		private bool GetDocSignStatus(T entity)
		{
			if (!entity.DocSign.Any()) return false;
			return entity.DocSign != null && entity.DocSign.First().SignTime != default(DateTime) && entity.DocSign.First().Counter != default(long);
		}

		private T GetFileEntity(FileModel file)
		{
			return new T
			{
				Counter = file.Counter,
				AuthorId = file.AuthorId,
				FileName = file.FileName,
				TextFile = file.TextFile
			};
		}

		private FileModel GetFileModel(T fileEntity, long teamworkId)
		{
			return new FileModel
			{
				Counter = fileEntity.Counter,
				AuthorId = fileEntity.AuthorId,
				FileName = fileEntity.FileName,
				TeamworkId = teamworkId,
				IsDocSign = GetDocSignStatus(fileEntity),
				TextFile = fileEntity.TextFile
			};
		}
	}
}