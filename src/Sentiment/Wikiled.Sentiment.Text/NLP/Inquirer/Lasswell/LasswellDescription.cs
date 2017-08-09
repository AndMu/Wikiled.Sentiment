using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell
{
    /// <summary>
    /// his dictionary divides language into four deference domains: power, rectitude, respect, affiliation, 
    /// and four welfare domains: wealth, well-being, enlightenment and skill. Within each domain, there may 
    /// be such subcategories as gains, losses, participants, ends, and arenas.
    ///  In addition to subcategory counts, there is a total count for each domain. 
    /// The dictionary authors avoided categorizing a word or word sense in more than one domain and one subcategory in that domain, 
    /// even though more than one domain or more than one subcategory may be relevant. 
    /// However, a few consistency errors have been uncovered by the spreadsheet conversion.
    /// </summary>
    public class LasswellDescription
    {
        public LasswellDescription()
        {
            Power = new PowerData();
            Rectitude = new RectitudeData();
            Respect = new RespectData();
            Wealth = new WealthData();
            Enlightenment = new EnlightenmentData();
            Skill = new SkillData();
            Remaining = new RemainingData();
            WellBeing = new WellBeingData();
            Affection = new AffectionData();
        }

        /// <summary>
        /// A valuing of having the influence to affect the policies of others.
        /// </summary>
        [InfoCategory("Power")]
        public PowerData Power { get; private set; }

        /// <summary>
        /// Concerned with moral values and has fewer subcategories:
        /// </summary>
        [InfoCategory("Rectitude")]
        public RectitudeData Rectitude { get; private set; }

        /// <summary>
        /// Valuing of status, honor, recognition and prestige
        /// </summary>
        [InfoCategory("Respect")]
        public RespectData Respect { get; private set; }

        /// <summary>
        /// Wealth is the valuing of having it.
        /// </summary>
        [InfoCategory("Wealth")]
        public WealthData Wealth { get; private set; }

        /// <summary>
        /// Knowledge, insight, and information concerning personal and cultural relations
        /// </summary>
        [InfoCategory("Enlightenment")]
        public EnlightenmentData Enlightenment { get; private set; }

        /// <summary>
        /// Reflect the valuing of skills. especially those of the arts in the aesthetics subcategory
        /// </summary>
        [InfoCategory("Skill")]
        public SkillData Skill { get; private set; }

        /// <summary>
        /// Remaining Lasswell dictionary categories not specific to one of the value domains.
        /// </summary>
        [InfoCategory("Remaining")]
        public RemainingData Remaining { get; private set; }

        /// <summary>
        /// Well-being refers, according to Lasswell, to the "health and safety of the organism".
        /// </summary>
        [InfoCategory("Well Being")]
        public WellBeingData WellBeing { get; private set; }

        /// <summary>
        /// Affection is the valuing of love and friendship
        /// </summary>
        [InfoCategory("Affection")]
        public AffectionData Affection { get; private set; }
    }
}
