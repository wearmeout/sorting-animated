using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FractionalSorting.Visualization.Common
{
    public class BindableBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void Set<T>(ref T storage, T value, 
                                     [CallerMemberName]string name = null)
        {
            if (object.Equals(storage, value))
                return;

            storage = value;
            RaisePropertyChanged(name);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName]
                                                    string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
