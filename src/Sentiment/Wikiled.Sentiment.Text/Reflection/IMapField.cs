using System;

namespace Wikiled.Sentiment.Text.Reflection
{
    public interface IMapField
    {
        T GetValue<T>(object instance);

        void SetValue(object instance, object value);

        IMapCategory Category { get; }

        string Name { get; }

        string Description { get; }

        Type ValueType { get; }

        bool IsOptional { get; }

        int Order { get; }
    }
}