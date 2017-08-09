using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell
{
    /// <summary>
    ///     A valuing of having the influence to affect the policies of others.
    /// </summary>
    public class PowerData : DataItem
    {
        public override string Name => "Power";

        /// <summary>
        ///     Referring to political places and environments except nation-states.
        /// </summary>
        [InfoField("PowAren")]
        public bool IsArenas { get; private set; }

        /// <summary>
        ///     Concerned with a tools or forms of invoking formal power.
        /// </summary>
        [InfoField("PowAuth")]
        public bool IsAuthoritative { get; private set; }

        /// <summary>
        ///     Individual and collective actors in power process
        /// </summary>
        [InfoField("PowAuPt")]
        public bool IsAuthoritativeParticipant { get; private set; }

        /// <summary>
        ///     Ways of conflicting
        /// </summary>
        [InfoField("PowCon")]
        public bool IsConflict { get; private set; }

        /// <summary>
        ///     Ways of cooperating
        /// </summary>
        [InfoField("PowCoop")]
        public bool IsCooperation { get; private set; }

        /// <summary>
        ///     Recognized ideas about power relations and practices.
        /// </summary>
        [InfoField("PowDoct")]
        public bool IsDoctrine { get; private set; }

        /// <summary>
        ///     The goals of the power proces
        /// </summary>
        [InfoField("PowEnds")]
        public bool IsEnd { get; private set; }

        /// <summary>
        ///     Power increasing
        /// </summary>
        [InfoField("PowGain")]
        public bool IsGain { get; private set; }

        /// <summary>
        ///     Power decreasing
        /// </summary>
        [InfoField("PowLoss")]
        public bool IsLoss { get; private set; }

        /// <summary>
        ///     Non-authoritative actors (such as followers) in the power process.
        /// </summary>
        [InfoField("PowPt")]
        public bool IsOrdinaryParticipant { get; private set; }

        /// <summary>
        ///     Not in other subcategories
        /// </summary>
        [InfoField("PowOth")]
        public bool IsResidual { get; private set; }

        /// <summary>
        ///     Whole domain
        /// </summary>
        [InfoField("PowTot")]
        public bool IsTotal { get; private set; }
    }
}
