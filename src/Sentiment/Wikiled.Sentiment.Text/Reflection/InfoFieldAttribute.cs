using System;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InfoFieldAttribute : Attribute
    {
        public InfoFieldAttribute(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Name = name;
            Order = 999;
        }

        public int Order { get; private set; }

        public string Name { get; private set; }

        public string Description { get; set; }

        public bool IsOptional { get; set; }
    }
}
