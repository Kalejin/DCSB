using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using DCSB.Business;
using DCSB.Input;
using DCSB.Models;
using DCSB.Utils;
using System.Threading.Tasks;
using DCSB.Colors;
using System.Linq;

namespace DCSB.ViewModels
{
    public class ViewModel : ObservableObject
    {
        private ConfigurationManager _configurationManager;
        private OpenFileManager _openFileManager;
        private ShortcutManager _shortcutManager;
        private SoundManager _soundManager;
        private UpdateManager _updateManager;
        private KeyboardInput _keyboardInput;
        private PaletteManager _paletteManager;

        private double _previousVolume;
        private double _previousPrimaryVolume;
        private double _previousSecondaryVolume;

        public ViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
            AccentSwatches = Swatches.Where(x => x.IsAccented);

            ApplicationStateModel = new ApplicationStateModel();
            _configurationManager = new ConfigurationManager();
            ConfigurationModel = _configurationManager.Load();
            if (ConfigurationModel.PresetCollection.Count == 0) ConfigurationModel.PresetCollection.Add(new Preset() { Name = "New Preset" } );
            _openFileManager = new OpenFileManager();

            _soundManager = new SoundManager(ConfigurationModel.PrimaryOutputDevice, ConfigurationModel.SecondaryOutputDevice)
            {
                Volume = ConfigurationModel.Volume / 100f,
                PrimaryDeviceVolume = ConfigurationModel.PrimaryDeviceVolume / 100f,
                SecondaryDeviceVolume = ConfigurationModel.SecondaryDeviceVolume / 100f,
                Overlap = ConfigurationModel.Overlap
            };
            _shortcutManager = new ShortcutManager(ApplicationStateModel, ConfigurationModel, _soundManager);
            _updateManager = new UpdateManager();
            _paletteManager = new PaletteManager();
            _paletteManager.SetLightDark(DarkTheme);
            _paletteManager.ReplacePrimaryColor(PrimaryColor);
            _paletteManager.ReplaceAccentColor(AccentColor);

            PresetConfigurationViewModel = new PresetConfigurationViewModel(ApplicationStateModel, ConfigurationModel);

            ConfigurationModel.PropertyChanged += (sender, e) => _configurationManager.Save((ConfigurationModel)sender);

            ConfigurationModel.CounterShortcuts.Next.Command = NextCounterCommand;
            ConfigurationModel.CounterShortcuts.Previous.Command = PreviousCounterCommand;
            ConfigurationModel.CounterShortcuts.Increment.Command = IncrementCommand;
            ConfigurationModel.CounterShortcuts.Decrement.Command = DecrementCommand;
            ConfigurationModel.CounterShortcuts.Reset.Command = ResetCommand;
            ConfigurationModel.SoundShortcuts.Pause.Command = PauseCommand;
            ConfigurationModel.SoundShortcuts.Continue.Command = ContinueCommand;
            ConfigurationModel.SoundShortcuts.Stop.Command = StopCommand;

            Task.Run(() => _updateManager.AutoUpdateCheck(Version));
        }

        public IntPtr WindowHandle
        {
            set
            {
                _keyboardInput = new KeyboardInput(value);
                _keyboardInput.KeyDown += _shortcutManager.KeyDown;
                _keyboardInput.KeyPress += _shortcutManager.KeyPress;

                BindKeysViewModel = new BindKeysViewModel(_keyboardInput);
            }
        }

        public ApplicationStateModel ApplicationStateModel { get; }

        public ConfigurationModel ConfigurationModel { get; }

        public PresetConfigurationViewModel PresetConfigurationViewModel { get; }

        public BindKeysViewModel BindKeysViewModel { get; protected set; }

        public IEnumerable<Swatch> Swatches { get; }

        public IEnumerable<Swatch> AccentSwatches { get; }

