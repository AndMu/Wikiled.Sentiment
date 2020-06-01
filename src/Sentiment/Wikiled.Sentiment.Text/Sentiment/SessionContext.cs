using System;
using Microsoft.Extensions.Logging;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class SessionContext : ISessionContext, IDisposable
    {
        private readonly ILogger<SessionContext> log;

        private readonly NullAspectDectector nullDetector = new NullAspectDectector();

        private IAspectDectector aspect;

        private bool disableInvertors;

        private bool disableFeatureSentiment;

        public SessionContext(ILogger<SessionContext> log)
        {
            this.log = log;
            log.LogDebug("Creating");
        }

        public IAspectDectector Aspect => aspect ?? nullDetector;

        public int NGram { get; set; } = 1;

        public string SvmPath { get; set; }

        public bool UseOriginalCase { get; set; }

        public bool DisableFeatureSentiment
        {
            get => disableFeatureSentiment;
            set
            {
                log.LogInformation("DisableFeatureSentiment: {0}", value);
                disableFeatureSentiment = value;
            }
        }

        public bool UseBuiltInSentiment { get; set; }

        public bool ExtractAttributes { get; set; }

        public bool DisableInvertors
        {
            get => disableInvertors;
            set
            {
                log.LogInformation("DisableInvertors: {0}", value);
                disableInvertors = value;
            }
        }

        public ISentimentDataHolder Lexicon { get; set; }

        public void ChangeAspect(IAspectDectector aspectDetector)
        {
            log.LogInformation("Changing aspect detector");
            aspect = aspectDetector;
        }

        public void Reset()
        {
            log.LogInformation("Reset");
            ChangeAspect(null);
            DisableInvertors = false;
            DisableFeatureSentiment = false;
        }

        public void Dispose()
        {
        }
    }
}
