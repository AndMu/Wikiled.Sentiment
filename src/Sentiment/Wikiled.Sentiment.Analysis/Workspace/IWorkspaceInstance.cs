using Wikiled.Sentiment.Analysis.Workspace.Aspects;
using Wikiled.Sentiment.Analysis.Workspace.Configuration;
using Wikiled.Sentiment.Analysis.Workspace.Data;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Workspace
{
    public interface IWorkspaceInstance
    {
        IAspectsHandler AspectsHandler { get; }

        WorkspaceConfiguration Configuration { get; }

        string CrossValidationDataPath { get; }

        string DataPath { get; }

        string ProjectPath { get; }

        IDataSet TestingData { get; }

        string TestingDataPath { get; }

        IDataSet TrainingData { get; }

        IWordsHandler WordsHandler { get; }

        void Init();

        void LoadConfiguration();

        void LoadTesting(string path);

        void LoadTraining(string path);

        void Register(ISpecificWorkspaceInstance specificWorkspace);

        void SaveConfiguration();
    }
}
