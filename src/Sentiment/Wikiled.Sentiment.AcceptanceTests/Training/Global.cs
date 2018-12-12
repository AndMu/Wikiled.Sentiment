using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Wikiled.Amazon.Logic;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.AcceptanceTests.Helpers;

namespace Wikiled.Sentiment.AcceptanceTests.Training
{
    [SetUpFixture]
    public class Global
    {
        private readonly ILogger logger = ApplicationLogging.CreateLogger< Global>();

        public static MainBaseLine ElectronicBaseLine { get; private set; }

        public static MainBaseLine VideoBaseLine { get; private set; }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ElectronicBaseLine = new MainBaseLine("B0002L5R78", ProductCategory.Electronics);
            VideoBaseLine = new MainBaseLine("B0026127Y8", ProductCategory.Video);
            logger.LogInformation("Starting training...");
            var timer = new Stopwatch();
            timer.Start();
            await ElectronicBaseLine.Train().ConfigureAwait(false);
            await VideoBaseLine.Train().ConfigureAwait(false);
            timer.Stop();
            Console.WriteLine("Training took: {0}", timer.Elapsed);
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
