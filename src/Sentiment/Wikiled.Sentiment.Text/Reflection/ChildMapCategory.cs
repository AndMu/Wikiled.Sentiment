using System.Reflection;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.Text.Reflection
{
    public class ChildMapCategory : MapCategory
    {
        private readonly PropertyInfo propertyInfo;

        public ChildMapCategory(IMapCategory parent, string name, PropertyInfo propertyInfo)
            : base(parent.IsPropertyName, name, propertyInfo.PropertyType)
        {
            Guard.NotNull(() => parent, parent);
            Guard.NotNull(() => propertyInfo, propertyInfo);
            Parent = parent;
            this.propertyInfo = propertyInfo;
        }

        public override object ResolveInstance(object parent)
        {
            return propertyInfo.GetValue(parent, null);
        }

        public override IMapCategory Parent { get; }

        public override string FullName => Parent.FullName + " " + base.FullName;
    }
}
