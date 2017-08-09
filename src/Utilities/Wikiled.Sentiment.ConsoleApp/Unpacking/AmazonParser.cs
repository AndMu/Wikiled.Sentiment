using System.Linq;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Analysis.Amazon;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    // amazon -Source=E:\Data\Amazon\Cell_Phones_&_Accessories.txt -Out=E:\Data\Amazon\reviews3.xml -Total=1000 -Skip=1000
    // amazon -Source=E:\Data\Amazon\Electronics.txt -Out=E:\Data\Amazon\Samsung.xml -Total=1000 -Product=Samsung 
    //-MinPrice=200
    public class AmazonParser : Command
    {
        [Required]
        public string Source { get; set; }

        [Required]
        public string Out { get; set; }
        
        public int? Total { get; set; }

        public int? Skip { get; set; }

        public int? MinPrice { get; set; }

        public string Product { get; set; }

        public override void Execute()
        {
            if (Total.HasValue &&
                !Skip.HasValue)
            {
                Skip = 0;
            }
            
            AmazonParserLogic parser = new AmazonParserLogic();
            parser.MinPrice = MinPrice;
            parser.Product = Product;
            parser.Parse(Source).Select(item => item.CreateDocument()).ParseAndSave(Out, Skip ?? 0, Total);
        }
    }
}
