using System.Collections.Generic;
using System.IO;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.MachineLearning;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Text.Data.Weighting
{
    public class SimpleSentimentSource : ISimpleSentimentSource
    {
        private readonly string path;

        private Dictionary<IWordItem, double> positive;

        private Dictionary<IWordItem, double> negative;

        public SimpleSentimentSource(string path)
        {
            ArgumentValidation.CheckStringArgument(path, "path");
            this.path = path;
            Load();
        }

        public double? Measure(IWordItem item)
        {
            SvmResult result = new SvmResult();
            double value;
            if (positive.TryGetValue(item, out value))
            {
                result.SvmDistance = value;
            }

            if (negative.TryGetValue(item, out value))
            {
                result.SvmDistance = -value;
            }

            return result.Positivity;
        }

        private Dictionary<IWordItem, double> ReadData(string file)
        {
            file = Path.Combine(path, file);
            if (!File.Exists(file))
            {
                return new Dictionary<IWordItem, double>(SimpleWordItemEquality.Instance);
            }

            return ReadTabResourceDataFileExtension.ReadData(BasicWordsHandler.Instance, file, false);
        }

        private void Load()
        {
            positive = ReadData("positive.txt");
            negative = ReadData("negative.txt");
        }

        public IEnumerable<IWordItem> Positive => positive.Keys;

        public IEnumerable<IWordItem> Negative => negative.Keys;
    }
}
