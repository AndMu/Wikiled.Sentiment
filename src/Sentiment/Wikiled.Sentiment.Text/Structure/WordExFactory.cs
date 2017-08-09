using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Structure
{
    public class WordExFactory
    {
        public static WordEx Construct(IWordItem item)
        {
            Guard.NotNull(() => item, item);
            var word = new WordEx(item);
            word.NormalizedEntity = item.NormalizedEntity;
            word.EntityType = item.Entity;
            word.Type = item.POS.Tag;
            word.IsAspect = item.IsFeature;
            word.IsStop = item.IsStopWord;

            return word;
        }
    }
}
