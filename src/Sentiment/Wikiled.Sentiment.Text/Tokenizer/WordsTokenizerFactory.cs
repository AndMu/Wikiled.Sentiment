using System.Linq;
using Wikiled.Common.Arguments;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class WordsTokenizerFactory : IWordsTokenizerFactory
    {
        public const string Grouped = @"[\w\d_']+|[\w]+";

        public const string GroupedWithSymbols = @"[\w\d_']+|[\w]+|[,;]|[!.?]$";

        public const string LettersAndDigits = @"\w+";

        //    "\\S+"   (anything not whitespace)
        //    "\\w+"    ( A-Z, a-z, 0-9, _ )
        //    "[\\p{L}\\p{N}_]+|[\\p{P}]+"   (a group of only letters and numbers OR
        //                                    a group of only punctuation marks)
        public const string NotWhiteSpace = @"\S+";

        private readonly RegexSplitter splitter;

        private readonly IWordItemFactory wordItemFactory;

        public WordsTokenizerFactory(
            string pattern,
            IWordItemFactory wordItemFactory,
            CombinedPipeline<string> pipeline,
            CombinedPipeline<WordEx> wordItemPipeline)
        {
            Guard.NotNull(() => pipeline, pipeline);
            Guard.NotNull(() => wordItemFactory, wordItemFactory);
            Guard.NotNull(() => wordItemPipeline, wordItemPipeline);

            splitter = new RegexSplitter(pattern);
            Pipeline = pipeline;
            this.wordItemFactory = wordItemFactory;
            WordItemPipeline = wordItemPipeline;
        }

        public CombinedPipeline<string> Pipeline { get; }

        public CombinedPipeline<WordEx> WordItemPipeline { get; }

        public IWordsTokenizer Create(string sentence)
        {
            if (string.IsNullOrEmpty(sentence))
            {
                return NullWordsTokenizer.Instance;
            }

            var words = splitter.Split(sentence).ToArray();
            if (words.Length == 0)
            {
                return NullWordsTokenizer.Instance;
            }

            return new WordTokenizer(sentence, wordItemFactory, Pipeline, WordItemPipeline, words);
        }
    }
}
