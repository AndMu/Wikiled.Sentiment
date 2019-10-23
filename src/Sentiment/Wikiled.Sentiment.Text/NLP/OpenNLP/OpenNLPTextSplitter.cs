using Microsoft.Extensions.Logging;
using SharpNL.Chunker;
using SharpNL.POSTag;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.NLP.Repair;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Cache;
using Wikiled.Text.Analysis.Structure.Light;
using Wikiled.Text.Analysis.Tokenizer;
using Wikiled.Text.Analysis.Tokenizer.Pipelined;

namespace Wikiled.Sentiment.Text.NLP.OpenNLP
{
    public class OpenNLPTextSplitter : BaseTextSplitter
    {
        private readonly ILogger<OpenNLPTextSplitter> log;

        private readonly ISentenceTokenizer sentenceSplitter;

        private readonly ITreebankWordTokenizer tokenizer;

        private readonly IContextSentenceRepairHandler repairHandler;

        private ChunkerME chunker;

        private POSTaggerME posTagger;

        public OpenNLPTextSplitter(
            ILogger<OpenNLPTextSplitter> log,
            ILexiconConfiguration configuration,
            ICachedDocumentsSource cache,
            ISentenceTokenizerFactory tokenizerFactory,
            IContextSentenceRepairHandler repairHandler)
            : base(log, cache)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.log = log ?? throw new ArgumentNullException(nameof(log));
            log.LogDebug("Creating with resource path: {0}", configuration.ResourcePath);
            this.repairHandler = repairHandler ?? throw new ArgumentNullException(nameof(repairHandler));
            
            tokenizer = TreebankWordTokenizer.Tokenizer;
            sentenceSplitter = tokenizerFactory.Create(true, false);
            LoadModels(configuration.ResourcePath);
        }

        protected override LightDocument ActualProcess(ParseRequest request)
        {
            var sentences = sentenceSplitter.Split(request.Document.Text).ToArray();
            var sentenceDataList = new List<SentenceData>(sentences.Length);
            foreach (var sentence in sentences)
            {
                var text = repairHandler.Repair(sentence);
                var sentenceData = new SentenceData { Text = text };
                sentenceData.Tokens = tokenizer.Tokenize(sentenceData.Text);
                if (sentenceData.Tokens.Length <= 0)
                {
                    continue;
                }

                sentenceData.Tags = posTagger.Tag(sentenceData.Tokens);
                sentenceData.Chunks = chunker.ChunkAsSpans(sentenceData.Tokens, sentenceData.Tags).ToArray();
                sentenceDataList.Add(sentenceData);
            }

            var document = new LightDocument();
            document.Text = request.Document.Text;
            foreach (SentenceData sentenceData in sentenceDataList)
            {
                if (string.IsNullOrWhiteSpace(sentenceData.Text))
                {
                    continue;
                }

                var currentSentence = new LightSentence();
                currentSentence.Text = sentenceData.Text;
                document.Sentences.Add(currentSentence);
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
                    var wordData = new LightWord();
                    wordData.Tag = sentenceData.Tags[i];
                    wordData.Text = sentenceData.Tokens[i];
                    //IWordItem wordItem = handler.CreateWord(word, tag);
                    //wordItem.WordIndex = index;
                    //WordEx wordData = WordExFactory.Construct(wordItem);
                    currentSentence.Words.Add(wordData);
                    
                    if (chunks.TryGetValue(i, out Span chunk))
                    {
                        wordData.Phrase = chunk.Type;
                    }
                }
            }

            return document;
        }

        public override void Dispose()
        {
            chunker?.Dispose();
            posTagger?.Dispose();
            base.Dispose();
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
