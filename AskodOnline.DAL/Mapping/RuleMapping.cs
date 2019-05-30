using AskodOnline.Data.Objects;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace AskodOnline.DataAccess.Mapping
{
    public class RuleMapping : IAutoMappingOverride<RuleEntity>
    {
        public void Override(AutoMapping<RuleEntity> mapping)
        {
            mapping.DynamicUpdate();
            mapping.Id(t => t.Counter);
        }
    }
}
