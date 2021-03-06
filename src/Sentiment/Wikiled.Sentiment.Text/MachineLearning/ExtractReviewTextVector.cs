﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Text.MachineLearning
{
    public class ExtractReviewTextVector : ExtractTextVectorBase
    {
        private readonly IParsedReview review;

        private readonly INRCDictionary dictionary;

        public ExtractReviewTextVector(INRCDictionary dictionary, IParsedReview review)
        {
            this.review = review ?? throw new ArgumentNullException(nameof(review));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        protected override RatingData GetRating()
        {
            return review.CalculateRawRating();
        }

        protected override void Additional()
        {
            SentimentVector vector = dictionary.Extract(review.ImportantWords);
            foreach (var probability in vector.GetProbabilities().Where(item => item.Probability > 0))
            {
                AddItem(null, $"DIMENSION_{probability.Data}", probability.Probability);
            }

            base.Additional();
        }

        protected override IEnumerable<ISentence> GetSentences()
        {
            return review.Sentences;
        }
    }
}
