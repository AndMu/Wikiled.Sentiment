using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Ascriptive social categories as well as general references to people and animals
    /// </summary>
    public class SocialData : DataItem
    {
        /// <summary>
        /// Referring to racial or ethnic characteristics
        /// </summary>
        [InfoField("Race")]
        public bool IsRacial { get; private set; }

        /// <summary>
        /// Denoting kinship
        /// </summary>
        [InfoField("Kin@")]
        public bool IsKin { get; private set; }

        /// <summary>
        /// Referring to men and social roles associated with men. 
        /// </summary>
        [InfoField("MALE")]
        public bool IsMale { get; private set; }

        /// <summary>
        /// Referring to women and social roles associated with women
        /// </summary>
        [InfoField("Female")]
        public bool IsFemale { get; private set; }

        /// <summary>
        /// Associated with infants through adolescents
        /// </summary>
        [InfoField("Nonadlt")]
        public bool IsNonAdult { get; private set; }

        /// <summary>
        /// General references to humans, including roles
        /// </summary>
        [InfoField("HU")]
        public bool IsHuman { get; private set; }

        /// <summary>
        /// References to animals, fish, birds, and insects, including their collectivities.
        /// </summary>
        [InfoField("ANI")]
        public bool IsAnimal { get; private set; }

        public override string Name => "Social";
    }
}
