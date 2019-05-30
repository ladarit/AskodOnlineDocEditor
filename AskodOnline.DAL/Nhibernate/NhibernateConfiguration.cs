using AskodOnline.Data.Objects;
using AskodOnline.DataAccess.Mapping;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Driver;

namespace AskodOnline.DataAccess.Nhibernate
{
    public class NhibernateConfiguration
    {
        public static ISessionFactory CreateSessionFactory()
        {
            var cfg = new AutomappingConfiguration();

            var sessionFactory = Fluently.Configure()
                .Database(GetDataBaseConfiguration())
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<RuleMapping>();
                    m.AutoMappings.Add(() =>
                        AutoMap.AssemblyOf<BaseEntity>(cfg).IgnoreBase<BaseEntity>()
                            .UseOverridesFromAssemblyOf<RuleMapping>());
                }).BuildSessionFactory();

            return sessionFactory;
        }

        private static IPersistenceConfigurer GetDataBaseConfiguration()
        {
            return OracleClientConfiguration.Oracle10.ConnectionString(c => c.FromConnectionStringWithKey("Oracle")).Driver<OracleManagedDataClientDriver>().ShowSql();
        }
    }
}