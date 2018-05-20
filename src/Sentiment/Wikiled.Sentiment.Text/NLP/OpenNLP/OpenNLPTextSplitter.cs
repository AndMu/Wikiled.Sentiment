using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using SharpNL.Chunker;
using SharpNL.POSTag;
using SharpNL.Utility;
using Wikiled.Common.Arguments;
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

        private readonly IWordsHandler handler;

        private readonly ISentenceTokenizer sentenceSplitter;

        private readonly ITreebankWordTokenizer tokenizer;

        private ChunkerME chunker;

        private POSTaggerME posTagger;

        public OpenNLPTextSplitter(IWordsHandler handler, string resourcesFolder, ICachedDocumentsSource cache)
            : base(handler, cache)
        {
            Guard.NotNull(() => resourcesFolder, resourcesFolder);
            Guard.NotNull(() => handler, handler);
            Guard.NotNullOrEmpty(() => resourcesFolder, resourcesFolder);
            log.Debug("Creating with resource path: {0}", resourcesFolder);
            this.handler = handler;
            tokenizer = new TreebankWordTokenizer();
            sentenceSplitter = SentenceTokenizer.Create(handler, true, false);
            LoadModels(resourcesFolder);
        }

        protected override Document ActualProcess(ParseRequest request)
        {
            var sentences = sentenceSplitter.Split(request.Document.Text).ToArray();
            var sentenceDataList = new List<SentenceData>(sentences.Length);

            for (int i = 0; i < sentences.Length; i++)
            {
                var sentenceData = new SentenceData { Text = sentences[i] };
                sentenceData.Tokens = tokenizer.Tokenize(sentenceData.Text);
                if (sentenceData.Tokens.Length > 0)
                {
                    sentenceData.Tags = posTagger.Tag(sentenceData.Tokens);
                    sentenceData.Chunks = chunker.ChunkAsSpans(sentenceData.Tokens, sentenceData.Tags).ToArray();
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
                Dictionary<int, Span> chunks = new Dictionary<int, Span>();
                foreach (var chunk in sentenceData.Chunks)
                {
                    for (int i = chunk.Start; i < chunk.End; i++)
                    {
                        chunks[i] = chunk;
                    }
                }

                for (int i = 0; i < sentenceData.Tokens.Length; i++)
                {
                    var tag = sentenceData.Tags[i];
                    var word = sentenceData.Tokens[i];
                    var wordItem = handler.WordFactory.CreateWord(word, tag);
                    wordItem.WordIndex = index;
                    var wordData = WordExFactory.Construct(wordItem);
                    currentSentence.Add(wordData);
                    index++;
                    if (chunks.TryGetValue(i, out var chunk))
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
