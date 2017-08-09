using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Style.Surface
{
    /// <summary>
    /// Simple Surface data
    /// </summary>
    public class SurfaceData : IDataSource
    {
        public SurfaceData(TextBlock text)
        {
            Guard.NotNull(() => text, text);
            Text = text;
            Sentence = new SentenceSurface(text);
            Characters = new CharactersSurface(text);
            Words = new WordSurface(text);
        }

        public void Load()
        {
            Words.Load();
            Sentence.Load();
            Characters.Load();
        }

        [InfoCategory("Words Surface")]
        public WordSurface Words { get; private set; }

        public TextBlock Text { get; private set; }

        [InfoCategory("Sentences Surface")]
        public SentenceSurface Sentence { get; private set; }

        [InfoCategory("Characters Surface")]
        public CharactersSurface Characters { get; private set; }
    }
}
