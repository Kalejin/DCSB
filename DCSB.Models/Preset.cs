using GalaSoft.MvvmLight;
using DCSB.Utils;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DCSB.Models
{
    public class Preset : ObservableObject, IBindable
    {
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

        private ObservableObjectCollection<Counter> _counterCollection;
        public ObservableObjectCollection<Counter> CounterCollection
        {
            get { return _counterCollection; }
            set
            {
                _counterCollection = value;
                RaisePropertyChanged("CounterCollection");
            }
        }

        private ObservableObjectCollection<Sound> _soundCollection;
        public ObservableObjectCollection<Sound> SoundCollection
        {
            get { return _soundCollection; }
            set
            {
                _soundCollection = value;
                RaisePropertyChanged("SoundCollection");
            }
        }

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

        private Counter _selectedCounter;
        [XmlIgnore]
        public Counter SelectedCounter
        {
            get { return _selectedCounter; }
            set
            {
                _selectedCounter = value;
                RaisePropertyChanged("SelectedCounter");
            }
        }

        private Sound _selectedSound;
        [XmlIgnore]
        public Sound SelectedSound
        {
            get { return _selectedSound; }
            set
            {
                _selectedSound = value;
                RaisePropertyChanged("SelectedSound");
            }
        }

        public Preset()
        {
            _keys = new ObservableCollection<VKey>();
            _counterCollection = new ObservableObjectCollection<Counter>();
            _soundCollection = new ObservableObjectCollection<Sound>();

            Keys.CollectionChanged += (sender, e) => RaisePropertyChanged("Keys");
            CounterCollection.CollectionChanged += (sender, e) => RaisePropertyChanged("CounterCollection");
            CounterCollection.CollectionChanged += (sender, e) => RaisePropertyChanged("SelectedCounter");
            SoundCollection.CollectionChanged += (sender, e) => RaisePropertyChanged("SoundCollection");
            SoundCollection.CollectionChanged += (sender, e) => RaisePropertyChanged("SelectedSound");
        }
    }
}
