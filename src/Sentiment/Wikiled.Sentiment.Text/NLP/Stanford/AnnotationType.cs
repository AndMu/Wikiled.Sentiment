using edu.stanford.nlp.ling;
using edu.stanford.nlp.semgraph;
using edu.stanford.nlp.sentiment;
using edu.stanford.nlp.time;
using edu.stanford.nlp.trees;
using java.lang;

namespace Wikiled.Sentiment.Text.NLP.Stanford
{
    public class AnnotationType
    {
        private AnnotationType()
        {
            PartOfSpeech = new CoreAnnotations.PartOfSpeechAnnotation().getClass();
            Sentence = new CoreAnnotations.SentencesAnnotation().getClass();
            Tokens = new CoreAnnotations.TokensAnnotation().getClass();
            Tree = new TreeCoreAnnotations.TreeAnnotation().getClass();
            Lemma = new CoreAnnotations.LemmaAnnotation().getClass();
            Text = new CoreAnnotations.TextAnnotation().getClass();
            NamedEntity = new CoreAnnotations.NamedEntityTagAnnotation().getClass();
            NormalizedEntity = new CoreAnnotations.NormalizedNamedEntityTagAnnotation().getClass();
            AllTimex = new TimeAnnotations.TimexAnnotations().getClass();
            Timex = new TimeAnnotations.TimexAnnotation().getClass();
            TimeExpression = new TimeExpression.Annotation().getClass();
            DocDate = new CoreAnnotations.DocDateAnnotation().getClass();
            Sentiment = new SentimentCoreAnnotations.SentimentClass().getClass();
        }

        public static AnnotationType Instance { get; } = new AnnotationType();

        public Class AllTimex { get; }

        public Class CollapsedCCProcessedDependencies { get; }

        public Class DocDate { get; }

        public Class Lemma { get; }

        public Class NamedEntity { get; }

        public Class NormalizedEntity { get; }

        public Class PartOfSpeech { get; }

        public Class Sentence { get; }

        public Class Sentiment { get; }

        public Class Text { get; }

        public Class TimeExpression { get; }

        public Class Timex { get; }

        public Class Tokens { get; }

        public Class Tree { get; }
    }
}
