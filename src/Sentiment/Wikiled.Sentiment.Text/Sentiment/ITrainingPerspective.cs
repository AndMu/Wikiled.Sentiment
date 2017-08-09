using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Text.Sentiment
{
    public interface ITrainingPerspective
    {
        IMachineSentiment MachineSentiment { get; }

        TrainingHeader TrainingHeader { get; }
    }
}