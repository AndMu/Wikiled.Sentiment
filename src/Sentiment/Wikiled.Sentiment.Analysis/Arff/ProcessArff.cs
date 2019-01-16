using System;
using Wikiled.Arff.Logic;
using Wikiled.Common.Utilities.Helpers;
using Wikiled.Sentiment.Text.Data;
using Wikiled.Sentiment.Text.MachineLearning;
using Constants = Wikiled.Sentiment.Text.Data.Constants;

namespace Wikiled.Sentiment.Analysis.Arff
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
            
            dataSet.HasId = true;
            dataSet.HasDate = true;
        }

        public override void PopulateArff(IParsedReview review, PositivityType positivity)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            review.Reset();
            review.Vector.GenerateUsingImportantOnly = true;
            AddData(review.Document.Id, review.Date, review.Vector, positivity);
            review.Vector.GenerateUsingImportantOnly = false;
        }

        private IArffDataRow AddData(string id, DateTime? date, ExtractTextVectorBase extractTextVector, PositivityType positivity)
        {
            var cells = extractTextVector.GetCells();
            if (cells.Count == 0)
            {
                return null;
            }

            lock (DataSet)
            {
                IArffDataRow review = string.IsNullOrEmpty(id) ? DataSet.AddDocument() : DataSet.GetOrCreateDocument(id);
                review.Class.Value = positivity;
                review.Date = date ?? DateTime.Today;
                foreach (var cell in cells)
                {
                    var name = cell.Name;
                    var data = review.AddRecord(name);
                    if (data != null)
                    {
                        data.Header.Source = cell.Item;
                        data.Value = cell.Value;
                    }
                }

                return review;
            }
        }
    }
}
