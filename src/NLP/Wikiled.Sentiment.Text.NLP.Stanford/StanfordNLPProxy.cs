using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using edu.stanford.nlp.ie;
using Wikiled.Sentiment.Text.Parser;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.tagger.maxent;
using edu.stanford.nlp.time;
using edu.stanford.nlp.trees;
using edu.stanford.nlp.util;
using java.util;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Sentiment.Text.Structure;
using Wikiled.Text.Analysis.Structure;
using Document = Wikiled.Text.Analysis.Structure.Document;

namespace Wikiled.Sentiment.Text.NLP.Stanford
{
    public class StanfordNLPProxy
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Lazy<AnnotationPipeline> pipelineHolder;

        private readonly object syncRoot = new object();

        private readonly IWordsHandler wordsHandlersManager;

        private readonly DirectoryInfo modelsDirectory;

        public StanfordNLPProxy(string resources, IWordsHandler wordsHandlersManager)
        {
            Guard.NotNull(() => wordsHandlersManager, wordsHandlersManager);
            Guard.NotNullOrEmpty(() => resources, resources);
            this.wordsHandlersManager = wordsHandlersManager;

            modelsDirectory = new DirectoryInfo(resources + @"\models");
            if (!modelsDirectory.Exists)
            {
                throw new ArgumentOutOfRangeException("resources", resources);
            }

            pipelineHolder = new Lazy<AnnotationPipeline>(InitPipeline);
        }

