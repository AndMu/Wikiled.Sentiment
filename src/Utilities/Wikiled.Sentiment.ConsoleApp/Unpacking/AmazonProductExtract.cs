using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Sentiment.Text.Async;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    // amazonProduct -Source=E:\Data\Amazon\Electronics.txt -Out=E:\Data\Amazon\Out -Minimum=100
    public class AmazonProductExtract : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Required]
        public string Source { get; set; }

        [Required]
        public string Out { get; set; }

        public int Minimum { get; set; }

        public override void Execute()
        {
            log.Info("Extracting: {0}", Source);
            AmazonParserLogic parser = new AmazonParserLogic();
            Dictionary<string, Dictionary<string, Document>> documents = new Dictionary<string, Dictionary<string, Document>>();
            foreach (var currentDocument in parser.Parse(Source).Select(item => item.CreateDocument()))
            {
                documents.GetSafeCreate(currentDocument.Title)[currentDocument.Id] = currentDocument;
            }

            log.Info("Found products: {0}", documents.Count);
            new DirectoryInfo(Out).EnsureDirectoryExistence();

            Parallel.ForEach(
                documents.OrderByDescending(item => item.Value.Count).Where(item => item.Value.Count > Minimum),
                AsyncSettings.DefaultParallel,
                products =>
                {
                    var name = products.Key.CreatePureLetterText();
                    if (name.Length > 100)
                    {
                        name = name.Substring(0, 100);
                    }

                    products.Value.Values.ParseAndSave(Path.Combine(Out, name + ".xml"));
                });
        }
    }
}
