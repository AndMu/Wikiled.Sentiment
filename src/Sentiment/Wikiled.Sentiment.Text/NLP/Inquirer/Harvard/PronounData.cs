using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Pronoun reflecting an "I" vs. "we" vs. "you" orientation, as well as names
    /// </summary>
    public class PronounData : DataItem
    {
        /// <summary>
        /// Pronoun referring to the singular self
        /// </summary>
        [InfoField("Self")]
        public bool IsSelf { get; private set; }

        /// <summary>
        /// Pronoun referring to the inclusive self ("we", etc.)
        /// </summary>
        [InfoField("Our")]
        public bool IsOur { get; private set; }

        /// <summary>
        /// Pronoun indicating another person is being addressed directly.
        /// </summary>
        [InfoField("You")]
        public bool IsYou { get; private set; }

        /// <summary>
        /// Only contains names identified in the Harvard IV dictionary
        /// </summary>
        [InfoField("Name")]
        public bool IsName { get; private set; }

        public override string Name => "Pronoun";
    }
}
