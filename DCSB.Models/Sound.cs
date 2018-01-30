using DCSB.Utils;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace DCSB.Models
{
    public class Sound : ObservableObject, IBindable
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private ObservableCollection<string> _files;
        public ObservableCollection<string> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                RaisePropertyChanged("Files");
            }
        }

        private ObservableCollection<VKey> _keys;
        public ObservableCollection<VKey> Keys
        {
            get { return _keys; }
            set
            {
                _keys = value;
                RaisePropertyChanged("Keys");
            }
        }

        private int _volume;
        public int Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                RaisePropertyChanged("Volume");
            }
        }

        private bool _loop;
        public bool Loop
        {
            get { return _loop; }
            set
            {
                _loop = value;
                RaisePropertyChanged("Loop");
            }
        }

        private string _error;
        [XmlIgnore]
        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged("Error");
            }
        }

        public Sound()
        {
            _keys = new ObservableCollection<VKey>();
            _files = new ObservableCollection<string>();

            _keys.CollectionChanged += (sender, e) => RaisePropertyChanged("Keys");
            _files.CollectionChanged += (sender, e) => RaisePropertyChanged("Files");
            _files.CollectionChanged += (sender, e) => ValidateFiles();

            Volume = 100;
        }

        private void ValidateFiles()
        {
            Error = null;
            List<string> missing_files = new List<string>();
            foreach (string file in Files)
            {
                if (!File.Exists(file))
                {
                    missing_files.Add(file);
                }
            }
            if (missing_files.Count != 0)
            {
                Error = string.Format("Following files do not exist:\n{0}", string.Join("\n", missing_files));
            }
        }
    }
}
