using AskodOnline.AdminDAL.Objects;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace AskodOnline.AdminDAL.Mapping
{
    public class AdminEntityMapping : IAutoMappingOverride<AdminEntity>
    {
        public void Override(AutoMapping<AdminEntity> mapping)
        {
            mapping.Table("USERS");
            mapping.Id(t => t.Id);
            mapping.Map(r => r.Login).Column("LOGIN").Not.Update();
			mapping.Map(r => r.Password).Column("PASSWORD").Not.Update();
	        mapping.Map(r => r.RefreshToken);
        }
    }
}
