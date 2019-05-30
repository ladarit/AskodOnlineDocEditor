using System.Threading.Tasks;
using System.Web.Http;
using AskodOnline.Data.Objects;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Models;

namespace AskodOnline.Editor.Controllers
{
	public class FileOperationsController : ApiController
	{
		private readonly IFileStore<FileEntity> _fileStore;

		public FileOperationsController(IFileStore<FileEntity> fileStore)
		{
			_fileStore = fileStore;
		}


		[HttpPost]
		[AllowAnonymous]
		[Route("api/pingfileediting")]
		public async Task<IHttpActionResult> PingFileEditingAsync(FileUserModel model)
		{
			var fileModel = await _fileStore.PingFileEditingAsync(model.File, model.User, model.Interval);
			return Ok(new { error = fileModel.ErrorMessage });
		}
	}

	public class FileUserModel
	{
		public FileModel File { get; set; }

		public UserModel User { get; set; }

		public int Interval { get; set; }
	}
}
