using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection.Data
{
    public class DataTree : IDataTree
    {
        private readonly List<IDataTree> branches = new List<IDataTree>();

        private readonly IMapCategory currentCategory;

        private readonly IDataItemFactory dataItemFactory;

        private readonly List<IDataItem> leafs = new List<IDataItem>();

        public DataTree(object instance, IMapCategory category)
            : this(instance, category, MapFieldDataItemFactory.Instance)
        {
        }

        public DataTree(object instance, IMapCategory category, IDataItemFactory dataItemFactory)
        {
            Guard.NotNull(() => category, category);
            Guard.NotNull(() => dataItemFactory, dataItemFactory);
            Instance = instance;
            currentCategory = category;
            this.dataItemFactory = dataItemFactory;
            Build();
        }

        public IEnumerable<IDataItem> AllLeafs
        {
            get
            {
                foreach (var leaf in Leafs)
                {
                    yield return leaf;
                }

                foreach (var dataTree in Branches)
                {
                    foreach (var leaf in dataTree.AllLeafs)
                    {
                        yield return leaf;
                    }
                }
            }
        }

        public IEnumerable<IDataItem> AllLeafsSorted
        {
            get
            {
                return AllLeafs.OrderBy(item => item.Name);
            }
        }

        public IList<IDataTree> Branches => branches;

        public string FullName => currentCategory.FullName;

        public object Instance { get; }

        public IList<IDataItem> Leafs => leafs;

        public string Name => currentCategory.Name;

        private void Build()
        {
            foreach (IMapCategory mapCategory in currentCategory.Categories)
            {
                branches.Add(dataItemFactory.Create(this, mapCategory));
            }

            foreach (IMapField field in currentCategory.Fields)
            {
                IDataItem item = dataItemFactory.Create(this, field);
                leafs.Add(item);
            }

            foreach (var item in currentCategory.GetOtherLeafs(Instance))
            {
                leafs.Add(item);
            }
        }
    }
}
