using System;
using System.Linq;
using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Arff
{
    public static class ArffExtensions
    {
        public static void SplitDate(this IArffDataSet dataSet)
        {
            dataSet.Header.RegisterNumeric(Constants.DATE_X);
            dataSet.Header.RegisterNumeric(Constants.DATE_Y);
            var docs = dataSet.Documents.ToArray();
            for (var i = 0; i < dataSet.TotalDocuments; i++)
            {
                var vector = ((DateTime)docs[i][Constants.DATE].Value).GetYearVector();
                docs[i].AddRecord(Constants.DATE_X).Value = Convert.ToDouble(vector.X);
                docs[i].AddRecord(Constants.DATE_Y).Value = Convert.ToDouble(vector.Y);
            }
        }
    }
}
