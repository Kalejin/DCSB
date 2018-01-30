using GalaSoft.MvvmLight;
using DCSB.Utils;

namespace DCSB.Models
{
    public class CounterShortcuts : ObservableObject
    {
        private Shortcut _next;
        public Shortcut Next
        {
            get { return _next; }
            set
            {
                _next = value;
                RaisePropertyChanged("Next");
            }
        }

        private Shortcut _previous;
        public Shortcut Previous
        {
            get { return _previous; }
            set
            {
                _previous = value;
                RaisePropertyChanged("Previous");
            }
        }

        private Shortcut _increment;
        public Shortcut Increment
        {
            get { return _increment; }
            set
            {
                _increment = value;
                RaisePropertyChanged("Increment");
            }
        }

        private Shortcut _decrement;
        public Shortcut Decrement
        {
            get { return _decrement; }
            set
            {
                _decrement = value;
                RaisePropertyChanged("Decrement");
            }
        }

        private Shortcut _reset;
        public Shortcut Reset
        {
            get { return _reset; }
            set
            {
                _reset = value;
                RaisePropertyChanged("Reset");
            }
        }

        public CounterShortcuts()
        {
            _next = new Shortcut();
            _previous = new Shortcut();
            _increment = new Shortcut();
            _decrement = new Shortcut();
            _reset = new Shortcut();

            _next.PropertyChanged += (sender, e) => RaisePropertyChanged("Next");
            _previous.PropertyChanged += (sender, e) => RaisePropertyChanged("Previous");
            _increment.PropertyChanged += (sender, e) => RaisePropertyChanged("Increment");
            _decrement.PropertyChanged += (sender, e) => RaisePropertyChanged("Decrement");
            _reset.PropertyChanged += (sender, e) => RaisePropertyChanged("Reset");

            _next.Keys.Add(VKey.MULTIPLY);
            _increment.Keys.Add(VKey.ADD);
            _decrement.Keys.Add(VKey.SUBTRACT);
        }
    }
}
