using NodaTime;
using NodaTime.Fields;
using NUnit.Framework;
using Wikiled.Sentiment.TestLogic.Shared.Helpers;
using Wikiled.SUTime.Time;
using Wikiled.Sentiment.Text.NLP.Temporal;

namespace Wikiled.Sentiment.Text.Tests.NLP.Temporal
{
    [TestFixture]
    public class TimeBucketTests
    {
        [Test]
        public void Constructor()
        {
            var timeBucket = new TimeBucket();
            Assert.IsNull(timeBucket.Granuality);
            Assert.AreEqual(DateTimeFieldType.Year, timeBucket.ZoomGranuality);
            Assert.AreEqual(Granularity.Root, timeBucket.GranularityLevel);
        }

        [Test]
        public void Parse()
        {
            var timeBucket = new TimeBucket();
            timeBucket.AddWord(new TestWordItem(), new IsoDateTimeItem(new LocalDateTime(1978, 2, 23, 4, 30)));
            timeBucket.AddWord(new TestWordItem(), new IsoDateTimeItem(new LocalDateTime(1978, 3, 23, 4, 30)));
            timeBucket.AddWord(new TestWordItem(), new IsoDateTimeItem(new LocalDateTime(1979, 2, 23, 4, 30)));
            timeBucket.Parse();

            CheckBucked(timeBucket, 1978, 1979, 0, null, DateTimeFieldType.Year, 4);
            CheckBucked(timeBucket.Zoomed[1], 2, 3, 1978, DateTimeFieldType.Year, DateTimeFieldType.MonthOfYear, 12);
            CheckBucked(timeBucket.Zoomed[1].Zoomed[1], 23, 23, 2, DateTimeFieldType.MonthOfYear, DateTimeFieldType.DayOfMonth, 31);
            CheckBucked(timeBucket.Zoomed[1].Zoomed[1].Zoomed[22], 4, 4, 23, DateTimeFieldType.DayOfMonth, DateTimeFieldType.HourOfDay, 24);
            CheckBucked(timeBucket.Zoomed[1].Zoomed[1].Zoomed[22].Zoomed[4], 30, 30, 4, DateTimeFieldType.HourOfDay, DateTimeFieldType.MinuteOfHour, 60);

            CheckBucked(timeBucket.Zoomed[2], 2, 2, 1979, DateTimeFieldType.Year, DateTimeFieldType.MonthOfYear, 12);
            CheckBucked(timeBucket.Zoomed[2].Zoomed[1], 23, 23, 2, DateTimeFieldType.MonthOfYear, DateTimeFieldType.DayOfMonth, 31);
            CheckBucked(timeBucket.Zoomed[2].Zoomed[1].Zoomed[22], 4, 4, 23, DateTimeFieldType.DayOfMonth, DateTimeFieldType.HourOfDay, 24);
            CheckBucked(timeBucket.Zoomed[2].Zoomed[1].Zoomed[22].Zoomed[4], 30, 30, 4, DateTimeFieldType.HourOfDay, DateTimeFieldType.MinuteOfHour, 60);
        }

        public void CheckBucked(TimeBucket bucket, int min, int max, int value, DateTimeFieldType granuality, DateTimeFieldType zoom, int totalZoom)
        {
            Assert.AreEqual(value, bucket.Value);
            Assert.AreEqual(totalZoom, bucket.Zoomed.Length);
            Assert.AreEqual(granuality, bucket.Granuality);
            Assert.AreEqual(zoom, bucket.ZoomGranuality);
            Assert.AreEqual(max, bucket.MaximumValue);
            Assert.AreEqual(min, bucket.MinimumValue);
        }
    }
}
