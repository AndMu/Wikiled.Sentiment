using System.Collections.Generic;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    public class ItemConfiguration
    {
        public ItemConfiguration()
        {
            Files = new List<string>();
            Folders = new List<string>();
        }

        public double Stars { get; set; }

        public string Name { get; set; }

        public List<string> Files { get; set; }

        public List<string> Folders { get; set; }
    }
}
