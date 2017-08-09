using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Reflection.Data;

namespace Wikiled.Sentiment.Text.Reflection
{
    public class MapCategory : IMapCategory
    {
        private readonly List<IMapCategory> categories = new List<IMapCategory>();

        private Lazy<IMapField[]> allChildFields;

        private Dictionary<string, IMapField> fieldMap;

        private Lazy<Dictionary<string, IMapField>> map;

        private readonly List<IMapField> fields = new List<IMapField>();

        private Lazy<IMapField[]> sortedFields;

        public MapCategory(bool isPropertyName, string name, Type type)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Guard.NotNull(() => type, type);
            IsPropertyName = isPropertyName;
            Name = name;
            OwnerType = type;
            Reset();
        }

        public virtual IEnumerable<IMapCategory> Categories
        {
            get
            {
                foreach(var mapCategory in categories)
                {
                    if(!mapCategory.IsCollapsed)
                    {
                        yield return mapCategory;
                    }
                    else
                    {
                        foreach(var childCategory in mapCategory.Categories)
                        {
                            yield return childCategory;
                        }
                    }
                }
            }
        }

        public virtual IMapField[] Fields => sortedFields.Value;

        public virtual string FullName => Name;

        public virtual IMapCategory Parent => null;

        public string[] ActualProperties => FieldMap.Keys.ToArray();

        public IMapField[] AllChildFields => allChildFields.Value;

        public bool IsCollapsed { get; set; }

        public bool IsPropertyName { get; }

        public string Name { get; }

        public Type OwnerType { get; }

        private Dictionary<string, IMapField> FieldMap => map.Value;

        public IMapField this[string name] => FieldMap[name];

        public virtual IEnumerable<IDataItem> GetOtherLeafs(object instance)
        {
            yield break;
        }

        public virtual object ResolveInstance(object parent)
        {
            return parent;
        }

        public void AddCategory(IMapCategory mapCategory)
        {
            categories.Add(mapCategory);
            Reset();
        }

        public void AddField(IMapField field)
        {
            fields.Add(field);
            Reset();
        }

        public bool ContainsField(string name)
        {
            return FieldMap.ContainsKey(name);
        }

        protected virtual IEnumerable<IMapField> ConstructAllChildFields()
        {
            foreach(var field in Fields)
            {
                yield return field;
            }

            foreach(var mapCategory in categories)
            {
                foreach(var field in mapCategory.AllChildFields)
                {
                    yield return field;
                }
            }
        }

        private Dictionary<string, IMapField> ConstructMap()
        {
            fieldMap = new Dictionary<string, IMapField>();
            foreach(var field in AllChildFields)
            {
                if(fieldMap.ContainsKey(field.Name))
                {
                    throw new ArgumentOutOfRangeException(field.Name + "name is not unique");
                }

                fieldMap[field.Name] = field;
            }

            return fieldMap;
        }

        private void Reset()
        {
            if(map == null ||
               map.IsValueCreated)
            {
                map = new Lazy<Dictionary<string, IMapField>>(ConstructMap);
            }

            if(allChildFields == null ||
               allChildFields.IsValueCreated)
            {
                allChildFields = new Lazy<IMapField[]>(() => ConstructAllChildFields().ToArray());
            }

            if(sortedFields == null ||
               sortedFields.IsValueCreated)
            {
                sortedFields = new Lazy<IMapField[]>(() => fields.OrderBy(item => item.Order).ToArray());
            }
        }
    }
}
