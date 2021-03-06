﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Common.Extensions;
using Wikiled.MachineLearning.Mathematics.Vectors;
using Wikiled.MachineLearning.Normalization;
using Wikiled.Text.Analysis.Reflection.Data;

namespace Wikiled.Sentiment.Text.Extensions
{
    public static class DataTreeExtension
    {
        public static VectorData CreateVector(this IDataTree tree, NormalizationType normalization, bool usePrefix = true)
        {
            if (tree is null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            var vectors = new List<SimpleCell>();
            CreateVector("Data", tree, vectors, usePrefix);
            vectors = vectors.OrderBy(item => item.Name).ToList();
            return new VectorDataFactory().CreateSimple(normalization, vectors.Select(item => (ICell)item).ToArray());
        }

        private static void CreateVector(string prefix, IDataTree tree, List<SimpleCell> vector, bool usePrefix = true)
        {
            var actualPrefix = usePrefix ? prefix : string.Empty;
            vector.AddRange(tree.Leafs.Select(leaf => new SimpleCell(actualPrefix + leaf.Name.CreatePureLetterText(), (double)leaf.Value)));
            var treeName = tree.Name.CreatePureLetterText();
            foreach (var branch in tree.Branches)
            {
                CreateVector(usePrefix ? $"{prefix}_{treeName}_" : string.Empty, branch, vector);
            }
        }
    }
}
