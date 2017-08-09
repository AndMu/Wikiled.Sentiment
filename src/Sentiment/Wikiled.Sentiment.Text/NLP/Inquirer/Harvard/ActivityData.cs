using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Referring to roles, collectivities, rituals, and forms of interpersonal relations, 
    /// often within one of these institutional contexts.
    /// </summary>
    public class ActivityData : DataItem
    {
        /// <summary>
        /// Referring to identifiable and standardized individual human behavior patterns, as used by sociologists.
        /// </summary>
        [InfoField("Role")]
        public bool IsRole { get; private set; }

        /// <summary>
        /// Referring to all human collectivities (not animal). Used in disambiguation.
        /// </summary>
        [InfoField("COLL")]
        public bool IsCollectivity { get; private set; }

        /// <summary>
        /// Socially defined ways for doing work.
        /// </summary>
        [InfoField("Work")]
        public bool IsWork { get; private set; }

        /// <summary>
        /// Non-work social rituals
        /// </summary>
        [InfoField("Ritual")]
        public bool IsRitual { get; private set; }

        /// <summary>
        /// Socially-defined interpersonal processes (formerly called "IntRel", for interpersonal relations).
        /// </summary>
        [InfoField("SocRel")]
        public bool IsSociallyRelation { get; private set; }

        public override string Name => "Activity";
    }
}
