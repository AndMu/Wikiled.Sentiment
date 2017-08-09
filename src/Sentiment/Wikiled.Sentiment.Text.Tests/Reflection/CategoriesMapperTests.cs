using System;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Text.Reflection;
using Wikiled.Sentiment.Text.Tests.Reflection.TestData;

namespace Wikiled.Sentiment.Text.Tests.Reflection
{
    [TestFixture]
    public class CategoriesMapperTests
    {
        [Test]
        public void RegularConstruct()
        {
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            Assert.AreEqual(3, construction.AllChildFields.Count());
            Assert.AreEqual(2, construction.Fields.Count());
            Assert.AreEqual(2, construction.Categories.Count());
            Assert.AreEqual(1, construction.Categories.First().Fields.Count());
        }

        [Test]
        public void ResolveInstance()
        {
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            MainItem main = new MainItem();
            Assert.AreEqual(main, construction.ResolveInstance(main));
            Assert.AreEqual(main.SubCat, construction.Categories.First().ResolveInstance(main));
        }
      
        [Test]
        public void ConstructNotAllowed()
        {
            var mapper = new CategoriesMapper();
            Assert.Throws<ArgumentOutOfRangeException>(() => mapper.Construct<AnotherMainItem>());
        }

        [Test]
        public void GetValue()
        {
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            MainItem main = new MainItem();
            main.IsGood = true;
            main.Total = 2;
            main.SubCat.Weight = 4;
            Assert.AreEqual(true, construction["IsGood"].GetValue<bool>(main));
            Assert.AreEqual(4, construction["Weight"].GetValue<int>(main.SubCat));
            Assert.AreEqual(2, construction["Total"].GetValue<int>(main));
        }

        [Test]
        public void SetValue()
        {
            var mapper = new CategoriesMapper();
            IMapCategory construction = mapper.Construct<MainItem>();
            MainItem main = new MainItem();
            construction["IsGood"].SetValue(main, true);
            construction["Weight"].SetValue(main.SubCat, 10);
            construction["Total"].SetValue(main, 20);
            Assert.AreEqual(true, main.IsGood);
            Assert.AreEqual(20, main.Total);
            Assert.AreEqual(10, main.SubCat.Weight);
        }
    }
}
