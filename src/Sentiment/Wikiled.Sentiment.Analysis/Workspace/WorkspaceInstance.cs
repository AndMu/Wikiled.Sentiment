using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Workspace.Aspects;
using Wikiled.Sentiment.Analysis.Workspace.Configuration;
using Wikiled.Sentiment.Analysis.Workspace.Data;
using Wikiled.Sentiment.Text.Parser;

namespace Wikiled.Sentiment.Analysis.Workspace
{
    public class WorkspaceInstance : IWorkspaceInstance
    {
        private const string CrossValidationFolder = "Cross";

        private const string SvmFolder = "SVM";

        private const string TestingFolder = "Testing";

        private readonly string configurationPath;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly List<ISpecificWorkspaceInstance> specificInstances = new List<ISpecificWorkspaceInstance>();

        private readonly object syncRoot = new object();

        private readonly string testingPath;

        private readonly string trainingPath;

        private readonly Func<IWordsHandler> wordsHandlerSource;

        private IDataSet testingData;

        private IDataSet trainingData;

        public WorkspaceInstance(string name, string path, Func<IWordsHandler> wordsHandlerSource)
        {
            Guard.NotNull(() => wordsHandlerSource, wordsHandlerSource);
            Guard.NotNullOrEmpty(() => path, path);
            Guard.NotNullOrEmpty(() => name, name);
            this.wordsHandlerSource = wordsHandlerSource;
            log.Debug("WorkspaceInstance: {0} <{1}>", name, path);
            ProjectPath = path;
            Configuration = new WorkspaceConfiguration();
            Configuration.Name = name;
            configurationPath = Path.Combine(ProjectPath, "configuration.xml");
            trainingPath = Path.Combine(ProjectPath, "trainingData.xml");
            testingPath = Path.Combine(ProjectPath, "testingData.xml");
            AspectsHandler = new AspectsHandler(Path.Combine(ProjectPath, "features.xml"), wordsHandlerSource);
        }

        public virtual string DataPath => Path.Combine(ProjectPath, SvmFolder);

        public IAspectsHandler AspectsHandler { get; }

        public WorkspaceConfiguration Configuration { get; private set; }

        public string CrossValidationDataPath => Path.Combine(ProjectPath, CrossValidationFolder);

        public bool IsInit { get; private set; }

        public string ProjectPath { get; }

        public IDataSet TestingData
        {
            get
            {
                return testingData;
            }
            private set
            {
                if (testingData != null)
                {
                    testingData.Changed -= TestingDataChanged;
                }

                testingData = value;
                testingData.Changed += TestingDataChanged;
            }
        }

        public string TestingDataPath => Path.Combine(ProjectPath, TestingFolder);

        public IDataSet TrainingData
        {
            get
            {
                return trainingData;
            }
            private set
            {
                if (trainingData != null)
                {
                    trainingData.Changed -= TrainingDataChanged;
                }

                trainingData = value;
                trainingData.Changed += TrainingDataChanged;
            }
        }

        public IWordsHandler WordsHandler => wordsHandlerSource();

        public void Init()
        {
            IsInit = true;
            TrainingData = new DataSet(false, Create());
            TestingData = new DataSet(true, Create());
        }

        public void LoadConfiguration()
        {
            Guard.IsValid(() => IsInit, IsInit, item => item, "Workspace is not init");
            log.Debug("LoadConfiguration: <{0}>", configurationPath);
            var document = XDocument.Load(configurationPath);
            Configuration = document.XmlDeserialize<WorkspaceConfiguration>();
            LoadTraining(trainingPath);
            LoadTesting(testingPath);
        }

        public void LoadTesting(string path)
        {
            log.Debug("LoadTesting: <{0}>", path);
            var data = GetData(path, true);
            if (data != null)
            {
                TestingData = data;
            }

            TestingData.LoadDocuments(TestingDataPath);
        }

        public void LoadTraining(string path)
        {
            log.Debug("LoadTraining: <{0}>", path);
            var data = GetData(path, false);
            if (data != null)
            {
                if (data.DataConfiguration.Type != Configuration.DataSourceType)
                {
                    throw new WorkspaceException("Data source is different from expected in workspace");
                }

                TrainingData = data;
            }

            if (Directory.Exists(CrossValidationDataPath))
            {
                log.Debug("Found cross validation folder. Loading...");
                foreach (var directory in Directory.GetDirectories(CrossValidationDataPath))
                {
                    TrainingData.LoadDocuments(Path.Combine(directory, TestingFolder));
                }
            }
            else
            {
                log.Debug("Cross validation folder not found {0}", CrossValidationDataPath);
            }
        }

        public void Register(ISpecificWorkspaceInstance specificWorkspace)
        {
            Guard.IsValid(() => IsInit, IsInit, item => item, "Workspace is not init");
            Guard.NotNull(() => specificWorkspace, specificWorkspace);
            specificInstances.Add(specificWorkspace);
            specificWorkspace.LoadConfiguration();
        }

        public void SaveConfiguration()
        {
            Guard.IsValid(() => IsInit, IsInit, item => item, "Workspace is not init");
            log.Debug("SaveConfiguration: <{0}>", configurationPath);
            Configuration.XmlSerialize().Save(configurationPath);
            TrainingData.DataConfiguration.XmlSerialize().Save(trainingPath);
            TestingData.DataConfiguration.XmlSerialize().Save(testingPath);
            foreach (var specificWorkspaceInstance in specificInstances)
            {
                specificWorkspaceInstance.SaveConfiguration();
            }
        }

        private DataConfiguration Create()
        {
            log.Debug("Create");
            var data = new DataConfiguration();
            data.Type = Configuration.DataSourceType;
            if (Configuration.DataSourceType == DataSourceType.PositiveNegative ||
               Configuration.DataSourceType == DataSourceType.PositiveNeutralNegative)
            {
                data.Configurations.Add(
                    new ItemConfiguration
                    {
                        Name = "Positive",
                        Stars = 5
                    });

                if (Configuration.DataSourceType == DataSourceType.PositiveNeutralNegative)
                {
                    data.Configurations.Add(
                        new ItemConfiguration
                        {
                            Name = "Neutral",
                            Stars = 3
                        });
                }

                data.Configurations.Add(
                    new ItemConfiguration
                    {
                        Name = "Negative",
                        Stars = 1
                    });
            }

            return data;
        }

        private DataSet GetData(string path, bool isTesting)
        {
            log.Debug("GetData: <{0}> ({1})", path, isTesting);
            if (!File.Exists(path))
            {
                log.Warn("GetData Path not found: <{0}>", path);
                return null;
            }

            XDocument document = XDocument.Load(path);
            var configuration = document.XmlDeserialize<DataConfiguration>();
            return new DataSet(isTesting, configuration);
        }

        public void TestingDataChanged(object sender, ModificationEventArgs e)
        {
            log.Debug("testingData_Changed: <{0}>", trainingPath);
            if (e.Modification == Modification.Forced ||
               e.Modification == Modification.CollectionChanged)
            {
                return;
            }

            lock (syncRoot)
            {
                TestingData.DataConfiguration.XmlSerialize().Save(testingPath);
            }
        }

        private void TrainingDataChanged(object sender, ModificationEventArgs e)
        {
            log.Debug("trainingData_Changed: <{0}>", trainingPath);
            if (e.Modification == Modification.Forced ||
               e.Modification == Modification.CollectionChanged)
            {
                return;
            }

            lock (syncRoot)
            {
                TrainingData.DataConfiguration.XmlSerialize().Save(trainingPath);
            }
        }
    }
}
