using Wikiled.Core.Utility.Arguments;

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

        public string[] RawCategories { get; }

        public string Word { get; }

        public InquirerDescription Description { get; }
    }
}
