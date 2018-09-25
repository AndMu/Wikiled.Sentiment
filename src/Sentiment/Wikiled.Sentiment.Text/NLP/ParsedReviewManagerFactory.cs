using System;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.NLP;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.Structure;
using Wikiled.Text.Inquirer.Logic;

namespace Wikiled.Sentiment.Text.NLP
{
    public class ParsedReviewManagerFactory : IParsedReviewManagerFactory
    {
        private readonly IWordsHandler manager;

        private readonly IWordFactory wordsFactory;

        private readonly INRCDictionary nrcDictionary;

        private readonly IRawTextExtractor extractor;

        private readonly IInquirerManager inquirerManager;

        public ParsedReviewManagerFactory(IWordsHandler manager,
                                          IWordFactory wordsFactory,
                                          INRCDictionary nrcDictionary,
                                          IRawTextExtractor extractor,
                                          IInquirerManager inquirerManager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.wordsFactory = wordsFactory ?? throw new ArgumentNullException(nameof(wordsFactory));
            this.nrcDictionary = nrcDictionary ?? throw new ArgumentNullException(nameof(nrcDictionary));
            this.extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            this.inquirerManager = inquirerManager ?? throw new ArgumentNullException(nameof(inquirerManager));
        }

        public IParsedReviewManager Resolve(Document document, ISentimentDataHolder lexicon = null)
        {
            var loader = manager;
            var factory = wordsFactory;
            if (lexicon != null)
            {
                loader = new CustomWordsDataLoader(manager, lexicon);
                factory = new WordOccurenceFactory(loader, extractor, inquirerManager);
            }

            return new ParsedReviewManager(loader, factory, nrcDictionary, document);
        }
    }
}
