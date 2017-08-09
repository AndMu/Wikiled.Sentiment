using System;
using System.Linq;
using System.Reflection;
using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer
{
    public abstract class DataItem
    {
        private PropertyInfo[] properties;

        public abstract string Name { get; }

        public bool HasData
        {
            get
            {
                if (properties == null)
                {
                    properties = GetType().GetProperties()
                        .Where(property => Attribute.IsDefined(property, typeof (InfoFieldAttribute)))
                        .ToArray();
                }

                return properties
                    .Select(propertyInfo => (bool?) propertyInfo.GetValue(this, null))
                    .Any(value => value == true);
            }
        }
    }
}
