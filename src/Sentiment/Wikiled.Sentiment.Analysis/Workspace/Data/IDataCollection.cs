using System;
using System.Collections.ObjectModel;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.Data.Review;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    public interface IDataCollection
    {
        event EventHandler<ModificationEventArgs> Changed;

        double Accuracy { get; }

        ItemConfiguration Configuration { get; }

        double F1 { get; }

        ObservableCollection<SingleProcessingData> Items { get; }

        string Name { get; }

        PositivityType PositivityType { get; }

        double Precision { get; }

        double Recall { get; }

        double Stars { get; }

        void AddFiles(params string[] files);

        void AddFolder(string folder);

        void Clear();

        void ForceRefresh();
    }
}
