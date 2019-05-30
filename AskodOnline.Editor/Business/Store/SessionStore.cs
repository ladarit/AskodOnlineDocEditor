using System.Web;
using System.Web.SessionState;

namespace AskodOnline.Editor.Business.Store
{
    public static class SessionStore
    {
        public static SessionIDManager Manager = new SessionIDManager();

        public static string SetNewSessionId()
        {
            var newId = Manager.CreateSessionID(HttpContext.Current);
            var redirected = false;
            var IsAdded = false;
            Manager.SaveSessionID(HttpContext.Current, newId, out redirected, out IsAdded);
            return newId;
        }

        public static string GetSessionId()
        {
            return HttpContext.Current.Session.SessionID;
        }
    }
}