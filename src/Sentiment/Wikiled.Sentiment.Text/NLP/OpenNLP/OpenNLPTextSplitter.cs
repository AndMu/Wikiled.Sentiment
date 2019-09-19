using Microsoft.Extensions.Logging;
using SharpNL.Chunker;
using SharpNL.POSTag;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Analysis.Tokenizer;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;

namespace Wikiled.Sentiment.Text.NLP.OpenNLP
{
    public class OpenNLPTextSplitter : BaseTextSplitter
    {
        private readonly ILogger<OpenNLPTextSplitter> log;

        private readonly IWordFactory handler;

        private readonly ISentenceTokenizer sentenceSplitter;

        private readonly ITreebankWordTokenizer tokenizer;

        private readonly IContextSentenceRepairHandler repairHandler;

        private ChunkerME chunker;

        private POSTaggerME posTagger;

        public OpenNLPTextSplitter(
            ILogger<OpenNLPTextSplitter> log,
            IWordFactory handler,
            ILexiconConfiguration configuration,
            ICachedDocumentsSource cache,
            ISentenceTokenizerFactory tokenizerFactory,
            IContextSentenceRepairHandler repairHandler)
            : base(cache)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.log = log ?? throw new ArgumentNullException(nameof(log));
            log.LogDebug("Creating with resource path: {0}", configuration.ResourcePath);
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.repairHandler = repairHandler ?? throw new ArgumentNullException(nameof(repairHandler));
            
            tokenizer = TreebankWordTokenizer.Tokenizer;
            sentenceSplitter = tokenizerFactory.Create(true, false);
            LoadModels(configuration.ResourcePath);
        }

        protected override Document ActualProcess(ParseRequest request)
        {
            var sentences = sentenceSplitter.Split(request.Document.Text).ToArray();
            var sentenceDataList = new List<SentenceData>(sentences.Length);
            for (var i = 0; i < sentences.Length; i++)
            {
                var text = repairHandler.Repair(sentences[i]);
                var sentenceData = new SentenceData { Text = text };
                sentenceData.Tokens = tokenizer.Tokenize(sentenceData.Text);
                if (sentenceData.Tokens.Length > 0)
                {
                    sentenceData.Tags = posTagger.Tag(sentenceData.Tokens);
                    sentenceData.Chunks = chunker.ChunkAsSpans(sentenceData.Tokens, sentenceData.Tags).ToArray();
                    sentenceDataList.Add(sentenceData);
                }
            }

            var document = new Document(request.Document.Text);
            foreach (SentenceData sentenceData in sentenceDataList)
            {
                if (string.IsNullOrWhiteSpace(sentenceData.Text))
                {
                    continue;
                }

                var currentSentence = new SentenceItem(sentenceData.Text);
                document.Add(currentSentence);
                var index = 0;
                var chunks = new Dictionary<int, Span>();
                foreach (Span chunk in sentenceData.Chunks)
                {
                    for (var i = chunk.Start; i < chunk.End; i++)
                    {
                        chunks[i] = chunk;
                    }
                }

                for (var i = 0; i < sentenceData.Tokens.Length; i++)
                {
                    var tag = sentenceData.Tags[i];
                    var word = sentenceData.Tokens[i];
                    IWordItem wordItem = handler.CreateWord(word, tag);
                    wordItem.WordIndex = index;
                    WordEx wordData = WordExFactory.Construct(wordItem);
                    currentSentence.Add(wordData);
                    index++;
                    if (chunks.TryGetValue(i, out Span chunk))
                    {
                        wordData.Phrase = chunk.Type;
                    }
                }
            }

            return document;
        }

        private void LoadModels(string resourcesFolder)
        {
            POSModel posModel;
            using (var modelFile = new FileStream(Path.Combine(resourcesFolder, @"1.5/en-pos-maxent.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                posModel = new POSModel(modelFile);
            }

            ChunkerModel chunkerModel;
            using (var modelFile = new FileStream(Path.Combine(resourcesFolder, @"1.5/en-chunker.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                chunkerModel = new ChunkerModel(modelFile);
            }

            posTagger = new POSTaggerME(posModel);
            chunker = new ChunkerME(chunkerModel);
        }
    }
}
