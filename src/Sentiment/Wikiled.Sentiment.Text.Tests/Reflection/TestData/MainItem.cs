using System.Collections.Generic;
using Wikiled.Sentiment.Text.Reflection;

namespace Wikiled.Sentiment.Text.Tests.Reflection.TestData
{
    [InfoCategory("Main")]
    public class MainItem
    {
        public MainItem()
        {
            Data = new Dictionary<string, int>();
            SubCatHidden = new ThirdMainItem();
            SubCat = new AnotherMainItem();
        }

        [InfoCategory("Child Cat")]
        public AnotherMainItem SubCat { get; private set; }

        [InfoCategory("Hidden", IsCollapsed = true)]
        public ThirdMainItem SubCatHidden { get; set; }

        [InfoField("Total")]
        public int Total { get; set; }

        [InfoField("IsGood", Description = "Is it really?")]
        public bool IsGood { get; set; }

        [InfoArrayCategory("Data", "Key", "Value")]
        public Dictionary<string, int> Data { get; private set; }
    }
}
