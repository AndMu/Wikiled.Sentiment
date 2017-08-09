using System.Collections.Generic;
using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class WordTokenizer : IWordsTokenizer
    {
        private readonly IPipeline<string> pipeline;

        private readonly IWordItemFactory wordItemFactory;

        private readonly IPipeline<WordEx> wordItemPipeline;

        private readonly string[] words;

        public WordTokenizer(
            string sentence,
            IWordItemFactory wordItemFactory,
            IPipeline<string> pipeline,
            IPipeline<WordEx> wordItemPipeline,
            string[] words)
        {
            Guard.NotNull(() => pipeline, pipeline);
            Guard.NotNull(() => wordItemFactory, wordItemFactory);
            Guard.NotNull(() => wordItemPipeline, wordItemPipeline);
            Guard.NotNullOrEmpty(() => sentence, sentence);
            Guard.IsValid(() => words, words, item => item != null && item.Length > 0, "Words");
            this.pipeline = pipeline;
            this.wordItemFactory = wordItemFactory;
            this.wordItemPipeline = wordItemPipeline;
            this.words = words;
            SentenceText = sentence;
        }

        public string SentenceText { get; }

        public IEnumerable<WordEx> GetWordItems()
        {
            return wordItemPipeline.Process(GetWords().Select(item => WordExFactory.Construct(wordItemFactory.Construct(item))));
        }

        public IEnumerable<string> GetWords()
        {
            foreach(var word in pipeline.Process(words))
            {
                if(string.IsNullOrEmpty(word))
                {
                    continue;
                }

                yield return word;
            }
        }
    }
}
