using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Wikiled.Common.Helpers;

namespace Wikiled.Sentiment.Text.Resources
{
    public class ConfigurationHandler : IConfigurationHandler
    {
        private readonly Dictionary<string, string> overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private string startingLocation;

        public string StartingLocation
        {
            get => startingLocation ?? (startingLocation =  Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            set => startingLocation = value;
        }

        public string GetConfiguration(string name)
        {
            if (!TryGetConfiguration(name, out string value))
            {
                throw new NullReferenceException(name + " not found");
            }

            return value;
        }

        public string ResolvePath(string name)
        {
            var resourcePath = Path.Combine(StartingLocation, GetConfiguration(name));
            return new DirectoryInfo(resourcePath).FullName;
        }

        public T SafeGetConfiguration<T>(string name, T defaultValue)
        {
            return !TryGetConfiguration(name, out T value) ? defaultValue : value;
        }

        public void SetConfiguration(string name, string value)
        {
            overrides[name] = value;
        }

        public bool TryGetConfiguration<T>(string name, out T outValue)
        {
            outValue = default(T);
            if (!overrides.TryGetValue(name, out var value))
            {
                value = ConfigurationManager.AppSettings[name];
            }

            return value != null && Converter.TryConvert(value, out outValue);
        }
    }
}
