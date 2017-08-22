using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Style.Obscurity
{
    public class VocabularyObscurity : IDataSource
    {
        public VocabularyObscurity(TextBlock text)
        {
            Internet = new SpecificObscrunity(text, FrequencyListManager.Instance.Internet);
            BNC = new SpecificObscrunity(text, FrequencyListManager.Instance.BNC);
            Reuters = new SpecificObscrunity(text, FrequencyListManager.Instance.Reuters);
            Subtitles = new SpecificObscrunity(text, FrequencyListManager.Instance.Subtitles);
        }

        [InfoCategory("Reuters", Ignore = true)]
        public SpecificObscrunity Reuters { get; }

        [InfoCategory("Internet", Ignore = true)]
        public SpecificObscrunity Internet { get; }

        [InfoCategory("BNC")]
        public SpecificObscrunity BNC { get; }

        [InfoCategory("Subtitles", Ignore = true)]
        public SpecificObscrunity Subtitles { get; }

        public void Load()
        {
            Reuters.Load();
            Internet.Load();
            BNC.Load();
            Subtitles.Load();
        }

        public AllObscrunityData GetData()
        {
            return new AllObscrunityData
                   {
                       BNC = BNC.GetData(),
                       Internet = Internet.GetData(),
                       Reuters = Reuters.GetData(),
                       Subtitles = Subtitles.GetData()
                   };
        }
    }
}
