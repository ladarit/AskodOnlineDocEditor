using System;
using System.IO;
using AskodOnline.AdminDAL.Mapping;
using AskodOnline.AdminDAL.Objects;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace AskodOnline.AdminDAL.Nhibernate
{
    public class SqlLiteNhibernateConfiguration
    {
        public static ISessionFactory CreateSessionFactory()
        {
            var cfg = new AutomappingConfiguration();
#if DEBUG
			string baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "..\\AskodOnline.AdminDAL\\users.db");
#else
			string baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "bin\\users.db");
#endif
			var sessionFactory = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile(baseFolder))
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<RuleMapping>();
                    m.AutoMappings.Add(() =>
                        AutoMap.AssemblyOf<BaseEntity>(cfg).IgnoreBase<BaseEntity>()
                            .UseOverridesFromAssemblyOf<RuleMapping>());
                }).BuildSessionFactory();

            return sessionFactory;
        }
    }
}