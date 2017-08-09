using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.Text.NLP.Style.Obscurity;
using Wikiled.Sentiment.Text.NLP.Style.Readability;
using Wikiled.Sentiment.Text.NLP.Style.Surface;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Reflection;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    [InfoCategory("Text Style")]
    public class TextBlock : IDataSource
    {
        private readonly Dictionary<string, List<WordEx>> lemmaDictionary = new Dictionary<string, List<WordEx>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<WordEx>> wordDictionary = new Dictionary<string, List<WordEx>>(StringComparer.OrdinalIgnoreCase);

        public TextBlock(IWordsHandler handler, SentenceItem[] sentences,  bool load = true)
        {
            Guard.NotEmpty(() => sentences, sentences);
            Sentences = sentences;
            Surface = new SurfaceData(this);
            Readability = new ReadabilityDataSource(this);
            Words = (from sentence in Sentences
                     from word in sentence.Words
                     select word).ToArray();
            Guard.IsValid(() => Words, Words, item => item.Length > 0, "sentences");
            var pure = new List<WordEx>();
            foreach (var word in Words)
            {
                if (word.Text.HasLetters() ||
                    (word.Text.Length > 0 && char.IsDigit(word.Text[0])))
                {
                    pure.Add(word);
                }
                
                lemmaDictionary.GetSafeCreate(handler.Extractor.GetWord(word.Text)).Add(word);
                wordDictionary.GetSafeCreate(word.Text).Add(word);
            }

            PureWords = pure.ToArray();
            VocabularyObscurity = new VocabularyObscurity(this);
            SyntaxFeatures = new SyntaxFeatures(handler, this);
            InquirerFinger = new InquirerFingerPrint(this);

            if (load)
            {
                Load();
            }
        }

        public void Load()
        {
            TotalCharacters = Sentences.Sum(item => item.CountCharacters());
            Surface.Load();
            InquirerFinger.Load();
            SyntaxFeatures.Load();
            VocabularyObscurity.Load();
            Readability.Load();
        }

        [InfoCategory("Inquirer Based Info", IsCollapsed = true)]
        public InquirerFingerPrint InquirerFinger { get; }

        [InfoCategory("Syntax Features")]
        public SyntaxFeatures SyntaxFeatures { get; }

        [InfoCategory("Vocabulary Obscurity")]
        public VocabularyObscurity VocabularyObscurity { get; }

        [InfoCategory("Readability")]
        public ReadabilityDataSource Readability { get; }

        [InfoCategory("Text Surface")]
        public SurfaceData Surface { get; }

        public SentenceItem[] Sentences { get; }

        public WordEx[] PureWords { get; }

        public WordEx[] Words { get; }

        public int TotalCharacters { get; private set; }
       
        public int TotalLemmas => lemmaDictionary.Count;

        public int TotalWordTokens => wordDictionary.Count;
    }
}
