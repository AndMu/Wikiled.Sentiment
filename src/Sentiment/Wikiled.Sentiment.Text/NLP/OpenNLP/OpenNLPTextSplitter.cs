using Microsoft.Extensions.Logging;
using SharpNL.Chunker;
using SharpNL.POSTag;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.NLP.NER;
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

        private readonly ILexiconConfig configuration;

        private readonly ISentenceTokenizer sentenceSplitter;

        private readonly ITreebankWordTokenizer tokenizer;

        private readonly ISentenceRepairHandler repairHandler;

        private ChunkerME chunker;

        private POSTaggerME posTagger;

        private readonly IEnumerable<INamedEntityRecognition> neResolver;

        public OpenNLPTextSplitter(ILogger<OpenNLPTextSplitter> log,
                                   ILexiconConfig configuration,
                                   ICachedDocumentsSource cache,
                                   ISentenceTokenizerFactory tokenizerFactory,
                                   ISentenceRepairHandler repairHandler,
                                   IEnumerable<INamedEntityRecognition> neResolver)
            : base(log, cache)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.repairHandler = repairHandler ?? throw new ArgumentNullException(nameof(repairHandler));
            this.neResolver = neResolver;
            log.LogDebug("Creating with resource path: {0}", configuration.Resources);
            tokenizer = TreebankWordTokenizer.Tokenizer;
            sentenceSplitter = tokenizerFactory.Create(true, false);
            LoadModels();
        }

        protected override LightDocument ActualProcess(ParseRequest request)
        {
            // NOT Thread Safe
            var sentences = sentenceSplitter.Split(request.Document.Text).ToArray();

            var document = new LightDocument();
            document.Text = request.Document.Text;
            document.Sentences = new LightSentence[sentences.Length];

            int added = 0;

            foreach (var sentence in sentences)
            {
                var text = repairHandler.Repair(sentence);
                if (sentence != text)
                {
                    log.LogTrace("Sentence repaired!");
                }

                var result = ProcessSentence(text);
                if (result != null)
                {
                    document.Sentences[added] = result;
                    added++;
                } 
            }

            if (added < document.Sentences.Length)
            {
                var sentencesData = document.Sentences;
                Array.Resize(ref sentencesData, added);
                document.Sentences = sentencesData;
            }

            return document;
        }

        private LightSentence ProcessSentence(string text)
        {
            var tokens = tokenizer.Tokenize(text);
            if (tokens.Length <= 0)
            {
                return null;
            }

            var tags = posTagger.Tag(tokens);
            var currentSentence = new LightSentence();
            currentSentence.Text = text;
            currentSentence.Words = new LightWord[tokens.Length];

            for (var i = 0; i < tokens.Length; i++)
            {
                var wordData = new LightWord();
                wordData.Tag = tags[i];
                wordData.Text = tokens[i];
                currentSentence.Words[i] = wordData;
            }

            NERExtraction(currentSentence, tokens);
            PhraseExtraction(currentSentence, tokens, tags);
            return currentSentence;
        }

        private void NERExtraction(LightSentence currentSentence, string[] tokens)
        {
            foreach (var ner in neResolver.SelectMany(item => item.Tag(tokens)))
            {
                for (var i = ner.Start; i < ner.End; i++)
                {
                    currentSentence.Words[i].Entity = ner.Type;
                }
            }
        }

        private void PhraseExtraction(LightSentence currentSentence, string[] tokens, string[] tags)
        {
            var chunks = chunker.ChunkAsSpans(tokens, tags);
            foreach (var chunk in chunks)
            {
                for (var i = chunk.Start; i < chunk.End; i++)
                {
                    currentSentence.Words[i].Phrase = chunk.Type;
                }
            }
        }

        public override void Dispose()
        {
            chunker?.Dispose();
            posTagger?.Dispose();
            base.Dispose();
        }

        private void LoadModels()
        {
            POSModel posModel;
            using (var modelFile = new FileStream(Path.Combine(configuration.Resources, configuration.NlpModels, "en-pos-maxent.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                posModel = new POSModel(modelFile);
            }

            ChunkerModel chunkerModel;
            using (var modelFile = new FileStream(Path.Combine(configuration.Resources, configuration.NlpModels, "en-chunker.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                chunkerModel = new ChunkerModel(modelFile);
            }

            posTagger = new POSTaggerME(posModel);
            chunker = new ChunkerME(chunkerModel);
        }
    }
}
