using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell
{
    /// <summary>
    /// Remaining Lasswell dictionary categories not specific to one of the value domains.
    /// </summary>
    public class RemainingData : DataItem
    {
        /// <summary>
        /// General words of accomplishment
        /// </summary>
        [InfoField("TrnGain")]
        public bool IsTransactionGain { get; private set; }

        /// <summary>
        /// General words of not accomplishing, but having setbacks instead.
        /// </summary>
        [InfoField("TrnLoss")]
        public bool IsTransactionLoss { get; private set; }

        /// <summary>
        /// Transaction or exchange in a broad sense, but not necessarily of gain or loss
        /// </summary>
        [InfoField("TranLw")]
        public bool IsTransaction { get; private set; }

        /// <summary>
        /// General words referring to means and utility or lack of same. Overlaps little with Means category
        /// </summary>
        [InfoField("MeansLw")]
        public bool IsMeans { get; private set; }

        /// <summary>
        /// Desired or undesired ends or goals
        /// </summary>
        [InfoField("EndsLw")]
        public bool IsEnds { get; private set; }

        /// <summary>
        /// Settings, other than power related arenas in PowAren.
        /// </summary>
        [InfoField("ArenaLw")]
        public bool IsArena { get; private set; }

        /// <summary>
        /// Actors not otherwise defined by the dictionary
        /// </summary>
        [InfoField("PtLw")]
        public bool IsActor { get; private set; }

        /// <summary>
        /// Nation - needs updating
        /// </summary>
        [InfoField("Nation")]
        public bool IsNation { get; private set; }

        /// <summary>
        /// "a negation of value preference", nihilism, disappointment and futility.
        /// </summary>
        [InfoField("Anomie")]
        public bool IsAnomie { get; private set; }

        /// <summary>
        /// Negative affect "denoting negative feelings and emotional rejection.
        /// </summary>
        [InfoField("NegAff")]
        public bool IsNegativeAffect { get; private set; }

        /// <summary>
        /// positive affect "denoting positive feelings, acceptance, appreciation and emotional support.
        /// </summary>
        [InfoField("PosAff")]
        public bool IsPositiveAffect  { get; private set; }

        /// <summary>
        /// "a feeling of sureness, certainty and firmness."
        /// </summary>
        [InfoField("SureLw")]
        public bool IsSure { get; private set; }

        /// <summary>
        /// "a feeling of sureness, certainty and firmness."
        /// </summary>
        [InfoField("If")]
        public bool IsIf { get; private set; }

        /// <summary>
        /// "that show the denial of one sort or another. "
        /// </summary>
        [InfoField("NotLw")]
        public bool IsNot { get; private set; }

        /// <summary>
        /// "a general space-time category"
        /// </summary>
        [InfoField("TimeSpc")]
        public bool IsTimeSpace { get; private set; }

        /// <summary>
        /// Formats, standards, tools and conventions of communication. almost entirely a subset of the 895 words in ConForm category
        /// </summary>
        [InfoField("FormLw")]
        public bool IsFormat { get; private set; }

        public override string Name => "Remaining";
    }
}
