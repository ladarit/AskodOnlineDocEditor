using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AskodOnline.Editor.Business.Store;
using AskodOnline.Editor.Helpers;
using AskodOnline.Security;
using Newtonsoft.Json;

namespace AskodOnline.Editor.Models
{
    public class AuthenticateAttribute : AuthorizeAttribute
    {
        protected readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            try
            {
                if (HttpContext.Current.Request.Form.Keys.Count != 0)
                {
                    long.TryParse(HttpContext.Current.Request.Form.GetValues("userCounter")?.First(), out var userCounter);
                    long.TryParse(HttpContext.Current.Request.Form.GetValues("docCounter")?.First(), out var docCounter);
                    var userExists = CheckUserExists(userCounter, docCounter);
                    if (userExists)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                        return;
                    }
                    var receivedFromClientKey = HttpContext.Current.Request.Form.GetValues("encryptedKey")?.First();
                    if (!string.IsNullOrEmpty(receivedFromClientKey) && CryptManager.Keys.Any())
                    {
                        var decryptedKey = CryptManager.Decrypt(receivedFromClientKey);
                        if (CryptManager.Keys.Contains(decryptedKey))
                        {
                            CryptManager.Keys.Remove(decryptedKey);
                            var lang = HttpContext.Current.Request.Form.GetValues("userLang")?.First();
                            if (userCounter == 0)
                            {
                                throw new Exception();
                            }
                            var authCookie = CreateAuthCookie(userCounter, docCounter);
                            var langCookie = CreateLocalizationCookie(lang);
                            filterContext.HttpContext.Response.Cookies.Add(authCookie);
                            filterContext.HttpContext.Response.Cookies.Add(langCookie);
                            return;
                        }
                    }
                }


                var cookieFromClient = HttpContext.Current.Request.Cookies["AskodOnlineDocEditor"];
                if (cookieFromClient != null)
                {
                    var decyptedMetaData = CryptManager.Decrypt(HttpUtility.UrlDecode(cookieFromClient.Value));
                    var oldMetadata = JsonConvert.DeserializeObject<MetaDataModel>(decyptedMetaData);
                    var userExists = CheckUserExists(oldMetadata.UserCounter, oldMetadata.DocumentCounter);
                    if (userExists)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                        return;
                    }

                    if (oldMetadata.SessionId.Equals(SessionStore.GetSessionId()))
                    {
                        var authCookie = CreateAuthCookie(oldMetadata.UserCounter, oldMetadata.DocumentCounter);
                        filterContext.HttpContext.Response.Cookies.Add(authCookie);
                        return;
                    }
                }

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }
            catch (Exception e)
            {
                Log.Error(e);
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }
        }

        private HttpCookie CreateAuthCookie(long userCounter, long documentCounter)
        {
            var metaData = new MetaDataModel
            {
                SessionId = SessionStore.SetNewSessionId(),
                UserCounter = userCounter,
                DocumentCounter = documentCounter
            };
            return new HttpCookie("AskodOnlineDocEditor")
            {
                Value = HttpUtility.UrlEncode(CryptManager.Encrypt(JsonConvert.SerializeObject(metaData)))
            };
        }

        private HttpCookie CreateLocalizationCookie(string langCode)
        {
            var list = new List<string> { "uk-UA", "ru-RU", "en-US" };
            var lang = list.Where(t => t.ToLower() == langCode.ToLower()).DefaultIfEmpty("\"ru-RU\"").First();
            return new HttpCookie("AODECulture")
            {
                Value = HttpUtility.UrlEncode(lang)
            };
        }

        private bool CheckUserExists(long userCounter, long documentCounter)
        {
            var browser = HttpContext.Current.Request.Browser;
            var sameRoom = UsersGroupsStore.UsersGroups.FirstOrDefault(t => t.DocumentCounter == documentCounter);
            if (sameRoom == null)
            {
                return false;
            }
            var userAgent = new BrowserModel(browser.Type, browser.Version);
            return sameRoom.Users.Any(u => u.Counter == userCounter && u.Agent != userAgent);
        }
    }
}