using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using NLog;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Mathematics;
using Wikiled.Sentiment.Text.Data.Review;
using Wikiled.Sentiment.Text.Extensions;

namespace Wikiled.Sentiment.Analysis.Workspace.Data
{
    internal class SingleDataCollection : IDataCollection
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, bool> addedFiles = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        
        private bool isReady;

        private PrecisionRecallCalculator<PositivityType> positiveNegativeStats = new PrecisionRecallCalculator<PositivityType>();

        private bool suspendEvents;

        public SingleDataCollection(ItemConfiguration configuration)
        {
            Guard.NotNull(() => configuration, configuration);
            Configuration = configuration;
            Items = new ObservableCollection<SingleProcessingData>();
            Items.CollectionChanged += Items_CollectionChanged;
            Init();
        }

        public event EventHandler<ModificationEventArgs> Changed;

        public double Accuracy => positiveNegativeStats.GetAccuracy(PositivityType);

        public ItemConfiguration Configuration { get; }

        public double F1 => positiveNegativeStats.F1(PositivityType);

        public ObservableCollection<SingleProcessingData> Items { get; }

        public string Name => Configuration.Name;

        public PositivityType PositivityType
        {
            get
            {
                if (Stars > 3.5)
                {
                    return PositivityType.Positive;
                }

                if (Stars < 2.5)
                {
                    return PositivityType.Negative;
                }

                return PositivityType.Neutral;
            }
        }

        public double Precision => positiveNegativeStats.GetPrecision(PositivityType);

        public double Recall => positiveNegativeStats.GetRecall(PositivityType);

        public double Stars => Configuration.Stars;

        public void AddFiles(params string[] files)
        {
            Guard.NotNull(() => files, files);
            log.Debug("AddFiles: <{0}>", files.Length);
            if (files.Length == 0)
            {
                return;
            }

            files = AddFileInternal(files);
            if (files.Length > 0 &&
                isReady)
            {
                Configuration.Files.AddRange(files);
                FireChanged(Modification.AddedItems);
            }
        }

        public void AddFolder(string folder)
        {
            log.Debug("AddFolder {0}", folder);
            var files = Directory.EnumerateFiles(folder).ToArray();
            if (files.Length > 0)
            {
                log.Debug("AddFolder - found {0} files", files.Length);
                AddFileInternal(files);
            }

            if (isReady)
            {
                Configuration.Folders.Add(folder);
                FireChanged(Modification.AddedItems);
            }
        }

        public void Clear()
        {
            suspendEvents = true;
            Configuration.Files.Clear();
            Configuration.Folders.Clear();
            Items.Clear();
            addedFiles.Clear();
            RecalculateStats();
            suspendEvents = false;
            FireChanged(Modification.Clear);
        }

        public void ForceRefresh()
        {
            RecalculateStats();
            FireChanged(Modification.Forced);
        }

        private void Add(IEnumerable<SingleProcessingData> singleProcessingDatas)
        {
            foreach (var data in singleProcessingDatas)
            {
                Items.Add(data);
            }

            RecalculateStats();
        }

        private string[] AddFileInternal(params string[] files)
        {
            List<SingleProcessingData> items = new List<SingleProcessingData>(files.Length);
            List<string> filtered = new List<string>();
            foreach (var file in files)
            {
                if (addedFiles.ContainsKey(file))
                {
                    log.Info("File <{0}> is already added", file);
                    continue;
                }

                if (!File.Exists(file))
                {
                    log.Warn("File <{0}> not found", file);
                    continue;
                }

                log.Debug("Adding <{0}>", file);
                filtered.Add(file);
                addedFiles[file] = true;
                var item = new SingleProcessingData(File.ReadAllText(file));
                item.Stars = Configuration.Stars;
                items.Add(item);
            }

            Add(items);
            return filtered.ToArray();
        }

        private void FireChanged(Modification modification)
        {
            if (!suspendEvents &&
                isReady)
            {
                Changed?.Invoke(this, new ModificationEventArgs(modification));
            }
        }

        private void Init()
        {
            foreach (var folder in Configuration.Folders)
            {
                AddFolder(folder);
            }

            AddFiles(Configuration.Files.ToArray());
            isReady = true;
        }

        private void RecalculateStats()
        {
            positiveNegativeStats = new PrecisionRecallCalculator<PositivityType>();
            foreach (var singleProcessingData in Items)
            {
                positiveNegativeStats.Add(
                    PositivityType,
                    singleProcessingData.Document?.GetPositivity());
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FireChanged(Modification.CollectionChanged);
        }
    }
}
