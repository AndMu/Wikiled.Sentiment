using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP.Inquirer;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    public static class InquirerManagerExtension
    {
        public static InquirerDefinition GetWordDefinitions(this InquirerManager inquirer, WordEx word)
        {
            IWordItem item = word.UnderlyingWord as IWordItem;
            return item != null ? GetWordDefinitions(inquirer, item) : inquirer.GetDefinitions(word.Text);
        }

        public static InquirerDefinition GetWordDefinitions(this InquirerManager inquirer, IWordItem word)
        {
            Guard.NotNull(() => word, word);
            foreach (var text in word.GetPossibleText().Distinct())
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
