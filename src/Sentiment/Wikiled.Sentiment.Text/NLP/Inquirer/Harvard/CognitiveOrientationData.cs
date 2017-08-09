using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Cognitive orientation (knowing, assessment, and problem solving)
    /// </summary>
    public class CognitiveOrientationData : DataItem
    {
        /// <summary>
        /// Referring to the presence or absence of rational thought processes.
        /// </summary>
        [InfoField("Think")]
        public bool IsThinkinkg { get; private set; }

        /// <summary>
        /// Indicating awareness or unawareness, certainty or uncertainty, similarity or difference, generality or specificity, importance or unimportance, presence or absence, as well as components of mental classes, concepts or ideas.
        /// </summary>
        [InfoField("Know")]
        public bool IsKnowing { get; private set; }

        /// <summary>
        /// Denoting presumption that occurrence of one phenomenon is necessarily preceded, accompanied or followed by the occurrence of another.
        /// </summary>
        [InfoField("Causal")]
        public bool IsCausal{ get; private set; }

        /// <summary>
        /// Indicating moral imperative.
        /// </summary>
        [InfoField("Ought")]
        public bool IsOught{ get; private set; }

        /// <summary>
        /// Referring to the perceptual process of recognizing or identifying something by means of the senses.
        /// </summary>
        [InfoField("Perceiv")]
        public bool IsPerceiving{ get; private set; }

        /// <summary>
        /// Comparison
        /// </summary>
        [InfoField("Compare")]
        public bool IsComparison{ get; private set; }

        /// <summary>
        /// Word which imply judgment and evaluation, whether positive or negative, including means-ends judgments.
        /// </summary>
        [InfoField("Eval@")]
        public bool IsEvaluation{ get; private set; }

        /// <summary>
        /// Word (mostly verb) referring to the mental processes associated with problem solving.
        /// </summary>
        [InfoField("Solve")]
        public bool IsSolving{ get; private set; }

        /// <summary>
        /// Reflecting tendency to use abstract vocabulary
        /// </summary>
        [InfoField("Abs@")]
        public bool IsAbstract{ get; private set; }

        /// <summary>
        /// Indicating qualities or degrees of qualities which can be detected or measured by the human senses. Virtues and vices are separate.
        /// </summary>
        [InfoField("Quality")]
        public bool IsQuality{ get; private set; }

        /// <summary>
        /// Indicating the assessment of quantity, including the use of numbers
        /// </summary>
        [InfoField("Quan")]
        public bool IsQuantity{ get; private set; }

        /// <summary>
        /// Numbers
        /// </summary>
        [InfoField("NUMB")]
        public bool IsNumber { get; private set; }

        /// <summary>
        /// Ordinal words
        /// </summary>
        [InfoField("ORD")]
        public bool IsOrdinal { get; private set; }

        /// <summary>
        /// Cardinal words
        /// </summary>
        [InfoField("CARD")]
        public bool IsCardinal { get; private set; }

        /// <summary>
        /// Indicating an assessment of frequency or pattern of recurrences, as well as words indicating an assessment of nonoccurrence or low frequency. (Also used in disambiguation)
        /// </summary>
        [InfoField("FREQ")]
        public bool IsFrequency { get; private set; }

        /// <summary>
        ///  Referring to distance and its measures. (Used in disambiguation)
        /// </summary>
        [InfoField("DIST")]
        public bool IsDistance { get; private set; }

        /// <summary>
        /// Indicating a time consciousness, including when events take place and time taken in an action. Includes velocity words as well
        /// </summary>
        [InfoField("Time@")]
        public bool IsTime { get; private set; }

        /// <summary>
        /// More restrictive Time category
        /// </summary>
        [InfoField("TIME")]
        public bool IsTime2 { get; private set; }

        /// <summary>
        /// Indicating a consciousness of location in space and spatial relationships
        /// </summary>
        [InfoField("Space")]
        public bool IsSpace { get; private set; }

        /// <summary>
        /// Position
        /// </summary>
        [InfoField("POS")]
        public bool IsPosition { get; private set; }

        /// <summary>
        /// GetRelativeDimension 
        /// </summary>
        [InfoField("DIM")]
        public bool IsDimension { get; private set; }

        /// <summary>
        /// Indicating a consciousness of abstract relationships between people, places, objects and ideas, apart from relations in space and time. 
        /// </summary>
        [InfoField("Rel")]
        public bool IsRelationships { get; private set; }

        /// <summary>
        /// Color, used in disambiguation
        /// </summary>
        [InfoField("COLOR")]
        public bool IsColor { get; private set; }

        public override string Name => "Cognitive orientation";
    }
}
