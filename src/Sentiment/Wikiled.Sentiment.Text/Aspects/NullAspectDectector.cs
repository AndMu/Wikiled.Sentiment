using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Aspects
{
    public class NullAspectDectector : IAspectDectector
    {
        public bool IsAspect(IWordItem word)
        {
            return false;
        }

        public bool IsAttribute(IWordItem word)
        {
            return false;
        }

        public IEnumerable<IWordItem> AllFeatures
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<IWordItem> AllAttributes
        {
            get
            {
                yield break;
            }
        }
    }
}