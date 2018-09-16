using NLog;
using Wikiled.Sentiment.Text.Aspects;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class SentimentContext : ISentimentContext
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly NullAspectDectector nullDetector = new NullAspectDectector();

        private IAspectDectector aspect;

        private bool disableInvertors;

        private bool disableFeatureSentiment;

        public IAspectDectector Aspect => aspect ?? nullDetector;

        public bool DisableFeatureSentiment
        {
            get => disableFeatureSentiment;
            set
            {
                log.Info("DisableFeatureSentiment: {0}", value);
                disableFeatureSentiment = value;
            }
        }

        public bool DisableInvertors
        {
            get => disableInvertors;
            set
            {
                log.Info("DisableInvertors: {0}", value);
                disableInvertors = value;
            }
        }

        public void ChangeAspect(IAspectDectector aspect)
        {
            log.Info("Changing aspect detector");
            this.aspect = aspect;
        }
    }
}
