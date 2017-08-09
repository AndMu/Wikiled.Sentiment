using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class TextVectorCell : ICell
    {
        public TextVectorCell(IItem item, double value)
            : this(item.Text, value)
        {
            Item = item;
        }

        public TextVectorCell(IItem item, string text, double value)
            : this(text, value)
        {
            Item = item;
        }

        public TextVectorCell(string text, double value)
        {
            Name = text;
            Value = value;
        }

        public IItem Item { get; }

        public string Name { get; }

        public double Value { get; }

        public object Clone()
        {
            return new TextVectorCell(Item, Name, Value);
        }

        public override string ToString()
        {
            return $"Vector: {Name}:{Item}:{Value}";
        }
    }
}
