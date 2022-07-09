using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CVLib
{
    public class CvViewModelBase : INotifyPropertyChanged
    {
        #region

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (Equals(properValue, newValue)) return;

            properValue = newValue;
            OnPropertyChanged(propertyName);
        }

        #endregion
    }
}