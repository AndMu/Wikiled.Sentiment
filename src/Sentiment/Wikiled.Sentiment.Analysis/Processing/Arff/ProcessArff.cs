using System;
using Wikiled.Arff.Persistence;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;

namespace Wikiled.Sentiment.Analysis.Processing.Arff
{
    public class ProcessArff : ProcessArffBase
    {
        public ProcessArff(IArffDataSet dataSet) 
            : base(dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            dataSet.Header.RegisterDate(Constants.DATE);
        }

        public override void PopulateArff(IParsedReview review, PositivityType positivity)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            review.Reset();
            review.Vector.GenerateUsingImportantOnly = true;
            AddData(review.Date, review.Vector, positivity);
            review.Vector.GenerateUsingImportantOnly = false;
        }

        private void AddData(DateTime? date, ExtractTextVectorBase extractTextVector, PositivityType positivity)
        {
            var cells = extractTextVector.GetCells();
            if (cells.Count == 0)
            {
                return;
            }

            lock (DataSet)
            {
                IArffDataRow review = DataSet.AddDocument();
                review.Class.Value  = positivity;
                review.AddRecord(Constants.DATE).Value = date ?? DateTime.Today;
                foreach (var cell in cells)
                {
                    string name = cell.Name;
                    if (string.Compare(cell.Name, Constants.DATE, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        name = name + "_";
                    }

                    var data = review.AddRecord(name);
                    if (data != null)
                    {
                        data.Header.Source = cell.Item;
                        data.Value = cell.Value;
                    }
                }
            }
        }
    }
}
