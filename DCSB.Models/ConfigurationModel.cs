using GalaSoft.MvvmLight;
using DCSB.Utils;
using System.Xml.Serialization;
using DCSB.Colors;

namespace DCSB.Models
{
    public class ConfigurationModel : ObservableObject
    {
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

        private int _primaryDeviceVolume;
        public int PrimaryDeviceVolume
        {
            get { return _primaryDeviceVolume; }
            set
            {
                _primaryDeviceVolume = value;
                RaisePropertyChanged("PrimaryDeviceVolume");
            }
        }

        private int _secondaryDeviceVolume;
        public int SecondaryDeviceVolume
        {
            get { return _secondaryDeviceVolume; }
            set
            {
                _secondaryDeviceVolume = value;
                RaisePropertyChanged("SecondaryDeviceVolume");
            }
        }

        private bool _overlap;
        public bool Overlap
        {
            get { return _overlap; }
            set
            {
                _overlap = value;
                RaisePropertyChanged("Overlap");
            }
        }

        private OutputDevice _primaryOutputDevice;
        public OutputDevice PrimaryOutputDevice
        {
            get { return _primaryOutputDevice; }
            set
            {
                _primaryOutputDevice = value;
                RaisePropertyChanged("PrimaryOutputDevice");
            }
        }

        private OutputDevice _secondaryOutputDevice;
        public OutputDevice SecondaryOutputDevice
        {
            get { return _secondaryOutputDevice; }
            set
            {
                _secondaryOutputDevice = value;
                RaisePropertyChanged("SecondaryOutputDevice");
            }
        }

        private double _windowWidth;
        public double WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                _windowWidth = value;
                RaisePropertyChanged("WindowWidth");
            }
        }

        private double _windowHeight;
        public double WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                _windowHeight = value;
                RaisePropertyChanged("WindowHeight");
            }
        }

        private double _countersWidth;
        public double CountersWidth
        {
            get { return _countersWidth; }
            set
            {
                _countersWidth = value;
                RaisePropertyChanged("CountersWidth");
            }
        }

        private double _soundsWidth;
        public double SoundsWidth
        {
            get { return _soundsWidth; }
            set
            {
                _soundsWidth = value;
                RaisePropertyChanged("SoundsWidth");
            }
        }

        private bool _minimizeToTray;
        public bool MinimizeToTray
        {
            get { return _minimizeToTray; }
            set
            {
                _minimizeToTray = value;
                RaisePropertyChanged("MinimizeToTray");
            }
        }

        private DisplayOption _enable;
        public DisplayOption Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                RaisePropertyChanged("Enable");
            }
        }

        private bool _darkTheme;
        public bool DarkTheme
        {
            get { return _darkTheme; }
            set
            {
                _darkTheme = value;
                RaisePropertyChanged("DarkTheme");
            }
        }

        private string _primaryColor;
        public string PrimaryColor
        {
            get { return _primaryColor; }
            set
            {
                _primaryColor = value;
                RaisePropertyChanged("PrimaryColor");
            }
        }

        private string _accentColor;
        public string AccentColor
        {
            get { return _accentColor; }
            set
            {
                _accentColor = value;
                RaisePropertyChanged("AccentColor");
            }
        }

        private ObservableObjectCollection<Preset> _presetCollection;
        public ObservableObjectCollection<Preset> PresetCollection
        {
            get { return _presetCollection; }
            set
            {
                _presetCollection = value;
                RaisePropertyChanged("PresetCollection");
            }
        }

        private int _selectedPresetIndex;
        public int SelectedPresetIndex
        {
            get { return _selectedPresetIndex; }
            set
            {
                _selectedPresetIndex = value;
                RaisePropertyChanged("SelectedPreset");
            }
        }

        [XmlIgnore]
        public Preset SelectedPreset
        {
            get { return _selectedPresetIndex < PresetCollection.Count ? PresetCollection[_selectedPresetIndex] : PresetCollection[0]; }
            set
            {
                int index = PresetCollection.IndexOf(value);
                SelectedPresetIndex = index == -1 ? 0 : index;
            }
        }

        private CounterShortcuts _counterShortcuts;
        public CounterShortcuts CounterShortcuts
        {
            get { return _counterShortcuts; }
            set
            {
                _counterShortcuts = value;
                RaisePropertyChanged("CounterShortcuts");
            }
        }

        private SoundShortcuts _soundShortcuts;
        public SoundShortcuts SoundShortcuts
        {
            get { return _soundShortcuts; }
            set
            {
                _soundShortcuts = value;
                RaisePropertyChanged("SoundShortcuts");
            }
        }

        public ConfigurationModel()
        {
            PresetCollection = new ObservableObjectCollection<Preset>();
            CounterShortcuts = new CounterShortcuts();
            SoundShortcuts = new SoundShortcuts();
            PrimaryOutputDevice = new OutputDevice(-1, "Default Output Device");
            SecondaryOutputDevice = new OutputDevice(-2, "Disabled");

            PresetCollection.CollectionChanged += (sender, e) => RaisePropertyChanged("PresetCollection");
            PresetCollection.CollectionChanged += (sender, e) => RaisePropertyChanged("SelectedPreset");
            CounterShortcuts.PropertyChanged += (sender, e) => RaisePropertyChanged("CounterShortcuts");
            SoundShortcuts.PropertyChanged += (sender, e) => RaisePropertyChanged("SoundShortcuts");

            Volume = 100;
            PrimaryDeviceVolume = 100;
            SecondaryDeviceVolume = 100;
            WindowHeight = 300;
            WindowWidth = 500;
            CountersWidth = 1;
            SoundsWidth = 1;
        }
    }
}
