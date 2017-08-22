using System.Linq;
using Wikiled.Arff.Normalization;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Sentiment.Text.Anomaly;
using Wikiled.Text.Inquirer.Reflection;
using Wikiled.Text.Inquirer.Reflection.Data;

namespace Wikiled.Sentiment.Text.NLP.NRC
{
    public static class SentimentVectorExtension
    {
        private static readonly IMapCategory MapProbabilityOnly = new CategoriesMapper().Construct<ItemProbabilityHolder>();

        public static DataTree GetTree(this SentimentVector vector, bool occurences = true)
        {
            var items = occurences ? vector.GetOccurences() : vector.GetProbabilities();
            return new DataTree(new ItemProbabilityHolder(items.ToArray()), MapProbabilityOnly);
        }

        public static VectorData GetVector(this SentimentVector vector, NormalizationType normalization)
        {
            return GetTree(vector, false).CreateVector(normalization, false);
        }
    }
}
