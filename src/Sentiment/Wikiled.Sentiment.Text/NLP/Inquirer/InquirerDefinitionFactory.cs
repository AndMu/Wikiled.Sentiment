using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.NLP.Inquirer.Harvard;
using Wikiled.Sentiment.Text.Reflection;
using Wikiled.Sentiment.Text.Reflection.Data;

namespace Wikiled.Sentiment.Text.NLP.Inquirer
{
    public class InquirerDefinitionFactory
    {
        public static readonly InquirerDefinitionFactory Instance = new InquirerDefinitionFactory();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly IMapCategory map = new CategoriesMapper().Construct<InquirerDescription>();

        private readonly Dictionary<string, bool> ignoreTable = new Dictionary<string, bool>();

        private InquirerDefinitionFactory()
        {
            ignoreTable["H4"] = true;
            ignoreTable["H4Lvd"] = true;
            ignoreTable["Pstv"] = true;
            ignoreTable["Ngtv"] = true;
            ignoreTable["Positiv"] = true;
            ignoreTable["Negativ"] = true;
            ignoreTable["Lvd"] = true;
            ignoreTable["COM"] = true;
        }

        public InquirerDescription Construct(InquirerRecord record)
        {
            InquirerDescription description = new InquirerDescription();
            ILookup<string, string> lookup = record.RawCategories.ToLookup(item => item, item => item);
            if (lookup.Contains("Positiv"))
            {
                description.Harward.Sentiment = new SentimentData(PositivityType.Positive);
            }

            if (lookup.Contains("Negativ"))
            {
                description.Harward.Sentiment = new SentimentData(PositivityType.Negative);
            }

            DataTree tree = new DataTree(description, map);
            IDictionary<string, IDataItem> leafLookup = tree.AllLeafs.ToDictionary(item => item.Name, item => item);
            int total = 0;
            foreach (var category in record.RawCategories)
            {
                total++;
                if (total == record.RawCategories.Length)
                {
                    int index = category.IndexOf("|");
                    description.Information = index == 0
                                                  ? category.Substring(1).Trim()
                                                  : category;
                }
                else if (string.IsNullOrEmpty(category) ||
                    ignoreTable.ContainsKey(category))
                {
                }
                else if (!map.ContainsField(category))
                {
                    if (total == record.RawCategories.Length - 1)
                    {
                        description.OtherTags = category;
                    }
                    else
                    {
                        log.Debug("Root field: {0} for word: {1}", category, record.Word);
                    }
                }
                else
                {
                    leafLookup[category].Value = true;
                }
            }

            return description;
        }
    }
}