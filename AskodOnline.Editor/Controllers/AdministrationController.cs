using System.Web.Mvc;

namespace AskodOnline.Editor.Controllers
{
    public class AdministrationController : Controller
    {
        [Route("admin")]
        public ActionResult Admin()
        {
            return View();
        }
    }
}