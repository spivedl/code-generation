using System;

namespace CodeGeneration.Models.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class ReadOnlyAttribute : Attribute
    {
        public bool IsReadOnly;

        public ReadOnlyAttribute()
        {
            IsReadOnly = true;
        }

        public ReadOnlyAttribute(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }
    }
}
