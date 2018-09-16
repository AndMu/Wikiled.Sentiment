using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autofac;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Mathematics.Vectors.Serialization;
using Wikiled.MachineLearning.Normalization;
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

        public TrainingClient(IProcessingPipeline pipeline, string svmPath)
        {
            if (string.IsNullOrEmpty(svmPath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(svmPath));
            }

            this.pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
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

        public async Task Train(IObservable<IParsedDocumentHolder> reviews)
        {
            analyze.SvmPath = svmPath;
            analyze.InitEnvironment();
            log.Info("Starting Training");
            using (Observable.Interval(TimeSpan.FromSeconds(30))
                           .Subscribe(item => log.Info(pipeline.Monitor)))
            {
                await pipeline.ProcessStep(reviews)
                              .Select(AdditionalProcessing)
                              .Select(
                                  item =>
                                  {
                                      pipeline.Monitor.Increment();
                                      return item;
                                  });
            }

            SelectAdditional();
            var arff = ArffDataSet.Create<PositivityType>("MAIN");
            var factory = UseBagOfWords ? new UnigramProcessArffFactory() : (IProcessArffFactory)new ProcessArffFactory();
            arffProcess = factory.Create(arff);

            using (Observable.Interval(TimeSpan.FromSeconds(30))
                             .Subscribe(item => log.Info(pipeline.Monitor)))
            {
                await pipeline.ProcessStep(reviews)
                              .Select(
                                  item => Observable.Start(
                                      () =>
                                      {
                                          var result = ProcessSingleItem(item);
                                          pipeline.Monitor.Increment();
                                          return result;
                                      }))
                              .Merge();
            }

            log.Info("Cleaning up ARFF....");
            if (!UseAll)
            {
                await arffProcess.CleanupDataHolder(3, 10).ConfigureAwait(false);
            }

            analyze.SetArff(arff);
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
            pipeline.ContainerHolder.Container.Resolve<INRCDictionary>().ExtractToVector(sentimentVector, context.Review.Items);
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

            if (context.Original.Stars > 3)
            {
                arffProcess.PopulateArff(context.Review, PositivityType.Positive);
            }
            else
            {
                arffProcess.PopulateArff(context.Review, PositivityType.Negative);
            }

            return context;
        }

        private void SelectAdditional()
        {
            log.Info("Extracting aspects...");
            var serializer = pipeline.ContainerHolder.Container.Resolve<IAspectSerializer>();
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

            pipeline.ContainerHolder.Context.ChangeAspect(new AspectDectector(features, attributes));
            var vector = sentimentVector.GetVector(NormalizationType.None);
            new JsonVectorSerialization(Path.Combine(analyze.SvmPath, "sentiment_vector.json")).Serialize(new[] { vector });
            log.Info("Extracting features... DONE!");
        }
    }
}
