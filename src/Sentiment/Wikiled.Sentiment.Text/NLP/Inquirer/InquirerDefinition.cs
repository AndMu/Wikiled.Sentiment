using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;

namespace Wikiled.Sentiment.Text.NLP.Inquirer
{
    public class InquirerDefinition
    {
        public InquirerDefinition(string word, InquirerRecord[] records)
        {
            Guard.NotNullOrEmpty(() => word, word);
            Guard.NotNull(() => records, records);
            Word = word;
            Records = records;
        }

        public string Word { get; private set; }

        public InquirerRecord[] Records { get; private set; }
    }
}
