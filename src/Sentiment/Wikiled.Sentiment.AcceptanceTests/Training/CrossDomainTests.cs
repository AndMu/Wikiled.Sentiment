﻿using System;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Amazon.Logic;

namespace Wikiled.Sentiment.AcceptanceTests.Training
{
    [TestFixture]
    public class CrossDomainTests
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [TestCase("B0026127Y8", ProductCategory.Video, 99)]
        [TestCase("9562910334", ProductCategory.Book, 99)]
        public async Task TestElectronics(string product, ProductCategory category, int accuracy)
        {
            log.Info("TestElectronics: {0} {1}", product, category);
            var result = await Global.ElectronicBaseLine.Test(product, category).ConfigureAwait(false);
            Assert.AreEqual(accuracy, Math.Round(result.Performance.GetSingleAccuracy(true), 2) * 100);
        }

        [TestCase("B0002L5R78", ProductCategory.Electronics, 96)]
        public async Task TestVideo(string product, ProductCategory category, int accuracy)
        {
            log.Info("TestVideo: {0} {1}", product, category);
            var result = await Global.VideoBaseLine.Test(product, category).ConfigureAwait(false);
            Assert.AreEqual(accuracy, Math.Round(result.Performance.GetSingleAccuracy(true), 2)* 100);
        }
    }
}
