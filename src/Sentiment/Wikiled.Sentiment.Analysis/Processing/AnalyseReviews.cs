using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.MachineLearning.Svm.Extensions;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.Sentiment.Analysis.Anomaly;
using Wikiled.Sentiment.Analysis.Processing.Arff;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class AnalyseReviews : ITrainingPerspective
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly List<AnalyseReviewsSubset> testingSet = new List<AnalyseReviewsSubset>();

        private readonly List<AnalyseReviewsSubset> trainingSet = new List<AnalyseReviewsSubset>();

        private readonly IWordsHandler wordsHandler;

        private IArffDataSet currentSet;

        private IDataFilter filter;

        private IMachineSentiment machineSentiment;

        private IProcessingData testing;

        private IProcessingData training;

        public AnalyseReviews(IWordsHandler wordsHandler)
        {
            Guard.NotNull(() => wordsHandler, wordsHandler);
            this.wordsHandler = wordsHandler;
            Name = "default";
            MachineSentiment = new NullMachineSentiment();
            TrainingHeader = TrainingHeader.CreateDefault();
            TrainingHeader.GridSelection = true;
            ProcessArffFactory = new ProcessArffFactory();
        }

        public static PrecisionStars GlobalStars { get; } = new PrecisionStars();

        public IAspectDectector AspectDectector { get; private set; }

        public TrainingType ContextSvm { get; set; }

        public bool ExtracFeatures { get; set; }

        public IMachineSentiment MachineSentiment { get; private set; }

        public TrainingType MainSvm { get; set; }

        public string Name { get; set; }

        public int Negative { get; set; }

        public int Positive { get; set; }

        public IProcessArffFactory ProcessArffFactory { get; set; }

        public PrecisionStars Stars { get; private set; }

        public string SvmPath { get; set; }

        public TrainingHeader TrainingHeader { get; set; }

        private PrecisionRecallCalculator<bool> PositiveNegative { get; set; }

        private PositiveNegative PositiveNegativeStats { get; set; }

        private IEnumerable<AnalyseReviewsSubset> TestingSet => testingSet;

        public void AddTesting(IProcessingData data)
        {
            Guard.NotNull(() => data, data);
            testing = data;
            Init();
        }

        public void AddTraining(IProcessingData data)
        {
            Guard.NotNull(() => data, data);
            training = data;
            Init();
        }

        public void Filter(IDataFilter attached)
        {
            if (filter == null)
            {
                filter = attached;
                if (filter == null)
                {
                    log.Info("Creating filter...");
                    var anomalyData = ProcessingDataAnomalyFactory.Instance.Create(training);
                    anomalyData.Extract();
                    filter = new AnomalyDataFilter(anomalyData);
                }
            }

            Init();
        }

        public IArffDataSet GetTrainingArff()
        {
            log.Info("Creating Testing ARFF...");
            var dataHolderTesting = Create("Testing");
            PopulateTestingArff(dataHolderTesting);
            dataHolderTesting.Save(Path.Combine(SvmPath, "testing_data.arff"));
            var trainingPath = Path.Combine(SvmPath, "data.arff");
            var traingingModel = Path.Combine(SvmPath, "training.model");
            if (File.Exists(trainingPath) &&
                File.Exists(traingingModel))
            {
                var trainingData = ArffDataSet.Load<PositivityType>(trainingPath);
                var model = Model.Read(traingingModel);
                SvmTestClient testClient = new SvmTestClient(trainingData, model);
                testClient.Test(dataHolderTesting, SvmPath);
            }

            return dataHolderTesting;
        }

        public void InitEnvironment()
        {
            if (Directory.Exists(SvmPath))
            {
                Directory.Delete(SvmPath, true);
            }

            Directory.CreateDirectory(SvmPath);
        }

        public void LoadMainSvm()
        {
            log.Info("Loading SVM vectors...");
            MachineSentiment = MachineSentiment<PositivityType>.Load(SvmPath);
            TrainingHeader.Normalization = MachineSentiment.Header.Normalization;
        }

        public void ResetStats()
        {
            Stars = new PrecisionStars();
            PositiveNegativeStats = new PositiveNegative();
        }

        public void SetArff(IArffDataSet dataSet)
        {
            dataSet.FullSave(SvmPath, TrainingHeader);
            currentSet = dataSet;
        }

        public void SetFull(TrainingTestingData data)
        {
            testing = data.Testing;
            training = data.Training;
            Init();
        }

        public async Task SetMainSvm()
        {
            switch (MainSvm)
            {
                case TrainingType.Train:
                    await TrainGenericSvm().ConfigureAwait(false);
                    break;
                case TrainingType.Use:
                    LoadMainSvm();
                    break;
            }
        }

        public void Test()
        {
            log.Info("Testing Set...");
            AspectDectector = wordsHandler.AspectDectector;
            foreach (var reviewsSubset in TestingSet)
            {
                log.Info($"Processing {reviewsSubset.PositivityType} reviews...");
                reviewsSubset.ProcessReviews();
            }
        }

        private IArffDataSet Create(string name)
        {
            return ArffDataSet.Create<PositivityType>(name);
        }

        private IArffDataSet CreateMainArff()
        {
            log.Info("Creating Training ARFF...");
            var dataHolder = Create("Main_SVM");
            PopulateArff(dataHolder);
            dataHolder.FullSave(SvmPath, TrainingHeader);
            return dataHolder;
        }

        private SingleProcessingData[] FilterCollection(IEnumerable<SingleProcessingData> collection)
        {
            return filter == null ? collection.ToArray() : collection.Where(item => filter.CanInclude(item)).ToArray();
        }

        private void Init()
        {
            var totalTraining = training?.AllReviews.Count() ?? 0;
            var totalTesting = testing?.AllReviews.Count() ?? 0;

            log.Info("Starting with total items: " + (totalTraining + totalTesting));
            log.Info(
                "Training with {0} positive and {1} negative and testing with {2} positive, {3} negative ...",
                training?.Positive.Count(),
                training?.Negative.Count(),
                testing?.Positive.Count(),
                testing?.Negative.Count());

            trainingSet.Clear();
            if (training != null)
            {
                trainingSet.Add(
                    new AnalyseReviewsSubset(
                        wordsHandler,
                        this,
                        FilterCollection(training.Positive),
                        PositivityType.Positive));
                trainingSet.Add(
                    new AnalyseReviewsSubset(
                        wordsHandler,
                        this,
                        FilterCollection(training.Negative),
                        PositivityType.Negative));
                trainingSet.Add(
                    new AnalyseReviewsSubset(
                        wordsHandler,
                        this,
                        FilterCollection(training.Neutral),
                        PositivityType.Neutral));
            }

            log.Debug("After anomality reduction, we have in training: {0}", trainingSet.Count);
            testingSet.Clear();
            if (testing != null)
            {
                testingSet.Add(
                    new AnalyseReviewsSubset(
                        wordsHandler,
                        this,
                        testing.Positive.ToArray(),
                        PositivityType.Positive));
                testingSet.Add(
                    new AnalyseReviewsSubset(
                        wordsHandler,
                        this,
                        testing.Negative.ToArray(),
                        PositivityType.Negative));
                testingSet.Add(
                    new AnalyseReviewsSubset(
                        wordsHandler,
                        this,
                        testing.Neutral.ToArray(),
                        PositivityType.Neutral));
            }
        }

        private void PopulateArff(IArffDataSet dataSet)
        {
            var data = ProcessArffFactory.Create(dataSet);
            foreach (var reviewsSubset in trainingSet)
            {
                if (reviewsSubset.PositivityType == PositivityType.Neutral)
                {
                    continue;
                }

                data.PopulateArff(reviewsSubset.Processings.Select(item => item.Review).ToArray(), reviewsSubset.PositivityType);
            }

            data.CleanupDataHolder(3, 3);
            data.Normalize(TrainingHeader.Normalization);
        }

        private void PopulateTestingArff(IArffDataSet dataSet)
        {
            var data = ProcessArffFactory.Create(dataSet);
            foreach (var reviewsSubset in testingSet)
            {
                data.PopulateArff(reviewsSubset.Processings.Select(item => item.Review).ToArray(), reviewsSubset.PositivityType);
            }

            data.Normalize(TrainingHeader.Normalization);
        }

        private async Task TrainGenericSvm()
        {
            try
            {
                Guard.IsValid(() => Positive, Positive, i => i > 0, "Non zero");
                Guard.IsValid(() => Negative, Negative, i => i > 0, "Non zero");
                var dataSet = currentSet ?? CreateMainArff();
                var machine = await MachineSentiment<PositivityType>.Train(dataSet, TrainingHeader, CancellationToken.None).ConfigureAwait(false);
                machine.Save(SvmPath);
                machineSentiment = MachineSentiment<PositivityType>.Load(SvmPath);
                log.Info("SVM Training Completed...");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }
    }
}
