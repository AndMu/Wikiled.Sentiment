using System.Collections.Generic;

namespace Wikiled.Sentiment.Text.Tokenizer
{
    public class CombinedPipeline<T> : IPipeline<T>
    {
        public List<IPipeline<T>> Pipelines { get; }

        public CombinedPipeline(params IPipeline<T>[] pipelines)
        {
            Pipelines = new List<IPipeline<T>>();
            if (pipelines != null && 
                pipelines.Length > 0)
            {
                Pipelines.AddRange(pipelines);
            }
        }

        public IEnumerable<T> Process(IEnumerable<T> words)
        {
            foreach (var pipeline in Pipelines)
            {
                words = pipeline.Process(words);
            }

            return words;
        }
    }
}
