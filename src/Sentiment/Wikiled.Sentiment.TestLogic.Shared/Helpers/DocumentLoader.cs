using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Containers;
using Wikiled.Sentiment.Text.NLP;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Sentiment;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.TestLogic.Shared.Helpers
{
    public class DocumentLoader
    {
        private readonly ISessionContainer helper;

        private readonly string path;

        public DocumentLoader(ISessionContainer helper)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"MachineLearning/Data/");
        }

        public async Task<Document> InitDocument(string name = "cv000_29416.txt")
        {
            var result = await helper.GetTextSplitter().Process(new ParseRequest(File.ReadAllText(Path.Combine(path, name)))).ConfigureAwait(false);
            var review = helper.Resolve<Func<Document, IParsedReviewManager>>()(result).Create();
            var documentFromReview = new DocumentFromReviewFactory();
            return documentFromReview.ReparseDocument(new NullRatingAdjustment(review));
        }

        public async Task<Document> InitDocumentWithWords()
        {
            var document = await InitDocument().ConfigureAwait(false);
            var words = document.Words.ToArray();
            for (var i = 0; i < words.Length; i++)
            {
                words[i].CalculatedValue = i % 3;
            }

            return document;
        }

        public async Task<List<Document>> InitDocumentsWithWords()
        {
            var documents = new List<Document>();
            foreach (var file in Directory.GetFiles(path, "*.txt"))
            {
                var document = await InitDocument(file).ConfigureAwait(false);
                var words = document.Words.ToArray();
                for (var i = 0; i < words.Length; i++)
                {
                    words[i].Value = i % 3;
                }

                documents.Add(document);
            }

            return documents;
        }
    }
}
