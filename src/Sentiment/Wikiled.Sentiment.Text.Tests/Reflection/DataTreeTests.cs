using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Reflection;
using Wikiled.Sentiment.Text.Reflection.Data;
using Wikiled.Sentiment.Text.Tests.Reflection.TestData;

namespace Wikiled.Sentiment.Text.Tests.Reflection
{
    [TestFixture]
    public class DataTreeTests
    {
        [Test]
        public void Create()
        {
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            MainItem main = new MainItem();
            DataTree tree = new DataTree(main, construction);
            Assert.AreEqual(main, tree.Instance);
            Assert.AreEqual(construction.Categories.Count(), tree.Branches.Count);
            Assert.AreEqual(main.SubCat, tree.Branches[0].Instance);
            Assert.AreEqual(construction.Fields.Count(), tree.Leafs.Count);
            Assert.AreEqual(construction.AllChildFields.Count(), tree.AllLeafs.Count());
        }

        [Test]
        public void TestCollection()
        {
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            MainItem main = new MainItem();
            DataTree tree = new DataTree(main, construction);
            Assert.AreEqual(0, tree.Branches[1].Leafs.Count);
            main.Data["Test"] = 4;
            tree = new DataTree(main, construction);
            Assert.AreEqual(1, tree.Branches[1].Leafs.Count);

            Assert.AreEqual(4, tree.Branches[1].Leafs[0].Value);
            Assert.AreEqual("Test", tree.Branches[1].Leafs[0].Name);
            Assert.AreEqual("Test", tree.Branches[1].Leafs[0].Description);
        }

        [Test]
        public void GetSetValue()
        {
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            MainItem main = new MainItem();
            main.IsGood = true;
            main.Total = 2;
            main.SubCat.Weight = 4;
            DataTree tree = new DataTree(main, construction);
            Assert.AreEqual(true, tree.Leafs[1].Value);
            Assert.AreEqual(2, tree.Leafs[0].Value);
            Assert.AreEqual(4, tree.Branches[0].Leafs[0].Value);

            tree.Leafs[1].Value = false;
            Assert.AreEqual(false, tree.Leafs[1].Value);
            Assert.AreEqual(false, main.IsGood);

            tree.Leafs[0].Value = 88;
            Assert.AreEqual(88, tree.Leafs[0].Value);
            Assert.AreEqual(88, main.Total);

            tree.Branches[0].Leafs[0].Value = 7;
            Assert.AreEqual(7, tree.Branches[0].Leafs[0].Value);
            Assert.AreEqual(7, main.SubCat.Weight);
        }

        [Test]
        public void DictionaryValue()
        {
            Dictionary<string, double> map = new Dictionary<string, double>();
            map["IsGood"] = 0.1;
            map["Weight"] = 3;
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            MainItem main = new MainItem();
            main.IsGood = true;
            main.Total = 2;
            main.SubCat.Weight = 4;
            DataTree tree = new DataTree(main, construction, new DictionaryDataItemFactory(map));
            Assert.AreEqual(0.1, Math.Round((double)tree.Leafs[1].Value, 2));
            Assert.AreEqual(0, tree.Leafs[0].Value);
            Assert.AreEqual(3, tree.Branches[0].Leafs[0].Value);
        }
    }
}
