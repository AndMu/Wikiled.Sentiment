using System.Collections.Generic;
using Wikiled.Sentiment.Analysis.Workspace.Data.Selection;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    public class DataConfiguration
    {
        public DataConfiguration()
        {
            Configurations  = new List<ItemConfiguration>();
            PSentiSources = new List<FileSelectionData>();
        }

        public List<ItemConfiguration> Configurations { get; set; }

        public List<FileSelectionData> PSentiSources { get; set; }

        public DataSourceType Type { get; set; }
    }

}
