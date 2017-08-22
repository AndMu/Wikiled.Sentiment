using System;

namespace Wikiled.Sentiment.Text.Data.Weighting
{
    public class ItemCoefficient : IItemCoefficient
    {
        private const double coefficient = 0.01;

        public ItemCoefficient(string text)
            :this(text, 1)
        {
        }

        public ItemCoefficient(string text, double value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException("text");
            }
            Text = text;
            Value = value;
        }

        public IItemCoefficient Readjust(double value)
        {
            if (value == 0)
            {
                value = 10 * coefficient;
            }
            else
            {
                value = Math.Abs(value);
                if (value > 4)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                value *= -coefficient;
            }
            value = Value * (1 + value);
            value = value < 0.1 ? 0.1 : value;
            value = value > 2 ? 2 : value; 
            return new ItemCoefficient(Text, value);
        }

        public string Text { get;  }

        public double Value { get; protected set; }
    }
}
