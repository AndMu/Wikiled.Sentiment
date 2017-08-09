using System;
using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Words
{
    public class SimpleWordItemEquality : IEqualityComparer<IWordItem>
    {
        public static readonly SimpleWordItemEquality Instance = new SimpleWordItemEquality(false);

        public static readonly SimpleWordItemEquality Precise = new SimpleWordItemEquality(true);

        private readonly bool precise;

        private SimpleWordItemEquality(bool precise)
        {
            this.precise = precise;
        }

        public bool Equals(IWordItem x, IWordItem y)
        {
            if (ReferenceEquals(x, y))
            {
                return true; 
            }

            if (x == null ||
                y == null)
            {
                return false;
            }

            if (string.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (precise)
            {
                return false;
            }

            if (string.Compare(x.Stemmed, y.Stemmed, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(IWordItem obj)
        {
            // put into buckets by stemmed, so we avoid pottentialy equal words to appear in different buckets
            return precise ? obj.Text.GetHashCode() : obj.Stemmed.GetHashCode();
        }
    }
}
