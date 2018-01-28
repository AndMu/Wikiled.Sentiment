using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
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
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Analysis.Stats;
using Wikiled.Sentiment.Text.Aspects;
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

        private readonly StatisticsCalculator statistics = new StatisticsCalculator();

        private IArffDataSet arff;

        private IProcessArff arffProcess;

        private int error;

        private ITrainingPerspective perspective;

        private readonly IProcessingPipeline pipeline;

        public TestingClient(IProcessingPipeline pipeline, string svmPath = null)
        {
            Guard.NotNull(() => pipeline, pipeline);
            if (string.IsNullOrEmpty(svmPath))
            {
                DisableSvm = true;
            }

            this.pipeline = pipeline;
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

        public string SvmPath { get; }

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
                    var aspect = pipeline.Splitter.DataLoader.AspectFactory.ConstructSerializer().Deserialize(features);
                    pipeline.Splitter.DataLoader.AspectDectector = aspect;
                    pipeline.Splitter.DataLoader.DisableFeatureSentiment = true;
                }
                else
                {
                    log.Warn("{0} aspects file not found", path);
                }
            }

            
            log.Info("Processing...");
        }

        public IObservable<ProcessingContext> Process()
        {
            var documentSelector = pipeline.ProcessStep().Select(item => Observable.Start(() => RetrieveData(item))).Merge();
            return documentSelector
                .Where(item => item != null);
        }

        public void Save(string path)
        {
            path.EnsureDirectoryExistence();
            log.Info("Saving results [{0}]...", path);
            result.ToArray().XmlSerialize().Save(Path.Combine(path, "result.xml"));
            var aspectSentiments = AspectSentiment.GetResults();
            aspectSentiments.XmlSerialize().Save(Path.Combine(path, "aspect_sentiment.xml"));
            var vector = SentimentVector.GetVector(NormalizationType.None);
            vector.XmlSerialize().Save(Path.Combine(path, "sentiment_vector.xml"));
            Holder.Save(Path.Combine(path, "result.csv"));
            arffProcess.Normalize(NormalizationType.L2);
            arff.FullSave(path);
        }
   
        private ProcessingContext RetrieveData(ProcessingContext context)
        {
            try
            {
                RatingAdjustment adjustment = new RatingAdjustment(context.Review, perspective);
                pipeline.Splitter.DataLoader.NRCDictionary.ExtractToVector(SentimentVector, context.Review.Items);
                context.Processed = context.Review.GenerateDocument(adjustment);
                AspectSentiment.Process(context.Review);
                Holder.AddResult(new ResultRecord(context.Original.Id, context.Original.Stars, adjustment.Rating.StarsRating, context.Review.GetAllSentiments().Length));
                if (context.Original.Stars == null)
                {
                    log.Debug("Document doesn't have star assigned");
                }

                if (adjustment.Rating.StarsRating == null)
                {
                    statistics.AddUnknown();
                    arffProcess.PopulateArff(context.Review, PositivityType.Negative);
                }
                else
                {
                    if (context.Original.Stars != null)
                    {
                        statistics.Add(adjustment.Rating.StarsRating.Value, context.Original.Stars.Value);
                        if (context.Original.Stars != 3)
                        {
                            Performance.Add(
                                context.Original.Stars > 3,
                                adjustment.Rating.IsPositive);
                        }
                    }

                    arffProcess.PopulateArff(context.Review, context.Original.Stars > 3 ? PositivityType.Positive : PositivityType.Negative);
                }
            }
            catch (Exception)
            {
                Interlocked.Increment(ref error);
                throw;
            }

            if (context.Processed != null)
            {
                result.Add(context.Processed);
            }

            return context;
        }
    }
}
