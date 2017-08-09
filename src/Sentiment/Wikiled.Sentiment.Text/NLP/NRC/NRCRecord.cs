using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Wikiled.Sentiment.Text.NLP.NRC
{
    [XmlRoot("NRC")]
    public class NRCRecord : ICloneable
    {
        public IEnumerable<SentimentCategory> GetDefinedCategories()
        {
            return Enum.GetValues(typeof(SentimentCategory)).Cast<SentimentCategory>().Where(HasValue);
        }

        public void Invert()
        {
            if (IsJoy)
            {
                IsJoy = false;
                IsSadness = true;
            }
            else if (IsSadness)
            {
                IsJoy = true;
                IsSadness = false;
            }

            if (IsAnger)
            {
                IsAnger = false;
                IsFear = true;
            }
            else if (IsFear)
            {
                IsAnger = true;
                IsFear = false;
            }

            if (IsTrust)
            {
                IsTrust = false;
                IsDisgust = true;
            }
            else if (IsDisgust)
            {
                IsTrust = true;
                IsDisgust = false;
            }

            if (IsAnticipation)
            {
                IsAnticipation = false;
                IsSurprise = true;
            }
            else if (IsSurprise)
            {
                IsAnticipation = true;
                IsSurprise = false;
            }

            if (IsPositive)
            {
                IsPositive = false;
                IsNegative = true;
            }
            else if (IsNegative)
            {
                IsPositive = true;
                IsNegative = false;
            }
        }

        public bool HasValue(SentimentCategory category)
        {
            switch (category)
            {
                case SentimentCategory.Anger:
                    return IsAnger;
                case SentimentCategory.Anticipation:
                    return IsAnticipation;
                case SentimentCategory.Disgust:
                    return IsDisgust;
                case SentimentCategory.Fear:
                    return IsFear;
                case SentimentCategory.Joy:
                    return IsJoy;
                case SentimentCategory.Sadness:
                    return IsSadness;
                case SentimentCategory.Surprise:
                    return IsSurprise;
                case SentimentCategory.Trust:
                    return IsTrust;
                case SentimentCategory.None:
                    return !HasAnyValue;
                default:
                    throw new ArgumentOutOfRangeException("category");
            }
        }

        public object Clone()
        {
            NRCRecord record = new NRCRecord();
            record.IsAnger = IsAnger;
            record.IsAnticipation = IsAnticipation;
            record.IsDisgust = IsDisgust;
            record.IsFear = IsFear;
            record.IsJoy = IsJoy;
            record.IsNegative = IsNegative;
            record.IsPositive = IsPositive;
            record.IsSadness = IsSadness;
            record.IsSurprise = IsSurprise;
            record.IsTrust = IsTrust;
            return record;
        }

        public bool HasAnyValue => IsAnger ||
                                   IsAnticipation ||
                                   IsDisgust ||
                                   IsFear ||
                                   IsJoy ||
                                   IsNegative ||
                                   IsPositive ||
                                   IsSadness ||
                                   IsSurprise ||
                                   IsTrust;

        [XmlAttribute("Anger"), DefaultValue(false)]
        public bool IsAnger { get; set; }

        [XmlAttribute("Anticipation"), DefaultValue(false)]
        public bool IsAnticipation { get; set; }

        [XmlAttribute("Disgust"), DefaultValue(false)]
        public bool IsDisgust { get; set; }

        [XmlAttribute("Fear"), DefaultValue(false)]
        public bool IsFear { get; set; }

        [XmlAttribute("Joy"), DefaultValue(false)]
        public bool IsJoy { get; set; }

        [XmlAttribute("Negative"), DefaultValue(false)]
        public bool IsNegative { get; set; }

        [XmlAttribute("Positive"), DefaultValue(false)]
        public bool IsPositive { get; set; }

        [XmlAttribute("Sadness"), DefaultValue(false)]
        public bool IsSadness { get; set; }

        [XmlAttribute("Surprise"), DefaultValue(false)]
        public bool IsSurprise { get; set; }

        [XmlAttribute("Trust"), DefaultValue(false)]
        public bool IsTrust { get; set; }
    }
}
