using DCSB.Utils;
using System.Collections.ObjectModel;

namespace DCSB.Models
{
    public interface IBindable
    {
        ObservableCollection<VKey> Keys { get; set; }
    }
}
