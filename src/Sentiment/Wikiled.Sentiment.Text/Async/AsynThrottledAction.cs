using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Wikiled.Sentiment.Text.Async
{
    public class AsynThrottledAction : IDisposable
    {
        private readonly Subject<Action> actionObservable;

        private readonly ManualResetEvent finished = new ManualResetEvent(false);

        private IDisposable observer;

        public AsynThrottledAction(TimeSpan timer)
        {
            actionObservable = new Subject<Action>();
            var throttled = actionObservable.Sample(timer);
            observer = throttled.Subscribe(PerformAction, OnCompleted);
        }

        public void Dispose()
        {
            if (observer != null)
            {
                actionObservable.OnCompleted();
                finished.WaitOne(500);
                observer.Dispose();
                observer = null;
            }
        }

        public void SaveAll(Action action)
        {
            actionObservable.OnNext(action);
        }

        private void OnCompleted()
        {
            finished.Set();
        }

        private void PerformAction(Action action)
        {
            action();
        }
    }
}
