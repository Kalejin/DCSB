using GalaSoft.MvvmLight;

namespace DCSB.Models
{
    public class SoundShortcuts : ObservableObject
    {
        private Shortcut _pause;
        public Shortcut Pause
        {
            get { return _pause; }
            set
            {
                _pause = value;
                RaisePropertyChanged("Pause");
            }
        }

        private Shortcut _continue;
        public Shortcut Continue
        {
            get { return _continue; }
            set
            {
                _continue = value;
                RaisePropertyChanged("Continue");
            }
        }

        private Shortcut _stop;
        public Shortcut Stop
        {
            get { return _stop; }
            set
            {
                _stop = value;
                RaisePropertyChanged("Stop");
            }
        }

        public SoundShortcuts()
        {
            _pause = new Shortcut();
            _continue = new Shortcut();
            _stop = new Shortcut();

            _pause.PropertyChanged += (sender, e) => RaisePropertyChanged("Pause");
            _continue.PropertyChanged += (sender, e) => RaisePropertyChanged("Continue");
            _stop.PropertyChanged += (sender, e) => RaisePropertyChanged("Stop");
        }
    }
}
