﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Wikiled.Common.Extensions;
using Wikiled.Sentiment.Text.Configuration;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.Sentiment.Text.Words;
using Wikiled.Text.Analysis.Dictionary;
using Wikiled.Text.Analysis.Dictionary.Streams;
using Wikiled.Text.Analysis.NLP;

namespace Wikiled.Sentiment.Text.NLP.Repair
{
    public class SentenceRepairHandler : ISentenceRepairHandler
    {
        private readonly List<SentenceRepair> repairs = new List<SentenceRepair>();

        private readonly string resourcesPath;

        private readonly IWordsHandler wordsHandlers;

        private readonly IWordFactory wordFactory;

        private readonly IWordsDictionary dictionary;

        private Dictionary<string, int> emoticons = new Dictionary<string, int>();

        private Dictionary<string, int> idioms = new Dictionary<string, int>();

        private Dictionary<string, string> slangs = new Dictionary<string, string>();

        public SentenceRepairHandler(ILexiconConfiguration path, IWordsHandler wordsHandlers, IWordFactory wordFactory, IWordsDictionary dictionary)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            resourcesPath = Path.Combine(path.LexiconPath, "Repair");
            this.wordsHandlers = wordsHandlers ?? throw new ArgumentNullException(nameof(wordsHandlers));
            this.wordFactory = wordFactory ?? throw new ArgumentNullException(nameof(wordFactory));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            Load();
        }

        public string Repair(string sentence)
        {
            if (string.IsNullOrEmpty(sentence))
            {
                return string.Empty;
            }

            foreach (var sentenceRepair in repairs)
            {
                sentence = sentenceRepair.Repair(sentence);
            }

            ReplacementOption option = ReplacementOption.IgnoreCase | ReplacementOption.WholeWord;
            foreach (var emoticon in emoticons)
            {
                sentence = RepairByLevel(emoticon.Value * 2, emoticon.Key, sentence);
            }

            foreach (var slang in slangs)
            {
                sentence = sentence.ReplaceString(slang.Key, slang.Value, option);
            }

            foreach (var emoticon in idioms)
            {
                sentence = RepairByLevel(emoticon.Value, emoticon.Key, sentence);
            }

            return sentence;
        }

        private void Load()
        {
            XDocument document = XDocument.Load(Path.Combine(resourcesPath, "Repair.xml"));
            var items = from item in document.XPathSelectElements("//Rules/Rule")
                        select item;
            foreach (var item in items)
            {
                var test = item.Element("Test");
                if (test == null)
                {
                    throw new NullReferenceException("test");
                }

                var match = item.Element("Match");
                if (match == null)
                {
                    throw new NullReferenceException("match");
                }

                var replace = item.Element("Replace");
                if (replace == null)
                {
                    throw new NullReferenceException("Replace");
                }

                var repair = new SentenceRepair(dictionary, test.Value, match.Value, replace.Value);
                var verify = item.Element("Verify");
                if (verify != null)
                {
                    repair.VerifyDictionary = verify.Value.Split(';')
                                                    .Select(int.Parse)
                                                    .ToArray();
                }

                repairs.Add(repair);
            }

            ReadSlang();
            ReadEmoticons();
            ReadIdioms();
        }

        private void ReadEmoticons()
        {
            DictionaryStream stream = new DictionaryStream(Path.Combine(resourcesPath, "EmoticonLookupTable.txt"), new FileStreamSource());
            emoticons = stream.ReadDataFromStream(int.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
        }

        private void ReadIdioms()
        {
            DictionaryStream stream = new DictionaryStream(Path.Combine(resourcesPath, "IdiomLookupTable.txt"), new FileStreamSource());
            idioms = stream.ReadDataFromStream(int.Parse).ToDictionary(item => item.Word, item => item.Value, StringComparer.OrdinalIgnoreCase);
        }

        private void ReadSlang()
        {
            slangs.Clear();
            DictionaryStream stream = new DictionaryStream(Path.Combine(resourcesPath, "SlangLookupTable.txt"), new FileStreamSource());
            foreach (var item in stream.ReadDataFromStream(item => item))
            {
                slangs[item.Word] = item.Value;
                if (wordsHandlers.IsSentiment(wordFactory.CreateWord(item.Word, "JJ")))
                {
                    slangs.Remove(item.Word);
                }
            }
        }

        private string RepairByLevel(int level, string value, string sentence)
        {
            string replaceValue = null;
            ReplacementOption option = ReplacementOption.IgnoreCase | ReplacementOption.WholeWord;
            switch (level)
            {
                case 0:
                    replaceValue = "xxxneutralxxx";
                    break;
                case 1:
                    replaceValue = "xxxgoodxxxone";
                    break;
                case 2:
                    replaceValue = "xxxgoodxxxtwo";
                    break;
                case 3:
                    replaceValue = "xxxgoodxxxthree";
                    break;
                case 4:
                    replaceValue = "xxxgoodxxxfour";
                    break;
                case -1:
                    replaceValue = "xxxbadxxxone";
                    break;
                case -2:
                    replaceValue = "xxxbadxxxtwo";
                    break;
                case -3:
                    replaceValue = "xxxbadxxxthree";
                    break;
                case -4:
                    replaceValue = "xxxbadxxxfour";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level", "Root level - " + level);
            }

            return sentence.ReplaceString(value, replaceValue, option);
        }
    }
}
