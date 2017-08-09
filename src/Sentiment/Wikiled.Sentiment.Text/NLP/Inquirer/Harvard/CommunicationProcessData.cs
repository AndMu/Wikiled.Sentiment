using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Processes of communicating
    /// </summary>
    public class CommunicationProcessData : DataItem
    {
        /// <summary>
        /// Relating to the form, format or media of the communication transaction.
        /// </summary>
        [InfoField("ComForm")]
        public bool IsCommunicationTransaction { get; private set; }

        /// <summary>
        /// Word to say and tell
        /// </summary>
        [InfoField("Say")]
        public bool IsSaying { get; private set; }

        public override string Name => "Communication";
    }
}
