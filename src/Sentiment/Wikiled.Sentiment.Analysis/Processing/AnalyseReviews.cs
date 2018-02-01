using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.MachineLearning.Svm.Extensions;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Sentiment;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class AnalyseReviews : ITrainingPerspective
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private IArffDataSet currentSet;

        public AnalyseReviews()
        {
            MachineSentiment = new NullMachineSentiment();
            TrainingHeader = TrainingHeader.CreateDefault();
            TrainingHeader.GridSelection = true;
        }

        public static PrecisionStars GlobalStars { get; } = new PrecisionStars();

        public IMachineSentiment MachineSentiment { get; private set; }

        public int Negative { get; set; }

        public int Positive { get; set; }

        public string SvmPath { get; set; }

        public TrainingHeader TrainingHeader { get; }

        public void InitEnvironment()
        {
            if (Directory.Exists(SvmPath))
            {
                Directory.Delete(SvmPath, true);
            }

            Directory.CreateDirectory(SvmPath);
        }

        public void SetArff(IArffDataSet dataSet)
        {
            dataSet.FullSave(SvmPath, TrainingHeader);
            currentSet = dataSet;
        }

        public async Task TrainSvm()
        {
            try
            {
                if (currentSet == null)
                {
                    throw new ArgumentNullException(nameof(currentSet));
                }

                Guard.IsValid(() => Positive, Positive, i => i > 0, "Can't train. Missing positive samples");
                Guard.IsValid(() => Negative, Negative, i => i > 0, "Can't train. Missing negative samples");
                var dataSet = currentSet;
                var machine = await MachineSentiment<PositivityType>.Train(dataSet, TrainingHeader, CancellationToken.None).ConfigureAwait(false);
                machine.Save(SvmPath);
                LoadSvm();
                log.Info("SVM Training Completed...");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        private void LoadSvm()
        {
            log.Info("Loading SVM vectors...");
            MachineSentiment = MachineSentiment<PositivityType>.Load(SvmPath);
            TrainingHeader.Normalization = MachineSentiment.Header.Normalization;
        }
    }
}
