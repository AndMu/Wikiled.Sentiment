using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Reflection.Data;

namespace Wikiled.Sentiment.Text.Reflection
{
    [InfoCategory("Wrapper")]
    public class ItemProbabilityHolder
    {
        public ItemProbabilityHolder(IList<IItemProbability<string>> probabilities)
        {
            Guard.NotNull(() => probabilities, probabilities);
            Probabilities = probabilities;
        }

        [InfoArrayCategory("List", "Data", "Probability")]
        public IList<IItemProbability<string>> Probabilities { get; private set; }
    }
}
