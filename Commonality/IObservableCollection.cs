using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commonality
{
    public interface IObservableCollection<T>: ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}
