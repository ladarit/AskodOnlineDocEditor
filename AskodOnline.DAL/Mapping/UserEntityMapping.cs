using AskodOnline.Data.Objects;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace AskodOnline.DataAccess.Mapping
{
    public class UserEntityMapping : IAutoMappingOverride<UserEntity>
    {
        public void Override(AutoMapping<UserEntity> mapping)
        {
            mapping.Table("USERS");
            mapping.Id(t => t.Counter);
            mapping.Map(r => r.Name).Column("FULLNAME").Not.Update();
            mapping.HasOne(x => x.Avatar);
        }
    }
}
