using System.Collections.Generic;
using System.IO;
using NLog;
using SharpNL.Analyzer;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Text.NLP.OpenNLP
{
    public class OpenNLPTextSplitter : BaseTextSplitter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IWordsHandler handler;

        private readonly AggregateAnalyzer analyzer;

        public OpenNLPTextSplitter(IWordsHandler handler, string resourcesFolder, ICachedDocumentsSource cache)
            : base(handler, cache)
        {
            Guard.NotNull(() => resourcesFolder, resourcesFolder);
            Guard.NotNull(() => handler, handler);
            Guard.NotNullOrEmpty(() => resourcesFolder, resourcesFolder);
            log.Debug("Creating with resource path: {0}", resourcesFolder);
            this.handler = handler;
            analyzer = new AggregateAnalyzer
                           {
                               Path.Combine(resourcesFolder, @"1.5\en-sent.bin"),
                               Path.Combine(resourcesFolder, @"1.5\en-chunker.bin"),
                               Path.Combine(resourcesFolder, @"1.5\en-token.bin"),
                               Path.Combine(resourcesFolder, @"1.5\en-pos-maxent.bin")
                           };
        }

        protected override Document ActualProcess(ParseRequest request)
        {
            var processing = new SharpNL.Document("en", request.Document.Text);
            analyzer.Analyze(processing);
            Document document = new Document(request.Document.Text);
            foreach (var sentenceData in processing.Sentences)
            {
                if (string.IsNullOrWhiteSpace(sentenceData.Text))
                {
                    continue;
                }

                var currentSentence = new SentenceItem(sentenceData.Text);
                document.Add(currentSentence);
                int index = 0;
                foreach (var chunk in sentenceData.Chunks)
                {
                    if (chunk.Tokens.Count > 1)
                    {
                        var phrase = chunk.Tag;
                        List<WordEx> phraseList = new List<WordEx>();
                        foreach (var child in chunk.Tokens)
                        {
                            var wordItem = handler.WordFactory.CreateWord(child.Lexeme, child.POSTag);
                            wordItem.WordIndex = index;
                            var wordEx = WordExFactory.Construct(wordItem);
                            currentSentence.Add(wordEx);
                            phraseList.Add(wordEx);
                            index++;
                        }

                        foreach (var wordEx in phraseList)
                        {
                            wordEx.Phrase = phrase;
                        }
                    }
                    else
                    {
                        var word = chunk.Tokens[0];
                        var wordItem = handler.WordFactory.CreateWord(word.Lexeme, word.POSTag);
                        wordItem.WordIndex = index;
                        currentSentence.Add(WordExFactory.Construct(wordItem));
                        index++;
                    }
                }
            }

            return document;
        }
    }
}
