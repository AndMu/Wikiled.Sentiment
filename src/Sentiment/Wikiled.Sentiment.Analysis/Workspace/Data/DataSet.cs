using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Analysis.Workspace.Data.Selection;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    internal class DataSet : IDataSet
    {
        public event EventHandler<ModificationEventArgs> Changed;

        private readonly List<IDataCollection> collections = new List<IDataCollection>();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, IFileSelection> pSentiFiles =
            new Dictionary<string, IFileSelection>(StringComparer.OrdinalIgnoreCase);

        private bool isInitInProgress;

        private bool suspendEvent;

        public DataSet(bool isTesting, DataConfiguration dataConfiguration)
        {
            Guard.NotNull(() => dataConfiguration, dataConfiguration);
            DataConfiguration = dataConfiguration;
            log.Debug("Creating DataRow testing:<{0}> Type:<{1}>", isTesting, DataConfiguration.Type);
            IsTesting = isTesting;
            Init();
        }

        public IEnumerable<IDataCollection> Collections => collections;

        public IEnumerable<SingleProcessingData> Data
        {
            get
            {
                return Collections.SelectMany(dataCollection => dataCollection.Items);
            }
        }

        public DataConfiguration DataConfiguration { get; }

        public bool IsReady
        {
            get
            {
                if (IsTesting)
                {
                    return collections.Count > 1 &&
                           collections.Any(item => item.Items.Count > 0);
                }

                return collections.Count > 1 &&
                       collections.All(item => item.Items.Count >= 10);
            }
        }

        public bool IsTesting { get; }

        public void AddData(IProcessingData data)
        {
            var positive = Collections.FirstOrDefault(item => item.PositivityType == PositivityType.Positive);
            var negative = Collections.FirstOrDefault(item => item.PositivityType == PositivityType.Negative);
            var neutral = Collections.FirstOrDefault(item => item.PositivityType == PositivityType.Neutral);

            AddItem(positive, data.Positive);
            AddItem(negative, data.Negative);
            AddItem(neutral, data.Neutral);
        }

        public void AddPFile(IFileSelection file)
        {
            log.Debug("Adding PSenti file: {0}", file);
            if (pSentiFiles.ContainsKey(file.Configuration.File))
            {
                log.Debug("File is already added: {0}", file);
                return;
            }

            pSentiFiles[file.Configuration.File] = file;
            AddData(file.Data);

            if (!isInitInProgress)
            {
                DataConfiguration.PSentiSources.Add(file.Configuration);
            }

            SingleChanged(this, new ModificationEventArgs(Modification.AddedItems));
            ForceRefresh();
        }

        public TrainingTestingData ContructData()
        {
            var data = new TrainingTestingData();
            foreach (var dataCollection in Collections)
            {
                foreach (var item in dataCollection.Items)
                {
                    if (IsTesting)
                    {
                        data.Testing.Add(dataCollection.PositivityType, item);
                    }
                    else
                    {
                        data.Training.Add(dataCollection.PositivityType, item);
                    }
                }
            }

            return data;
        }

        public void ForceRefresh()
        {
            log.Debug("ForceRefresh");
            foreach (var dataCollection in collections)
            {
                dataCollection.ForceRefresh();
            }
        }

        public void LoadDocuments(string path)
        {
            log.Debug("LoadDocuments: {0}", path);
            if (!Directory.Exists(path))
            {
                log.Warn("Path: {0} doesn't exist", path);
                return;
            }

            string resultsPath = Path.Combine(path, "results.xml");
            Document[] documents;

            try
            {
                documents = XDocument
                    .Load(resultsPath)
                    .XmlDeserialize<Document[]>();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (File.Exists(resultsPath))
                {
                    log.Debug("Removing file as it is corupted: {0}", resultsPath);
                    File.Delete(resultsPath);
                }

                return;
            }

            var lookup = (from collection in collections
                          from item in collection.Items
                          where !string.IsNullOrEmpty(item.Text)
                          select item).ToLookup(item => item.Text.Trim().SanitizeXmlString(), StringComparer.OrdinalIgnoreCase);

            foreach (var document in documents)
            {
                if (string.IsNullOrEmpty(document.Text))
                {
                    continue;
                }

                var data = lookup[document.Text.Trim().SanitizeXmlString()].ToArray();
                if (data.Length == 0)
                {
                    log.Debug("Data not found");
                }

                foreach (var singleProcessingData in data)
                {
                    singleProcessingData.Document = document;
                }
            }

            ForceRefresh();
        }

        private void AddItem(IDataCollection collection, SingleProcessingData[] data)
        {
            if (collection == null)
            {
                log.Debug("Ignore AddItem - collection is null");
                return;
            }

            if (data == null ||
                data.Length == 0)
            {
                log.Debug("Ignore AddItem - no data");
                return;
            }

            log.Debug("AddItem - {0} items", data.Length);
            foreach (var item in data)
            {
                collection.Items.Add(item);
            }
        }

        private void Init()
        {
            log.Debug("Init");
            isInitInProgress = true;
            foreach (var dataCollection in DataConfiguration.Configurations)
            {
                var single = new SingleDataCollection(dataCollection);
                single.Changed += SingleChanged;
                collections.Add(single);
            }

            foreach (var file in DataConfiguration.PSentiSources)
            {
                AddPFile(file.Construct());
            }

            isInitInProgress = false;
        }

        private void SingleChanged(object sender, ModificationEventArgs e)
        {
            if (suspendEvent)
            {
                return;
            }

            // in current implementation on any clear action we will remove all items
            // improve later to handle only what is requested.

            if (e.Modification == Modification.Clear)
            {
                suspendEvent = true;
                pSentiFiles.Clear();
                DataConfiguration.PSentiSources.Clear();
                foreach (var dataCollection in collections.Where(item => item != sender))
                {
                    dataCollection.Clear();
                }

                suspendEvent = false;
            }

            if (Changed != null)
            {
                Changed(this, e);
            }
        }
    }
}
