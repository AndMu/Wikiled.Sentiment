using System;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InfoArrayCategoryAttribute : Attribute
    {
        public InfoArrayCategoryAttribute(string name, string textField, string valueField)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Guard.NotNullOrEmpty(() => textField, textField);
            Guard.NotNullOrEmpty(() => valueField, valueField);
            Name = name;
            TextField = textField;
            ValueField = valueField;
        }
        
        public string Name { get; private set; }

        public string TextField { get; private set; }

        public string ValueField { get; private set; }
    }
}
