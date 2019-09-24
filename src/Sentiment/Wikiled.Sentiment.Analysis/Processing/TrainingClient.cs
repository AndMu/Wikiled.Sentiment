using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wikiled.Arff.Extensions;
using Wikiled.Arff.Logic;
using Wikiled.MachineLearning.Mathematics.Vectors.Serialization;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Sentiment.Analysis.Arff;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class TrainingClient : ITrainingClient
    {
        private readonly ILogger<TrainingClient> log;

        private readonly AnalyseReviews analyze;

        private readonly MainAspectHandler featureExtractor;

        private readonly SentimentVector sentimentVector;

        private IProcessArff arffProcess;

        private readonly IClientContext clientContext;

        public TrainingClient(ILogger<TrainingClient> log, IClientContext clientContext)
        {
            this.clientContext = clientContext ?? throw new ArgumentNullException(nameof(clientContext));
            if (string.IsNullOrEmpty(clientContext.Context.SvmPath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(clientContext.Context.SvmPath));
            }
            
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            SentimentVector = new SentimentVector();
            analyze = new AnalyseReviews();
            featureExtractor = new MainAspectHandler(new AspectContextFactory());
            sentimentVector = new SentimentVector();
        }

        public IProcessingPipeline Pipeline => clientContext.Pipeline;

        public ISentimentDataHolder Lexicon
        {
            get => clientContext.Context.Lexicon;
            set => clientContext.Context.Lexicon = value;
        }

        public string OverrideAspects { get; set; }

        public SentimentVector SentimentVector { get; }

        public bool UseAll { get; set; }

        public bool UseBagOfWords { get; set; }

        public bool DisableAspects { get; set; }

        public async Task Train(IObservable<IParsedDocumentHolder> reviews)
        {
            analyze.SvmPath = clientContext.Context.SvmPath;
            analyze.InitEnvironment();
            log.LogInformation("Starting Training...");
            using (Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(item => log.LogInformation(clientContext.Pipeline.Monitor.ToString())))
            {
                await clientContext.Pipeline.ProcessStep(reviews)
                              .Select(AdditionalProcessing);
            }

            log.LogInformation(clientContext.Pipeline.Monitor.ToString());
            if (!DisableAspects)
            {
                SelectAdditional();
            }

            clientContext.Pipeline.ResetMonitor();
            IArffDataSet arff = ArffDataSet.Create<PositivityType>("MAIN");
            IProcessArffFactory factory = UseBagOfWords ? new UnigramProcessArffFactory() : (IProcessArffFactory)new ProcessArffFactory();
            arffProcess = factory.Create(arff);

            using (Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(item => log.LogInformation(clientContext.Pipeline.Monitor.ToString())))
            {
                await clientContext.Pipeline.ProcessStep(reviews)
                              .Select(
                                  item => Observable.Start(
                                      () =>
                                      {
                                          ProcessingContext result = ProcessSingleItem(item);
                                          return result;
                                      }))
                              .Merge();
            }

            log.LogInformation(clientContext.Pipeline.Monitor.ToString());
            log.LogInformation("Cleaning up ARFF....");
            if (!UseAll)
            {
                await arffProcess.CleanupDataHolder(3, 10).ConfigureAwait(false);
            }

            arff = arff.Sort();
            analyze.SetArff(arff);
            try
            {
                await analyze.TrainSvm().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
            }
        }

        private ProcessingContext AdditionalProcessing(ProcessingContext context)
        {
            if (context.Review == null)
            {
                return null;
            }

            featureExtractor.Process(context.Review);
            clientContext.NrcDictionary.ExtractToVector(sentimentVector, context.Review.ImportantWords);
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
                log.LogDebug("Ignoring 3 stars...");
                return null;
            }

            arffProcess.PopulateArff(context.Review, context.Original.Stars > 3 ? PositivityType.Positive : PositivityType.Negative);
            return context;
        }

        private void SelectAdditional()
        {
            log.LogInformation("Extracting aspects...");
            IAspectSerializer serializer = clientContext.AspectSerializer;
            Text.Words.IWordItem[] features = featureExtractor.GetFeatures(100).ToArray();
            Text.Words.IWordItem[] attributes = featureExtractor.GetAttributes(100).ToArray();
            XDocument document = serializer.Serialize(features, attributes);
            var featuresFile = Path.Combine(analyze.SvmPath, "aspects.xml");
            document.Save(featuresFile);
            if (!string.IsNullOrEmpty(OverrideAspects))
            {
                log.LogInformation($"Overriding aspects with {OverrideAspects}");
                File.Copy(featuresFile, Path.Combine(analyze.SvmPath, "aspects_detected.xml"), true);
                File.Copy(OverrideAspects, featuresFile, true);
                var featuresXml = XDocument.Load(featuresFile);
                IAspectDectector aspect = serializer.Deserialize(featuresXml);
                features = aspect.AllFeatures.ToArray();
                attributes = aspect.AllAttributes.ToArray();
            }

            clientContext.Context.ChangeAspect(new AspectDectector(features, attributes));
            MachineLearning.Mathematics.Vectors.VectorData vector = sentimentVector.GetVector(NormalizationType.None);
            new JsonVectorSerialization(Path.Combine(analyze.SvmPath, "sentiment_vector.json")).Serialize(new[] { vector });
            log.LogInformation("Extracting features... DONE!");
        }
    }
}
