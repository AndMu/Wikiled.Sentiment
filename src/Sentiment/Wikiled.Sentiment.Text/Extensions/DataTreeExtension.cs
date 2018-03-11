using System.Collections.Generic;
using System.Linq;
using Wikiled.Arff.Normalization;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.Text.Analysis.Reflection.Data;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class DataTreeExtension
    {
        public static VectorData CreateVector(this IDataTree tree, NormalizationType normalization, bool usePrefix = true)
        {
            Guard.NotNull(() => tree, tree);
            List<SimpleCell> vectors = new List<SimpleCell>();
            CreateVector("Data", tree, vectors, usePrefix);
            vectors = vectors.OrderBy(item => item.Name).ToList();
            return new VectorDataFactory().CreateSimple(normalization, vectors.Select(item => (ICell)item).ToArray());
        }

        private static void CreateVector(string prefix, IDataTree tree, List<SimpleCell> vector, bool usePrefix = true)
        {
            var actualPrefix = usePrefix ? prefix : string.Empty;
            vector.AddRange(tree.Leafs.Select(leaf => new SimpleCell(actualPrefix + leaf.Name.CreatePureLetterText(), (double)leaf.Value)));
            string treeName = tree.Name.CreatePureLetterText();
            foreach (var branch in tree.Branches)
            {
                CreateVector(usePrefix ? $"{prefix}_{treeName}_" : string.Empty, branch, vector);
            }
        }
    }
}
