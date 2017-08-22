using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Resources;
using Wikiled.Core.Utility.Serialization;
using Wikiled.MachineLearning.Clustering;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.NLP.NRC;
using Wikiled.Sentiment.Text.NLP.Style;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Logic;
using Wikiled.Text.Inquirer.Reflection;
using Wikiled.Text.Inquirer.Reflection.Data;

namespace Wikiled.Sentiment.Text.Anomaly
{
    public class DocumentAnomalyDetector
    {
        private const int MovingAverage = 3;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly IMapCategory mapFull = new CategoriesMapper().Construct<TextBlock>();

        private readonly CosineSimilarityDistance distanceLogic = new CosineSimilarityDistance();

        private readonly IInquirerManager inquirer;

        private readonly SentenceItem[] sentences;

        private readonly IWordsHandler wordsHandler;

        private ILookup<SentenceItem, SentenceItem> anomalyLookup;

        private double anomalyThreshold;

        private double windowSize;

        public DocumentAnomalyDetector(IInquirerManager inquirer, IWordsHandler wordsHandler, IParsedReview document)
        {
            Guard.NotNull(() => wordsHandler, wordsHandler);
            Guard.NotNull(() => document, document);
            Guard.NotNull(() => inquirer, inquirer);
            this.wordsHandler = wordsHandler;
            this.inquirer = inquirer;
            Document = document.GenerateDocument(NullRatingAdjustment.Instance);
            sentences = Document.Sentences.ToArray();
            AnomalyThreshold = 0.1;
            windowSize = 0.1;
        }

        public SentenceItem[] Anomaly { get; private set; }

        public int AnomalySentencesCount => (int)Math.Ceiling(sentences.Length * AnomalyThreshold);

        public double AnomalyThreshold
        {
            get => anomalyThreshold;
            set
            {
                if (value <= 0 ||
                    value > 1)
                {
                    throw new ArgumentOutOfRangeException("AnomalyThreshold");
                }

                anomalyThreshold = value;
            }
        }

        public AnomalyVectorType AnomalyVectorType { get; set; }

        public Document Document { get; }

        public int MinimumSentencesCount => (int)Math.Ceiling(sentences.Length * WindowSize);

        public double MinimumWordsCount
        {
            get
            {
                return (int)Math.Ceiling(sentences.Sum(item => item.Words.Count) * WindowSize);
            }
        }

        public VectorData RemainingVector { get; set; }

        public bool UseSentimentClusters { get; set; }

        public bool UseVector { get; set; }

        public double WindowSize
        {
            get => windowSize;
            set
            {
                if (value <= 0 ||
                    value > 1)
                {
                    throw new ArgumentOutOfRangeException("WindowSize");
                }

                windowSize = value;
            }
        }

        public SentenceItem[] WithoutAnomaly { get; private set; }

