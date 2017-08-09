using System.IO;
using System.Linq;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Workspace;

namespace Wikiled.Sentiment.Analysis.Tests.Workspace
{
    [TestFixture]
    public class WorkspaceInstanceTests
    {
        [Test]
        public void Construct()
        {
            WorkspaceInstance instance = new WorkspaceInstance("Name", "Path", () => null);
            Assert.AreEqual("Name", instance.Configuration.Name);
            Assert.AreEqual("Path", instance.ProjectPath);
        }

        [Test]
        public void LoadSave()
        {
            WorkspaceInstance instance = new WorkspaceInstance("Name", ".", () => null);
            instance.Init();
            instance.SaveConfiguration();
            Assert.IsTrue(File.Exists(Path.Combine(instance.ProjectPath, "configuration.xml")));
            instance.LoadConfiguration();
            Assert.AreEqual("Name", instance.Configuration.Name);
            Assert.AreEqual(".", instance.ProjectPath);
        }

        [Test]
        public void ItemChangedChanged()
        {
            WorkspaceInstance instance = new WorkspaceInstance("Name", TestContext.CurrentContext.TestDirectory, () => null);
            instance.Init();
            string trainingPath = Path.Combine(instance.ProjectPath, "trainingData.xml");
            string testingPath = Path.Combine(instance.ProjectPath, "testingData.xml");
            if (File.Exists(trainingPath))
            {
                File.Delete(trainingPath);
            }
            if (File.Exists(testingPath))
            {
                File.Delete(testingPath);
            }

            instance.TrainingData.Collections.First().AddFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData\1.txt"));
            Assert.IsTrue(File.Exists(trainingPath));
            Assert.IsFalse(File.Exists(testingPath));
            instance.TestingData.Collections.First().AddFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Workspace\Data\TestData\1.txt"));
            Assert.IsTrue(File.Exists(trainingPath));
            Assert.IsTrue(File.Exists(testingPath));
        }
    }
}
