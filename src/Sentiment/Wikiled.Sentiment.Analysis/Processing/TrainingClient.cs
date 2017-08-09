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
using Wikiled.Core.Utility.Logging;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Processing.Arff;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.NLP.NRC;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class TrainingClient
    {
        private IObservable<IParsedDocumentHolder> data;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ISplitterHelper splitter;

        private readonly string svmPath;

        private readonly AnalyseReviews analyse;

        private IProcessArff arffProcess;

        private readonly MainAspectHandler featureExtractor;

        private int negative;

        private int positive;

        private readonly SentimentVector sentimentVector;

        private PerformanceMonitor monitor;

        public TrainingClient(ISplitterHelper splitter, IObservable<IParsedDocumentHolder> reviews, string svmPath)
        {
            Guard.NotNull(() => splitter, splitter);
            Guard.NotNull(() => reviews, reviews);
            Guard.NotNull(() => svmPath, svmPath);
        
            this.splitter = splitter;
            data = reviews;
            this.svmPath = svmPath;
            SentimentVector = new SentimentVector();
            splitter.Load();
            analyse = new AnalyseReviews(splitter.DataLoader);
            featureExtractor = new MainAspectHandler(new AspectContextFactory());
            sentimentVector = new SentimentVector();
        }

        public SentimentVector SentimentVector { get; }

        public string OverrideFeatures { get; set; }

        public bool UseBagOfWords { get; set; }

        public bool UseAll { get; set; }

        public async Task Train()
        {
            analyse.SvmPath = svmPath;
            analyse.InitEnvironment();
            log.Info("Starting Training");

            monitor = new PerformanceMonitor(100);
            var selected = data
                .SelectMany(item => Observable.Start(() => AdditionalProcessing(item)))
                .Merge();
            await selected.LastOrDefaultAsync();

            monitor = new PerformanceMonitor(100);
            SelectAdditional();

            var arff = ArffDataSet.Create<PositivityType>("MAIN");
            var factory = UseBagOfWords ? new UnigramProcessArffFactory() : (IProcessArffFactory) new ProcessArffFactory();
            arffProcess = factory.Create(arff);
            selected = data
                .SelectMany(item => Observable.Start(() => ProcessSingleItem(item)))
                .Merge();

            await selected.LastOrDefaultAsync();
            data = null;
            log.Info("Cleaning up ARFF");
            if (!UseAll)
            {
                arffProcess.CleanupDataHolder(3, 10);
            }

            analyse.TrainingHeader.Normalization = NormalizationType.L2;
            arffProcess.Normalize(analyse.TrainingHeader.Normalization);
            analyse.SetArff(arff);
            analyse.Positive = positive;
            analyse.Negative = negative;
            try
            {
                await analyse.SetMainSvm().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task<IParsedReview> AdditionalProcessing(IParsedDocumentHolder reviewHolder)
        {
            try
            {
                monitor.CountTotal();
                var doc = await reviewHolder.GetParsed().ConfigureAwait(false);
                var review = doc.GetReview(splitter.DataLoader);
                if (review != null)
                {
                    featureExtractor.Process(review);
                    sentimentVector.Extract(review.Items);
                }

                return review;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                monitor.Increment();
            }

            return null;
        }

        private async Task<IParsedReview> ProcessSingleItem(IParsedDocumentHolder reviewHolder)
        {
            var doc = reviewHolder.Original;
            monitor.CountTotal();
            if (doc == null)
            {
                return null;
            }

            if (doc.Stars == 3)
            {
                log.Debug("Ignoring 3 stars...");
                return null;
            }

            try
            {
                var parsed = await reviewHolder.GetParsed().ConfigureAwait(false);
                var review = parsed.GetReview(splitter.DataLoader);
                SingleProcessingData item = new SingleProcessingData();
                item.InitDocument(review);

                if (doc.Stars > 3)
                {
                    arffProcess.PopulateArff(review, PositivityType.Positive);
                    Interlocked.Increment(ref positive);
                }
                else
                {
                    arffProcess.PopulateArff(review, PositivityType.Negative);
                    Interlocked.Increment(ref negative);
                }

                return review;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                monitor.Increment();
            }

            return null;
        }

        private void SelectAdditional()
        {
            log.Info("Extracting features...");
            AspectSerializer serializer = new AspectSerializer(splitter.DataLoader);
            var features = featureExtractor.GetFeatures(100).ToArray();
            var attributes = featureExtractor.GetAttributes(100).ToArray();
            var document = serializer.Serialize(features, attributes);
            var featuresFile = Path.Combine(analyse.SvmPath, "features.xml");
            document.Save(featuresFile);
            if (!string.IsNullOrEmpty(OverrideFeatures))
            {
                log.Info($"Overriding features with {OverrideFeatures}");
                File.Copy(featuresFile, Path.Combine(analyse.SvmPath, "features_detected.xml"), true);
                File.Copy(OverrideFeatures, featuresFile, true);
                XDocument featuresXml = XDocument.Load(featuresFile);
                var aspect = serializer.Deserialize(featuresXml);
                features = aspect.AllFeatures.ToArray();
                attributes = aspect.AllAttributes.ToArray();
            }

            splitter.DataLoader.AspectDectector = new AspectDectector(features, attributes);
            var vector = sentimentVector.GetVector(NormalizationType.None);
            vector.XmlSerialize().Save(Path.Combine(analyse.SvmPath, "sentiment_vector.xml"));
            log.Info("Extracting features... DONE!");
        }
    }
}
