using Wikiled.Arff.Persistence;
using Wikiled.Common.Arguments;
using Wikiled.Sentiment.Text.Data;

namespace Wikiled.Sentiment.Analysis.Processing.Arff
{
    public class UnigramProcessArff : ProcessArffBase
    {
        public UnigramProcessArff(IArffDataSet dataSet)
            : base(dataSet)
        {
        }

        public override void PopulateArff(IParsedReview current, PositivityType positivity)
        {
            Guard.NotNull(() => current, current);
            lock (DataSet)
            {
                var review = DataSet.AddDocument();
                review.Class.Value = positivity;

                foreach (var word in current.Items)
                {
                    var item = review.AddRecord(word.Text);
                    if (item == null)
                    {
                        continue;
                    }

                    var existing = (double?)item.Value;
                    if (existing == null)
                    {
                        existing = 0;
                    }

                    item.Value = (double)1 + existing.Value;
                }
            }
        }
    }
}
