
namespace Wikiled.Sentiment.Text.Sentiment
{
    public class WordSentimentValueData
    {
        public WordSentimentValueData(string word, SentimentValueData data)
        {
            Word = word ?? throw new System.ArgumentNullException(nameof(word));
            Data = data ?? throw new System.ArgumentNullException(nameof(data));
        }

        public string Word { get; }

        public SentimentValueData Data { get; }
    }
}
