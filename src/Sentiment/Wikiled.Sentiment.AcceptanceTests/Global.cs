using System;
using NLog.Extensions.Logging;
using NUnit.Framework;
using Wikiled.Common.Logging;
using Wikiled.MachineLearning.Mathematics;

namespace Wikiled.Sentiment.AcceptanceTests
{
    [SetUpFixture]
    public class Global
    {
        [OneTimeSetUp]
        public void Setup()
        {
            GlobalSettings.Random = new Random(48);
            Accord.Math.Random.Generator.Seed = 0;
            ApplicationLogging.LoggerFactory.AddNLog();
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
