using System;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class InfoCategoryAttribute : Attribute
    {
        public InfoCategoryAttribute(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Name = name;
        }

        public string Name { get; private set; }

        public bool IsCollapsed { get; set; }

        public bool Ignore { get; set; }
    }
}
