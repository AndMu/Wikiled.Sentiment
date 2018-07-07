using System;

namespace Wikiled.Sentiment.Text.Data.Weighting
{
    public class ItemCoefficient : IItemCoefficient
    {
        private const double coefficient = 0.01;

        public ItemCoefficient(string text, double value = 1)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text;
            Value = value;
        }

        public string Text { get; }

        public double Value { get; }

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
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                value *= -coefficient;
            }

            value = Value * (1 + value);
            value = value < 0.1 ? 0.1 : value;
            value = value > 2 ? 2 : value;
            return new ItemCoefficient(Text, value);
        }
    }
}
