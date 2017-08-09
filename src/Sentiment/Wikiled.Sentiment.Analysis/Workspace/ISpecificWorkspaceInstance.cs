namespace Wikiled.Sentiment.Analysis.Workspace
{
    public interface ISpecificWorkspaceInstance
    {
        void LoadConfiguration();
        void SaveConfiguration();
        IWorkspaceInstance Workspace { get; }
    }
}
