using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Structure
{
    public class DocumentFromReviewFactory : IDocumentFromReviewFactory
    {
        public static readonly DocumentFromReviewFactory Instance = new DocumentFromReviewFactory();

        private DocumentFromReviewFactory() { }

        public Document ReparseDocument(IParsedReview review, IRatingAdjustment adjustment)
        {
            Guard.NotNull(() => review, review);
            Guard.NotNull(() => adjustment, adjustment);
            var document = new Document();
            document.DocumentTime = review.Document.DocumentTime;
            document.Text = review.Document.Text;
            document.Stars = review.Document.Stars;
            document.Id = review.Document.Id;
            document.Author = review.Document.Author;

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
