using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Wikiled.Sentiment.Analysis.Workspace;
using Wikiled.Sentiment.Analysis.Workspace.Configuration;
using Wikiled.Sentiment.Analysis.Workspace.Data;

namespace Wikiled.Sentiment.Analysis.Tests.Workspace
{
    [TestFixture]
    public class WorkspaceInstanceFactoryTests
    {
        private MockRepository mock;
        private IWorkspaceInstance instance;
        private WorkspaceConfiguration configuration;

        [SetUp]
        public void Setup()
        {
            mock = new MockRepository();
            instance = mock.StrictMock<IWorkspaceInstance>();
            configuration = new WorkspaceConfiguration();
        }

        [Test]
        public void Create()
        {
            string assignedName = null;
            string assignedPath = null;
            Expect.Call(instance.Init);
            Expect.Call(instance.SaveConfiguration);
            Expect.Call(instance.Configuration)
                .Repeat.Any()
                .Return(configuration);
            mock.ReplayAll();
            WorkspaceInstanceFactory factory = new WorkspaceInstanceFactory(
                ProjectType.SVMBagOfWords, 
                (name, path) =>
                    {
                        assignedName = name;
                        assignedPath = path;
                        return instance;
                    });

            var createInstance = factory.Create(DataSourceType.PositiveNegative, "TestName", ".");
            Assert.AreEqual(instance, createInstance);
            Assert.AreEqual(DataSourceType.PositiveNegative, configuration.DataSourceType);
            Assert.AreEqual(ProjectType.SVMBagOfWords, configuration.ProjectType);
            Assert.AreEqual("TestName", configuration.Name);
            Assert.AreEqual("TestName", assignedName);
            Assert.AreEqual(@".\TestName", assignedPath);
            Assert.IsTrue(Directory.Exists("TestName"));
            mock.VerifyAll();
        }

        [Test]
        public void Open()
        {
            string assignedName = null;
            string assignedPath = null;
            Expect.Call(instance.Init);
            Expect.Call(instance.LoadConfiguration);
            Expect.Call(instance.Configuration)
                  .Repeat.Any()
                  .Return(configuration);
            mock.ReplayAll();
            WorkspaceInstanceFactory factory = new WorkspaceInstanceFactory(
                ProjectType.SVMBagOfWords,
                (name, path) =>
                {
                    assignedName = name;
                    assignedPath = path;
                    return instance;
                });

            var createInstance = factory.Open(@".\TestName\");
            Assert.AreEqual(instance, createInstance);
            Assert.AreEqual(DataSourceType.PositiveNegative, configuration.DataSourceType);
            Assert.AreEqual(ProjectType.SVMBagOfWords, configuration.ProjectType);
            Assert.AreEqual("TestName", assignedName);
            Assert.AreEqual(@".\TestName\", assignedPath);
            mock.VerifyAll();
        }
    }
}
