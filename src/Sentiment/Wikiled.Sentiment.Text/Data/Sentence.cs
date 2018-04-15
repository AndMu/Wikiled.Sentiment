using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.Data
{
    internal class Sentence : ISentence
    {
        private readonly List<ISentencePart> parts = new List<ISentencePart>();

        public Sentence(IParsedReview review, SentenceItem original)
        {
            Guard.NotNull(() => review, review);
            Guard.NotNull(() => original, original);
            Review = review;
            Original = original;
            Index = review.Sentences.Count;
            CreateNewPart();
        }

        public ISentencePart CurrentPart => parts.Count > 0 ? parts[parts.Count - 1] : null;

        public int Index { get; }

        public ISentence Next { get; set; }

        public IEnumerable<IWordItem> Occurrences
        {
            get
            {
                return Parts.SelectMany(part => part.Occurrences);
            }
        }

        public SentenceItem Original { get; }

        public IEnumerable<ISentencePart> Parts
        {
            get
            {
                return parts.Where(item => item.Occurrences.Count > 0);
            }
        }

        public ISentence Previous { get; set; }

        public IParsedReview Review { get; }

        public string Text => Original.Text;

        public void CreateNewPart()
        {
            var part = CurrentPart;
            if (part?.Occurrences.Count == 0)
            {
                parts.Remove(part);
            }

            parts.Add(new SentencePart(this, part));
        }

        public override string ToString()
        {
            return $"[{Occurrences.Count()}]: {Text}";
        }
    }
}
