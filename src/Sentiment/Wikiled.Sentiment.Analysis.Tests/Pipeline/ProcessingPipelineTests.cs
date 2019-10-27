using System;
using System.Linq;
using System.Reactive;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;

namespace Wikiled.Sentiment.Analysis.Tests.Pipeline
{
    [TestFixture]
    public class ProcessingPipelineTests : ReactiveTest
    {
        private ProcessingPipeline instance;

        private ITestableObservable<IParsedDocumentHolder> documentSource;

        private TestScheduler scheduler;

        private ILogger<ProcessingPipeline> logger;

        private Mock<IParsedDocumentHolder> holder;

        private Mock<IParsedReviewManager> manager;

        private Mock<IParsedReview> review;

        [SetUp]
        public void SetUp()
        {
            logger = NullLogger<ProcessingPipeline>.Instance;
            scheduler = new TestScheduler();
            manager = new Mock<IParsedReviewManager>();
            review = new Mock<IParsedReview>();
            manager.Setup(item => item.Create()).Returns(review.Object);
            holder = new Mock<IParsedDocumentHolder>();
            documentSource = scheduler.CreateColdObservable(
                new Recorded<Notification<IParsedDocumentHolder>>(
                    TimeSpan.FromSeconds(1).Ticks,
                    Notification.CreateOnNext(holder.Object)),
                new Recorded<Notification<IParsedDocumentHolder>>(
                    TimeSpan.FromSeconds(2).Ticks,
                    Notification.CreateOnNext(holder.Object)),
                new Recorded<Notification<IParsedDocumentHolder>>(
                    TimeSpan.FromSeconds(3).Ticks,
                    Notification.CreateOnCompleted<IParsedDocumentHolder>()));
            
            instance = CreateProcessingPipeline();
        }

        [Test]
        public void Construct() 
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(logger, null, doc => manager.Object));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(logger, scheduler, null));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(null, scheduler, doc => manager.Object));
        }

        [Test]
        public void ProcessStep()
        {
            var subscriber = scheduler.CreateObserver<ProcessingContext>();
            instance.Processing(documentSource)
                    .Subscribe(subscriber);
            scheduler.AdvanceBy(TimeSpan.FromSeconds(4).Ticks);
            Assert.AreEqual(3, subscriber.Messages.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, subscriber.Messages.Last().Value.Kind);
        }

        private ProcessingPipeline CreateProcessingPipeline()
        {
            return new ProcessingPipeline(logger, scheduler, doc => manager.Object);
        }
    }
}
