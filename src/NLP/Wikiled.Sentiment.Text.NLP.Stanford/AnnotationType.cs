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
            var loadingType = typeof(com.sun.codemodel.@internal.ClassType); // IKVM.OpenJDK.Tools
            loadingType = typeof(com.sun.org.apache.xalan.@internal.xsltc.trax.TransformerFactoryImpl); // IKVM.OpenJDK.XML.Transform
            loadingType= typeof(com.sun.org.glassfish.external.amx.AMX); // IKVM.OpenJDK.XML.WebService

            PartOfSpeech = new CoreAnnotations.PartOfSpeechAnnotation().getClass();
            Sentence = new CoreAnnotations.SentencesAnnotation().getClass();
            Tokens = new CoreAnnotations.TokensAnnotation().getClass();
            Tree = new TreeCoreAnnotations.TreeAnnotation().getClass();
            CollapsedCCProcessedDependencies = new SemanticGraphCoreAnnotations.CollapsedCCProcessedDependenciesAnnotation().getClass();
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

        public Class Sentiment { get; private set; }

        public Class DocDate { get; private set; }

        public Class PartOfSpeech { get; private set; }

        public Class TimeExpression { get; private set; }

        public Class AllTimex { get; private set; }

        public Class Timex { get; private set; }

        public Class NormalizedEntity { get; private set; }

        public Class NamedEntity { get; private set; }

        public Class Text { get; private set; }

        public Class Lemma { get; private set; }

        public Class CollapsedCCProcessedDependencies { get; private set; }

        public Class Tree { get; private set; }

        public Class Tokens { get; private set; }

        public Class Sentence { get; private set; }

        public static AnnotationType Instance { get; } = new AnnotationType();
    }
}

