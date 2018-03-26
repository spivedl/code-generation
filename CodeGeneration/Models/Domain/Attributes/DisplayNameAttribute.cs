using System;

namespace CodeGeneration.Models.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName;

        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
