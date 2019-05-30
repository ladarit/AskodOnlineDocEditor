using System.Threading.Tasks;
using AskodOnline.Data.Objects;
using AskodOnline.Editor.Models;

namespace AskodOnline.Editor.Business.Interfaces
{
	public interface IFileStore<T> where T : FileEntity
	{
		Task<FileModel> GetFileByCounterAsync(long counter);

		Task<string> GetFileExtensionByCounterAsync(long counter);

		Task<TransactionResult> UpdateFileAsync(FileModel file, UserModel user);

		Task<TransactionResult> AddFileAsNewVersionAsync(FileModel file, UserModel user);

		Task<TransactionResult> PingFileEditingAsync(FileModel file, UserModel user, int interval);

		Task<long> GetFileFistVersionCounter(long counter);
	}
}