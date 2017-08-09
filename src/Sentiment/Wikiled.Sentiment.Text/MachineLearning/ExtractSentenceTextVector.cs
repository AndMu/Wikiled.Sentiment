using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class ExtractSentenceTextVector : ExtractTextVectorBase
    {
        private readonly ISentence sentence;

        public ExtractSentenceTextVector(ISentence sentence)
        {
            Guard.NotNull(() => sentence, sentence);
            this.sentence = sentence;
        }

        public override bool UsePureWord => true;

        protected override RatingData GetRating()
        {
            return sentence.CalculateRating();
        }

        protected override IEnumerable<ISentence> GetSentences()
        {
            yield return sentence;
        }
    }
}
