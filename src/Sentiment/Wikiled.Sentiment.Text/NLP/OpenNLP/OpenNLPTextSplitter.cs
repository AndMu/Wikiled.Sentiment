using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.PosTagger;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Tokenizer;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;

namespace Wikiled.Sentiment.Text.NLP.OpenNLP
{
    public class OpenNLPTextSplitter : BaseTextSplitter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ITreebankWordTokenizer tokenizer;

        private readonly EnglishMaximumEntropyPosTagger tagger;

        private readonly EnglishTreebankChunker chunker;

        private readonly IWordsHandler handler;

        private readonly ISentenceTokenizer sentenceSplitter;

        public OpenNLPTextSplitter(IWordsHandler handler, string resourcesFolder, ICachedDocumentsSource cache)
            : base(handler, cache)
        {
            Guard.NotNull(() => resourcesFolder, resourcesFolder);
            Guard.NotNull(() => handler, handler);
            Guard.NotNullOrEmpty(() => resourcesFolder, resourcesFolder);
            log.Debug("Creating with resource path: {0}", resourcesFolder);
            sentenceSplitter = SentenceTokenizer.Create(handler, true, false);
            tokenizer = new TreebankWordTokenizer();
            tagger = new EnglishMaximumEntropyPosTagger(
                         Path.Combine(resourcesFolder, @"Models\EnglishPOS.nbin"),
                         Path.Combine(resourcesFolder, @"Models\Parser\tagdict"));
            chunker = new EnglishTreebankChunker(Path.Combine(resourcesFolder, @"Models\EnglishChunk.nbin"));
            this.handler = handler;
        }

        protected override Document ActualProcess(ParseRequest request)
        {
            var sentences = sentenceSplitter.Split(request.Document.Text).ToArray();
            var sentenceDataList = new List<SentenceData>(sentences.Length);

            //// !!!!! SVARBU mulki nenaudok jokiu Parallel sitoje klasese
            //// !!!!! uzpisi atminti - tik fixuotas poolas
            for (int i = 0; i < sentences.Length; i++)
            {
                var sentenceData = new SentenceData { Text = sentences[i] };
                sentenceData.Tokens = tokenizer.Tokenize(sentenceData.Text);
                if (sentenceData.Tokens.Length > 0)
                {
                    sentenceData.Tags = tagger.Tag(sentenceData.Tokens);
                    sentenceData.Chunks = chunker.GetChunks(sentenceData.Tokens, sentenceData.Tags).ToArray();
                    sentenceDataList.Add(sentenceData);
                }
            }

            Document document = new Document(request.Document.Text);
            foreach (var sentenceData in sentenceDataList)
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
                    if (chunk.TaggedWords.Count > 1)
                    {
                        var phrase = chunk.Tag;
                        List<WordEx> phraseList = new List<WordEx>();
                        foreach (var child in chunk.TaggedWords)
                        {
                            var wordItem = handler.WordFactory.CreateWord(child.Word, child.Tag);
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
                        var word = chunk.TaggedWords[0];
                        var wordItem = handler.WordFactory.CreateWord(word.Word, word.Tag);
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