        public void Detect()
        {
            log.Debug("Detect");
            if (sentences.Length <= 3)
            {
                log.Debug("Detect - text too short");
                return;
            }

            var ratings = sentences.Select(item => item.CalculateSentiment())
                                   .MovingAverage(3)
                                   .ToArray();

            ClusterRegion[] clusters = ClusterFlow.GetRegions(ratings, MovingAverage);
            ConcurrentBag<IItemProbability<SentenceItem[]>> list = new ConcurrentBag<IItemProbability<SentenceItem[]>>();
            IEnumerable<SentenceItem[]> sentenceClusters = UseSentimentClusters
                                                               ? GetSentencesBlockForRegions(clusters)
                                                               : GetSentencesBlock();
            Parallel.ForEach(
                sentenceClusters,
                AsyncSettings.DefaultParallel,
                segment =>
                    {
                        if (segment.Length == 0)
                        {
                            return;
                        }

                        VectorData currentVector = GetVector(segment, NormalizationType.L2);
                        var remainingSentences = sentences.Where(item => !segment.Contains(item)).ToArray();
                        if (remainingSentences.Length == 0)
                        {
                            return;
                        }

                        VectorData remainingVector = RemainingVector ??
                                                     GetVector(remainingSentences, NormalizationType.L2);
                        double distance = distanceLogic.Measure(
                            currentVector,
                            remainingVector);
                        list.Add(
                            new ItemProbability<SentenceItem[]>(segment)
                                {
                                    Probability = Math.Abs(distance)
                                });
                    });

            var processed = list.OrderBy(item => item.Probability);
            Reset();

            List<SentenceItem> excluding = new List<SentenceItem>();
            foreach (var itemProbability in processed)
            {
                if (excluding.Distinct().Count() >= AnomalySentencesCount)
                {
                    break;
                }

                excluding.AddRange(itemProbability.Data);
            }

            if (excluding.Count > 0)
            {
                Anomaly = excluding.ToArray();
                anomalyLookup = Anomaly.ToLookup(item => item);
                WithoutAnomaly = sentences.Where(item => !anomalyLookup.Contains(item)).ToArray();
            }
        }

        public SentenceItem[] GetData(ClusterRegion region)
        {
            int start = region.StartIndex == 0
                            ? 0
                            : region.StartIndex + MovingAverage / 2;
            int end = region.EndIndex + MovingAverage / 2;
            end = end > sentences.Length - 1 ? sentences.Length - 1 : end;

            List<SentenceItem> items = new List<SentenceItem>();
            for (int i = start; i < end; i++)
            {
                items.Add(sentences[i]);
            }

            return items.ToArray();
        }

        public VectorData GetDocumentVector(NormalizationType normalization)
        {
            return GetVector(Document.Sentences, normalization);
        }

        public bool IsInAnomaly(SentenceItem sentence)
        {
            return anomalyLookup != null && anomalyLookup.Contains(sentence);
        }

        public void LoadBaseVector()
        {
            RemainingVector = null;
            if (!UseVector)
            {
                return;
            }

            string vectorFile = Path.Combine(EnvironmentSettings.StartingLocation, "vector.xml");
            if (File.Exists(vectorFile))
            {
                AnomalyVector anomalyVector = XDocument.Load(vectorFile).XmlDeserialize<AnomalyVector>();
                RemainingVector = anomalyVector.GetData(AnomalyVectorType);
                RemainingVector.ChangeNormalization(NormalizationType.L2);
            }
        }

        private IEnumerable<SentenceItem[]> GetSentencesBlock()
        {
            return sentences.WindowedEx(
                MinimumSentencesCount,
                data => data.Select(item => item.Words.Count).Sum() >= MinimumWordsCount);
        }

        private IEnumerable<SentenceItem[]> GetSentencesBlockForRegions(ClusterRegion[] regions)
        {
            return regions.Select(GetData);
        }

        private DataTree GetTree(TextBlock block)
        {
            SentimentVector vector = new SentimentVector();
            vector.Extract(block.Words);
            return vector.GetTree();
        }

        private VectorData GetVector(IEnumerable<SentenceItem> normalBlock, NormalizationType normalization)
        {
            TextBlock normal = new TextBlock(inquirer, wordsHandler, normalBlock.ToArray(), true);
            DataTree tree;
            switch (AnomalyVectorType)
            {
                case AnomalyVectorType.Full:
                    tree = new DataTree(normal, mapFull);
                    break;
                case AnomalyVectorType.Inquirer:
                    tree = normal.InquirerFinger.InquirerProbabilities;
                    break;
                case AnomalyVectorType.SentimentCategory:
                    tree = GetTree(normal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("AnomalyVectorType");
            }

            VectorData vector = tree.CreateVector(normalization);
            return vector;
        }

        private void Reset()
        {
            Anomaly = new SentenceItem[] { };
            WithoutAnomaly = sentences;
            anomalyLookup = null;
        }
    }
}
