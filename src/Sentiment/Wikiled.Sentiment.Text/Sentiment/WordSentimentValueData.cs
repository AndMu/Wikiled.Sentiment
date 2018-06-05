using Wikiled.Common.Arguments;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class WordSentimentValueData
    {
        public WordSentimentValueData(string word, SentimentValueData data)
        {
            Guard.NotNull(() => Word, Word);
            Guard.NotNull(() => data, data);
            Word = word;
            Data = data;
        }

        public string Word { get; }

        public SentimentValueData Data { get; }
    }
}
