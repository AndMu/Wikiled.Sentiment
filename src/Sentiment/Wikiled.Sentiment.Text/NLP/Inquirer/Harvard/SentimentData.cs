using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// These categories reflect Charles Osgood's semantic differential findings regarding basic language universals.
    /// </summary>
    public class SentimentData : DataItem
    {
        public SentimentData(PositivityType type)
        {
            Type = type;
        }

        /// <summary>
        /// Sentiment polarity
        /// </summary>
        public PositivityType Type { get; }

        /// <summary>
        /// Implying an active orientation.
        /// </summary>
        [InfoField("Active")]
        public bool IsActive { get; private set; }

        /// <summary>
        /// Indicating a passive orientation
        /// </summary>
        [InfoField("Passive")]
        public bool IsPassive { get; private set; }

        /// <summary>
        /// Implying weakness
        /// </summary>
        [InfoField("Weak")]
        public bool IsWeak { get; private set; }

        /// <summary>
        /// Implying strength
        /// </summary>
        [InfoField("Strong")]
        public bool IsStrong { get; private set; }

        /// <summary>
        /// Indicating affiliation or supportiveness
        /// </summary>
        [InfoField("Affil")]
        public bool IsSupportive { get; private set; }

        /// <summary>
        /// Indicating an attitude or concern with hostility or aggressiveness
        /// </summary>
        [InfoField("Hostile")]
        public bool IsHostile { get; private set; }

        /// <summary>
        /// Indicating a concern with power, control or authority
        /// </summary>
        [InfoField("Power")]
        public bool IsPower { get; private set; }

        /// <summary>
        /// Connoting submission to authority or power, dependence on others, vulnerability to others, or withdrawal
        /// </summary>
        [InfoField("Submit")]
        public bool IsSubmit { get; private set; }

        public override string Name => "Sentiment";

        public override bool HasData => Type != PositivityType.Neutral || base.HasData;
    }
}
