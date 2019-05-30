using System;
using AskodOnline.AdminDAL.Objects;
using FluentNHibernate.Automapping;

namespace AskodOnline.AdminDAL.Nhibernate
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return typeof(BaseEntity).IsAssignableFrom(type) || IsComponent(type);
        }
    }
}