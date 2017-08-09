using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.AcceptanceTests.Helpers;
using Wikiled.Sentiment.Analysis.Amazon;

namespace Wikiled.Sentiment.AcceptanceTests.Training
{
    [SetUpFixture]
    public class Global
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static MainBaseLine ElectronicBaseLine { get; private set; }

        public static MainBaseLine VideoBaseLine { get; private set; }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ElectronicBaseLine = new MainBaseLine("B0002L5R78", ProductCategory.Electronics);
            VideoBaseLine = new MainBaseLine("B0026127Y8", ProductCategory.Video);
            logger.Info("Starting training...");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            await Task.WhenAll(ElectronicBaseLine.Train(), VideoBaseLine.Train());
            timer.Stop();
            Console.WriteLine("Training took: {0}", timer.Elapsed);
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
