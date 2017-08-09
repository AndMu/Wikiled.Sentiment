using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// "Yes", "No", negation and interjections.
    /// </summary>
    public class NegationInterjectionsData : DataItem
    {
        /// <summary>
        /// Directly indicating agreement, including word senses "of course", "to say the least", "all right".
        /// </summary>
        [InfoField("Yes")]
        public bool IsYes { get; private set; }

        /// <summary>
        /// Directly indicating disagreement, with the word "no" itself disambiguated to separately identify absence or negation.
        /// </summary>
        [InfoField("No")]
        public bool IsNo { get; private set; }

        /// <summary>
        /// Refer to reversal or negation, including about 20 "dis" words, 40 "in" words, and 100 "un" words, as well as several senses of the word "no" itself; generally signals a downside view
        /// </summary>
        [InfoField("Negate")]
        public bool IsNegate { get; private set; }

        /// <summary>
        /// Includes exclamations as well as casual and slang references, words categorized "yes" and "no" such as "amen" or "nope", as well as other words like "damn" and "farewell".
        /// </summary>
        [InfoField("Intrj")]
        public bool IsIntrj { get; private set; }

        public override string Name => "Negation";
    }
}
