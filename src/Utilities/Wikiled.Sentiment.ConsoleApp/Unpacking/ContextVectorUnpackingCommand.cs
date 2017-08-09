using System.IO;
using System.Xml.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Processing.Context;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    public class ContextVectorUnpackingCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Required]
        public string Out { get; set; }

        [Required]
        public string Source { get; set; }

        public override void Execute()
        {
            log.Info("Starting vector unpacking");
            string path = Path.Combine(Source, "results.xml");
            if(!File.Exists(path))
            {
                log.Error("{0} not found", path);
                return;
            }

            log.Info("Loading: {0}", path);
            var documents = XDocument.Load(path).XmlDeserialize<Document[]>();
            log.Info("Processing: {0} documents", documents.Length);
            VectorsExtractor vectorsExtractor = new VectorsExtractor(3);
            int current = 1;
            foreach(var document in documents)
            {
                log.Info("Processing: {0}/{1}", current, documents.Length);
                vectorsExtractor.Process(document);
                current++;
            }

            log.Info("Saving results: {0}", Out);
            vectorsExtractor.Save(Out);
        }
    }
}
