using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Structure
{
    public class DocumentFromReviewFactory : IDocumentFromReviewFactory
    {
        public Document ReparseDocument(IRatingAdjustment adjustment)
        {
            Guard.NotNull(() => adjustment, adjustment);
            Guard.NotNull(() => adjustment.Review, adjustment.Review);
            Guard.NotNull(() => adjustment.Review.Document, adjustment.Review.Document);
            var document = new Document();
            document.DocumentTime = adjustment.Review.Document.DocumentTime;
            document.Text = adjustment.Review.Document.Text;
            document.Stars = adjustment.Review.Document.Stars;
            document.Id = adjustment.Review.Document.Id;
            document.Author = adjustment.Review.Document.Author;

            if (adjustment.Rating != null)
            {
                document.Stars = adjustment.Rating.StarsRating;
            }

            if (adjustment.Review.Text == null)
            {
                document.Text = adjustment.Review.Text;
            }

            foreach (var sentence in adjustment.Review.Sentences)
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
                    word.Value = wordItem.Relationship.Sentiment?.DataValue?.Value;
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

                    sentenceItem.Add(word);
                }
            }

            return document;
        }
    }
}
