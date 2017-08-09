namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public interface ISentenceRepairHandler
    {
        string Repair(string sentence);
    }
}