        public GridLength CountersWidth
        {
            get { return new GridLength(ConfigurationModel.CountersWidth, GridUnitType.Star); }
            set
            {
                ConfigurationModel.CountersWidth = value.Value;
                RaisePropertyChanged("CountersWidth");
            }
        }

        public GridLength SoundsWidth
        {
            get { return new GridLength(ConfigurationModel.SoundsWidth, GridUnitType.Star); }
            set
            {
                ConfigurationModel.SoundsWidth = value.Value;
                RaisePropertyChanged("SoundsWidth");
            }
        }

        public Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public double CurrentVolume
        {
            get { return ConfigurationModel.Volume; }
            set
            {
                ConfigurationModel.Volume = (int)value;
                _soundManager.Volume = ConfigurationModel.Volume / 100f;
                RaisePropertyChanged("CurrentVolume");
            }
        }

        public double PrimaryDeviceVolume
        {
            get { return ConfigurationModel.PrimaryDeviceVolume; }
            set
            {
                ConfigurationModel.PrimaryDeviceVolume = (int)value;
                _soundManager.PrimaryDeviceVolume = ConfigurationModel.PrimaryDeviceVolume / 100f;
                RaisePropertyChanged("PrimaryDeviceVolume");
            }
        }

        public double SecondaryDeviceVolume
        {
            get { return ConfigurationModel.SecondaryDeviceVolume; }
            set
            {
                ConfigurationModel.SecondaryDeviceVolume = (int)value;
                _soundManager.SecondaryDeviceVolume = ConfigurationModel.SecondaryDeviceVolume / 100f;
                RaisePropertyChanged("SecondaryDeviceVolume");
            }
        }

        public bool Overlap
        {
            get { return ConfigurationModel.Overlap; }
            set
            {
                ConfigurationModel.Overlap = value;
                _soundManager.Overlap = ConfigurationModel.Overlap;
                RaisePropertyChanged("Overlap");
            }
        }

        public DisplayOption Enable
        {
            get { return ConfigurationModel.Enable; }
            set
            {
                ConfigurationModel.Enable = value;
                RaisePropertyChanged("Enable");
                if (ConfigurationModel.Enable != DisplayOption.Sounds && ConfigurationModel.Enable != DisplayOption.Both)
                {
                    _soundManager.Stop();
                }
            }
        }

        public bool DarkTheme
        {
            get { return ConfigurationModel.DarkTheme; }
            set
            {
                ConfigurationModel.DarkTheme = value;
                _paletteManager.SetLightDark(value);
                RaisePropertyChanged("DarkTheme");
            }
        }

        public Swatch PrimaryColor
        {
            get { return Swatches.Where(x => x.Name == ConfigurationModel.PrimaryColor).FirstOrDefault(); }
            set
            {
                ConfigurationModel.PrimaryColor = value.Name;
                _paletteManager.ReplacePrimaryColor(value);
                RaisePropertyChanged("PrimaryColor");
            }
        }

        public Swatch AccentColor
        {
            get { return AccentSwatches.Where(x => x.Name == ConfigurationModel.AccentColor).FirstOrDefault(); }
            set
            {
                ConfigurationModel.AccentColor = value.Name;
                _paletteManager.ReplaceAccentColor(value);
                RaisePropertyChanged("SecondaryColor");
            }
        }

        public IList<OutputDevice> AvailableOutputDevices
        {
            get { return _soundManager.EnumerateDevices(); }
        }

        public IList<OutputDevice> SecondaryOutputDevices
        {
            get
            {
                IList<OutputDevice> devices = AvailableOutputDevices;
                devices.Insert(0, new OutputDevice(-2, "Disabled"));
                return devices;
            }
        }

