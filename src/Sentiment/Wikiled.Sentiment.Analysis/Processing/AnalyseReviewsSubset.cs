using System;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP.Style.Description;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class AnalyseReviewsSubset
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly AnalyseReviews parent;

        private readonly IWordsHandler wordsHandler;

        public AnalyseReviewsSubset(IWordsHandler wordsHandler, AnalyseReviews parent, SingleProcessingData[] processings, PositivityType orientation)
        {
            Guard.NotNull(() => wordsHandler, wordsHandler);
            Guard.NotNull(() => parent, parent);
            Guard.NotNull(() => processings, processings);
            this.parent = parent;
            Processings = processings;
            PositivityType = orientation;
            this.wordsHandler = wordsHandler;
        }

        public void ProcessReviews()
        {
            Parallel.ForEach(
                Processings,
                AsyncSettings.DefaultParallel,
                ProcessReview);
        }

        public void ProcessReview(SingleProcessingData current)
        {
            log.Debug("ProcessReview");
            try
            {
                var review = current.Review;
                if (review == null)
                {
                    log.Error("DataRow is null: {0}", current.Text);
                    return;
                }

                review.Reset();
                RatingAdjustment adjustment = new RatingAdjustment(review, parent);
                var calcRating = adjustment.Rating;
                var stars = calcRating.StarsRating;
                var type = PositivityType.Neutral;
                if (calcRating.RawRating.HasValue &&
                    calcRating.RawRating != 0)
                {
                    type = calcRating.RawRating > 0
                        ? PositivityType.Positive
                        : PositivityType.Negative;
                }
                else if (calcRating.RawRating == 0)
                {
                    type = PositivityType.Neutral;
                }

                lock (parent)
                {
                    parent.Stars?.Add(current.Stars, stars);
                    AnalyseReviews.GlobalStars.Add(current.Stars, stars);
                }

                var doc = review.GenerateDocument(adjustment);
                if (current.Document != null)
                {
                    doc.Init(current.Document);
                }

                current.Document = doc;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public SingleProcessingData[] Processings { get; }

        public PositivityType PositivityType { get; }
    }
}
