using System;
using System.Collections.Generic;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Workspace.Data.Selection;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    public interface IDataSet : IProcessingDataSource
    {
        event EventHandler<ModificationEventArgs> Changed;

        void ForceRefresh();

        void AddData(IProcessingData data);

        void AddPFile(IFileSelection file);

        void LoadDocuments(string path);

        IEnumerable<IDataCollection> Collections { get; }

        DataConfiguration DataConfiguration { get; }

        bool IsReady { get; }

        bool IsTesting { get; }
    }
}
