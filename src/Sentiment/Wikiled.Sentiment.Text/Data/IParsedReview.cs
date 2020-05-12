using System;
using System.Collections.Generic;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data
{
    public interface IParsedReview
    {
        ISessionContext Context { get; }

        DateTime? Date { get; }

        Document Document { get; }

        IEnumerable<IWordItem> ImportantWords { get; }

        IEnumerable<IWordItem> AllWords { get; }

        IList<ISentence> Sentences { get; }

        string Text { get; }

        ExtractTextVectorBase Vector { get; }

        RatingData CalculateRawRating();

        SentimentValue[] GetAllSentiments();

        void Reset();
    }
}
