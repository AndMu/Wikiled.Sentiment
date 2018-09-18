namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class NullSentenceRepairHandler : ISentenceRepairHandler
    {
        public string Repair(string sentence)
        {
            return sentence;
        }
    }
}
