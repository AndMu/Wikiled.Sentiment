using NUnit.Framework;
using Wikiled.Arff.Persistence;
using Wikiled.Sentiment.Text.NLP.Inquirer;

namespace Wikiled.Sentiment.Text.Tests.NLP.Inquirer
{
    [TestFixture]
    public class InquirerManagerTests
    {
        private InquirerManager manager;

        [OneTimeSetUp]
        public void SetupGlobal()
        {
            manager = new InquirerManager();
            manager.Load();
        }

        [Test]
        public void Total()
        {
            Assert.AreEqual(8639, manager.Total);
        }

        [Test]
        public void GetBody()
        {
            var definitions = manager.GetDefinitions("body");
            Assert.AreEqual(5, definitions.Records.Length);
            Assert.AreEqual("78% noun-adj: The physical structure and substance of an animal, living  or dead, usually human", definitions.Records[0].Description.Information);
            Assert.AreEqual("Noun", definitions.Records[0].Description.OtherTags);
        }

        [Test]
        public void InquirerDescription()
        {
            InquirerDescription definition = new InquirerDescription();
            Assert.IsFalse(definition.Harward.Activity.HasData);
            Assert.IsFalse(definition.Harward.Feeling.HasData);
            Assert.IsFalse(definition.Harward.Institution.HasData);
            Assert.IsFalse(definition.Harward.Sentiment.HasData);
            Assert.IsFalse(definition.Harward.Social.HasData);
            Assert.IsFalse(definition.Harward.Statement.HasData);
            Assert.IsFalse(definition.Harward.Location.HasData);
        }

