using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Text.NLP.Style.Description
{
    public class WordStyle
    {
        public InquirerData Inquirer { get; set; }

        public NRCRecord NRC { get; set; }
    }
}
