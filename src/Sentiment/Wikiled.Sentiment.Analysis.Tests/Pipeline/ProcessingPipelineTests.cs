using System;
using System.Linq;
using System.Reactive;
using Autofac;
using Autofac.Extras.Moq;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Analysis.Pipeline;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Tests.Pipeline
{
    [TestFixture]
    public class ProcessingPipelineTests : ReactiveTest
    {
        private ProcessingPipeline instance;

        private ITestableObservable<IParsedDocumentHolder> documentSource;

        private TestScheduler scheduler;

        private Mock<IParsedDocumentHolder> holder;

        private Mock<IContainerHelper> container;

        private Mock<IParsedReviewManager> manager;

        private Mock<IParsedReview> review;

        [SetUp]
        public void SetUp()
        {
            scheduler = new TestScheduler();
            manager = new Mock<IParsedReviewManager>();
            var factory = new Mock<IParsedReviewManagerFactory>();
            factory.Setup(item => item.Resolve(It.IsAny<Document>(), null)).Returns(manager.Object);
            container = new Mock<IContainerHelper>();
            review = new Mock<IParsedReview>();
            manager.Setup(item => item.Create()).Returns(review.Object);
            holder = new Mock<IParsedDocumentHolder>();
            var innerContainer = AutoMock.GetLoose();
            innerContainer.Provide(factory.Object);
            container.Setup(item => item.Container).Returns(innerContainer.Container);
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
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(null, container.Object));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(scheduler, null));
        }

        [Test]
        public void ProcessStep()
        {
            var subscriber = scheduler.CreateObserver<ProcessingContext>();
            instance.ProcessStep(documentSource)
                    .Subscribe(subscriber);
            scheduler.AdvanceBy(TimeSpan.FromSeconds(4).Ticks);
            Assert.AreEqual(3, subscriber.Messages.Count);
            Assert.AreEqual(NotificationKind.OnCompleted, subscriber.Messages.Last().Value.Kind);
        }

        private ProcessingPipeline CreateProcessingPipeline()
        {
            return new ProcessingPipeline(scheduler, container.Object);
        }
    }
}
