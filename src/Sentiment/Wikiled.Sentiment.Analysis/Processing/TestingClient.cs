using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Xml.Linq;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Arguments;
using Wikiled.Common.Extensions;
using Wikiled.Common.Serialization;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Sentiment.Analysis.Processing.Arff;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class TestingClient
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IProcessingPipeline pipeline;

        private readonly StatisticsCalculator statistics = new StatisticsCalculator();

        private IArffDataSet arff;

        private IProcessArff arffProcess;

        private int error;

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

        public string AspectPath { get; set; }

        public AspectSentimentTracker AspectSentiment { get; }

        public bool DisableAspects { get; set; }

        public bool DisableSvm { get; set; }

        public int Errors => error;

        public IMachineSentiment MachineSentiment { get; private set; }

        public PrecisionRecallCalculator<bool> Performance { get; } = new PrecisionRecallCalculator<bool>();

        public SentimentVector SentimentVector { get; }

        public string SvmPath { get; }

        public bool UseBagOfWords { get; set; }

        public bool TrackArff { get; set; }

        public SemaphoreSlim ProcessingSemaphore { get; set; }

        public string GetPerformanceDescription()
        {
            return string.Format($"{Performance.GetTotalAccuracy()} RMSE:{statistics.CalculateRmse():F2}");
        }

        public void Init()
        {
            MachineSentiment = DisableSvm ? new NullMachineSentiment() : Text.MachineLearning.MachineSentiment.Load(SvmPath);

            arff = ArffDataSet.Create<PositivityType>("MAIN");
            var factory = UseBagOfWords ? new UnigramProcessArffFactory() : (IProcessArffFactory)new ProcessArffFactory();
            arffProcess = TrackArff ? factory.Create(arff) : null;

            log.Info("Track ARFF: {0}", TrackArff);

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
            var documentSelector = pipeline.ProcessStep().Select(RetrieveData);
            return documentSelector.Where(item => item != null);
        }

        public void Save(string path)
        {
            path.EnsureDirectoryExistence();
            log.Info("Saving results [{0}]...", path);
            var aspectSentiments = AspectSentiment.GetResults();
            aspectSentiments.XmlSerialize().Save(Path.Combine(path, "aspect_sentiment.xml"));
            var vector = SentimentVector.GetVector(NormalizationType.None);
            vector.XmlSerialize().Save(Path.Combine(path, "sentiment_vector.xml"));
            arff.Save(Path.Combine(path, "data.arff"));
        }

        private ProcessingContext RetrieveData(ProcessingContext context)
        {
            try
            {
                var adjustment = RatingAdjustment.Create(context.Review, MachineSentiment);
                pipeline.Splitter.DataLoader.NRCDictionary.ExtractToVector(SentimentVector, context.Review.Items);
                context.Processed = context.Review.GenerateDocument(adjustment);
                AspectSentiment.Process(context.Review);
                context.Adjustment = adjustment;
                if (context.Original.Stars == null)
                {
                    log.Debug("Document doesn't have star assigned");
                }

                if (adjustment.Rating.StarsRating == null)
                {
                    statistics.AddUnknown();
                    arffProcess?.PopulateArff(context.Review, PositivityType.Negative);
                }
                else
                {
                    if (context.Original.Stars != null)
                    {
                        statistics.Add(adjustment.Rating.StarsRating.Value, context.Original.Stars.Value);
                        if (context.Original.Stars != 3)
                        {
                            Performance.Add(context.Original.Stars > 3, adjustment.Rating.IsPositive);
                        }
                    }

                    arffProcess?.PopulateArff(context.Review, context.Original.Stars > 3 ? PositivityType.Positive : PositivityType.Negative);
                }
            }
            catch
            {
                Interlocked.Increment(ref error);
                throw;
            }
            finally
            {
                ProcessingSemaphore?.Release();
            }

            return context;
        }
    }
}
