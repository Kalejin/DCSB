using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Serialization;
using DCSB.Utils;
using GalaSoft.MvvmLight;

namespace DCSB.Models
{
    public class Shortcut : ObservableObject, IBindable
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

        [XmlIgnore]
        public ICommand Command { get; set; }

        public Shortcut()
        {
            _keys = new ObservableCollection<VKey>();

            _keys.CollectionChanged += (sender, e) => RaisePropertyChanged("Keys");
        }
    }
}
