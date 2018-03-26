using System;

namespace CodeGeneration.Models.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class PrimaryKeyAttribute : Attribute
    {
        public bool IsPrimaryKey;

        public PrimaryKeyAttribute()
        {
            IsPrimaryKey = true;
        }

        public PrimaryKeyAttribute(bool isPrimaryKey)
        {
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
