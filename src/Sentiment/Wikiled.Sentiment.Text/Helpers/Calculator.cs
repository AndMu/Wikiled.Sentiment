namespace Wikiled.Sentiment.Text.Helpers
{
    public class Calculator<T>
    {
        public static T Add(T left, T right)
        {
            return (dynamic)left + (dynamic)right;
        }

        public static T Subtract(T left, T right)
        {
            return (dynamic)left - (dynamic)right;
        }

        public static T Multiply(T left, T right)
        {
            return (dynamic)left * (dynamic)right;
        }

        public static T Divide(T left, T right)
        {
            return (dynamic)left / (dynamic)right;
        }
    }
}
