using System;
using NUnit.Framework;
using Wikiled.MachineLearning.Mathematics;

namespace Wikiled.Sentiment.Integration.Tests
{
    [SetUpFixture]
    public class Global
    {
        [OneTimeSetUp]
        public void Setup()
        {
            GlobalSettings.Random = new Random(48);
            Accord.Math.Random.Generator.Seed = 48;
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
