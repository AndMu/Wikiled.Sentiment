using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Other process or change words
    /// </summary>
    public class OtherProcessData : DataItem
    {
        /// <summary>
        /// Processes found in nature, birth to death.
        /// </summary>
        [InfoField("NatrPro")]
        public bool IsNaturalProcess { get; private set; }
        
        /// <summary>
        /// Change process
        /// </summary>
        [InfoField("Begin")]
        public bool IsBegin { get; private set; }

        /// <summary>
        /// Change process - Words indicating change without connotation of increase, decrease, beginning or ending
        /// </summary>
        [InfoField("Vary")]
        public bool IsVary { get; private set; }

        /// <summary>
        /// Change process
        /// </summary>
        [InfoField("Increas")]
        public bool IsIncrease { get; private set; }

        /// <summary>
        /// Change process
        /// </summary>
        [InfoField("Decreas")]
        public bool IsDecrease { get; private set; }

        /// <summary>
        /// Change process
        /// </summary>
        [InfoField("Finish")]
        public bool IsFinish { get; private set; }

        /// <summary>
        /// Movement category
        /// </summary>
        [InfoField("Stay")]
        public bool IsStay { get; private set; }

        /// <summary>
        /// Movement category
        /// </summary>
        [InfoField("Rise")]
        public bool IsRise { get; private set; }

        /// <summary>
        /// Movement category
        /// </summary>
        [InfoField("Exert")]
        public bool IsExert { get; private set; }

        /// <summary>
        /// Movement category
        /// </summary>
        [InfoField("Fetch")]
        public bool IsFetch { get; private set; }

        /// <summary>
        /// Movement category
        /// </summary>
        [InfoField("Travel")]
        public bool IsTravel { get; private set; }

        /// <summary>
        /// Movement category
        /// </summary>
        [InfoField("Fall")]
        public bool IsFall { get; private set; }

        public override string Name => "Other process";
    }
}
