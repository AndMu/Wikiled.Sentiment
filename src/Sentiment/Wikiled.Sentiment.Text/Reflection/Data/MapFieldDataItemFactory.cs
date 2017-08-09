namespace Wikiled.Sentiment.Text.Reflection.Data
{
    public class MapFieldDataItemFactory : IDataItemFactory
    {
        public static readonly MapFieldDataItemFactory Instance = new MapFieldDataItemFactory();

        private MapFieldDataItemFactory()
        {
        }

        public IDataItem Create(IDataTree tree, IMapField field)
        {
            return new MapFieldDataItem(tree, field);
        }

        public IDataTree Create(IDataTree tree, IMapCategory mapCategory)
        {
            object instance = mapCategory.Parent.IsCollapsed
                                  ? mapCategory.ResolveInstance(mapCategory.Parent.ResolveInstance(tree.Instance))
                                  : mapCategory.ResolveInstance(tree.Instance);

            return mapCategory.OwnerType == typeof (DataTree)
                       ? (DataTree)instance
                       : new DataTree(instance, mapCategory);
        }
    }
}
