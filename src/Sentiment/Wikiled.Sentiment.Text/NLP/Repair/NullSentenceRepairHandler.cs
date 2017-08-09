namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class NullSentenceRepairHandler : ISentenceRepairHandler
    {
        public static NullSentenceRepairHandler Instance { get; } = new NullSentenceRepairHandler();

        private NullSentenceRepairHandler(){}

        public string Repair(string sentence)
        {
            return sentence;
        }
    }
}
