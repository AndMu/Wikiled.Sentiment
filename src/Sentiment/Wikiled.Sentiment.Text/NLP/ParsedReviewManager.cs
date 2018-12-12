using System;
using System.Linq;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP
{
    public class ParsedReviewManager : IParsedReviewManager
    {
        private readonly IContextWordsHandler manager;

        private readonly Document document;

        private ParsedReview review;

        private readonly IWordFactory wordsFactory;

        private readonly INRCDictionary nrcDictionary;

        public ParsedReviewManager(IContextWordsHandler manager, IWordFactory wordsFactory, INRCDictionary nrcDictionary, Document document)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.nrcDictionary = nrcDictionary ?? throw new ArgumentNullException(nameof(nrcDictionary));
            this.wordsFactory = wordsFactory ?? throw new ArgumentNullException(nameof(wordsFactory));
        }

        private bool CanSplit => review.CurrentSentence.CurrentPart.Occurrences.Count > 0 &&
                                 Previous != null &&
                                 !Previous.IsConjunction();

        private IWordItem Previous
            => review.CurrentSentence.CurrentPart.Occurrences.Count == 0
                ? null
                : review.CurrentSentence.CurrentPart.Occurrences[
                    review.CurrentSentence.CurrentPart.Occurrences.Count - 1];

        public IParsedReview Create()
        {
            if (review != null)
            {
                return review;
            }

            review = new ParsedReview(nrcDictionary, document);
            foreach (var sentence in document.Sentences)
            {
                CreateSentence(sentence);
                IPhrase phrase = null;
                string phraseWord = null;
                for (var i = 0; i < sentence.Words.Count; i++)
                {
                    var documentWord = sentence.Words[i];
                    if (documentWord.Phrase != null)
                    {
                        if (phraseWord != documentWord.Phrase)
                        {
                            phraseWord = documentWord.Phrase;
                            phrase = documentWord.UnderlyingWord as IPhrase ??
                                     wordsFactory.CreatePhrase(phraseWord);
                        }
                    }
                    else
                    {
                        phrase = null;
                        phraseWord = null;
                    }

                    // !! we need to create new - because if we use underlying
                    // we can lose if words is changed to aspect
                    IWordItem word = wordsFactory.CreateWord(documentWord.Text, documentWord.Type);
                    word.NormalizedEntity = documentWord.NormalizedEntity;
                    word.Entity = documentWord.EntityType;
                    word.WordIndex = i;
                    AddWord(word, i == sentence.Words.Count - 1);
                    phrase?.Add(word);
                }
            }

            foreach (var sentence in review.Sentences)
            {
                foreach (var phrase in sentence.Occurrences.GetPhrases().Where(item => item.AllWords.Count() > 1))
                {
                    phrase.IsSentiment = manager.IsSentiment(phrase);
                    phrase.IsFeature = manager.IsFeature(phrase);
                    phrase.IsTopAttribute = manager.IsAttribute(phrase);
                }
            }
            
            return review;
        }

        private void AddWord(IWordItem occurrence, bool last)
        {
            if (occurrence is null)
            {
                throw new ArgumentNullException(nameof(occurrence));
            }

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
            if (sentence is null)
            {
                throw new ArgumentNullException(nameof(sentence));
            }

            review.AddNewSentence(sentence);
        }
    }
}
