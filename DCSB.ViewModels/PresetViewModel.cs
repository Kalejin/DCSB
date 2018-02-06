using DCSB.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace DCSB.ViewModels
{
    public class PresetViewModel : ObservableObject
    {
        public PresetViewModel(ConfigurationModel configurationModel)
        {
            _configurationModel = configurationModel;
        }

        private ConfigurationModel _configurationModel;
        public ConfigurationModel ConfigurationModel
        {
            get { return _configurationModel; }
        }

        private Preset _selectedPreset;
        public Preset SelectedPreset
        {
            get { return _selectedPreset; }
            set
            {
                _selectedPreset = value;
                RaisePropertyChanged("SelectedPreset");
            }
        }

        public ICommand AddPresetCommand
        {
            get { return new RelayCommand(AddPreset); }
        }
        private void AddPreset()
        {
            Preset preset = new Preset() { Name = "New Preset" };
            _configurationModel.PresetCollection.Add(preset);
            SelectedPreset = preset;
        }

        public ICommand RemovePresetCommand
        {
            get { return new RelayCommand(RemovePreset); }
        }
        private void RemovePreset()
        {
            if (SelectedPreset != null)
            {
                if (_configurationModel.PresetCollection.Count == 1)
                {
                    AddPreset();
                    _configurationModel.PresetCollection.Remove(SelectedPreset);
                }
                else
                {
                    if (SelectedPreset == _configurationModel.SelectedPreset)
                    {
                        _configurationModel.SelectedPresetIndex = 0;
                    }
                    _configurationModel.PresetCollection.Remove(SelectedPreset);
                }
            }
        }

        public ICommand ClonePresetCommand
        {
            get { return new RelayCommand(ClonePreset); }
        }
        private void ClonePreset()
        {
            if (SelectedPreset != null)
            {
                _configurationModel.PresetCollection.Add((Preset)SelectedPreset.Clone());
            }
        }
    }
}
