using System;
using System.Reflection;
using AskodOnline.Editor.Business.Attributes;
using AskodOnline.Editor.Helpers;
using Castle.DynamicProxy;
using NHibernate;

namespace AskodOnline.Editor.Business.Store
{
    public class OracleDbConnectionInterceptor : AbstractDbConnectionInterceptor
	{
		/// <summary>
		/// Creates a new NhUnitOfWorkInterceptor object.
		/// </summary>
		/// <param name="oracleSessionFactory">Nhibernate session factory.</param>
		public OracleDbConnectionInterceptor(ISessionFactory oracleSessionFactory)
        {
            SessionFactory = oracleSessionFactory;
	        Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public override void Intercept(IInvocation invocation)
        {
            //If there is a running transaction, just run the method
            if (DbConnectionUnit.Current != null || !AttributesHelper.HasDbConnectionAttribute(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }
            try
            {
                DbConnectionUnit.Current = new DbConnectionUnit(SessionFactory);
                DbConnectionUnit.Current.BeginTransaction();

                try
                {
                    invocation.Proceed();
                    DbConnectionUnit.Current.Commit();
                }
                catch(Exception ex)
                {
                    try
                    {
                        Log.Error(ex);
                        DbConnectionUnit.Current.Rollback();
                    }
                    catch(Exception rollbackEx)
                    {
                        Log.Error(rollbackEx);
                    }
                }
            }
            finally
            {
                DbConnectionUnit.Current = null;
            }
        }
    }
}