using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell
{
    /// <summary>
    /// Concerned with moral values and has fewer subcategories:
    /// </summary>
    public class RectitudeData : DataItem
    {
        /// <summary>
        /// Values concerning the social order.
        /// </summary>
        [InfoField("RcEthic")]
        public bool IsEthics { get; private set; }

        /// <summary>
        /// Invoke transcendental, mystical or supernatural grounds for rectitude.
        /// </summary>
        [InfoField("RcRelig")]
        public bool IsReligion { get; private set; }

         /// <summary>
        /// Worship and forgiveness
        /// </summary>
        [InfoField("RcGain")]
        public bool IsGain { get; private set; }

        /// <summary>
        /// Sin and denounce
        /// </summary>
        [InfoField("RcLoss")]
        public bool IsLoss { get; private set; }

         /// <summary>
        /// Including heaven and the high-frequency word "ought".
        /// </summary>
        [InfoField("RcEnds")]
        public bool IsEnds { get; private set; }

        /// <summary>
        /// The whole domain
        /// </summary>
        [InfoField("RcTot")]
        public bool IsTotal { get; private set; }

        public override string Name => "Rectitude";
    }
}
