using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell
{
    /// <summary>
    /// Well-being refers, according to Lasswell, to the "health and safety of the organism".
    /// </summary>
    public class WellBeingData : DataItem
    {
        /// <summary>
        /// Words related to a gain in well being
        /// </summary>
        [InfoField("WlbGain")]
        public bool IsGain { get; private set; }

        /// <summary>
        /// Words related to a loss in a state of well being, including being upset.
        /// </summary>
        [InfoField("WlbLoss")]
        public bool IsLoss { get; private set; }

        /// <summary>
        /// Words connoting the physical aspects of well being, including its absence.
        /// </summary>
        [InfoField("WlbPhys")]
        public bool IsPhysicalAspect { get; private set; }

        /// <summary>
        /// Roles that evoke a concern for well-being, including infants, doctors, and vacationers.
        /// </summary>
        [InfoField("WlbPt")]
        public bool IsRole { get; private set; }

        /// <summary>
        /// Words connoting the psychological aspects of well being, including its absence.
        /// </summary>
        [InfoField("WlbPsyc")]
        public bool IsPsychological { get; private set; }

        /// <summary>
        /// Words in well-being domain
        /// </summary>
        [InfoField("WlbTot")]
        public bool IsTotal { get; private set; }

        public override string Name => "Well-being";
    }
}
