﻿using System;
using System.Linq;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Structure
{
    public class DocumentFromReviewFactory : IDocumentFromReviewFactory
    {
        private readonly INRCDictionary nrcDictionary;

        public DocumentFromReviewFactory(INRCDictionary nrcDictionary)
        {
            this.nrcDictionary = nrcDictionary ?? throw new ArgumentNullException(nameof(nrcDictionary));
        }

        public Document ReparseDocument(IRatingAdjustment adjustment)
        {
            if (adjustment?.Review?.Document == null)
            {
                throw new ArgumentNullException(nameof(adjustment));
            }

            adjustment.CalculateRating();
            var document = new Document();
            document.DocumentTime = adjustment.Review.Document.DocumentTime;
            document.Stars = adjustment.Review.Document.Stars;
            document.Id = adjustment.Review.Document.Id;
            document.Author = adjustment.Review.Document.Author;

            if (adjustment.Rating != null)
            {
                document.Stars = adjustment.Rating.StarsRating;
            }

            bool buildText = false;
            if (adjustment.Review.Text != null)
            {
                document.Text = adjustment.Review.Text;
                buildText = true;
            }

            var vector = new SentimentVector();

            foreach (var sentence in adjustment.Review.Sentences)
            {
                if (string.IsNullOrWhiteSpace(sentence.Text))
                {
                    continue;
                }

                var sentenceItem = new SentenceItem(sentence.Text);
                document.Add(sentenceItem, buildText);
                foreach (var wordItem in sentence.Occurrences)
                {
                    var word = WordExFactory.Construct(wordItem);
                    word.IsStop = wordItem.IsStopWord;
                    word.Phrase = wordItem.Parent?.Text;
                    word.NormalizedEntity = wordItem.NormalizedEntity;
                    word.Span = word.Text;
                    var sentiment = wordItem.Relationship.Sentiment?.DataValue;
                    if (sentiment != null)
                    {
                        word.Value = sentiment.Value;
                        word.Span = wordItem.Relationship.Sentiment.Span;
                    }

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

                    if (adjustment.Review.Context.ExtractAttributes)
                    {
                        PopulateAttributes(vector, word, wordItem);
                    }

                    sentenceItem.Add(word);
                }
            }

            document.Text = adjustment.Review.Document.Text;
            if (adjustment.Review.Context.ExtractAttributes)
            {
                document.Attributes = vector.GetProbabilities().ToDictionary(item => item.Data, item => item.Probability.ToString());
            }

            return document;
        }

        private void PopulateAttributes(SentimentVector vector, WordEx word, IWordItem original)
        {
            var record = nrcDictionary.FindRecord(word);
            vector.ExtractData(record);
            word.Emotions = record?.GetDefinedCategories().ToArray();
            word.Attributes = original.Inquirer.Records.SelectMany(item => item.Description.Attributes).ToArray();
        }
    }
}
