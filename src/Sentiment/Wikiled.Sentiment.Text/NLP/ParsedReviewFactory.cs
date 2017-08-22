using System;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public class ParsedReviewFactory
    {
        private readonly IWordsHandler manager;

        private readonly Document document;

        private ParsedReview review;

        public ParsedReviewFactory(IWordsHandler manager, Document document)
        {
            Guard.NotNull(() => manager, manager);
            Guard.NotNull(() => document, document);
            this.manager = manager;
            this.document = document;
        }

        private bool CanSplit => review.CurrentSentence.CurrentPart.Occurrences.Count > 0 &&
                                 Previous != null &&
                                 !Previous.IsConjunction();

        private IWordItem Previous => review.CurrentSentence.CurrentPart.Occurrences.Count == 0 ? null : review.CurrentSentence.CurrentPart.Occurrences[review.CurrentSentence.CurrentPart.Occurrences.Count - 1];

        public ParsedReview Create()
        {
            if (review != null)
            {
                return review;
            }

            review = new ParsedReview(manager.NRCDictionary, document);
            foreach (var sentence in document.Sentences)
            {
                CreateSentence(sentence);
                IPhrase phrase = null;
                string phraseWord = null;
                for (int i = 0; i < sentence.Words.Count; i++)
                {
                    var documentWord = sentence.Words[i];
                    if (documentWord.Phrase != null)
                    {
                        if (phraseWord != documentWord.Phrase)
                        {
                            phraseWord = documentWord.Phrase;
                            phrase = documentWord.UnderlyingWord as IPhrase ??
                                     manager.WordFactory.CreatePhrase(phraseWord);
                        }
                    }
                    else
                    {
                        phrase = null;
                        phraseWord = null;
                    }

                    // !! we need to create new - because if we use underlying
                    // we can lose if words is changed to aspect
                    IWordItem word = manager.WordFactory.CreateWord(documentWord.Text, documentWord.Type);
                    word.NormalizedEntity = documentWord.NormalizedEntity;
                    word.Entity = documentWord.EntityType;
                    word.WordIndex = i;
                    AddWord(word, i == sentence.Words.Count - 1);
                    phrase?.Add(word);
                }
            }

            foreach (var sentence in review.Sentences)
            {
                foreach (var phrase in sentence.Occurrences.GetPhrases())
                {
                    phrase.IsSentiment = manager.IsSentiment(phrase);
                    phrase.IsFeature = manager.IsFeature(phrase);
                    phrase.IsTopAttribute = manager.AspectDectector.IsAttribute(phrase);
                }
            }
            
            return review;
        }

        private void AddWord(IWordItem occurrence, bool last)
        {
            Guard.NotNull(() => occurrence, occurrence);
            if (!occurrence.IsSimple)
            {
                throw new ArgumentOutOfRangeException(nameof(occurrence));
            }

            if (!last &&
               CanSplit &&
               occurrence.IsConjunction())
            {
                review.CurrentSentence.CreateNewPart();
            }

            review.CurrentSentence.CurrentPart.AddItem(occurrence);
        }

        private void CreateSentence(SentenceItem sentence)
        {
            Guard.NotNull(() => sentence, sentence);
            review.AddNewSentence(sentence);
        }
    }
}