        [Test]
        public void GetCelebration()
        {
            var definitions = manager.GetDefinitions("celebration");
            Assert.AreEqual(1, definitions.Records.Length);
            Assert.AreEqual(PositivityType.Positive, definitions.Records[0].Description.Harward.Sentiment.Type);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsSupportive);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsActive);
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsHostile);
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsPassive);
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsPower);
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsStrong);
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsSubmit);
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsWeak);
        }

        [Test]
        public void IsActive()
        {
            var definitions = manager.GetDefinitions("ABIDE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsActive);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("ABANDON");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsActive);
        }

        [Test]
        public void IsHostile()
        {
            var definitions = manager.GetDefinitions("ABHOR");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsHostile);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("ABANDON");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsHostile);
        }

        [Test]
        public void IsPassive()
        {
            var definitions = manager.GetDefinitions("ABATE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsPassive);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("ABANDON");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsPassive);
        }

        [Test]
        public void IsPower()
        {
            var definitions = manager.GetDefinitions("WORLD-FAMOUS");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsPower);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("ABANDON");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsPower);
        }

        [Test]
        public void IsStrong()
        {
            var definitions = manager.GetDefinitions("WON");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsStrong);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("ABANDON");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsStrong);
        }

        [Test]
        public void IsSubmit()
        {
            var definitions = manager.GetDefinitions("WITHDRAWN");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsSubmit);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("ABANDON");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsSubmit);
        }

        [Test]
        public void IsSupportive()
        {
            var definitions = manager.GetDefinitions("SUPPORTER");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsSupportive);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("ABANDON");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsSupportive);
        }

        [Test]
        public void IsWeak()
        {
            var definitions = manager.GetDefinitions("SUPERFICIAL");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.IsWeak);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Sentiment.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Sentiment.IsWeak);
        }

        [Test]
        public void IsArousal()
        {
            var definitions = manager.GetDefinitions("SHY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.IsArousal);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Feeling.IsArousal);
        }

        [Test]
        public void IsEmotion()
        {
            var definitions = manager.GetDefinitions("INDIFFERENCE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.IsEmotion);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Feeling.IsEmotion);
        }

        [Test]
        public void IsFeel()
        {
            var definitions = manager.GetDefinitions("SHAMEFUL");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.IsFeel);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Feeling.IsFeel);
        }

        [Test]
        public void IsPain()
        {
            var definitions = manager.GetDefinitions("SCRAPE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.IsPain);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Feeling.IsPain);
        }

        [Test]
        public void IsPleasure()
        {
            var definitions = manager.GetDefinitions("GAILY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.IsPleasure);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Feeling.IsPleasure);
        }

        [Test]
        public void IsVice()
        {
            var definitions = manager.GetDefinitions("FRAIL");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.IsVice);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Feeling.IsVice);
        }

        [Test]
        public void IsVirtue()
        {
            var definitions = manager.GetDefinitions("FORTUNATE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.IsVirtue);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Feeling.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Feeling.IsVirtue);
        }

        [Test]
        public void IsAcademic()
        {
            var definitions = manager.GetDefinitions("EXPERIMENTAL");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsAcademic);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsAcademic);
        }

        [Test]
        public void IsDoctrine()
        {
            var definitions = manager.GetDefinitions("DISBELIEF");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsDoctrine);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsDoctrine);
        }

        [Test]
        public void IsEconomic()
        {
            var definitions = manager.GetDefinitions("DESTITUTE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsEconomic);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsEconomic);
        }

        [Test]
        public void IsExchange()
        {
            var definitions = manager.GetDefinitions("CONSUMPTION");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsExchange);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsExchange);
        }

        [Test]
        public void IsExpression()
        {
            var definitions = manager.GetDefinitions("CONJURE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsExpression);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsExpression);
        }

        [Test]
        public void IsLegal()
        {
            var definitions = manager.GetDefinitions("CERTIFY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsLegal);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsLegal);
        }

        [Test]
        public void IsMilitary()
        {
            var definitions = manager.GetDefinitions("CAVALRY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsMilitary);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsMilitary);
        }

        [Test]
        public void IsPolitical()
        {
            var definitions = manager.GetDefinitions("CAPITOL");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsPolitical);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsPolitical);
        }

        [Test]
        public void IsReligious()
        {
            var definitions = manager.GetDefinitions("BISHOP");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.IsReligious);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Institution.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Institution.IsReligious);
        }

        [Test]
        public void IsOverstated()
        {
            var definitions = manager.GetDefinitions("BILLION");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Statement.IsOverstated);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Statement.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Statement.IsOverstated);
        }

        [Test]
        public void IsUnderstated()
        {
            var definitions = manager.GetDefinitions("BEWILDER");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Statement.IsUnderstated);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Statement.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Statement.IsUnderstated);
        }

        [Test]
        public void IsCollectivity()
        {
            var definitions = manager.GetDefinitions("ASSEMBLY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.IsCollectivity);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Activity.IsCollectivity);
        }

        [Test]
        public void IsRitual()
        {
            var definitions = manager.GetDefinitions("BALLOT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.IsRitual);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Activity.IsRitual);
        }

        [Test]
        public void IsRole()
        {
            var definitions = manager.GetDefinitions("BAKER");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.IsRole);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Activity.IsRole);
        }

        [Test]
        public void IsSociallyRelation()
        {
            var definitions = manager.GetDefinitions("AVOIDANCE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.IsSociallyRelation);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.HasData);
            definitions = manager.GetDefinitions("ATTACH");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Activity.IsSociallyRelation);
        }

        [Test]
        public void IsWork()
        {
            var definitions = manager.GetDefinitions("ATTACH");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.IsWork);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Activity.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Activity.IsWork);
        }

        [Test]
        public void IsAnimal()
        {
            var definitions = manager.GetDefinitions("ANT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.IsAnimal);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Social.IsAnimal);
        }

        [Test]
        public void IsFemale()
        {
            var definitions = manager.GetDefinitions("WOMEN");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.IsFemale);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Social.IsFemale);
        }

        [Test]
        public void IsHuman()
        {
            var definitions = manager.GetDefinitions("MALE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.IsHuman);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Social.IsHuman);
        }

        [Test]
        public void IsKin()
        {
            var definitions = manager.GetDefinitions("KINDRED");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.IsKin);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Social.IsKin);
        }

        [Test]
        public void IsMale()
        {
            var definitions = manager.GetDefinitions("HUSBAND");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.IsMale);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Social.IsMale);
        }

        [Test]
        public void IsNonAdult()
        {
            var definitions = manager.GetDefinitions("GRANDCHILDREN");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.IsNonAdult);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Social.IsNonAdult);
        }

        [Test]
        public void IsRacial()
        {
            var definitions = manager.GetDefinitions("ETHNIC");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.IsRacial);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Social.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Social.IsRacial);
        }

        [Test]
        public void IsAquatic()
        {
            var definitions = manager.GetDefinitions("CREEK");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.IsAquatic);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Location.IsAquatic);
        }

        [Test]
        public void IsLand()
        {
            var definitions = manager.GetDefinitions("CLIFF");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.IsLand);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Location.IsLand);
        }

        [Test]
        public void IsPlace()
        {
            var definitions = manager.GetDefinitions("CHURCH");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.IsPlace);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Location.IsPlace);
        }

        [Test]
        public void IsRegion()
        {
            var definitions = manager.GetDefinitions("CITY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.IsRegion);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Location.IsRegion);
        }

        [Test]
        public void IsRoute()
        {
            var definitions = manager.GetDefinitions("BOULEVARD");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.IsRoute);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Location.IsRoute);
        }

        [Test]
        public void IsSky()
        {
            var definitions = manager.GetDefinitions("BLIZZARD");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.IsSky);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Location.IsSky);
        }

        [Test]
        public void IsSocial()
        {
            var definitions = manager.GetDefinitions("BIRTHPLACE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.IsSocial);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Location.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Location.IsSocial);
        }

        [Test]
        public void IsBodyPart()
        {
            var definitions = manager.GetDefinitions("ANKLE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.IsBodyPart);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Object.IsBodyPart);
        }

        [Test]
        public void IsBuilding()
        {
            var definitions = manager.GetDefinitions("ATTIC");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.IsBuilding);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Object.IsBuilding);
        }

        [Test]
        public void IsCommunication()
        {
            var definitions = manager.GetDefinitions("VERSE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.IsCommunication);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Object.IsCommunication);
        }

        [Test]
        public void IsFood()
        {
            var definitions = manager.GetDefinitions("bread");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.IsFood);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Object.IsFood);
        }

        [Test]
        public void IsNatural()
        {
            var definitions = manager.GetDefinitions("URANIUM");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.IsNatural);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Object.IsNatural);
        }

        [Test]
        public void IsObject()
        {
            var definitions = manager.GetDefinitions("URANIUM");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.IsObject);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Object.IsObject);
        }

        [Test]
        public void IsVehicle()
        {
            var definitions = manager.GetDefinitions("car");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.IsVehicle);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Object.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Object.IsVehicle);
        }

        [Test]
        public void IsCommunicationTransaction()
        {
            var definitions = manager.GetDefinitions("ACCUSATION");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Communication.IsCommunicationTransaction);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Communication.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Communication.IsCommunicationTransaction);
        }

        [Test]
        public void IsSaying()
        {
            var definitions = manager.GetDefinitions("SAY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Communication.IsSaying);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Communication.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Communication.IsSaying);
        }

        [Test]
        public void HasCompleted()
        {
            var definitions = manager.GetDefinitions("ACHIEVE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasCompleted);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Motivation.HasCompleted);
        }

        [Test]
        public void HasFailed()
        {
            var definitions = manager.GetDefinitions("ABANDON");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasFailed);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Motivation.HasFailed);
        }

        [Test]
        public void IsGoal()
        {
            var definitions = manager.GetDefinitions("ACCOMPLISHMENT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.IsGoal);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Motivation.IsGoal);
        }

        [Test]
        public void IsMeans()
        {
            var definitions = manager.GetDefinitions("ALONG");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.IsMeans);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Motivation.IsMeans);
        }

        [Test]
        public void IsNeed()
        {
            var definitions = manager.GetDefinitions("CONCERN");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.IsNeed);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Motivation.IsNeed);
        }

        [Test]
        public void IsPersist()
        {
            var definitions = manager.GetDefinitions("CONSOLIDATE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.IsPersist);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Motivation.IsPersist);
        }

        [Test]
        public void IsTry()
        {
            var definitions = manager.GetDefinitions("CONTEND");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.IsTry);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Motivation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Motivation.IsTry);
        }

        [Test]
        public void IsBegin()
        {
            var definitions = manager.GetDefinitions("CONVENE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsBegin);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsBegin);
        }

        [Test]
        public void IsDecrease()
        {
            var definitions = manager.GetDefinitions("CORROSION");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsDecrease);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsDecrease);
        }

        [Test]
        public void IsExert()
        {
            var definitions = manager.GetDefinitions("CRIPPLE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsExert);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsExert);
        }

        [Test]
        public void IsFall()
        {
            var definitions = manager.GetDefinitions("DESCEND");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsFall);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsFall);
        }

        [Test]
        public void IsFetch()
        {
            var definitions = manager.GetDefinitions("DISTRIBUTE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsFetch);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsFetch);
        }

        [Test]
        public void IsFinish()
        {
            var definitions = manager.GetDefinitions("DOOMSDAY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsFinish);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsFinish);
        }

        [Test]
        public void IsIncrease()
        {
            var definitions = manager.GetDefinitions("ABSORBENT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsIncrease);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsIncrease);
        }

        [Test]
        public void IsNaturalProcess()
        {
            var definitions = manager.GetDefinitions("DRANK");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsNaturalProcess);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsNaturalProcess);
        }

        [Test]
        public void IsRise()
        {
            var definitions = manager.GetDefinitions("FLEW");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsRise);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsRise);
        }

        [Test]
        public void IsStay()
        {
            var definitions = manager.GetDefinitions("HALT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsStay);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsStay);
        }

        [Test]
        public void IsTravel()
        {
            var definitions = manager.GetDefinitions("HASTE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsTravel);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsTravel);
        }

        [Test]
        public void IsVary()
        {
            var definitions = manager.GetDefinitions("IMPETUOUS");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.IsVary);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.OtherProcess.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.OtherProcess.IsVary);
        }

        [Test]
        public void IsPoliticalBroader()
        {
            var definitions = manager.GetDefinitions("INFANTRY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.IsPolitical);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Broad.IsPolitical);
        }

        [Test]
        public void IsEconomicBroader()
        {
            var definitions = manager.GetDefinitions("INSTITUTE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.IsEconomic);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Broad.IsEconomic);
        }

        [Test]
        public void IsIsAbstractBroader()
        {
            var definitions = manager.GetDefinitions("LIBERALISM");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.IsAbstract);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Broad.IsAbstract);
        }

        [Test]
        public void IsIsAbstract()
        {
            var definitions = manager.GetDefinitions("LIBERALISM");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsAbstract);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsAbstract);
        }

        [Test]
        public void IsCausal()
        {
            var definitions = manager.GetDefinitions("ACCIDENT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsCausal);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsCausal);
        }

        [Test]
        public void IsColor()
        {
            var definitions = manager.GetDefinitions("BLACK");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsColor);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsColor);
        }

        [Test]
        public void IsComparison()
        {
            var definitions = manager.GetDefinitions("another");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsComparison);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsComparison);
        }

        [Test]
        public void IsDimension()
        {
            var definitions = manager.GetDefinitions("fat");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsDimension);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsDimension);
        }

        [Test]
        public void IsDistance()
        {
            var definitions = manager.GetDefinitions("FATHOM");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsDistance);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsDistance);
        }

        [Test]
        public void IsEvaluation()
        {
            var definitions = manager.GetDefinitions("FLAGRANT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsEvaluation);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsEvaluation);
        }

        [Test]
        public void IsFrequency()
        {
            var definitions = manager.GetDefinitions("FREQUENCY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsFrequency);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsFrequency);
        }

        [Test]
        public void IsKnowing()
        {
            var definitions = manager.GetDefinitions("FUNCTIONAL");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsKnowing);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsKnowing);
        }

        [Test]
        public void IsOught()
        {
            var definitions = manager.GetDefinitions("DEFERENCE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsOught);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsOught);
        }

        [Test]
        public void IsPerceiving()
        {
            var definitions = manager.GetDefinitions("DESCRY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsPerceiving);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsPerceiving);
        }

        [Test]
        public void IsPosition()
        {
            var definitions = manager.GetDefinitions("HIGH");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsPosition);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsPosition);
        }

        [Test]
        public void IsQuality()
        {
            var definitions = manager.GetDefinitions("HILARIOUS");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsQuality);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsQuality);
        }

        [Test]
        public void IsQuantity()
        {
            var definitions = manager.GetDefinitions("HUGE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsQuantity);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsQuantity);
        }

        [Test]
        public void IsRelationships()
        {
            var definitions = manager.GetDefinitions("IDENTICAL");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsRelationships);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsRelationships);
        }

        [Test]
        public void IsSolving()
        {
            var definitions = manager.GetDefinitions("IGNORE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsSolving);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsSolving);
        }

        [Test]
        public void IsSpace()
        {
            var definitions = manager.GetDefinitions("IMMENSE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsSpace);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsSpace);
        }

        [Test]
        public void IsThinkinkg()
        {
            var definitions = manager.GetDefinitions("IMPETUOUS");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsThinkinkg);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsThinkinkg);
        }

        [Test]
        public void IsTime()
        {
            var definitions = manager.GetDefinitions("INSTANTLY");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsTime);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsTime);
        }

        [Test]
        public void IsTime2()
        {
            var definitions = manager.GetDefinitions("INSTANT");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsTime2);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsTime2);
        }

        [Test]
        public void IsName()
        {
            var definitions = manager.GetDefinitions("AFRICA");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.IsName);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Pronoun.IsName);
        }

        [Test]
        public void IsOur()
        {
            var definitions = manager.GetDefinitions("LET'S");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.IsOur);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Pronoun.IsOur);
        }

        [Test]
        public void IsSelf()
        {
            var definitions = manager.GetDefinitions("ME");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.IsSelf);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Pronoun.IsSelf);
        }

        [Test]
        public void IsYou()
        {
            var definitions = manager.GetDefinitions("THEE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.IsYou);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Pronoun.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Pronoun.IsYou);
        }

        [Test]
        public void IsIntrj()
        {
            var definitions = manager.GetDefinitions("UGH");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.IsIntrj);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.NegationInterjections.IsIntrj);
        }

        [Test]
        public void IsNegate()
        {
            var definitions = manager.GetDefinitions("UNABLE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.IsNegate);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.NegationInterjections.IsNegate);
        }
        
        [Test]
        public void IsNo()
        {
            var definitions = manager.GetDefinitions("WRONG");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.IsNo);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.NegationInterjections.IsNo);
        }

        [Test]
        public void IsYes()
        {
            var definitions = manager.GetDefinitions("YA");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.IsYes);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.NegationInterjections.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.NegationInterjections.IsYes);
        }

        [Test]
        public void DoesDescribeMentalState()
        {
            var definitions = manager.GetDefinitions("ABHOR");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Verb.DoesDescribeMentalState);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Verb.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Verb.DoesDescribeMentalState);
        }

        [Test]
        public void GivesInterpretativeExplanation()
        {
            var definitions = manager.GetDefinitions("ABIDE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Verb.GivesInterpretativeExplanation);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Verb.HasData);
            definitions = manager.GetDefinitions("KILL");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Verb.GivesInterpretativeExplanation);
        }

        [Test]
        public void IsStraightDescriptive()
        {
            var definitions = manager.GetDefinitions("ACCELERATE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Verb.IsStraightDescriptive);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Verb.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Verb.IsStraightDescriptive);
        }

        [Test]
        public void IsApartRelationship()
        {
            var definitions = manager.GetDefinitions("ACCOUNTABLE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Adjective.IsApartRelationship);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Adjective.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Adjective.IsApartRelationship);
        }

        [Test]
        public void IsRelationship()
        {
            var definitions = manager.GetDefinitions("ACCURSED");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Adjective.IsRelationship);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Adjective.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Adjective.IsRelationship);
        }

        [Test]
        public void IsArenas()
        {
            var definitions = manager.GetDefinitions("ANTARCTICA");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsArenas);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsArenas);
        }

        [Test]
        public void IsAuthoritative()
        {
            var definitions = manager.GetDefinitions("ADMINISTER");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsAuthoritative);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsAuthoritative);
        }

        [Test]
        public void IsAuthoritativeParticipant()
        {
            var definitions = manager.GetDefinitions("ADMINISTRATOR");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsAuthoritativeParticipant);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsAuthoritativeParticipant);
        }

        [Test]
        public void IsConflict()
        {
            var definitions = manager.GetDefinitions("ABRUPT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsConflict);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsConflict);
        }

        [Test]
        public void IsCooperation()
        {
            var definitions = manager.GetDefinitions("ACCOMMODATE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsCooperation);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsCooperation);
        }

        [Test]
        public void IsDoctrineL()
        {
            var definitions = manager.GetDefinitions("BOURGEOISIE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsDoctrine);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsDoctrine);
        }

        [Test]
        public void IsEnd()
        {
            var definitions = manager.GetDefinitions("AMBITION");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsEnd);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsEnd);
        }

        [Test]
        public void IsLoss()
        {
            var definitions = manager.GetDefinitions("ARREST");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsLoss);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsLoss);
        }

        [Test]
        public void IsOrdinaryParticipant()
        {
            var definitions = manager.GetDefinitions("ARBITRATOR");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsOrdinaryParticipant);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsOrdinaryParticipant);
        }

        [Test]
        public void IsResidual()
        {
            var definitions = manager.GetDefinitions("ABORTIVE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsResidual);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsResidual);
        }

        [Test]
        public void IsTotal()
        {
            var definitions = manager.GetDefinitions("ABORTIVE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Power.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Power.IsTotal);
        }

        [Test]
        public void IsEnds()
        {
            var definitions = manager.GetDefinitions("CREATOR");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.IsEnds);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Rectitude.IsEnds);
        }

        [Test]
        public void IsEthics()
        {
            var definitions = manager.GetDefinitions("ADHERE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.IsEthics);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Rectitude.IsEthics);
        }

        [Test]
        public void IsGain()
        {
            var definitions = manager.GetDefinitions("BREACH");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.IsGain);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Rectitude.IsGain);
        }

        [Test]
        public void IsRectitudeLoss()
        {
            var definitions = manager.GetDefinitions("DISHONOR");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.IsLoss);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Rectitude.IsLoss);
        }

        [Test]
        public void IsReligion()
        {
            var definitions = manager.GetDefinitions("BELIEVER");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.IsReligion);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Rectitude.IsReligion);
        }

        [Test]
        public void IsRectitudeTotal()
        {
            var definitions = manager.GetDefinitions("ADHERE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Rectitude.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Rectitude.IsTotal);
        }

        [Test]
        public void RespectIsGain()
        {
            var definitions = manager.GetDefinitions("ADMIRE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.IsGain);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Respect.IsGain);
        }

        [Test]
        public void RespectIsLoss()
        {
            var definitions = manager.GetDefinitions("ADMISSION");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.IsLoss);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Respect.IsLoss);
        }

        [Test]
        public void RespectIsOther()
        {
            var definitions = manager.GetDefinitions("ADMIRABLE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.IsOther);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Respect.IsOther);
        }

        [Test]
        public void RespectIsTotal()
        {
            var definitions = manager.GetDefinitions("ADMIRE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Respect.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Respect.IsTotal);
        }

        [Test]
        public void WealthIsOther()
        {
            var definitions = manager.GetDefinitions("ABUNDANCE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.IsOther);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Wealth.IsOther);
        }

        [Test]
        public void WealthIsParticipant()
        {
            var definitions = manager.GetDefinitions("ASSESSOR");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.IsParticipant);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Wealth.IsParticipant);
        }

        [Test]
        public void WealthIsTotal()
        {
            var definitions = manager.GetDefinitions("ABUNDANCE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Wealth.IsTotal);
        }

        [Test]
        public void WealthIsTransaction()
        {
            var definitions = manager.GetDefinitions("AUCTION");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.IsTransaction);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Wealth.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Wealth.IsTransaction);
        }

        [Test]
        public void EnlightenmentIsEnds()
        {
            var definitions = manager.GetDefinitions("CONTEMPLATE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.IsEnds);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Enlightenment.IsEnds);
        }

        [Test]
        public void EnlightenmentIsGain()
        {
            var definitions = manager.GetDefinitions("ACQUAINT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.IsGain);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Enlightenment.IsGain);
        }

        [Test]
        public void EnlightenmentIsLoss()
        {
            var definitions = manager.GetDefinitions("BAFFLE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.IsLoss);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Enlightenment.IsLoss);
        }

        [Test]
        public void EnlightenmentIsOther()
        {
            var definitions = manager.GetDefinitions("ABSTRACT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.IsOther);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Enlightenment.IsOther);
        }

        [Test]
        public void EnlightenmentIsParticipant()
        {
            var definitions = manager.GetDefinitions("ADVISER");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.IsParticipant);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Enlightenment.IsParticipant);
        }

        [Test]
        public void EnlightenmentIsTotal()
        {
            var definitions = manager.GetDefinitions("ABSTRACT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Enlightenment.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Enlightenment.IsTotal);
        }

        [Test]
        public void SkillIsTotal()
        {
            var definitions = manager.GetDefinitions("AESTHETIC");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Skill.IsTotal);
        }

        [Test]
        public void SkillIsAesthetic()
        {
            var definitions = manager.GetDefinitions("AESTHETIC");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.IsAesthetic);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Skill.IsAesthetic);
        }

        [Test]
        public void SkillIsOther()
        {
            var definitions = manager.GetDefinitions("ADEPT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.IsOther);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Skill.IsOther);
        }

        [Test]
        public void SkillIsParticipant()
        {
            var definitions = manager.GetDefinitions("AMATEUR");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.IsParticipant);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Skill.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Skill.IsParticipant);
        }

        [Test]
        public void RemainingIsActor()
        {
            var definitions = manager.GetDefinitions("ATTENDANT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsActor);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsActor);
        }

        [Test]
        public void RemainingIsAnomie()
        {
            var definitions = manager.GetDefinitions("AIMLESS");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsAnomie);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsAnomie);
        }

        [Test]
        public void RemainingIsArena()
        {
            var definitions = manager.GetDefinitions("ATMOSPHERE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsArena);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsArena);
        }


        [Test]
        public void RemainingIsEnds()
        {
            var definitions = manager.GetDefinitions("ACCELERATE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsEnds);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsEnds);
        }

        [Test]
        public void RemainingIsFormat()
        {
            var definitions = manager.GetDefinitions("ADMISSIBLE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsFormat);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsFormat);
        }

        [Test]
        public void RemainingIsIf()
        {
            var definitions = manager.GetDefinitions("ALMOST");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsIf);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsIf);
        }

        [Test]
        public void RemainingIsMeans()
        {
            var definitions = manager.GetDefinitions("ABILITY");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsMeans);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsMeans);
        }

        [Test]
        public void RemainingIsNation()
        {
            var definitions = manager.GetDefinitions("ABYSSINIA");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsNation);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsNation);
        }

        [Test]
        public void RemainingIsNegativeAffect()
        {
            var definitions = manager.GetDefinitions("ABNORMAL");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsNegativeAffect);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsNegativeAffect);
        }

        [Test]
        public void RemainingIsNot()
        {
            var definitions = manager.GetDefinitions("CANNOT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsNot);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsNot);
        }

        [Test]
        public void RemainingIsPositiveAffect()
        {
            var definitions = manager.GetDefinitions("ACCEPT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsPositiveAffect);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsPositiveAffect);
        }

        [Test]
        public void RemainingIsSure()
        {
            var definitions = manager.GetDefinitions("ACTUALITY");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsSure);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsSure);
        }

        [Test]
        public void RemainingIsTimeSpace()
        {
            var definitions = manager.GetDefinitions("ACCELERATION");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsTimeSpace);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsTimeSpace);
        }

        [Test]
        public void RemainingIsTransaction()
        {
            var definitions = manager.GetDefinitions("ABOLITION");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsTransaction);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsTransaction);
        }

        [Test]
        public void RemainingIsTransactionGain()
        {
            var definitions = manager.GetDefinitions("ACQUISITION");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsTransactionGain);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsTransactionGain);
        }

        [Test]
        public void RemainingIsTransactionLoss()
        {
            var definitions = manager.GetDefinitions("ACCIDENT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.IsTransactionLoss);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Remaining.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Remaining.IsTransactionLoss);
        }

        [Test]
        public void BroadIsEvaluation()
        {
            var definitions = manager.GetDefinitions("CONSTRUCTIVE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.IsEvaluation);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.Broad.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Harward.Broad.IsEvaluation);
        }

        [Test]
        public void WellBeingIsGain()
        {
            var definitions = manager.GetDefinitions("AMELIORATION");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.IsGain);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.WellBeing.IsGain);
        }

        [Test]
        public void WellBeingIsLoss()
        {
            var definitions = manager.GetDefinitions("CALAMITY");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.IsLoss);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.WellBeing.IsLoss);
        }

        [Test]
        public void WellBeingIsPsychological()
        {
            var definitions = manager.GetDefinitions("COMPULSIVE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.IsPsychological);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.WellBeing.IsPsychological);
        }

        [Test]
        public void WellBeingIsPhysicalAspect()
        {
            var definitions = manager.GetDefinitions("CANCER");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.IsPhysicalAspect);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.WellBeing.IsPhysicalAspect);
        }

        [Test]
        public void WellBeingIsRole()
        {
            var definitions = manager.GetDefinitions("CHILD");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.IsRole);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.WellBeing.IsRole);
        }

        [Test]
        public void WellBeingIsTotal()
        {
            var definitions = manager.GetDefinitions("CHILD");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.WellBeing.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.WellBeing.IsTotal);
        }

        [Test]
        public void AffectionsGain()
        {
            var definitions = manager.GetDefinitions("MERCY");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.IsGain);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Affection.IsGain);
        }

        [Test]
        public void AffectionIsLoss()
        {
            var definitions = manager.GetDefinitions("ABANDON");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.IsLoss);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Affection.IsLoss);
        }

        [Test]
        public void AffectionIsOther()
        {
            var definitions = manager.GetDefinitions("ACQUAINTANCE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.IsOther);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Affection.IsOther);
        }
      
        [Test]
        public void AffectionIsTotal()
        {
            var definitions = manager.GetDefinitions("ACQUAINTANCE");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.IsTotal);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Affection.IsTotal);
        }

        [Test]
        public void AffectionIsParticipant()
        {
            var definitions = manager.GetDefinitions("AUNT");
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.IsParticipant);
            Assert.AreEqual(true, definitions.Records[0].Description.Lasswell.Affection.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Lasswell.Affection.IsParticipant);
        }

        [Test]
        public void CognitiveOrientationIsNumber()
        {
            var definitions = manager.GetDefinitions("ONE");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsNumber);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsNumber);
        }

        [Test]
        public void CognitiveOrientationIsCardinal()
        {
            var definitions = manager.GetDefinitions("BILLION");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsCardinal);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsCardinal);
        }

        [Test]
        public void CognitiveOrientationIsOrdinal()
        {
            var definitions = manager.GetDefinitions("EIGHTEENTH");
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.IsOrdinal);
            Assert.AreEqual(true, definitions.Records[0].Description.Harward.CognitiveOrientation.HasData);
            definitions = manager.GetDefinitions("ABIDE");
            Assert.IsFalse(definitions.Records[0].Description.Harward.CognitiveOrientation.IsOrdinal);
        }
    }
}