        public int PrimaryOutputDeviceIndex
        {
            get
            {
                IList<OutputDevice> devices = AvailableOutputDevices;
                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].Number == ConfigurationModel.PrimaryOutputDevice.Number)
                    {
                        return i;
                    }
                }
                return 0;
            }
            set
            {
                ConfigurationModel.PrimaryOutputDevice = AvailableOutputDevices[value];
                _soundManager.ChangePrimaryDevice(ConfigurationModel.PrimaryOutputDevice);
                RaisePropertyChanged("PrimaryOutputDevice");
            }
        }

        public int SecondaryOutputDeviceIndex
        {
            get
            {
                IList<OutputDevice> devices = SecondaryOutputDevices;
                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].Number == ConfigurationModel.SecondaryOutputDevice.Number)
                    {
                        return i;
                    }
                }
                return 0;
            }
            set
            {
                ConfigurationModel.SecondaryOutputDevice = SecondaryOutputDevices[value];
                _soundManager.ChangeSecondaryDevice(ConfigurationModel.SecondaryOutputDevice);
                RaisePropertyChanged("SecondaryOutputDevice");
            }
        }

        public ICommand CheckForUpdatesCommand
        {
            get { return new RelayCommand(CheckForUpdates); }
        }
        private async void CheckForUpdates()
        {
            await _updateManager.ManualUpdateCheck(Version);
        }

        public ICommand PresetSelectedCommand
        {
            get { return new RelayCommand<Preset>(PresetSelected); }
        }
        private void PresetSelected(Preset selectedPreset)
        {
            ConfigurationModel.SelectedPreset = selectedPreset;
            foreach (Counter counter in selectedPreset.CounterCollection)
            {
                counter.ReadFromFile();
            }
        }

        public ICommand OpenSettingsCommand
        {
            get { return new RelayCommand(OpenSettings); }
        }
        private void OpenSettings()
        {
            ApplicationStateModel.SettingsOpened = true;
        }

        public ICommand OpenCounterCommand
        {
            get { return new RelayCommand(OpenCounter, AreCountersEnabled); }
        }
        private void OpenCounter()
        {
            if (ConfigurationModel.SelectedPreset.SelectedCounter != null)
            {
                ApplicationStateModel.ModifiedCounter = ConfigurationModel.SelectedPreset.SelectedCounter;
                ApplicationStateModel.CounterOpened = true;
            }
        }

        public ICommand OpenSoundCommand
        {
            get { return new RelayCommand(OpenSound, AreSoundsEnabled); }
        }
        private void OpenSound()
        {
            if (ConfigurationModel.SelectedPreset.SelectedSound != null)
            {
                ApplicationStateModel.ModifiedSound = ConfigurationModel.SelectedPreset.SelectedSound;
                ApplicationStateModel.SoundOpened = true;
            }
        }

        public ICommand OpenAboutCommand
        {
            get { return new RelayCommand(OpenAbout); }
        }
        private void OpenAbout()
        {
            ApplicationStateModel.AboutOpened = true;
        }

        public ICommand OpenCounterFileDialogCommand
        {
            get { return new RelayCommand(OpenCounterFileDialog, AreCountersEnabled); }
        }
        private void OpenCounterFileDialog()
        {
            string result = _openFileManager.OpenCounterFile();
            if (result != null)
            {
                ApplicationStateModel.ModifiedCounter.File = result;
            }
        }

        public ICommand OpenSoundFileDialogCommand
        {
            get { return new RelayCommand(OpenSoundFileDialog, AreSoundsEnabled); }
        }
        private void OpenSoundFileDialog()
        {
            string[] result = _openFileManager.OpenSoundFiles();
            if (result != null)
            {
                ConfigurationModel.SelectedPreset.SelectedSound.Files.Clear();
                foreach (string file in result)
                {
                    ApplicationStateModel.ModifiedSound.Files.Add(file);
                }
            }
        }

        public ICommand AddCounterCommand
        {
            get { return new RelayCommand(AddCounter, AreCountersEnabled); }
        }
        private void AddCounter()
        {
            Counter counter = new Counter();
            ConfigurationModel.SelectedPreset.CounterCollection.Add(counter);
            ConfigurationModel.SelectedPreset.SelectedCounter = counter;
            ApplicationStateModel.ModifiedCounter = counter;
            ApplicationStateModel.CounterOpened = true;
        }

        public ICommand RemoveCounterCommand
        {
            get { return new RelayCommand(RemoveCounter, AreCountersEnabled); }
        }
        private void RemoveCounter()
        {
            if (ConfigurationModel.SelectedPreset.SelectedCounter != null)
            {
                ConfigurationModel.SelectedPreset.CounterCollection.Remove(ConfigurationModel.SelectedPreset.SelectedCounter);
            }
        }

        public ICommand IncrementCommand
        {
            get { return new RelayCommand(Increment, AreCountersEnabled); }
        }
        private void Increment()
        {
            if (ConfigurationModel.SelectedPreset.SelectedCounter != null)
            {
                ConfigurationModel.SelectedPreset.SelectedCounter.Count += ConfigurationModel.SelectedPreset.SelectedCounter.Increment;
            }
        }

        public ICommand DecrementCommand
        {
            get { return new RelayCommand(Decrement, AreCountersEnabled); }
        }
        private void Decrement()
        {
            if (ConfigurationModel.SelectedPreset.SelectedCounter != null)
            {
                ConfigurationModel.SelectedPreset.SelectedCounter.Count -= ConfigurationModel.SelectedPreset.SelectedCounter.Increment;
            }
        }

        public ICommand ResetCommand
        {
            get { return new RelayCommand(Reset, AreCountersEnabled); }
        }
        private void Reset()
        {
            if (ConfigurationModel.SelectedPreset.SelectedCounter != null)
            {
                ConfigurationModel.SelectedPreset.SelectedCounter.Count = 0;
            }
        }

        public ICommand NextCounterCommand
        {
            get { return new RelayCommand(NextCounter, AreCountersEnabled); }
        }
        private void NextCounter()
        {
            if (ConfigurationModel.SelectedPreset.SelectedCounter == null )
            {
                if (ConfigurationModel.SelectedPreset.CounterCollection.Count != 0)
                {
                    ConfigurationModel.SelectedPreset.SelectedCounter = ConfigurationModel.SelectedPreset.CounterCollection[0];
                }
            }
            else
            {
                int currentIndex = ConfigurationModel.SelectedPreset.CounterCollection.IndexOf(ConfigurationModel.SelectedPreset.SelectedCounter);
                int nextIndex = (currentIndex + 1) % ConfigurationModel.SelectedPreset.CounterCollection.Count;
                ConfigurationModel.SelectedPreset.SelectedCounter = ConfigurationModel.SelectedPreset.CounterCollection[nextIndex];
            }
        }

        public ICommand PreviousCounterCommand
        {
            get { return new RelayCommand(PreviousCounter, AreCountersEnabled); }
        }
        private void PreviousCounter()
        {
            if (ConfigurationModel.SelectedPreset.SelectedCounter == null)
            {
                if (ConfigurationModel.SelectedPreset.CounterCollection.Count != 0)
                {
                    ConfigurationModel.SelectedPreset.SelectedCounter = ConfigurationModel.SelectedPreset.CounterCollection[0];
                }
            }
            else
            {
                int currentIndex = ConfigurationModel.SelectedPreset.CounterCollection.IndexOf(ConfigurationModel.SelectedPreset.SelectedCounter);
                int previousIndex = (currentIndex - 1 + ConfigurationModel.SelectedPreset.CounterCollection.Count) % ConfigurationModel.SelectedPreset.CounterCollection.Count;
                ConfigurationModel.SelectedPreset.SelectedCounter = ConfigurationModel.SelectedPreset.CounterCollection[previousIndex];
            }
        }

        public ICommand MuteCommand
        {
            get { return new RelayCommand(Mute); }
        }
        private void Mute()
        {
            if (CurrentVolume == 0)
            {
                CurrentVolume = _previousVolume;
            }
            else
            {
                _previousVolume = CurrentVolume;
                CurrentVolume = 0;
            }
        }

        public ICommand MutePrimaryCommand
        {
            get { return new RelayCommand(MutePrimary); }
        }
        private void MutePrimary()
        {
            if (PrimaryDeviceVolume == 0)
            {
                PrimaryDeviceVolume = _previousPrimaryVolume;
            }
            else
            {
                _previousPrimaryVolume = PrimaryDeviceVolume;
                PrimaryDeviceVolume = 0;
            }
        }

        public ICommand MuteSecondaryCommand
        {
            get { return new RelayCommand(MuteSecondary); }
        }
        private void MuteSecondary()
        {
            if (SecondaryDeviceVolume == 0)
            {
                SecondaryDeviceVolume = _previousSecondaryVolume;
            }
            else
            {
                _previousSecondaryVolume = SecondaryDeviceVolume;
                SecondaryDeviceVolume = 0;
            }
        }

        public ICommand AddSoundCommand
        {
            get { return new RelayCommand(AddSound, AreSoundsEnabled); }
        }
        private void AddSound()
        {
            Sound sound = new Sound();
            ConfigurationModel.SelectedPreset.SelectedSound = sound;
            ConfigurationModel.SelectedPreset.SoundCollection.Add(sound);
            ApplicationStateModel.ModifiedSound = sound;
            ApplicationStateModel.SoundOpened = true;
        }

        public ICommand RemoveSoundCommand
        {
            get { return new RelayCommand(RemoveSound, AreSoundsEnabled); }
        }
        private void RemoveSound()
        {
            ConfigurationModel.SelectedPreset.SoundCollection.Remove(ConfigurationModel.SelectedPreset.SelectedSound);
        }

        public ICommand PlayCommand
        {
            get { return new RelayCommand(Play, AreSoundsEnabled); }
        }
        private void Play()
        {
            if (ConfigurationModel.SelectedPreset.SelectedSound != null)
            {
                _soundManager.Play(ConfigurationModel.SelectedPreset.SelectedSound);
            }
        }

        public ICommand PauseCommand
        {
            get { return new RelayCommand(Pause, AreSoundsEnabled); }
        }
        private void Pause()
        {
            _soundManager.Pause();
        }

        public ICommand ContinueCommand
        {
            get { return new RelayCommand(Continue, AreSoundsEnabled); }
        }
        private void Continue()
        {
            _soundManager.Continue();
        }

        public ICommand StopCommand
        {
            get { return new RelayCommand(Stop, AreSoundsEnabled); }
        }
        private void Stop()
        {
            _soundManager.Stop();
        }

        public ICommand BindKeysCommand
        {
            get { return new RelayCommand<IBindable>(BindKeys); }
        }
        public void BindKeys(IBindable bindable)
        {
            ApplicationStateModel.ModifiedBindable = bindable;
            ApplicationStateModel.BindKeysOpened = true;
        }

        public ICommand CancelBindKeysCommand
        {
            get { return new RelayCommand(CancelBindKeys); }
        }
        public void CancelBindKeys()
        {
            ApplicationStateModel.BindKeysOpened = false;
            ApplicationStateModel.ModifiedBindable = null;
        }

        public ICommand ClearKeysCommand
        {
            get { return new RelayCommand(ClearKeys); }
        }
        private void ClearKeys()
        {
            ApplicationStateModel.BindKeysOpened = false;
            ApplicationStateModel.ModifiedBindable.Keys.Clear();
            ApplicationStateModel.ModifiedBindable = null;
        }

        public ICommand ClosingCommand
        {
            get { return new RelayCommand(Closing); }
        }
        private void Closing()
        {
            _configurationManager.Dispose();
        }

        private bool AreCountersEnabled()
        {
            return ConfigurationModel.Enable == DisplayOption.Counters || ConfigurationModel.Enable == DisplayOption.Both;
        }

        private bool AreSoundsEnabled()
        {
            return ConfigurationModel.Enable == DisplayOption.Sounds || ConfigurationModel.Enable == DisplayOption.Both;
        }
    }
}
