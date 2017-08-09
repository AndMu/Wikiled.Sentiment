using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.NLP.Inquirer.SocialCognition;
using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    public class HarwardDescription
    {
        public HarwardDescription()
        {
            Sentiment = new SentimentData(PositivityType.Neutral);
            Feeling = new FeelingData();
            Statement = new StatementData();
            Institution = new InstitutionData();
            Activity = new ActivityData();
            Social = new SocialData();
            Location = new LocationData();
            Object = new ObjectData();
            Communication = new CommunicationProcessData();
            Motivation = new MotivationData();
            OtherProcess = new OtherProcessData();
            CognitiveOrientation = new CognitiveOrientationData();
            Broad = new BroadData();
            Pronoun = new PronounData();
            NegationInterjections = new NegationInterjectionsData();
            Verb = new VerbData();
            Adjective = new AdjectiveData();
        }

        /// <summary>
        /// References to places, locations and routes between them
        /// </summary>
        [InfoCategory("Location")]
        public LocationData Location { get; private set; }

        /// <summary>
        /// Ascriptive social categories as well as general references to people and animals
        /// </summary>
        [InfoCategory("Social")]
        public SocialData Social { get; private set; }

        /// <summary>
        /// Referring to roles, collectivities, rituals, and forms of interpersonal relations, often within one of these institutional contexts
        /// </summary>
        [InfoCategory("Activity")]
        public ActivityData Activity { get; private set; }

        /// <summary>
        /// Reflecting the language of a particular "institution"
        /// </summary>
        [InfoCategory("Institution")]
        public InstitutionData Institution { get; private set; }

        /// <summary>
        /// Indicating overstatement and understatement, often reflecting presence or lack of emotional expressiveness:
        /// </summary>
        [InfoCategory("Statement")]
        public StatementData Statement { get; private set; }

        /// <summary>
        /// "Osgood" three semantic dimensions
        /// </summary>
        [InfoCategory("Sentiment")]
        public SentimentData Sentiment { get; set; }

        /// <summary>
        /// Pleasure, pain, virtue and vice
        /// </summary>
        [InfoCategory("Feeling")]
        public FeelingData Feeling { get; private set; }

        /// <summary>
        /// References to objects
        /// </summary>
        [InfoCategory("Object")]
        public ObjectData Object { get; private set; }

        /// <summary>
        /// Processes of communicating
        /// </summary>
        [InfoCategory("Communication")]
        public CommunicationProcessData Communication { get; private set; }

        /// <summary>
        /// Motivation-related
        /// </summary>
        [InfoCategory("Motivation")]
        public MotivationData Motivation { get; private set; }

        /// <summary>
        /// Other process or change
        /// </summary>
        [InfoCategory("OtherProcess")]
        public OtherProcessData OtherProcess { get; private set; }

        /// <summary>
        /// Cognitive orientation (knowing, assessment, and problem solving)
        /// </summary>
        [InfoCategory("CognitiveOrientation")]
        public CognitiveOrientationData CognitiveOrientation { get; private set; }

        /// <summary>
        /// Any other broader groups
        /// </summary>
        [InfoCategory("Broad")]
        public BroadData Broad { get; private set; }

        /// <summary>
        /// Pronoun reflecting an "I" vs. "we" vs. "you" orientation, as well as names
        /// </summary>
        [InfoCategory("Pronoun")]
        public PronounData Pronoun { get; private set; }

        /// <summary>
        /// "Yes", "No", negation and interjections.
        /// </summary>
        [InfoCategory("Pronoun")]
        public NegationInterjectionsData NegationInterjections { get; private set; }

        /// <summary>
        /// Verb - based social cognition
        /// </summary>
        [InfoCategory("Verb")]
        public VerbData Verb { get; private set; }

        /// <summary>
        /// Adjective - based social cognition
        /// </summary>
        [InfoCategory("Verb")]
        public AdjectiveData Adjective { get; private set; }
    }
}
