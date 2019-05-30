using System;
using System.Linq;
using System.Reflection;
using AskodOnline.Editor.Helpers;
using AskodOnline.Security;
using FluentScheduler;

namespace AskodOnline.Editor.Business.Store
{
    public class Sheduler : Registry
    {
        protected static readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Sheduler()
        {
            Schedule(GetCryptManagerKeysCleanUpAction()).ToRunOnceAt(DateTime.Now.AddHours(12)).AndEvery(12).Hours();
            Schedule(GetUserGroupStoreCollectionCleanUpAction()).ToRunOnceAt(DateTime.Now.AddHours(12)).AndEvery(12).Hours();
        }

        private Action GetCryptManagerKeysCleanUpAction()
        {
            return () =>
            {
                try
                {
                    if (CryptManager.Keys.Any())
                    {
                        CryptManager.Keys.Clear();
                        Log.Info("KeysCollection cleaned");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            };
        }

        private Action GetUserGroupStoreCollectionCleanUpAction()
        {
            return () =>
            {
                try
                {
                    if (UsersGroupsStore.UsersGroups.Any())
                    {
                        UsersGroupsStore.UsersGroups.Clear();
                        Log.Info("UsersGroupsCollection cleaned");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            };
        }
    }
}