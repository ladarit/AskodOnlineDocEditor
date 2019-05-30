using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Configuration;
using AskodOnline.Data.Objects;
using AskodOnline.Editor.Helpers;
using AskodOnline.Editor.Models;
using Oracle.ManagedDataAccess.Client;

namespace AskodOnline.Editor.Business.DataProvider
{
	public class DbProcedures
	{
		private static readonly string ConnectionString = WebConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;

		protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public async Task<TransactionResult> UpdateFile(FileEntity file, UserEntity user)
		{
			using (var connection = new OracleConnection(ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();
				try
				{
					using (var command = connection.CreateCommand())
					{
						command.Transaction = transaction;

						command.CommandType = CommandType.StoredProcedure;
						command.CommandText = "PKG_FILES.UpdateFile";

						//Код результату
						var resultCodeObject = command.AddInt64OutputParam("AResultCode");

						//Текст результату (якщо все успішно, то поверне NULL)
						var errorMessageObject = command.AddStringOutputParam("AResultMessage");

						command.AddInt64InputParam("AFileID", file.Counter);
						command.AddStringInputParam("AFileName", file.FileName);
						command.AddBlobInputParam("AFileBody", file.TextFile);
						command.AddInt64InputParam("AUserID", user.Counter);
						command.AddStringInputParam("ATerminal", ClientInformation.GetHostName());
						command.AddStringInputParam("AIP", ClientInformation.GetHostIp());
						command.AddInt64InputParam("ALang", 0);

						command.ExecuteNonQuery();

						transaction.Commit();
						transaction.Dispose();

						var errorMessage = errorMessageObject?.Value?.ToString();
						if (!string.IsNullOrEmpty(errorMessage))
							Log.Error($"PKG_FILES.UpdateFile {errorMessageObject.Value}");

						long.TryParse(resultCodeObject.Value.ToString(), out var resultCode);

						if (resultCode != 1)
						{
							IDictionary<long, string> codeDictionary = new Dictionary<long, string>
							{
								{1, "все ОК"},
								{0, "виникла помилка"},
								{2, "файл порожній (передано 0 байт)"},
								{3, "файл відсутній у БД"},
								{4, "файл не відрізняється від попередньої версії"},
								{5, "автор попередньої версії інший, тому можна робити тільки нову версію"},
								{6, "доступ до файлу обмежено (заборонено для поточного користувача"}
							};
							codeDictionary.TryGetValue(resultCode, out var errorCodeMessage);
							Log.Error($"PKG_FILES.UpdateFile {errorCodeMessage}");
						}

						return await Task.FromResult(new TransactionResult
						{
							ErrorCode = (int)resultCode,
							ErrorMessage = errorMessage,
							Command = "saveCurrentVersion"
						});
					}
				}
				catch (Exception e)
				{
					transaction.Rollback();
					Log.Error(e);
					return await Task.FromResult(new TransactionResult
					{
						ErrorCode = 0,
						ErrorMessage = "виникла помилка",
						Command = "saveCurrentVersion"
					});
				}
				finally
				{
					transaction.Dispose();
				}
			}
		}

		public async Task<TransactionResult> AddFileAsNewVersion(FileEntity file, UserEntity user)
		{
			using (var connection = new OracleConnection(ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();
				try
				{
					using (var command = connection.CreateCommand())
					{
						command.Transaction = transaction;

						command.CommandType = CommandType.StoredProcedure;
						command.CommandText = "PKG_FILES.AddFileAsNewVersion";

						//Код результату
						var resultCodeObject = command.AddInt64OutputParam("AResultCode");

						//Текст результату (якщо все успішно, то поверне NULL)
						var errorMessageObject = command.AddStringOutputParam("AResultMessage");

						//ID нового файла
						var fileIdObject = command.AddInt64OutputParam("AFileID");

						command.AddInt64InputParam("AParentFileID", file.Counter);
						command.AddStringInputParam("AFileName", file.FileName);
						command.AddBlobInputParam("AFileBody", file.TextFile);
						command.AddDateInputParam("AFileDate", null);
						command.AddInt64InputParam("AUserID", user.Counter);
						command.AddStringInputParam("ATerminal", ClientInformation.GetHostName());
						command.AddStringInputParam("AIP", ClientInformation.GetHostIp());
						command.AddInt64InputParam("ALang", 0);

						command.ExecuteNonQuery();

						transaction.Commit();
						transaction.Dispose();

						long.TryParse(resultCodeObject.Value.ToString(), out var resultCode);
						long.TryParse(fileIdObject.Value.ToString(), out var fileId);

						var errorMessage = errorMessageObject?.Value?.ToString();
						if (!string.IsNullOrEmpty(errorMessage) && resultCode != 5)
							Log.Error($"PKG_FILES.AddFileAsNewVersion {errorMessageObject.Value}");


						if (resultCode != 1 && resultCode != 5)
						{
							//todo make enum
							IDictionary<long, string> codeDictionary = new Dictionary<long, string>
							{
								{1, "все ОК"},
								{0, "виникла помилка"},
								{2, "файл порожній (передано 0 байт)"},
								{3, "файл відсутній у БД"},
								{4, "файл не відрізняється від попередньої версії"},
								{5, "автор попередньої версії інший, тому можна робити тільки нову версію"},
								{6, "доступ до файлу обмежено (заборонено для поточного користувача"}
							};
							codeDictionary.TryGetValue(resultCode, out var errorCodeMessage);
							Log.Error($"PKG_FILES.AddFileAsNewVersion {errorCodeMessage}");
						}

						return await Task.FromResult(new TransactionResult
						{
							ErrorCode = (int)resultCode,
							ErrorMessage = errorMessage,
							NewFileCounter = fileId,
							IsNewFile = true,
							Command = "saveNewVersion"
						});
					}
				}
				catch (Exception e)
				{
					transaction.Rollback();
					Log.Error(e);
					return await Task.FromResult(new TransactionResult
					{
						ErrorCode = 0,
						ErrorMessage = "виникла помилка",
						IsNewFile = true,
						Command = "saveNewVersion"
					});
				}
				finally
				{
					transaction.Dispose();
				}
			}
		}

		public async Task<TransactionResult> PingFileEditing(FileEntity file, UserEntity user, int interval)
		{
			using (var connection = new OracleConnection(ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();
				try
				{
					using (var command = connection.CreateCommand())
					{
						command.Transaction = transaction;

						command.CommandType = CommandType.StoredProcedure;
						command.CommandText = "PKG_FILES.PingFileEditing";

						command.AddInt64InputParam("AFileID", file.Counter);
						command.AddInt64InputParam("APingInterval", interval/1000);
						command.AddStringInputParam("AClientInfo", "ASKOD Online Document Editor");
						command.AddInt64InputParam("AUserID", user.Counter);
						command.AddStringInputParam("ATerminal", ClientInformation.GetHostName());
						command.AddStringInputParam("AIP", ClientInformation.GetHostIp());

						command.ExecuteNonQuery();

						transaction.Commit();
						transaction.Dispose();

						return await Task.FromResult(new TransactionResult
						{
							ErrorMessage = "",
							Command = "PingFileEditing"
						});
					}
				}
				catch (Exception e)
				{
					transaction.Rollback();
					Log.Error(e);
					return await Task.FromResult(new TransactionResult
					{
						ErrorCode = 0,
						ErrorMessage = e.Message,
						Command = "PingFileEditing"
					});
				}
				finally
				{
					transaction.Dispose();
				}
			}
		}
	}
}
