using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Processing.Arff;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class TrainingClient
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly AnalyseReviews analyze;

        private readonly MainAspectHandler featureExtractor;

        private readonly SentimentVector sentimentVector;

        private readonly IProcessingPipeline pipeline;

        private readonly string svmPath;

        private IProcessArff arffProcess;

        private int negative;

        private int positive;

        public TrainingClient(IProcessingPipeline pipeline, string svmPath)
        {
            Guard.NotNull(() => pipeline, pipeline);
            Guard.NotNull(() => svmPath, svmPath);

            this.pipeline = pipeline;
            this.svmPath = svmPath;
            SentimentVector = new SentimentVector();
            analyze = new AnalyseReviews();
            featureExtractor = new MainAspectHandler(new AspectContextFactory());
            sentimentVector = new SentimentVector();
        }

        public string OverrideAspects { get; set; }

        public SentimentVector SentimentVector { get; }

        public bool UseAll { get; set; }

        public bool UseBagOfWords { get; set; }

        public async Task Train()
        {
            analyze.SvmPath = svmPath;
            analyze.InitEnvironment();
            log.Info("Starting Training");
            using(Observable.Interval(TimeSpan.FromSeconds(30))
                           .Subscribe(item => log.Info(pipeline.Monitor)))
            {
                await pipeline.ProcessStep().Select(AdditionalProcessing);
            }

            SelectAdditional();
            var arff = ArffDataSet.Create<PositivityType>("MAIN");
            var factory = UseBagOfWords ? new UnigramProcessArffFactory() : (IProcessArffFactory)new ProcessArffFactory();
            arffProcess = factory.Create(arff);

            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(pipeline.Monitor)))
            {
                await pipeline.ProcessStep().Select(ProcessSingleItem);
            }

            log.Info("Cleaning up ARFF");
            if (!UseAll)
            {
                arffProcess.CleanupDataHolder(3, 10);
            }

            analyze.TrainingHeader.Normalization = NormalizationType.L2;
            arffProcess.Normalize(analyze.TrainingHeader.Normalization);
            analyze.SetArff(arff);
            analyze.Positive = positive;
            analyze.Negative = negative;
            try
            {
                await analyze.TrainSvm().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private ProcessingContext AdditionalProcessing(ProcessingContext context)
        {
            if (context.Review == null)
            {
                return null;
            }

            featureExtractor.Process(context.Review);
            pipeline.Splitter.DataLoader.NRCDictionary.ExtractToVector(sentimentVector, context.Review.Items);
            return context;
        }

        private ProcessingContext ProcessSingleItem(ProcessingContext context)
        {
            if (context.Original == null)
            {
                return null;
            }

            if (context.Original.Stars == 3)
            {
                log.Debug("Ignoring 3 stars...");
                return null;
            }

            SingleProcessingData item = new SingleProcessingData();
            item.InitDocument(context.Review);

            if (context.Original.Stars > 3)
            {
                arffProcess.PopulateArff(context.Review, PositivityType.Positive);
                Interlocked.Increment(ref positive);
            }
            else
            {
                arffProcess.PopulateArff(context.Review, PositivityType.Negative);
                Interlocked.Increment(ref negative);
            }

            return context;
        }

        private void SelectAdditional()
        {
            log.Info("Extracting aspects...");
            AspectSerializer serializer = new AspectSerializer(pipeline.Splitter.DataLoader);
            var features = featureExtractor.GetFeatures(100).ToArray();
            var attributes = featureExtractor.GetAttributes(100).ToArray();
            var document = serializer.Serialize(features, attributes);
            var featuresFile = Path.Combine(analyze.SvmPath, "aspects.xml");
            document.Save(featuresFile);
            if (!string.IsNullOrEmpty(OverrideAspects))
            {
                log.Info($"Overriding aspects with {OverrideAspects}");
                File.Copy(featuresFile, Path.Combine(analyze.SvmPath, "aspects_detected.xml"), true);
                File.Copy(OverrideAspects, featuresFile, true);
                XDocument featuresXml = XDocument.Load(featuresFile);
                var aspect = serializer.Deserialize(featuresXml);
                features = aspect.AllFeatures.ToArray();
                attributes = aspect.AllAttributes.ToArray();
            }

            pipeline.Splitter.DataLoader.AspectDectector = new AspectDectector(features, attributes);
            var vector = sentimentVector.GetVector(NormalizationType.None);
            vector.XmlSerialize().Save(Path.Combine(analyze.SvmPath, "sentiment_vector.xml"));
            log.Info("Extracting features... DONE!");
        }
    }
}
