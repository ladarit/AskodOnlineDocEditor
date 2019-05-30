using AskodOnline.Data.Objects;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace AskodOnline.DataAccess.Mapping
{
    public class DocSignEntityMapping : IAutoMappingOverride<DocSignEntity>
    {
        public void Override(AutoMapping<DocSignEntity> mapping)
        {
            mapping.Table("DOCSIGNS");
            mapping.Id(t => t.Counter);
            mapping.Map(r => r.AuthorId, "AUTHOR_ID").Not.Update();
            mapping.Map(r => r.SignTime, "SIGN_TIME").Not.Update();
        }
    }
}
