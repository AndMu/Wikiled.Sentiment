using System;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.ConsoleApp.Machine
{
    /// <summary>
    ///     training [-Year=2000 -Years=2] -Port=6379 [-SvmPath=.\SvmTwo] -Category=Electronics
    /// </summary>
    public class TrainingCommand : BaseRedis
    {
        public override bool IsTraining => true;

        protected override void Processing(IObservable<IParsedDocumentHolder> reviews)
        {
            TrainingClient client = new TrainingClient(Helper, reviews, ProcessingPath);
            client.Train().Wait();
        }
    }
}
