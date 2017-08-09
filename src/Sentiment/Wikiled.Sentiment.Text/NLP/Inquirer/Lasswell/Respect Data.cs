using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell
{
    /// <summary>
    /// Valuing of status, honor, recognition and prestige
    /// </summary>
    public class RespectData : DataItem
    {
        /// <summary>
        /// Garnering of respect, such as congratulations
        /// </summary>
        [InfoField("RspGain")]
        public bool IsGain { get; private set; }

        /// <summary>
        /// Losing of respect, such as shame.
        /// </summary>
        [InfoField("RspLoss")]
        public bool IsLoss { get; private set; }

        /// <summary>
        /// Respect that are neither gain nor loss
        /// </summary>
        [InfoField("RspOth")]
        public bool IsOther { get; private set; }

        /// <summary>
        /// Other
        /// </summary>
        [InfoField("RspTot")]
        public bool IsTotal { get; private set; }

        public override string Name => "Respect";
    }
}
