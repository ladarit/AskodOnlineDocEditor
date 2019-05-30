using AskodOnline.Data.Objects;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace AskodOnline.DataAccess.Mapping
{
    public class AvatarEntityMapping : IAutoMappingOverride<AvatarEntity>
    {
        public void Override(AutoMapping<AvatarEntity> mapping)
        {
            mapping.Table("MSG_USER_ATTRS");
            mapping.Id(t => t.Counter).Column("user_id");
            mapping.Map(r => r.Avatar).Column("PHOTO").Not.Update();
        }
    }
}
