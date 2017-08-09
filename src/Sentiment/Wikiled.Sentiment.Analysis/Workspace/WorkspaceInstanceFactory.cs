using System;
using System.IO;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Sentiment.Analysis.Workspace.Configuration;
using Wikiled.Sentiment.Analysis.Workspace.Data;

namespace Wikiled.Sentiment.Analysis.Workspace
{
    public class WorkspaceInstanceFactory : IWorkspaceInstanceFactory
    {
        private readonly ProjectType projectType;
        private readonly Func<string, string, IWorkspaceInstance> create;

        public WorkspaceInstanceFactory(ProjectType projectType, Func<string, string, IWorkspaceInstance> create)
        {
            Guard.NotNull(() => create, create);
            this.projectType = projectType;
            this.create = create;
        }

        public virtual IWorkspaceInstance Create(DataSourceType source, string name, string path)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Guard.NotNullOrEmpty(() => path, path);
            string projectPath = Path.Combine(path, name);
            Directory.CreateDirectory(projectPath);
            var instance = create(name, projectPath);
            instance.Configuration.DataSourceType = source;
            instance.Configuration.ProjectType = projectType;
            instance.Configuration.Name = name;
            instance.Init();
            instance.SaveConfiguration();
            return instance;
        }

        public virtual IWorkspaceInstance Open(string path)
        {
            Guard.NotNullOrEmpty(() => path, path);
            DirectoryInfo info = new DirectoryInfo(path);
            var instance = create(info.Name, path);
            instance.Init();
            instance.LoadConfiguration();
            instance.Configuration.ProjectType = projectType;
            return instance;
        }
    }
}
