using SharpNL.NameFind;
using SharpNL.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikiled.Sentiment.Text.Config;
using Wikiled.Sentiment.Text.NLP.NER;

namespace Wikiled.Sentiment.Text.NLP.OpenNLP
{
    public class OpenNlpNamedEntityRecognition : INamedEntityRecognition
    {
        private readonly ILexiconConfig configuration;

        private readonly List<NameFinderME> nameFinders = new List<NameFinderME>(7);

        public OpenNlpNamedEntityRecognition(ILexiconConfig configuration)
        {
            this.configuration = configuration;
            LoadModel("en-ner-date.bin");
            LoadModel("en-ner-location.bin");
            LoadModel("en-ner-money.bin");
            LoadModel("en-ner-organization.bin");
            LoadModel("en-ner-percentage.bin");
            LoadModel("en-ner-person.bin");
            LoadModel("en-ner-time.bin");
        }

        public IEnumerable<Span> Tag(string[] words)
        {
            return nameFinders.SelectMany(nameFinder => nameFinder.Find(words));
        }

        private void LoadModel(string modelFile)
        {
            using var stream = new FileStream(Path.Combine(configuration.Resources, configuration.NlpModels, modelFile), FileMode.Open, FileAccess.Read, FileShare.Read);
            var model = new TokenNameFinderModel(stream);
            nameFinders.Add(new NameFinderME(model));
        }
    }
}
