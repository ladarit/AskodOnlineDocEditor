using System;
using AskodOnline.Data.Objects;
using FluentNHibernate.Automapping;

namespace AskodOnline.DataAccess.Nhibernate
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return typeof(BaseEntity).IsAssignableFrom(type) || IsComponent(type);
        }
    }
}