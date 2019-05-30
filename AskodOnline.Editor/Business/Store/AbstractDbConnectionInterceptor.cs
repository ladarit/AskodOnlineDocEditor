using Castle.DynamicProxy;
using NHibernate;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace AskodOnline.Editor.Business.Store
{
	public abstract class AbstractDbConnectionInterceptor : IInterceptor
	{
		protected log4net.ILog Log;

		protected ISessionFactory SessionFactory;

		public virtual void Intercept(IInvocation invocation)
		{
		}
	}
}