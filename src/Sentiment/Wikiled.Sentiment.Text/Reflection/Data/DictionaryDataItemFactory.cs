using System.Collections.Generic;
using NLog;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection.Data
{
    public class DictionaryDataItemFactory : IDataItemFactory
    {
        private readonly Dictionary<string, double> map;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public DictionaryDataItemFactory(Dictionary<string, double> map)
        {
            Guard.NotNull(() => map, map);
            this.map = map;
        }

        public IDataItem Create(IDataTree tree, IMapField field)
        {
            double value;
            if (!map.TryGetValue(field.Name, out value))
            {
                if (!field.IsOptional)
                {
                    log.Warn("{0} value not found", field.Name);
                }
            }

            return new DataItem(tree.Name, field.Name, field.Description, value);
        }

        public IDataTree Create(IDataTree tree, IMapCategory mapCategory)
        {
            return new DataTree(map, mapCategory, this);
        }
    }
}
