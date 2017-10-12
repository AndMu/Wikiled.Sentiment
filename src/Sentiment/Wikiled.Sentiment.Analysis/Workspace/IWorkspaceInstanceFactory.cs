using Wikiled.Sentiment.Analysis.Workspace.Data;

namespace Wikiled.Sentiment.Analysis.Workspace
{
    public interface IWorkspaceInstanceFactory
    {
        IWorkspaceInstance Create(DataSourceType source, string name, string path);

        IWorkspaceInstance Open(string path);
    }
}