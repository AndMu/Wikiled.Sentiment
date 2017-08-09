using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer.Harvard
{
    /// <summary>
    /// Are usually also classified positive or negative, with virtue indicating strength and vice indicating weakness. 
    /// They provide more focus than the categories in the first two sections.
    /// </summary>
    public class FeelingData : DataItem
    {
        /// <summary>
        /// Indicating the enjoyment of a feeling, including words indicating confidence, interest and commitment.
        /// </summary>
        [InfoField("Pleasur")]
        public bool IsPleasure { get; private set; }

        /// <summary>
        /// Indicating suffering, lack of confidence, or commitment.
        /// </summary>
        [InfoField("Pain")]
        public bool IsPain { get; private set; }

        /// <summary>
        /// Describing particular feelings, including gratitude, apathy, and optimism, not those of pain or pleasure.
        /// </summary>
        [InfoField("Feel")]
        public bool IsFeel { get; private set; }

        /// <summary>
        /// Indicating excitation, aside from pleasures or pains, but including arousal of affiliation and hostility.
        /// </summary>
        [InfoField("Arousal")]
        public bool IsArousal { get; private set; }

        /// <summary>
        /// Indicating an assessment of moral approval or good fortune, especially from the perspective of middle-class society.
        /// </summary>
        [InfoField("Virtue")]
        public bool IsVirtue { get; private set; }

        /// <summary>
        /// Indicating an assessment of moral disapproval or misfortune.
        /// </summary>
        [InfoField("Vice")]
        public bool IsVice { get; private set; }

        /// <summary>
        /// Related to emotion that are used as a disambiguation category, but also available for general use.
        /// </summary>
        [InfoField("EMOT")]
        public bool IsEmotion { get; private set; }

        public override string Name => "Feeling";
    }
}
