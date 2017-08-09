using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Motivation-related 
    /// </summary>
    public class MotivationData : DataItem
    {
        /// <summary>
        /// Words related to the expression of need or intent.
        /// </summary>
        [InfoField("Need")]
        public bool IsNeed { get; private set; }

        /// <summary>
        /// Names of end-states towards which muscular or mental striving is directed.
        /// </summary>
        [InfoField("Goal")]
        public bool IsGoal { get; private set; }

        /// <summary>
        /// Words indicating activities taken to reach a goal, but not including words indicating that the goals have been achieved.
        /// </summary>
        [InfoField("Try")]
        public bool IsTry { get; private set; }

        /// <summary>
        /// Words denoting objects, acts or methods utilized in attaining goals. Only 16 words overlap with Lasswell dictionary 77-word category MeansLw.
        /// </summary>
        [InfoField("Means")]
        public bool IsMeans { get; private set; }

        /// <summary>
        /// Words indicating "stick to it" and endurance.
        /// </summary>
        [InfoField("Persist")]
        public bool IsPersist { get; private set; }

        /// <summary>
        /// Words indicating that goals have been achieved, apart from whether the action may continue. The termination of action is indicated by the category Finish.
        /// </summary>
        [InfoField("Complet")]
        public bool HasCompleted { get; private set; }

        /// <summary>
        /// Words indicating that goals have not been achieved.
        /// </summary>
        [InfoField("Fail")]
        public bool HasFailed { get; private set; }

        public override string Name => "Motivation";
    }
}
