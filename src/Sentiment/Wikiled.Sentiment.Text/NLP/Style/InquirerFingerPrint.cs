using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Sentiment.Text.NLP.Style.Description.Data;
using Wikiled.Text.Analysis.Reflection;
using Wikiled.Text.Analysis.Reflection.Data;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Data;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Text.NLP.Style
{
    [InfoCategory("Inquirer Based Info")]
    public class InquirerFingerPrint : IDataSource
    {
        private static readonly IMapCategory mapDefinition = new CategoriesMapper().Construct<InquirerDescription>();

        private readonly Dictionary<WordEx, Dictionary<string, bool>> wordLevelFingerPrint = new Dictionary<WordEx, Dictionary<string, bool>>();

        private readonly IInquirerManager inquirer;

        public InquirerFingerPrint(IInquirerManager inquirer, TextBlock text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            this.inquirer = inquirer ?? throw new ArgumentNullException(nameof(inquirer));
        }

        [InfoCategory("Inquirer Based Info")]
        public DataTree InquirerProbabilities { get; private set; }

        public TextBlock Text { get; }

        public InquirerData GetData(WordEx word)
        {
            wordLevelFingerPrint.TryGetValue(word, out Dictionary<string, bool> data);
            return data == null
                ? null
                : new InquirerData
                {
                    Categories = data.Where(item => item.Value).Select(item => item.Key).ToArray()
                };
        }

        public void Load()
        {
            Dictionary<string, int> map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in mapDefinition.ActualProperties)
            {
                var fields = mapDefinition[property];
                foreach (var field in fields)
                {
                    if (field.ValueType == typeof(bool))
                    {
                        map[property] = 0;
                    }
                }
            }

            map["Positive"] = 0;
            map["Negative"] = 0;
            foreach (var wordEx in Text.Words)
            {
                InquirerDefinition definition = inquirer.GetWordDefinitions(wordEx);
                Dictionary<string, bool> table = new Dictionary<string, bool>();
                wordLevelFingerPrint[wordEx] = table;
                foreach (var inquirerRecord in definition.Records)
                {
                    if (inquirerRecord.Description.Harward.Sentiment.Type != PositivityType.Neutral)
                    {
                        string sentiment = inquirerRecord.Description.Harward.Sentiment.Type.ToString();
                        map[sentiment] = map[sentiment] + 1;
                        table[sentiment] = true;
                    }

                    DataTree tree = new DataTree(inquirerRecord.Description, mapDefinition);
                    foreach (var leaf in tree.AllLeafs)
                    {
                        if (!(leaf.Value is bool) ||
                            !(bool) leaf.Value)
                        {
                            continue;
                        }

                        map[leaf.Name] = map[leaf.Name] + 1;
                        table[leaf.Name] = true;
                    }
                }
            }

            Dictionary<string, double> mapProbability = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            foreach (var record in map)
            {
                mapProbability[record.Key] = (double)record.Value / Text.Words.Length;
            }

            InquirerProbabilities = new DataTree(null, mapDefinition, new DictionaryDataItemFactory(mapProbability));
        }
    }
}
