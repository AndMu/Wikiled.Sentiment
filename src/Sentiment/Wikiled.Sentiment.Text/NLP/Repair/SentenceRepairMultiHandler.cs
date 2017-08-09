using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class SentenceRepairMultiHandler : ISentenceRepairHandler
    {
        private readonly List<ISentenceRepairHandler> handlers = new List<ISentenceRepairHandler>();

        public void Add(ISentenceRepairHandler handler)
        {
            handlers.Add(handler);
        }

        public string Repair(string sentence)
        {
            foreach (var handler in handlers)
            {
                sentence = handler.Repair(sentence);
            }

            return sentence;
        }
    }
}
