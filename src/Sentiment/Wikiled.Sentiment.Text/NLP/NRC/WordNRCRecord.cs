namespace Wikiled.Sentiment.Text.NLP.NRC
{
    public class WordNRCRecord
    {
        public WordNRCRecord(string word, NRCRecord record)
        {
            Word = word;
            Record = record;
        }

        public string Word { get; }

        public NRCRecord Record { get; }
    }
}
