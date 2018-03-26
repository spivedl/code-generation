using System;

namespace CodeGeneration.Models.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class ForeignKeyAttribute : Attribute
    {
        public bool IsForeignKey { get; set; }
        public string ItemSourceName { get; set; }

        public ForeignKeyAttribute() : this(true, null)
        {
        }

        public ForeignKeyAttribute(bool isForeignKey) : this(isForeignKey, null)
        {
        }

        public ForeignKeyAttribute(string itemSourceName) : this(!string.IsNullOrWhiteSpace(itemSourceName), itemSourceName)
        {
        }

        public ForeignKeyAttribute(bool isForeignKey, string itemSourceName)
        {
            IsForeignKey = isForeignKey;
            ItemSourceName = itemSourceName;
        }
    }
}
