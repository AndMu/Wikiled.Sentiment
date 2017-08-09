using Wikiled.Sentiment.Text.NLP.NRC;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;

namespace Wikiled.Sentiment.Text.NLP.Style.Description
{
    public class WordStyle
    {
        public InquirerData Inquirer { get; set; }

        public NRCRecord NRC { get; set; }
    }
}
