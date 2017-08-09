using Wikiled.Sentiment.Text.NLP.Inquirer.Harvard;
using Wikiled.Sentiment.Text.NLP.Inquirer.Lasswell;
using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.NLP.Inquirer
{
    /// <summary>
    /// Inquirer word description
    /// </summary>
    [InfoCategory("Inquirer Description")]
    public class InquirerDescription
    {
        public InquirerDescription()
        {
            Harward = new HarwardDescription();
            Lasswell = new LasswellDescription();
        }

        [InfoCategory("Harward")]
        public HarwardDescription Harward { get; private set; }

        [InfoCategory("Lasswell")]
        public LasswellDescription Lasswell { get; private set; }

        [InfoField("General Information", IsOptional = true)]
        public string Information { get; set; }

        public string OtherTags { get; set; }
    }
}
