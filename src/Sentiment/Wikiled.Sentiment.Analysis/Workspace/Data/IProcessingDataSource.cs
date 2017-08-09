using System.Collections.Generic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    public interface IProcessingDataSource
    {
        TrainingTestingData ContructData();

        IEnumerable<SingleProcessingData> Data { get; }
    }
}