using System;
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
            return word.UnderlyingWord is IWordItem item ? GetWordDefinitions(inquirer, item) : inquirer.GetDefinitions(word.Text);
        }

        public static InquirerDefinition GetWordDefinitions(this IInquirerManager inquirer, IWordItem word)
        {
            if (inquirer is null)
            {
                throw new ArgumentNullException(nameof(inquirer));
            }

            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

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
