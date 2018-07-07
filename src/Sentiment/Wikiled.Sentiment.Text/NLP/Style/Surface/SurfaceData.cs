using System;
using Wikiled.Text.Analysis.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Style.Surface
{
    /// <summary>
    ///     Simple Surface data
    /// </summary>
    public class SurfaceData : IDataSource
    {
        public SurfaceData(TextBlock text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Sentence = new SentenceSurface(text);
            Characters = new CharactersSurface(text);
            Words = new WordSurface(text);
        }

        [InfoCategory("Characters Surface")]
        public CharactersSurface Characters { get; }

        [InfoCategory("Sentences Surface")]
        public SentenceSurface Sentence { get; }

        public TextBlock Text { get; }

        [InfoCategory("Words Surface")]
        public WordSurface Words { get; }

        public void Load()
        {
            Words.Load();
            Sentence.Load();
            Characters.Load();
        }
    }
}
