using System.IO;
using System.Linq;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    /// <summary>
    /// amazontext -Source=E:\Data\Amazon\Home_&_Kitchen.txt -Out=E:\Data\out_kitchen.txt
    /// </summary>
    public class AmazonTextExtract : Command
    {
        [Required]
        public string Source { get; set; }

        [Required]
        public string Out { get; set; }

        public override void Execute()
        {
            AmazonParserLogic parser = new AmazonParserLogic();
            var text = parser.Parse(Source).Select(item => item.TextData.Text);
            using (StreamWriter writer = new StreamWriter(Out))
            {
                foreach (var item in text)
                {
                    writer.WriteLine(item.Replace(".", " ").Replace(",", " ").ToLower());
                }
            }
        }
    }
}
