﻿using Wikiled.Text.Analysis.POS;
using Wikiled.Sentiment.Text.NLP.Repair;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Tests.NLP.Repair
{
    [TestFixture]
    public class WordRepairRuleEngineTests
    {
        private WordsHandlerHelper helper;

        private TestWordItem wordItem;

        private WordRepairRule repairRule;

        [SetUp]
        public void Setup()
        {
            helper = new WordsHandlerHelper();
            wordItem = SetupItem();
        }

        [Test]
        public void TestIsInvertorMultipleRules()
        {
            var engine = new WordRepairRuleEngine(wordItem, repairRule);
            var rule1 = new Rule();
            var rule2 = new Rule();
            repairRule.Set = new[] { new RuleSet() };
            repairRule.Set[0].Rules = new[] { rule1, rule2 };
            rule1.Index = 1;
            rule1.NextWordPOS = WordType.Verb;
            rule2.Index = 1;
            rule2.Word = "working";
            var isInvertor = engine.Evaluate();
            Assert.IsTrue(isInvertor.Value);
        }

        [Test]
        public void TestIsInvertorMultipleRulesFail()
        {
            var wordItem = SetupItem();
            var engine = new WordRepairRuleEngine(wordItem, repairRule);
            var rule1 = new Rule();
            var rule2 = new Rule();
            repairRule.Set = new[] { new RuleSet() };
            repairRule.Set[0].Rules = new[] { rule1, rule2 };
            rule1.Index = 1;
            rule1.NextWordPOS = WordType.Verb;
            rule2.Index = 1;
            rule2.Word = "x";
            var isInvertor = engine.Evaluate();
            Assert.IsFalse(isInvertor.Value);
        }

        [Test]
        public void TestIsInvertorByPos()
        {
            var wordItem = SetupItem();
            var engine = new WordRepairRuleEngine(wordItem, repairRule);
            var rule = new Rule();
            repairRule.Set = new[] { new RuleSet() };
            repairRule.Set[0].Rules = new[] { rule };
            rule.Index = 1;
            rule.NextWordPOS = WordType.Verb;
            var isInvertor = engine.Evaluate();
            Assert.IsTrue(isInvertor.Value);
        }

        [Test]
        public void TestIsInvertorByPosFail()
        {
            var engine = new WordRepairRuleEngine(wordItem, repairRule);
            var rule = new Rule();
            repairRule.Set = new[] { new RuleSet() };
            repairRule.Set[0].Rules = new[] { rule };
            rule.Index = 1;
            rule.NextWordPOS = WordType.Noun;
            var isInvertor = engine.Evaluate();
            Assert.IsFalse(isInvertor.Value);
        }

        [Test]
        public void TestIsInvertorByPosAndEnding()
        {
            var engine = new WordRepairRuleEngine(wordItem, repairRule);
            var rule = new Rule();
            repairRule.Set = new[] { new RuleSet() };
            repairRule.Set[0].Rules = new[] { rule };
            rule.Index = 1;
            rule.NextWordPOS = WordType.Verb;
            rule.Ending = "ing";
            var isInvertor = engine.Evaluate();
            Assert.IsTrue(isInvertor.Value);
        }

        [Test]
        public void TestIsInvertorByPosAndEndingFail()
        {

            var engine = new WordRepairRuleEngine(wordItem, repairRule);
            var rule = new Rule();
            repairRule.Set = new[] { new RuleSet() };
            repairRule.Set[0].Rules = new[] { rule };
            rule.Index = 1;
            rule.NextWordPOS = WordType.Verb;
            rule.Ending = "ed";
            var isInvertor = engine.Evaluate();
            Assert.IsFalse(isInvertor.Value);
        }

        [Test]
        public void TestIsInvertorFail()
        {
            var engine = new WordRepairRuleEngine(wordItem, repairRule);
            var isInvertor = engine.Evaluate();
            Assert.IsFalse(isInvertor.Value);
        }

        private TestWordItem SetupItem()
        {
            wordItem = new TestWordItem(string.Empty);
            repairRule = new WordRepairRule();
            repairRule.SuccesfulResult = true;
            wordItem.Relationship = new WordItemRelationships(helper.Handler.Object, wordItem);
            var next = new TestWordItem("working");
            wordItem.Relationship.Next = next;
            next.POS = POSTags.Instance.VB;
            return wordItem;
        }
    }
}
