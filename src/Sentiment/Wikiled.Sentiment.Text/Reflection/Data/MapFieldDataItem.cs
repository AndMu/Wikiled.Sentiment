using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection.Data
{
    public class MapFieldDataItem : IDataItem
    {
        private readonly IMapField field;

        private object currentValue;

        private readonly object instance;

        private readonly IDataTree parent;

        public MapFieldDataItem(IDataTree parent, IMapField field)
        {
            Guard.NotNull(() => field, field);
            Guard.NotNull(() => parent, parent);
            Guard.NotNull(() => parent.Instance, parent.Instance);
            this.field = field;
            this.parent = parent;
            instance = parent.Instance;
            currentValue = field.GetValue<object>(instance);
        }

        public string FullName => parent.Name + " " + Name;

        public string Name => field.Name;

        public string Description => field.Description;

        public object Value
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                field.SetValue(instance, value);
            }
        }
    }
}
