using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection
{
    public class CategoriesMapper
    {
        public IMapCategory Construct<T>(bool isPropertyName = false)
        {
            return Construct(typeof (T), isPropertyName);
        }

        public IMapCategory Construct(Type type, bool isPropertyName = false)
        {
            Guard.NotNull(() => type, type);
            InfoCategoryAttribute rootAttribute = (InfoCategoryAttribute)
                        type.GetCustomAttributes(typeof(InfoCategoryAttribute), false).FirstOrDefault();
            if (rootAttribute == null)
            {
                throw new ArgumentOutOfRangeException("T");
            }

            MapCategory main = new MapCategory(isPropertyName, rootAttribute.Name, type);
            ProcessType(main);
            return main;
        }

        private void ProcessType(IMapCategory parent)
        {
            foreach (var mainProperty in GetProperties<InfoCategoryAttribute>(parent.OwnerType))
            {
                InfoCategoryAttribute attribute = GetAttribute<InfoCategoryAttribute>(mainProperty);
                if (attribute.Ignore)
                {
                    continue;
                }

                MapCategory mapCategory = new ChildMapCategory(parent, attribute.Name, mainProperty);
                mapCategory.IsCollapsed = attribute.IsCollapsed;
                parent.AddCategory(mapCategory);
                ProcessType(mapCategory);
            }

            ExtractField(parent);
            ExtractCollectionField(parent);
        }

        private static void ExtractCollectionField(IMapCategory parent)
        {
            foreach (var mainProperty in GetProperties<InfoArrayCategoryAttribute>(parent.OwnerType))
            {
                InfoArrayCategoryAttribute attribute = GetAttribute<InfoArrayCategoryAttribute>(mainProperty);
                MapCategory mapCategory = new CollectionMapCategory(parent, attribute, mainProperty);
                parent.AddCategory(mapCategory);
            }
        }

        private static void ExtractField(IMapCategory parent)
        {
            foreach (var fieldProperty in GetProperties<InfoFieldAttribute>(parent.OwnerType))
            {
                var ignore = GetAttribute<IgnoreAttribute>(fieldProperty);
                if (ignore != null)
                {
                    continue;
                }

                InfoFieldAttribute attribute = GetAttribute<InfoFieldAttribute>(fieldProperty);
                
                if (fieldProperty.PropertyType != typeof(string) &&
                    typeof(IEnumerable).IsAssignableFrom(fieldProperty.PropertyType))
                {
                    var mapCategory = new EnumerableMapCategory(parent, attribute.Name, fieldProperty);
                    parent.AddCategory(mapCategory);    
                }
                else
                {
                    parent.AddField(new MapField(parent, attribute, fieldProperty));    
                }
                
            }
        }

        private static TA GetAttribute<TA>(PropertyInfo property)
            where TA : Attribute
        {
            return (TA)property.GetCustomAttributes(typeof(TA), false).FirstOrDefault();
        }

        private static IEnumerable<PropertyInfo> GetProperties<TA>(Type sourceType)
            where TA : Attribute
        {
            return sourceType.GetProperties()
                .Where(property => Attribute.IsDefined(property, typeof(TA)));
        }
    }
}
