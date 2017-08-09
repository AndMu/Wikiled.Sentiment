namespace Wikiled.Sentiment.Text.NLP.Style.Description.Data
{
    public class ObscrunityData : INLPDataItem
    {
        public double Top100Words { get; set; }

        public double Top500Words { get; set; }

        public double Top1000Words { get; set; }

        public double Top5000Words { get;  set; }

        public double Top10000Words { get; set; }

        public double Top50000Words { get; set; }

        public double Top100000Words { get; set; }

        public double Top200000Words { get; set; }

        public double Top300000Words { get; set; }

        public object Clone()
        {
            return new ObscrunityData
            {
                Top100000Words = Top100000Words,
                Top10000Words = Top10000Words,
                Top1000Words = Top1000Words,
                Top100Words = Top100Words,
                Top200000Words = Top200000Words,
                Top300000Words = Top300000Words,
                Top50000Words = Top50000Words,
                Top5000Words = Top5000Words,
                Top500Words = Top500Words
            };
        }
    }
}
