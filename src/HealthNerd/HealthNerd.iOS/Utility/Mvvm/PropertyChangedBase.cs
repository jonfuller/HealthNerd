using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HealthNerd.iOS.Utility.Mvvm
{
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseAllPropertiesChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetPropertyAndRaise<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;

            OnPropertyChanged(propertyName);

            return true;
        }
    }
}