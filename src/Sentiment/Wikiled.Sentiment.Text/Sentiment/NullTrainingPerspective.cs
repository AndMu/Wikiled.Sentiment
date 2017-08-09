using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class NullTrainingPerspective : ITrainingPerspective
    {
        public static readonly NullTrainingPerspective Instance = new NullTrainingPerspective();

        private NullTrainingPerspective()
        {
            MachineSentiment = new NullMachineSentiment();
            TrainingHeader = TrainingHeader.CreateDefault();
        }

        public IMachineSentiment MachineSentiment { get; }


        public TrainingHeader TrainingHeader { get; }
    }
}
