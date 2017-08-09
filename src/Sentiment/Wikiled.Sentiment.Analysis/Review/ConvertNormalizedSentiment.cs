namespace Wikiled.Sentiment.Analysis.Review
{
    public static class ConvertNormalizedSentiment
    {
        public static SentimentStrength GetStrength(this double value)
        {
            if (value <= 0.1)
            {
                return SentimentStrength.Weak;
            }

            return value >= 0.38 ? SentimentStrength.Strong : SentimentStrength.Medium;
        }
    }
}
