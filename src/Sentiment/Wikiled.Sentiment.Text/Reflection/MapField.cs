using System;
using System.Reflection;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection
{
    public class MapField : IMapField
    {
        private readonly InfoFieldAttribute attribute;

        private readonly PropertyInfo propertyInfo;

        public MapField(IMapCategory category, InfoFieldAttribute attribute, PropertyInfo propertyInfo)
        {
            Guard.NotNull(() => category, category);
            Guard.NotNull(() => attribute, attribute);
            Guard.NotNull(() => propertyInfo, propertyInfo);
            Category = category;
            this.attribute = attribute;
            this.propertyInfo = propertyInfo;
        }

        public int Order => attribute.Order;

        public Type ValueType => propertyInfo.PropertyType;

        public bool IsOptional => attribute.IsOptional;

        public T GetValue<T>(object instance)
        {
            return (T)propertyInfo.GetValue(instance, null);
        }

        public void SetValue(object instance, object value)
        {
            propertyInfo.SetValue(instance, value, null);
        }

        public IMapCategory Category { get; }

        public string Name
        {
            get
            {
                if (Category != null &&
                    Category.IsPropertyName)
                {
                    return $"{Category.Name}_{propertyInfo.Name}";
                }

                return attribute.Name;
            }
        }

        public string Description => attribute.Description;
    }
}
