using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Indicating overstatement and understatement, often reflecting presence or lack of emotional expressiveness
    /// </summary>
    public class StatementData : DataItem
    {
        /// <summary>
        /// Indicating emphasis in realms of speed, frequency, causality, inclusiveness, quantity or quasi-quantity, accuracy, validity, scope, size, clarity, exceptionality, intensity, likelihood, certainty and extremity.
        /// </summary>
        [InfoField("Ovrst")]
        public bool IsOverstated { get; private set; }

        /// <summary>
        /// Indicating de-emphasis and caution in these realms.
        /// </summary>
        [InfoField("Undrst")]
        public bool IsUnderstated { get; private set; }

        public override string Name => "Statement";
    }
}
