using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data
{
    public class ParsedReview : IParsedReview
    {
        private readonly List<ISentence> allSentences = new List<ISentence>();

        private readonly string text;

        internal ParsedReview(INRCDictionary dictionary, Document document, ISessionContext context)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (context == null) throw new ArgumentNullException(nameof(context));

            if (document.DocumentTime.HasValue &&
                document.DocumentTime.Value.Ticks > 0)
            {
                Date = document.DocumentTime.Value;
            }

            Document = document;
            Context = context;
            text = document.Text;
            Vector = new ExtractReviewTextVector(dictionary, this);
        }

        public Document Document { get; }

        public ISentence CurrentSentence { get; private set; }

        public ISessionContext Context { get; }

        public DateTime? Date { get; }

        public IEnumerable<IWordItem> AllWords => Sentences.SelectMany(item => item.Occurrences);

        public IEnumerable<IWordItem> ImportantWords => Sentences.SelectMany(item => item.Occurrences.GetImportant());
        
        public IList<ISentence> Sentences
        {
            get
            {
                return allSentences.Where(item => item.Parts.Any()).ToList();
            }
        }

        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }

                if (!Sentences.Any())
                {
                    return string.Empty;
                }

                return Sentences
                    .Select(item => item.Text)
                    .AccumulateItems(". ");
            }
        }

        public ExtractTextVectorBase Vector { get; }

        public void AddNewSentence(SentenceItem sentence)
        {
            CurrentSentence = new Sentence(this, sentence);
            if (allSentences.Count > 0)
            {
                allSentences[allSentences.Count - 1].Next = CurrentSentence;
                CurrentSentence.Previous = allSentences[allSentences.Count - 1];
            }

            allSentences.Add(CurrentSentence);
        }

        public RatingData CalculateRawRating()
        {
            return RatingData.Accumulate(allSentences.Select(item => item.CalculateRating()));
        }

        public SentimentValue[] GetAllSentiments()
        {
            return (from item in ImportantWords
                    where item.Relationship.Sentiment != null
                    select item.Relationship.Sentiment)
                .ToArray();
        }

        public void Reset()
        {
            foreach (var sentence in Sentences)
            {
                foreach (var wordOccurrence in sentence.Occurrences)
                {
                    wordOccurrence.Reset();
                    var parent = wordOccurrence.Parent as Phrase;
                    parent?.Reset();
                }
            }
        }
    }
}
