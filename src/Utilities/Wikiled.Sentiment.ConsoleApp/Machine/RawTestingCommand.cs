using System;
using System.Reactive.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    /// rawTest -Articles="C:\Cloud\OneDrive\Study\Medical\articles.xml" -SvmPath=.\Svm -Out=.\results
    /// </summary>
    public class RawTestingCommand : BaseRawCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public string SvmPath { get; set; }

        [Required]
        public string Out { get; set; }

        protected override void Process(IObservable<IParsedDocumentHolder> reviews, ISplitterHelper splitter)
        {
            TestingClient client = new TestingClient(splitter, reviews, SvmPath);
            client.UseBagOfWords = UseBagOfWords;
            client.Init();
            client.Process().LastOrDefaultAsync().Wait();
            client.Save(Out);
            log.Info($"Testing performance {client.GetPerformanceDescritpion()}");
        }
    }
}
