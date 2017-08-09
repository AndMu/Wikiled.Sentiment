namespace Wikiled.Sentiment.Text.NLP.Style.Description.Data
{
    public class AllObscrunityData : INLPDataItem
    {
        public ObscrunityData Reuters { get; set; }

        public ObscrunityData Internet { get; set; }

        public ObscrunityData BNC { get; set; }

        public ObscrunityData Subtitles { get; set; }

        public object Clone()
        {
            return new AllObscrunityData
            {
                Reuters = Reuters,
                Internet = Internet,
                BNC = BNC,
                Subtitles = Subtitles
            };
        }
    }
}
