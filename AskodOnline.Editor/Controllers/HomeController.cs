using System.Web.Mvc;
using AskodOnline.Security;

namespace AskodOnline.Editor.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public JsonResult Authorize()
        {
            var key = KeyGenerator.RandomString(32);
            CryptManager.Keys.Add(key);
            return new JsonResult { Data = new { error = string.Empty, key }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
       
        public ActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}