using System;
using System.Linq;
using Wikiled.Text.Inquirer.Data;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    public static class InquirerDefinitionExtensions
    {
        public static bool HasDefined(this InquirerDefinition definition, Func<InquirerRecord, bool> condition)
        {
            return definition.Records.Any(condition);
        }
    }
}
