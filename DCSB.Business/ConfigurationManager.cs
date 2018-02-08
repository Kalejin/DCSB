using DCSB.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Xml.Serialization;

namespace DCSB.Business
{
    public class ConfigurationManager : IDisposable
    {
        private const string DirectoryName = "DCSB";
        private const string FileName = "config.xml";
        private readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), DirectoryName, FileName);

        private Timer _timer;
        private ConfigurationModel _model;

        private void SaveCallback(object state)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationModel));

            if (!File.Exists(ConfigPath))
            {
                CreateFile(ConfigPath);
            }
            using (FileStream stream = File.Open(ConfigPath, FileMode.Truncate))
            {
                serializer.Serialize(stream, _model);
            }
            Debug.WriteLine("Saved configuration");
            _timer.Dispose();
            _timer = null;
        }

        public ConfigurationModel Load()
        {
            if (!File.Exists(ConfigPath))
            {
                return new ConfigurationModel();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationModel));
            ConfigurationModel result;

            using (FileStream stream = File.Open(ConfigPath, FileMode.Open))
            {
                result = (ConfigurationModel)serializer.Deserialize(stream);
            }

            return result;
        }

        public void Save(ConfigurationModel model)
        {
            _model = model;
            if (_timer == null)
            {
                _timer = new Timer(SaveCallback, null, 1000, Timeout.Infinite);
            }
        }

        private void CreateFile(string filePath)
        {
            FileSecurity security = new FileSecurity();
            SecurityIdentifier securityIdentifier = new SecurityIdentifier("S-1-1-0");
            FileSystemAccessRule rule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow);
            security.AddAccessRule(rule);

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.Create(ConfigPath, 1, FileOptions.None, security).Close();
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                SaveCallback(null);
            }
        }
    }
}
