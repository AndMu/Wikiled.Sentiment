using System;
using System.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Processing.Pipeline;
using Wikiled.Sentiment.Text.Data.Review;

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

        [SetUp]
        public void SetUp()
        {
            scheduler = new TestScheduler();
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
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(null, mockSplitterHelper.Object, documentSource));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(scheduler, null, documentSource));
            Assert.Throws<ArgumentNullException>(() => new ProcessingPipeline(scheduler, mockSplitterHelper.Object, null));
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
            return new ProcessingPipeline(scheduler, mockSplitterHelper.Object, documentSource);
        }
    }
}
