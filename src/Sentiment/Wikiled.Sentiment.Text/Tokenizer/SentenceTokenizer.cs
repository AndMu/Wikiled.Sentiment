using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class SentenceTokenizer : ISentenceTokenizer
    {
        private const string SentencePattern = @"(\S.+?[.!?])?(?=\s+|$)|.+";

        private readonly RegexSplitter splitter;

        private SentenceTokenizer(string pattern, IWordsTokenizerFactory wordPipelineFactory)
        {
            splitter = new RegexSplitter(pattern);
            TokenizerFactory = wordPipelineFactory ?? throw new System.ArgumentNullException(nameof(wordPipelineFactory));
        }

        private SentenceTokenizer(IWordsTokenizerFactory wordPipelineFactory)
            : this(SentencePattern, wordPipelineFactory)
        {
        }

        public IWordsTokenizerFactory TokenizerFactory { get; }

        public static ISentenceTokenizer Create(IWordsHandler wordsHandler, bool simple, bool removeStopWords)
        {
            return Create(wordsHandler, WordsTokenizerFactory.NotWhiteSpace, simple, removeStopWords);
        }

        public static ISentenceTokenizer Create(IWordsHandler wordsHandler, string wordPattern, bool simple, bool removeStopWords)
        {
            List<IPipeline<WordEx>> pipelines = new List<IPipeline<WordEx>>();
            if (!simple)
            {
                pipelines.Add(InvertorPipeline.Instance);
            }

            if (removeStopWords)
            {
                pipelines.Add(new StopWordItemPipeline());
                pipelines.Add(new WordItemFilterOutPipeline(item => item.IsConjunction()));
            }

            WordsTokenizerFactory factory = new WordsTokenizerFactory(
                wordPattern,
                new SimpleWordItemFactory(wordsHandler),
                new CombinedPipeline<string>(LowerCasePipeline.Instance, WordCleanupPipeline.Instance, PunctuationPipeline.Instance),
                new CombinedPipeline<WordEx>(pipelines.ToArray()));
            return new SentenceTokenizer(factory);
        }

        public IEnumerable<IWordsTokenizer> Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));
            }

            string[] sentences = splitter.Split(text).ToArray();
            string saved = string.Empty;
            for (int i = 0; i < sentences.Length; i++)
            {
                string currentSentence = sentences[i].Trim();
                while (currentSentence.Length > 1 &&
                       currentSentence[currentSentence.Length - 2] == ' ')
                {
                    currentSentence = currentSentence.Remove(currentSentence.Length - 2, 1);
                }

                if (string.IsNullOrWhiteSpace(currentSentence))
                {
                    continue;
                }

                if (i < sentences.Length - 1)
                {
                    string nextSentence = sentences[i + 1];
                    bool found = currentSentence.Count(char.IsLetterOrDigit) <= 2;
                    if (!found)
                    {
                        for (int j = 0; j < nextSentence.Length && j <= 3; j++)
                        {
                            if (nextSentence[j] == '.')
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found)
                    {
                        saved += currentSentence;
                        continue;
                    }
                }

                if (!string.IsNullOrWhiteSpace(saved))
                {
                    currentSentence = saved + " " + currentSentence;
                }

                IWordsTokenizer wordsTokenizer = TokenizerFactory.Create(currentSentence);
                saved = string.Empty;
                if (wordsTokenizer != NullWordsTokenizer.Instance)
                {
                    yield return wordsTokenizer;
                }
            }
        }

        public IEnumerable<string> Split(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));
            }

            return Parse(text).Select(wordsTokenizer => wordsTokenizer.SentenceText);
        }
    }
}
