using System;

namespace CodeGeneration.Models.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class ScaffoldAttribute : Attribute
    {
        public bool ScaffoldProperty { get; set; }

        public ScaffoldAttribute()
        {
            ScaffoldProperty = true;
        }

        public ScaffoldAttribute(bool scaffoldProperty)
        {
            ScaffoldProperty = scaffoldProperty;
        }
    }
}
