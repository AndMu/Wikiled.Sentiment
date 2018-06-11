using System;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Structure
{
    public class DocumentFromReviewFactory : IDocumentFromReviewFactory
    {
        public static readonly DocumentFromReviewFactory Instance = new DocumentFromReviewFactory();

        private DocumentFromReviewFactory(){}

        public Document ReparseDocument(IParsedReview review, Document original, IRatingAdjustment adjustment)
        {
            Guard.NotNull(() => review, review);
            Guard.NotNull(() => original, original);
            Guard.NotNull(() => adjustment, adjustment);
            var document = new Document();
            if (original != null)
            {
                document.DocumentTime = original.DocumentTime;
                document.Text = original.Text;
                document.Stars = original.Stars;
                document.Id = original.Id;
                document.Author = original.Author;
            }
            else
            {
                document.DocumentTime = DateTime.Now;
            }

            if (adjustment.Rating != null)
            {
                document.Stars = adjustment.Rating.StarsRating;
            }

            if (review.Text == null)
            {
                document.Text = review.Text;
            }

            foreach (var sentence in review.Sentences)
            {
                if (string.IsNullOrWhiteSpace(sentence.Text))
                {
                    continue;
                }

                var sentenceItem = new SentenceItem(sentence.Text);
                document.Add(sentenceItem);
                foreach (var wordItem in sentence.Occurrences)
                {
                    var word = WordExFactory.Construct(wordItem);
                    word.IsStop = wordItem.IsStopWord;
                    word.Phrase = wordItem.Parent?.Text;
                    word.NormalizedEntity = wordItem.NormalizedEntity;
                    
                    if (!word.IsStop && wordItem.Relationship?.Sentiment != null)
                    {
                        word.Value = wordItem.Relationship.Sentiment.DataValue.Value;
                        word.IsAspect = wordItem.IsFeature;

                        SentimentValue value = adjustment.GetSentiment(wordItem);
                        if (value != null)
                        {
                            word.CalculatedValue = value.DataValue.Value;
                        }
                        else if (word.Value != null)
                        {
                            word.CalculatedValue = 0;
                        }
                    }

                    sentenceItem.Add(word);
                }
            }

            return document;
        }
    }
}
