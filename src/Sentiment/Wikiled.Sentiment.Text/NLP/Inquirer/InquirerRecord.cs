using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;

namespace Wikiled.Sentiment.Text.NLP.Inquirer
{
    public class InquirerRecord
    {
        public InquirerRecord(string word, string[] categories)
        {
            Guard.NotNullOrEmpty(() => word, word);
            Guard.NotEmpty(() => categories, categories);
            Word = word;
            RawCategories = categories;
            Description = InquirerDefinitionFactory.Instance.Construct(this);
        }

        public string[] RawCategories { get; private set; }

        public string Word { get; private set; }

        public InquirerDescription Description { get; private set; }
    }
}
