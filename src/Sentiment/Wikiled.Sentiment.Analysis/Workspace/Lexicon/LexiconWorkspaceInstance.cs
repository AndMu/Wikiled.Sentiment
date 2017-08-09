using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;

namespace Wikiled.Sentiment.Analysis.Workspace.Lexicon
{
    public class LexiconWorkspaceInstance : ILexiconWorkspaceInstance
    {
        public LexiconWorkspaceInstance(IWorkspaceInstance workspace)
        {
            Guard.NotNull(() => workspace, workspace);
            Workspace = workspace;
        }

        public void LoadConfiguration()
        {
        }

        public void SaveConfiguration()
        {
        }

        public IWorkspaceInstance Workspace { get; } 
    }
}
