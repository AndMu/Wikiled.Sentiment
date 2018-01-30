using System.Linq;
using System.Xml.Linq;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Processing
{
    public static class ProcessingDataExtension
    {
        public static IProcessingData Load(string fileName)
        {
            var data = XDocument.Load(fileName).XmlDeserialize<ProcessingData>();
            data.Sort();
            return data;
        }

        public static void Sort(this ProcessingData data)
        {
            data.Positive = data.Positive?.OrderBy(item => item.Date).ToArray();
            data.Neutral = data.Neutral?.OrderBy(item => item.Date).ToArray();
            data.Negative = data.Negative?.OrderBy(item => item.Date).ToArray();
        }

        public static void Populate(this IProcessingData data, ProcessingData another)
        {
            Guard.NotNull(() => data, data);
            Guard.NotNull(() => another, another);
            foreach (var item in data.Positive)
            {
                another.Add(PositivityType.Positive, item);
            }

            foreach (var item in data.Negative)
            {
                another.Add(PositivityType.Negative, item);
            }

            foreach (var item in data.Neutral)
            {
                another.Add(PositivityType.Neutral, item);
            }
        }

        public static TrainingTestingData GetCrossValidation(this IProcessingData data, int current, int folds = 10)
        {
            TrainingTestingData result = new TrainingTestingData();
            result.Training = new ProcessingData();
            result.Testing = new ProcessingData();
            data.Positive.GetData(result.Training, result.Testing, PositivityType.Positive, current, folds);
            data.Negative.GetData(result.Training, result.Testing, PositivityType.Negative, current, folds);
            data.Neutral.GetData(result.Training, result.Testing, PositivityType.Neutral, current, folds);
            return result;
        }

        private static void GetData(
            this SingleProcessingData[] data,
            ProcessingData training,
            ProcessingData testing,
            PositivityType type,
            int current,
            int folds = 10)
        {
            int oneFold = data.Length / folds;
            if (oneFold == 0)
            {
                return;
            }

            var startOfTest = data.Length - oneFold + (current * oneFold);
            startOfTest = startOfTest % data.Length;
            var end = startOfTest + oneFold;
            for (int i = 0; i < data.Length; i++)
            {
                if (i >= startOfTest &&
                    i < end)
                {
                    testing.Add(type, data[i]);
                }
                else
                {
                    training.Add(type, data[i]);
                }
            }
        }
    }
}
