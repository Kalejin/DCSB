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
        private const string TempFileName = "config_tmp.xml";
        private const string BackupFileName = "config_backup.xml";
        private readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), DirectoryName, FileName);
        private readonly string TempConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), DirectoryName, TempFileName);
        private readonly string BackupConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), DirectoryName, BackupFileName);

        private Timer _timer;
        private ConfigurationModel _model;

        private void SaveCallback(object state)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationModel));

            if (!File.Exists(TempConfigPath))
            {
                CreateFile(TempConfigPath);
            }
            using (FileStream stream = File.Open(TempConfigPath, FileMode.Truncate))
            {
                serializer.Serialize(stream, _model);
            }

            if (File.Exists(ConfigPath))
            {
                File.Replace(TempConfigPath, ConfigPath, BackupConfigPath, true);
            }
            else
            {
                File.Move(TempConfigPath, ConfigPath);
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
                try
                {
                    result = (ConfigurationModel)serializer.Deserialize(stream);
                    return result;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            MoveCorruptedConfig(ConfigPath);
            return new ConfigurationModel();
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

            File.Create(filePath, 1, FileOptions.None, security).Close();
        }

        private void MoveCorruptedConfig(string filePath)
        {
            string newPath = filePath.Replace(".xml", $"_corrupted_{DateTime.Now.Ticks}.xml");
            try
            {
                File.Move(filePath, newPath);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
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
