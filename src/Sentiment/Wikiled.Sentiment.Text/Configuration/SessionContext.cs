using Microsoft.Extensions.Logging;
using Wikiled.Common.Logging;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class SessionContext : ISessionContext
    {
        private static readonly ILogger log = ApplicationLogging.CreateLogger<SessionContext>();

        private readonly NullAspectDectector nullDetector = new NullAspectDectector();

        private IAspectDectector aspect;

        private bool disableInvertors;

        private bool disableFeatureSentiment;

        public SessionContext()
        {
            log.LogDebug("Creating");
        }

        public IAspectDectector Aspect => aspect ?? nullDetector;

        public string SvmPath { get; set; }

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
    }
}
