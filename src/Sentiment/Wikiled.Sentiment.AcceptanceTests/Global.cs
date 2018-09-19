using System;
using NUnit.Framework;
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
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
