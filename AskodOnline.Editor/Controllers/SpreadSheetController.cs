using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using AskodOnline.Data.Objects;
using AskodOnline.Editor.Business.Interfaces;
using AskodOnline.Editor.Helpers;
using AskodOnline.Editor.Models;
using DevExpress.Web.Mvc;
using DevExpress.Web.Office;
using Newtonsoft.Json;

namespace AskodOnline.Editor.Controllers
{
    public class SpreadSheetController : Controller
    {
        protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IFileStore<FileEntity> _fileStore;

        private readonly IUsersGroupsStore _usersGroupsStore;

        public SpreadSheetController(IFileStore<FileEntity> fileStore, IUsersGroupsStore usersGroupsStore)
        {
            _fileStore = fileStore;
            _usersGroupsStore = usersGroupsStore;
        }

        [HttpPost]
        [Authenticate]
        public async Task<ActionResult> SpreadsheetEdit()
        {
            try
            {
                long.TryParse(System.Web.HttpContext.Current.Request.Form.GetValues("userCounter")?.First(), out var userCounter);
                long.TryParse(System.Web.HttpContext.Current.Request.Form.GetValues("docCounter")?.First(), out var docCounter);
                if (docCounter == 0 || userCounter == 0)
                    return await Task.Run(() => View("AccessDenied"));
                var fileModel = await _fileStore.GetFileByCounterAsync(docCounter);
                if (fileModel?.TextFile != null)
                {
                    var serverSavedDocument = DocumentManager.FindDocument("spreadSheet_" + fileModel.TeamworkId);
                    if (serverSavedDocument != null && _usersGroupsStore.Find("spreadSheet_" + fileModel.TeamworkId).FirstOrDefault() == null)
                    {
                        DocumentManager.CloseDocument("spreadSheet_" + fileModel.TeamworkId);
                    }
                    ViewBag.userCounter = userCounter;
                    ViewBag.FileName = fileModel.FileName;
                    return View(fileModel);
                }
                throw new Exception();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return await Task.Run(() => View("Error"));
            }
        }

        [Authenticate]
        public async Task<ActionResult> CallbackSpreadsheetPartial(FileModel file, UserModel user, string command)
        {
            try
            {
                if (command == "loadLastSavedVersion")
                    return await ReopenDocument(file);

                if (command == "openLastSavedVersion")
                    return await Task.Run(() => SpreadsheetExtension.Open("spreadSheet_" + file.TeamworkId, "spreadSheet_" + file.TeamworkId));

                file.TextFile = SpreadsheetExtension.SaveCopy("spreadSheet_" + file.TeamworkId, new FileExtension().ResolveSpreadSheetFormat(file.FileName));

                if (new[] { "saveCurrentVersion", "saveNewVersion" }.Contains(command))
                {
                    var tResult = await (command == "saveCurrentVersion"
                        ? _fileStore.UpdateFileAsync(file, user)
                        : _fileStore.AddFileAsNewVersionAsync(file, user));
                    if (command == "saveNewVersion")
                    {
                        file.Counter = tResult.NewFileCounter;
                        file.AuthorId = user.Counter;
                        file.IsDocSign = false;
                    }
                    ViewBag.transactionResult = tResult;
                }

                return await Task.Run(() => PartialView("_SpreadsheetPartial", file));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return await Task.Run(() => View("Error"));
            }
        }

        [Authenticate]
        public async Task<ActionResult> CallbackSpreadsheetPartialNative()
        {
            try
            {
                var file = new FileModel();
                int.TryParse(System.Web.HttpContext.Current.Request.Params["file[Counter]"], out var docCounter);
                if (docCounter == 0)
                {
                    var fileString = Request.Params["file"];
                    if (!string.IsNullOrEmpty(fileString))
                        file = JsonConvert.DeserializeObject<FileModel>(fileString);
                }
                else
                {
                    file = new FileModel
                    {
                        FileName = System.Web.HttpContext.Current.Request.Params["file[FileName]"],
                        Counter = docCounter,
                        AuthorId = int.Parse(System.Web.HttpContext.Current.Request.Params["file[AuthorId]"]),
                        TeamworkId = int.Parse(System.Web.HttpContext.Current.Request.Params["file[TeamworkId]"]),
                        IsDocSign = bool.Parse(System.Web.HttpContext.Current.Request.Params["file[IsDocSign]"])
                    };
                }

                if (file.IsValid)
                    return await Task.Run(() => PartialView("_SpreadsheetPartial", file));

                throw new Exception("File model is not valid");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return await Task.Run(() => View("Error"));
            }
        }

        private async Task<ActionResult> ReopenDocument(FileModel file)
        {
            try
            {
                DocumentManager.CloseDocument("spreadSheet_" + file.TeamworkId);
                var restoredFile = await _fileStore.GetFileByCounterAsync(file.Counter);
                file.TextFile = restoredFile.TextFile;
                return await Task.Run(() =>
                    SpreadsheetExtension.Open("spreadSheet_" + file.TeamworkId, "spreadSheet_" + file.TeamworkId, new FileExtension().ResolveSpreadSheetFormat(file.FileName), () => file.TextFile)
                );
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return await Task.Run(() => View("Error"));
            }
        }
    }
}