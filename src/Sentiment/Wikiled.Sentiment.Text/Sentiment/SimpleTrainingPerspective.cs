using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public class SimpleTrainingPerspective : ITrainingPerspective
    {
        public SimpleTrainingPerspective(IMachineSentiment machine, TrainingHeader header)
        {
            MachineSentiment = machine;
            TrainingHeader = header;
        }
        
        public IMachineSentiment MachineSentiment { get; }

        public TrainingHeader TrainingHeader { get; }
    }
}
