using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Serialization;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Svm.Extensions;
using Wikiled.Sentiment.Analysis.Processing.Arff;
using Wikiled.Sentiment.Analysis.Stats;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class TestingClient
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentBag<Document> result = new ConcurrentBag<Document>();

        private readonly IObservable<IParsedDocumentHolder> reviews;

        private readonly ISplitterHelper splitter;

        private readonly StatisticsCalculator statistics = new StatisticsCalculator();

        private IArffDataSet arff;

        private IProcessArff arffProcess;

        private int error;

        private ITrainingPerspective perspective;

        public TestingClient(ISplitterHelper splitter, IObservable<IParsedDocumentHolder> reviews, string svmPath = null)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => reviews, reviews);
            if (string.IsNullOrEmpty(svmPath))
            {
                DisableSvm = true;
            }

            this.splitter = splitter;
            this.reviews = reviews;
            SvmPath = svmPath;
            AspectSentiment = new AspectSentimentTracker(new ContextSentimentFactory());
            SentimentVector = new SentimentVector();
        }

        public AspectSentimentTracker AspectSentiment { get; }

        public bool DisableAspects { get; set; }

        public bool DisableSvm { get; set; }

        public int Errors => error;

        public string AspectPath { get; set; }

        public ResultsHolder Holder { get; } = new ResultsHolder();

        public PrecisionRecallCalculator<bool> Performance { get; } = new PrecisionRecallCalculator<bool>();

        public SentimentVector SentimentVector { get; }

        public string SvmPath { get; set; }

        public bool UseBagOfWords { get; set; }

        public string GetPerformanceDescription()
        {
            return string.Format($"{Performance.GetTotalAccuracy()} RMSE:{statistics.CalculateRmse():F2}");
        }

        public void Init()
        {
            result.Clear();
            if (DisableSvm)
            {
                perspective = NullTrainingPerspective.Instance;
            }
            else
            {
                var machine = MachineSentiment<PositivityType>.Load(SvmPath);
                machine.FilterCoef(Path.Combine(SvmPath, "coef.txt"));
                perspective = new SimpleTrainingPerspective(machine, machine.Header);
            }

            arff = ArffDataSet.Create<PositivityType>("MAIN");
            var factory = UseBagOfWords ? new UnigramProcessArffFactory() : (IProcessArffFactory)new ProcessArffFactory();
            arffProcess = factory.Create(arff);

            if (!DisableAspects &&
                (!string.IsNullOrEmpty(AspectPath) || !string.IsNullOrEmpty(SvmPath)))
            {
                string path = string.IsNullOrEmpty(AspectPath) ? Path.Combine(SvmPath, "aspects.xml") : AspectPath;
                if (File.Exists(path))
                {
                    log.Info("Loading {0} aspects", path);
                    XDocument features = XDocument.Load(path);
                    var aspect = splitter.DataLoader.AspectFactory.ConstructSerializer().Deserialize(features);
                    splitter.DataLoader.AspectDectector = aspect;
                    splitter.DataLoader.DisableFeatureSentiment = true;
                }
                else
                {
                    log.Warn("{0} aspects file not found", path);
                }
            }

            log.Info("Processing...");
        }

        public IObservable<ParsingResult> Process()
        {
            var documentSelector = reviews.Where(item => item != null)
                                          .SelectMany(item => Observable.Start(() => Process(item)))
                                          .Merge();
            return documentSelector
                .Where(item => item != null)
                .SubscribeOn(TaskPoolScheduler.Default);
        }

        public void Save(string path)
        {
            path.EnsureDirectoryExistence();
            result.ToArray().XmlSerialize().Save(Path.Combine(path, "result.xml"));
            var aspectSentiments = AspectSentiment.GetResults();
            aspectSentiments.XmlSerialize().Save(Path.Combine(path, "aspect_sentiment.xml"));
            var vector = SentimentVector.GetVector(NormalizationType.None);
            vector.XmlSerialize().Save(Path.Combine(path, "sentiment_vector.xml"));
            Holder.Save(Path.Combine(path, "result.csv"));
            arffProcess.Normalize(NormalizationType.L2);
            arff.FullSave(path);
        }

        private async Task<ParsingResult> Process(IParsedDocumentHolder document)
        {
            var parsed = await RetrieveData(document).ConfigureAwait(false);
            if (parsed != null)
            {
                result.Add(parsed.Document);
            }

            return parsed;
        }

        private async Task<ParsingResult> RetrieveData(IParsedDocumentHolder holder)
        {
            try
            {
                var parsed = await holder.GetParsed().ConfigureAwait(false);
                var review = parsed.GetReview(splitter.DataLoader);
                var doc = holder.Original;
                RatingAdjustment adjustment = new RatingAdjustment(review, perspective);
                splitter.DataLoader.NRCDictionary.ExtractToVector(SentimentVector, review.Items);
                var document = review.GenerateDocument(adjustment);
                AspectSentiment.Process(review);
                if (doc.Stars != null)
                {
                    Holder.AddResult(doc.Id, doc.Stars.Value, adjustment.Rating.StarsRating);
                }
                else
                {
                    log.Debug("Document doesn't have star assigned");
                }

                if (adjustment.Rating.StarsRating == null)
                {
                    statistics.AddUnknown();
                    arffProcess.PopulateArff(review, PositivityType.Negative);
                }
                else
                {
                    if (doc.Stars != null)
                    {
                        statistics.Add(adjustment.Rating.StarsRating.Value, doc.Stars.Value);
                        if (doc.Stars != 3)
                        {
                            Performance.Add(
                                doc.Stars > 3,
                                adjustment.Rating.IsPositive);
                        }
                    }

                    arffProcess.PopulateArff(review, doc.Stars > 3 ? PositivityType.Positive : PositivityType.Negative);
                }

                return new ParsingResult(document, review);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref error);
                log.Error(ex);
            }

            return null;
        }
    }
}
