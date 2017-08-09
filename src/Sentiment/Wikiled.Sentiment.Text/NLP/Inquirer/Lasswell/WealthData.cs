using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell
{
    /// <summary>
    /// Wealth is the valuing of having it.
    /// </summary>
    public class WealthData : DataItem
    {
        /// <summary>
        /// Wealth participant - various roles in business and commerce.
        /// </summary>
        [InfoField("WltPt")]
        public bool IsParticipant { get; private set; }

        /// <summary>
        /// Wealth transaction - for pursuit of wealth, such as buying and selling.
        /// </summary>
        [InfoField("WltTran")]
        public bool IsTransaction { get; private set; }

        /// <summary>
        /// Wealth-related words not in the above, including economic domains and commodities.
        /// </summary>
        [InfoField("WltOth")]
        public bool IsOther { get; private set; }

        /// <summary>
        /// Wealth domain.
        /// </summary>
        [InfoField("WltTot")]
        public bool IsTotal { get; private set; }

        public override string Name => "Wealth";
    }
}
