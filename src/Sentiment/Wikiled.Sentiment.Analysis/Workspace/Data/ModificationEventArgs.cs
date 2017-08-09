using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    public class ModificationEventArgs : EventArgs
    {
        public ModificationEventArgs(Modification modification)
        {
            Modification = modification;
        }

        public Modification Modification { get; private set; }
    }
}
