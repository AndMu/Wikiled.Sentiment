using System;
using System.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Tests.Processing.Pipeline
{
    [TestFixture]
    public class ProcessingPipelineTests : ReactiveTest
    {
        private ProcessingPipeline instance;

        private ITestableObservable<IParsedDocumentHolder> documentSource;

        private Mock<ISplitterHelper> mockSplitterHelper;

        private TestScheduler scheduler;

        private Mock<IParsedDocumentHolder> holder;

        private Mock<IParsedReviewManagerFactory> reviewMock;

        private Mock<IParsedReviewManager> manager;

        private Mock<IParsedReview> review;

        [SetUp]
        public void SetUp()
        {
            scheduler = new TestScheduler();
            manager = new Mock<IParsedReviewManager>();
            reviewMock = new Mock<IParsedReviewManagerFactory>();
            review = new Mock<IParsedReview>();
            reviewMock.Setup(item => item.Create(It.IsAny<IWordsHandler>(), It.IsAny<Document>())).Returns(manager.Object);
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
            
            mockSplitterHelper = new Mock<ISplitterHelper>();
            instance = CreateProcessingPipeline();
        }

        [Test]
        public void Construct() 
        {
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(null, mockSplitterHelper.Object, documentSource, reviewMock.Object));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(scheduler, null, documentSource, reviewMock.Object));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(scheduler, mockSplitterHelper.Object, null, reviewMock.Object));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(scheduler, mockSplitterHelper.Object, documentSource, null));
        }

        [Test]
        public void ProcessStep()
        {
            var subscriber = scheduler.CreateObserver<ProcessingContext>();
            instance.ProcessStep()
                    .Subscribe(subscriber);
            scheduler.AdvanceBy(TimeSpan.FromSeconds(4).Ticks);
            Assert.AreEqual(3, subscriber.Messages.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, subscriber.Messages.Last().Value.Kind);
        }

        private ProcessingPipeline CreateProcessingPipeline()
        {
            return new ProcessingPipeline(scheduler, mockSplitterHelper.Object, documentSource, reviewMock.Object);
        }
    }
}
