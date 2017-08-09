using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Normalization;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.MachineLearning.Svm.Extensions;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.Sentiment.Analysis.Processing.Arff;
using Wikiled.Sentiment.Analysis.Svm;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Sentiment.Text.Tokenizer;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Subjectivity
{
    public class SubjectivityDetector : ITrainingPerspective
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly string path;

        private IMachineSentiment classifier;

        private readonly IWordsHandler wordsHandler;

        public SubjectivityDetector(IWordsHandler wordsHandler, string path)
        {
            Guard.NotNull(() => wordsHandler, wordsHandler);
            Guard.NotNullOrEmpty(() => path, path);
            this.path = path;
            this.wordsHandler = wordsHandler;
            MachineSentiment = new NullMachineSentiment();
            TrainingHeader = TrainingHeader.CreateDefault();
        }

        public bool Disabled { get; set; }

        public IMachineSentiment MachineSentiment { get; }

        public TrainingHeader TrainingHeader { get; }

        public double CheckSubjectivity(ISentence sentence)
        {
            Guard.NotNull(() => sentence, sentence);
            var extraction = new SimpleWordsExtraction(SentenceTokenizer.Create(wordsHandler, true, true));
            Document document = extraction.GetDocumentBySentences(sentence.Text, sentence);
            return CheckSubjectivity(document);
        }

        public async Task Train()
        {
            log.Info("Getting Subjectivity Detector...");
            var datasetPath = Path.Combine(path, @"Library\Standard\Subjectivity");
            var modelPath = Path.Combine(datasetPath, "training.model");
            if(!File.Exists(modelPath))
            {
                log.Info("Training Subjectivity Detector");
                var dataHolder = ArffDataSet.Create<PositivityType>("subjectivity");
                dataHolder.Header.CreateHeader = true;
                var processArff = new UnigramProcessArff(dataHolder);

                log.Info("Reading Subjective Sentences...");
                ProcessSentences(processArff, Path.Combine(datasetPath, "subjective.txt"), PositivityType.Positive, true);
                log.Info("Reading Objective Sentences...");
                ProcessSentences(processArff, Path.Combine(datasetPath, "objective.txt"), PositivityType.Negative, true);
                processArff.Normalize(NormalizationType.L2);
                dataHolder.FullSave(datasetPath, TrainingHeader);
                log.Info("Training SVM...");
                SvmTrainClient trainClient = new SvmTrainClient(dataHolder);
                var results = await trainClient.Train(TrainingHeader, CancellationToken.None).ConfigureAwait(false);
                results.Model.Write(modelPath);
                var testingDataHolder = ArffDataSet.Create<PositivityType>("subjectivity");
                testingDataHolder.Header.CreateHeader = true;
                var testingProcessArff = new UnigramProcessArff(testingDataHolder);

                ProcessSentences(
                    testingProcessArff,
                    Path.Combine(datasetPath, "subjective.txt"),
                    PositivityType.Positive,
                    false);
                ProcessSentences(
                    testingProcessArff,
                    Path.Combine(datasetPath, "objective.txt"),
                    PositivityType.Negative,
                    false);

                testingProcessArff.Normalize(NormalizationType.L2);
                SvmTestClient testClient = new SvmTestClient(dataHolder, results.Model);
                testClient.Test(testingDataHolder, datasetPath);
            }

            classifier = MachineSentiment<PositivityType>.Load(datasetPath);
            classifier.Arff.Clear();
        }

        private double CheckSubjectivity(Document document)
        {
            if(Disabled)
            {
                return 0;
            }

            var detector = new SimpleSvmDetector(classifier, NormalizationType.L2);
            var result = detector.Evaluate(document);
            return result.Result.Probability ?? 1;
        }

        private void ProcessSentences(
            IProcessArff processArff,
            string searchPath,
            PositivityType positivity,
            bool training)
        {
            var lines = File.ReadAllLines(searchPath);
            var linesFiltered = training ? lines.Take((int)(0.9 * lines.Length)).ToArray() : lines.Skip((int)(0.9 * lines.Length));
            var records = linesFiltered
                .Select(item => new SingleProcessingData {Text = item})
                .ToArray();
            processArff.PopulateArff(records.Select(item => item.Review).ToArray(), positivity);
        }
    }
}
