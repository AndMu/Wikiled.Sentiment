using NLog;
using Wikiled.Sentiment.Text.Aspects;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Configuration
{
    public class SentimentContext : ISentimentContext
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly NullAspectDectector nullDetector = new NullAspectDectector();

        private IAspectDectector aspect;

        private bool disableInvertors;

        private bool disableFeatureSentiment;

        public SentimentContext()
        {
            log.Debug("Creating");
        }

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

        public ISentimentDataHolder Lexicon { get; set; }

        public void ChangeAspect(IAspectDectector aspectDetector)
        {
            log.Info("Changing aspect detector");
            aspect = aspectDetector;
        }

        public void Reset()
        {
            log.Info("Reset");
            ChangeAspect(null);
            DisableInvertors = false;
            DisableFeatureSentiment = false;
        }
    }
}
