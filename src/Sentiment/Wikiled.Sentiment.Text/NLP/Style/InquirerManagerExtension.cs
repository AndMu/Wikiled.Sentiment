using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Data;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    public static class InquirerManagerExtension
    {
        public static InquirerDefinition GetWordDefinitions(this IInquirerManager inquirer, WordEx word)
        {
            IWordItem item = word.UnderlyingWord as IWordItem;
            return item != null ? GetWordDefinitions(inquirer, item) : inquirer.GetDefinitions(word.Text);
        }

        public static InquirerDefinition GetWordDefinitions(this IInquirerManager inquirer, IWordItem word)
        {
            Guard.NotNull(() => word, word);
            foreach (var text in word.GetPossibleText())
            {
                var current = inquirer.GetDefinitions(text);
                if (current.Records.Length > 0)
                {
                    return current;
                }
            }

            return new InquirerDefinition(word.Text, new InquirerRecord[] { });
        }
    }
}
