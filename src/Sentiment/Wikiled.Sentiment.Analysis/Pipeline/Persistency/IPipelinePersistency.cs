using System;

namespace Wikiled.Sentiment.Analysis.Pipeline.Persistency
{
    public interface IPipelinePersistency : IDisposable
    {
        bool ExtractStyle { get; set; }

        bool Debug { get; set; }

        void Start(string path);

        void Save(ProcessingContext context);

        void Dispose();
    }
}