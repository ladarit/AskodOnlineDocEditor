using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using AskodOnline.AdminDAL.Nhibernate;
using AskodOnline.DataAccess.Nhibernate;
using AskodOnline.Editor.Business.AdminStore;
using AskodOnline.Editor.Business.Attributes;
using AskodOnline.Editor.Business.Store;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNet.SignalR;
using NHibernate;

namespace AskodOnline.Editor
{
    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;

            //Register all controllers
            container.Register(

				//Oracle Nhibernate session factory
				Component.For<ISessionFactory>().Named("Oracle").UsingFactoryMethod(NhibernateConfiguration.CreateSessionFactory).LifeStyle.Singleton,

				//SqlLite Nhibernate session factory
				Component.For<ISessionFactory>().Named("SQLLite").UsingFactoryMethod(SqlLiteNhibernateConfiguration.CreateSessionFactory).LifeStyle.Singleton,

				//DbConnection interceptors
				Component.For<AbstractDbConnectionInterceptor>().ImplementedBy<OracleDbConnectionInterceptor>().DependsOn(Dependency.OnComponent(typeof(ISessionFactory), "Oracle")).LifeStyle.Transient,
				Component.For<AbstractDbConnectionInterceptor>().ImplementedBy<SqlLiteDbConnectionInterceptor>().DependsOn(Dependency.OnComponent(typeof(ISessionFactory), "SQLLite")).LifeStyle.Transient,
				
				//All managers
				Classes.FromAssembly(Assembly.GetAssembly(typeof(IAdminStore<>))).InSameNamespaceAs(typeof(IAdminStore<>)).WithService.DefaultInterfaces().LifestyleTransient(),

				//All managers
				Classes.FromAssembly(Assembly.GetAssembly(typeof(FileStore<>))).InSameNamespaceAs(typeof(FileStore<>)).WithService.DefaultInterfaces().LifestyleTransient(),

                //All MVC controllers
                Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient(),

                //All API controllers
				Classes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient(),

				//All signalR hubs
				Classes.FromThisAssembly().BasedOn<Hub>().LifestyleTransient()
            );
        }

        void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            //Intercept all methods of classes those have at least one method that has UnitOfWork attribute.
            foreach (var method in handler.ComponentModel.Implementation.GetMethods())
            {
                if (AttributesHelper.HasDbConnectionAttribute(method))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(OracleDbConnectionInterceptor)));
                    return;
                }
	            if (AttributesHelper.HasSqlLiteDbConnectionAttribute(method))
	            {
		            handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(SqlLiteDbConnectionInterceptor)));
		            return;
	            }
			}
        }

        
    }
}