        public Document Process(ParseRequest request)
        {
            try
            {
                DateTime defaultDate;
                if (request.Date.HasValue &&
                    request.Date.Value.Year > 1)
                {
                    defaultDate = request.Date.Value;
                }
                else
                {
                    log.Debug("Using default - todays date");
                    defaultDate = DateTime.Today;
                }

                var parsed = new Document(request.Document.Text);
                parsed.DocumentTime = defaultDate;
                Annotation document = ExtractAnnotation(parsed);
                ArrayList sentences = (ArrayList)document.get(AnnotationType.Instance.Sentence);
                Dictionary<CoreLabel, Timex> timexExpressions = new Dictionary<CoreLabel, Timex>();

                // a CoreMap is essentially a Map that uses class objects as keys and has values with custom types
                foreach (Annotation sentence in sentences)
                {
                    // this is the parse tree of the current sentence
                    //new TimeAnnotations.TimexAnnotations().getClass();
                    var timexAll = sentence.get(AnnotationType.Instance.AllTimex) as ArrayList;
                    if (timexAll != null)
                    {
                        foreach (CoreMap coreMap in timexAll)
                        {
                            var timexTokens = (ArrayList)coreMap.get(AnnotationType.Instance.Tokens);
                            var timex = (Timex)coreMap.get(AnnotationType.Instance.Timex);
                            foreach (CoreLabel label in timexTokens)
                            {
                                timexExpressions[label] = timex;
                            }
                        }
                    }

                    Tree tree = (Tree)sentence.get(AnnotationType.Instance.Tree);
                    var wordsList = tree.getLeaves();
                    var words = wordsList.toArray();
                    var tokens = ((ArrayList)sentence.get(AnnotationType.Instance.Tokens))
                        .toArray()
                        .Select(item => (CoreLabel)item)
                        .ToArray();

                    var sentenceText = SentenceUtils.listToString(wordsList);
                    var currentSentence = new SentenceItem(sentenceText);
                    if (words.Length > 0)
                    {
                        parsed.Add(currentSentence);
                    }

                    for (int i = 0; i < words.Length; i++)
                    {
                        LabeledScoredTreeNode node = (LabeledScoredTreeNode)words[i];
                        var mainNode = node.parent(tree);
                        string itemText = node.nodeString();
                        var token = FindLabel(tokens, i, itemText);
                        Timex timex;
                        timexExpressions.TryGetValue(token, out timex);

                        var sentiment = (string)token.get(AnnotationType.Instance.Sentiment);
                        var lemma = (string)token.get(AnnotationType.Instance.Lemma);
                        var tokenPos = (string)token.get(AnnotationType.Instance.PartOfSpeech);
                        var namedEntityText = (string)token.get(AnnotationType.Instance.NamedEntity);
                        var normalizedEntity = (string)token.get(AnnotationType.Instance.NormalizedEntity);
                        NamedEntities entity = NamedEntities.None;
                        if (!string.IsNullOrEmpty(namedEntityText))
                        {
                            if (string.Compare(namedEntityText, "O", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                entity = NamedEntities.None;
                            }
                            else if (!Enum.TryParse(namedEntityText, true, out entity))
                            {
                                entity = NamedEntities.None;
                                log.Warn("Failed to parse: {0} named entity for {1}", namedEntityText, itemText);
                            }
                        }

                        if (itemText.IndexOf("'", StringComparison.Ordinal) == 0)
                        // replace 's with their lemmas like is
                        {
                            itemText = lemma;
                        }

                        var occurrence = wordsHandlersManager.WordFactory.CreateWord(itemText, lemma, tokenPos);
                        occurrence.Entity = entity;
                    
                        occurrence.WordIndex = i;
                        occurrence.NormalizedEntity = normalizedEntity;
                        var item = WordExFactory.Construct(occurrence);
                        currentSentence.Add(item);

                        var phrase = mainNode.parent(tree);
                        if (phrase != null)
                        {
                            var posPhrase = ((CoreLabel)phrase.label()).value();
                            if (posPhrase.IndexOf('-') > 0) // we remove them temporary but later let's inculude
                            {
                                // look at this http://nlp.stanford.edu/fsnlp/dontpanic.pdf
                                posPhrase = posPhrase.Substring(0, posPhrase.IndexOf('-'));
                            }

                            item.Phrase = posPhrase;
                        }
                    }
                }

                return parsed;
            }
            catch (Exception exception)
            {
                log.Error(exception);
                throw;
            }
        }

        private CoreLabel FindLabel(CoreLabel[] labels, int index, string text)
        {
            for (int i = index; i < labels.Length; i++)
            {
                string textLabel = (string)labels[i].get(AnnotationType.Instance.Text);
                if (string.Compare(text, textLabel, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return labels[i];
                }
            }

            return null;
        }

        private Annotation ExtractAnnotation(Document doc)
        {
            lock (syncRoot)
            {
                try
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    // create an empty Annotation just with the given text
                    var document = new Annotation(doc.Text);
                    document.set(AnnotationType.Instance.DocDate, doc.DocumentTime.Value.ToString("yyyy-MM-dd"));
                    // run all Annotators on this text
                    pipelineHolder.Value.annotate(document);
                    timer.Stop();
                    log.Debug("Document parsed in {0:F2}ms", timer.ElapsedMilliseconds);
                    return document;
                }
                catch
                {
                    log.Error("Failed to parse: {0}", doc.Text);
                    throw;
                }
            }
        }

        private AnnotationPipeline InitPipeline()
        {
             log.Debug("Creating Parser...");
            // Annotation pipeline configuration
            var pipeline = new AnnotationPipeline();
            pipeline.addAnnotator(new TokenizerAnnotator(false));
            pipeline.addAnnotator(new WordsToSentencesAnnotator(false));
            pipeline.addAnnotator(
                new ParserAnnotator(modelsDirectory + @"\lexparser\englishPCFG.ser.gz", false, -1, new string[] { }));

            // Loading POS Tagger and including them into pipeline
            var tagger = new MaxentTagger(modelsDirectory +
                                          @"\pos-tagger\english-left3words\english-left3words-distsim.tagger");
            pipeline.addAnnotator(new POSTaggerAnnotator(tagger));

            // SUTime configuration
            var sutimeRules = modelsDirectory + @"\sutime\defs.sutime.txt,"
                              + modelsDirectory + @"\sutime\english.holidays.sutime.txt,"
                              + modelsDirectory + @"\sutime\english.sutime.txt";

            var props = new Properties();
            props.setProperty("sutime.rules", sutimeRules);
            props.setProperty("sutime.teRelHeurLevel", "MORE");
            props.setProperty("sutime.binders", "0");
            pipeline.addAnnotator(new TimeAnnotator("sutime", props));
            pipeline.addAnnotator(new MorphaAnnotator(false));


            props = new Properties();
            pipeline.addAnnotator(new BinarizerAnnotator("ba", props));

            props = new Properties();
            props.setProperty("sa.model", modelsDirectory + @"\sentiment\sentiment.ser.gz");
            pipeline.addAnnotator(new SentimentAnnotator("sa", props));

            var nerClasifier = new NERClassifierCombiner(
                true,
                false,
                false,
                modelsDirectory + @"\ner\english.all.3class.distsim.crf.ser.gz",
                modelsDirectory + @"\ner\english.muc.7class.distsim.crf.ser.gz",
                modelsDirectory + @"\ner\english.conll.4class.distsim.crf.ser.gz");

            pipeline.addAnnotator(new NERCombinerAnnotator(nerClasifier, false, 10, -1));

            return pipeline;
        }
    }
}
