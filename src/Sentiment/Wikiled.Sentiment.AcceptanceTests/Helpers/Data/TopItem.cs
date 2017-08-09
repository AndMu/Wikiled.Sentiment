using System.Text;

namespace Wikiled.Sentiment.AcceptanceTests.Helpers.Data
{
    public class TopItems
    {
        public int Total { get; set; }

        public string[] Items { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Top: {0}:", Total);
            foreach (var item in Items)
            {
                builder.AppendFormat(" [{0}]", item);
            }

            return base.ToString();
        }
    }
}
