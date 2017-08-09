using System.IO;
using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Svm.Extensions;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    public class ArffSort : BaseArffCommand
    {
        protected override void Process(string file, IArffDataSet arff)
        {
            var fileOut = Path.Combine(Out, Path.GetFileName(file));
            arff.SaveSorted(fileOut);
        }
    }
}
