namespace Wikiled.Sentiment.Text.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wikiled.Core.Utility.Arguments;
    using Wikiled.Sentiment.Text.Data;
    using Wikiled.Sentiment.Text.Extensions;
    using Wikiled.Sentiment.Text.Sentiment;
    using Wikiled.Sentiment.Text.Words;
    using Wikiled.Text.Analysis.POS;
    using Wikiled.Text.Analysis.Structure;

    public abstract class ExtractTextVectorBase
    {
        private readonly Dictionary<string, TextVectorCell> table = new Dictionary<string, TextVectorCell>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Pure word substitutes feature with xxx, so we make it aspect agnostic
        /// </summary>
        public virtual bool UsePureWord => false;

        public bool GenerateUsingImportantOnly { get; set; }

        public IList<TextVectorCell> GetCells()
        {
            table.Clear();
            var rating = GetRating();
            if (rating.HasValue)
            {
                AddItem(null, Constants.RATING_STARS, rating.RawRating.Value);
            }

            Additional();
            foreach (var reviewSentence in GetSentences())
            {
                ProcessSentence(reviewSentence);
            }

            return table.Values.ToList();
        }

        protected abstract RatingData GetRating();

        protected abstract IEnumerable<ISentence> GetSentences();

        protected virtual void Additional()
        {
        }

        protected void AddItem(IWordItem item, string word, double value)
        {
            Guard.NotNullOrEmpty(() => word, word);
            TextVectorCell cell;
            var addedCell = !table.TryGetValue(word, out cell) ? new TextVectorCell(item, word, value) : new TextVectorCell(item, word, cell.Value + value);
            table[word] = addedCell;
        }

        private void AddItemCell(IWordItem wordItem)
        {
            var word = GetWord(wordItem);
            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            AddItem(wordItem, word, GetValue(wordItem));
        }

        private double GetValue(IWordItem wordItem)
        {
            return wordItem.Relationship.Sentiment?.DataValue.Value ?? 1;
        }

        private string GetWord(IWordItem wordItem)
        {
            if (GenerateUsingImportantOnly && wordItem.POS.WordType != WordType.Adjective && wordItem.Entity != NamedEntities.Hashtag && !wordItem.IsTopAttribute && !wordItem.IsSentiment
                && !wordItem.IsFeature)
            {
                return null;
            }

            return wordItem.GenerateMask(UsePureWord);
        }

        private void ProcessSentence(ISentence reviewSentence)
        {
            foreach (var sentencePart in reviewSentence.Parts)
            {
                foreach (var wordItem in sentencePart.Occurrences.GetImportant())
                {
                    AddItemCell(wordItem);
                }
            }
        }
    }
}
