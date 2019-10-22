using System;
using System.Collections.Generic;

namespace Wikiled.Text.Analysis.Structure.Light
{
    public class LightDocument
    {
        public List<LightSentence> Sentences { get; set; } = new List<LightSentence>();

        public string Text { get; set; }

        public string Author { get; set; }

        public string Id { get; set; }

        public DateTime? DocumentTime { get; set; }

        public string Title { get; set; }
    }
}
