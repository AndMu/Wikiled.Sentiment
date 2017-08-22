using System;
using System.Runtime.Serialization;
using Wikiled.Sentiment.Text.Words;

namespace Wikiled.Sentiment.Text.Data
{
    [DataContract]
    public class NGramBlock
    {
        private string posMask;
        private string wordMask;

        public NGramBlock()
        {
        }

        public NGramBlock(IWordItem[] wordOccurrences)
        {
            if (wordOccurrences == null)
            {
                throw new ArgumentNullException("wordOccurrences");
            }

            WordOccurrences = wordOccurrences;
        }

        private void PopuplateMasks()
        {
            if (!string.IsNullOrEmpty(posMask))
            {
                return;
            }

            for (int i = 0; i < WordOccurrences.Length; i++)
            {
                if (i > 0)
                {
                    posMask += " ";
                    wordMask += " ";
                }

                posMask += WordOccurrences[i].POS.Tag;
                wordMask += WordOccurrences[i].Text;
            }
        }

        [DataMember]
        public IWordItem[] WordOccurrences { get; set; }

        [DataMember]
        public string PosMask
        {
            get
            {
                PopuplateMasks();
                return posMask;
            }
            set => posMask = value;
        }

        [DataMember]
        public string WordMask
        {
            get
            {
                PopuplateMasks();
                return wordMask;
            }
            set => wordMask = value;
        }
    }
}