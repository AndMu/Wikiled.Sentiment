using Wikiled.Sentiment.Analysis.Workspace.Data;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.Sentiment.Analysis.Workspace.Configuration
{
    public class WorkspaceConfiguration
    {
        public string Name { get; set; }
        public DataSourceType DataSourceType { get; set; }
        public ProjectType ProjectType { get; set; }
        public POSTaggerType POSTagger { get; set; }
        public LexiconTypes LexiconTypes { get; set; }
    }
}
