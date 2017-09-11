using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class TestPhrase : TestWordItem, IPhrase
    {
        private readonly List<IWordItem> words = new List<IWordItem>();

        public TestPhrase()
        {
            var relationship = new TestWordItemRelationship();
            Relationship = relationship;
            relationship.Owner = this;
            POS = POSTags.Instance.NN;
            IsSimple = false;
        }

        public void Add(IWordItem word)
        {
            words.Add(word);
        }

        public int NonStopWordCount => words.Count;

        public IEnumerable<IWordItem> AllWords => words;
    }
}
