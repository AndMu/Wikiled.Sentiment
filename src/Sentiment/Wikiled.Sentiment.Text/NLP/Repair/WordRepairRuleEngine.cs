using System;
using System.Linq;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Common.Extensions;
using Wikiled.Common.Helpers;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class WordRepairRuleEngine
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly WordRepairRule repairRule;

        private readonly IWordItem wordItem;

        public WordRepairRuleEngine(IWordItem item, WordRepairRule repairRule)
        {
            Guard.NotNull(() => item, item);
            wordItem = item;
            this.repairRule = repairRule;
        }

        public bool? Evaluate()
        {
            if (repairRule == null)
            {
                return null;
            }

            if (repairRule.Set == null)
            {
                return !repairRule.SuccesfulResult;
            }

            if (repairRule.Set.Any(EvaluateRuleSet))
            {
                return repairRule.SuccesfulResult;
            }

            return !repairRule.SuccesfulResult;
        }

        private bool EvaluateRules(Rule rule)
        {
            var item = wordItem.Relationship.GetNextByIndex(rule.Index);
            if (item == null)
            {
                return false;
            }

            if (rule.NextWordPOS != WordType.Unknown)
            {
                if (item.POS.WordType != rule.NextWordPOS)
                {
                    log.Debug("POS haven't matched: {0} - {1}", item.POS.WordType, rule.NextWordPOS);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(rule.Word))
            {
                return string.Compare(rule.Word, item.Text, StringComparison.OrdinalIgnoreCase) == 0;
            }

            return string.IsNullOrEmpty(rule.Ending) || item.Text.IsEnding(rule.Ending);
        }

        private bool EvaluateRuleSet(RuleSet set)
        {
            return set.Rules.All(EvaluateRules);
        }
    }
}
