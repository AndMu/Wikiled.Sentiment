using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Arff.Logic;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public class AnalyseReviews
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<AnalyseReviews>();

        private IArffDataSet currentSet;

        public AnalyseReviews()
        {
            Model = new NullMachineSentiment();
        }

        public IMachineSentiment Model { get; private set; }

        public string SvmPath { get; set; }


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

                IArffDataSet dataSet = currentSet;
                MachineSentiment machine = await MachineSentiment.Train(dataSet, CancellationToken.None).ConfigureAwait(false);
                machine.Save(SvmPath);
                LoadSvm();
                log.LogInformation("SVM Training Completed...");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
                throw;
            }
        }

        private void LoadSvm()
        {
            log.LogInformation("Loading SVM vectors...");
            Model = MachineSentiment.Load(SvmPath);
        }
    }
}
