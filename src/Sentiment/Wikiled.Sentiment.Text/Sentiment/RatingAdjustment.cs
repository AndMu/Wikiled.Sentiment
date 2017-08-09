using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class RatingAdjustment : IRatingAdjustment
    {
        private readonly Dictionary<IWordItem, SentimentValue> calculatedSentiments;

        public RatingAdjustment(IParsedReview review, ITrainingPerspective perspective)
        {
            Guard.NotNull(() => review, review);
            Guard.NotNull(() => perspective, perspective);
            Perspective = perspective;
            Review = review;
            calculatedSentiments = new Dictionary<IWordItem, SentimentValue>(SimpleWordItemEquality.Instance);
            Rating = new RatingData();
            CalculateRating();
        }

        public ITrainingPerspective Perspective { get; }

        public double AverageVectorSize { get; private set; }

        public double RHO { get; private set; }

        public double? RawRating { get; private set; }

        public MachineDetectionResult Result { get; private set; }

        public RatingData Rating { get; private set; }

        public IParsedReview Review { get; }

        public SentimentValue GetSentiment(IWordItem word)
        {
            SentimentValue value;
            calculatedSentiments.TryGetValue(word, out value);
            return value;
        }

        private void CalculateRating()
        {
            var vector = Perspective.MachineSentiment.GetVector(Review.Vector.GetCells(), Perspective.TrainingHeader.Normalization);
            Result = Perspective.MachineSentiment.CalculateRating(vector);
            AverageVectorSize = Perspective.MachineSentiment.Header.AverageVectorSize;

            if (vector == null ||
                vector.Length == 0)
            {
                Rating = Review.CalculateRawRating();
                RawRating = Rating.RawRating;
                RHO = 0;
                return;
            }

            RHO = vector.RHO;
            var bias = vector.RHO;
            var rating = vector.Cells.FirstOrDefault(item => item.Data.Name == Constants.RATING_STARS);
            if (rating != null)
            {
                bias += rating.Calculated;
            }
            
            foreach (var item in vector.Cells)
            {
                var cell = (TextVectorCell)item.Data;
                if (item.Data.Name != Constants.RATING_STARS &&
                    cell.Item != null)
                {
                    Add(new SentimentValue(
                            (IWordItem)cell.Item,
                            new SentimentValueData(item.Calculated, SentimentSource.AdjustedSVM)));
                }
            }

            var notAddedSentiments = Review.GetAllSentiments()
                                           .Where(item => !calculatedSentiments.ContainsKey(item.Owner))
                                           .ToArray();

            foreach (var sentiment in notAddedSentiments)
            {
                // unknown decrase45x
                var value = sentiment.DataValue.SentimentSource == SentimentSource.None ? sentiment.DataValue.Value / 2 : sentiment.DataValue.Value;
                Add(new SentimentValue(
                        sentiment.Owner,
                        new SentimentValueData(
                            value / vector.Normalization.Coeficient / 2,
                            SentimentSource.AdjustedCalculated)));
            }

            if (calculatedSentiments.Count > 0)
            {
                Add(new SentimentValue(
                    WordOccurrence.CreateBasic(Constants.BIAS, POSTags.Instance.JJ),
                    new SentimentValueData(bias, SentimentSource.AdjustedSVM)));
            }
        }

        private void Add(SentimentValue sentiment)
        {
            if (calculatedSentiments.ContainsKey(sentiment.Owner))
            {
                return;
            }

            if (sentiment.Owner.Relationship.Inverted != null)
            {
                calculatedSentiments[sentiment.Owner.Relationship.Inverted] =
                    new SentimentValue(sentiment.Owner.Relationship.Inverted, new SentimentValueData(0, SentimentSource.AdjustedCanceled));
            }

            Rating.AddSentiment(sentiment.DataValue);
            calculatedSentiments[sentiment.Owner] = sentiment;
        }
    }
}
