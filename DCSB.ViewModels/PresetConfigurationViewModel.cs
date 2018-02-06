using DCSB.Models;
using GalaSoft.MvvmLight;

namespace DCSB.ViewModels
{
    public class PresetConfigurationViewModel : ObservableObject
    {
        private ConfigurationModel _configurationModel;

        public PresetConfigurationViewModel(ConfigurationModel configurationModel)
        {
            _configurationModel = configurationModel;

            _leftPresetViewModel = new PresetViewModel(configurationModel);
            _rightPresetViewModel = new PresetViewModel(configurationModel);
        }

        private PresetViewModel _leftPresetViewModel;
        public PresetViewModel LeftPresetViewModel
        {
            get { return _leftPresetViewModel; }
        }

        private PresetViewModel _rightPresetViewModel;
        public PresetViewModel RightPresetViewModel
        {
            get { return _rightPresetViewModel; }
        }
    }
}
