using System.Collections.Generic;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class TestPhrase : TestWordItem, IPhrase
    {
        private readonly List<IWordItem> words = new List<IWordItem>();

        public TestPhrase(string text)
            : base(text)
        {
            IsSimple = false;
        }

        public void Add(IWordItem word)
        {
            word.Parent = this;
            words.Add(word);
        }

        public int NonStopWordCount => words.Count;

        public IEnumerable<IWordItem> AllWords => words;
    }
}
