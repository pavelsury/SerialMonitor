using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SerialMonitor.Business
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetNotifyingValueProperty<T>(ref T field, T value, Action actionOnSuccess = null, [CallerMemberName] string propertyName = "") where T: struct
        {
            if (field.Equals(value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            actionOnSuccess?.Invoke();
            return true;
        }

        protected bool SetNotifyingProperty(ref string field, string value, [CallerMemberName] string propertyName = "")
        {
            if (ReferenceEquals(field, value))
            {
                return false;
            }

            if (field?.Equals(value) == true)
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetNotifyingProperty<T>(ref T? field, T? value, [CallerMemberName] string propertyName = "") where T: struct
        {
            if (ReferenceEquals(field, value))
            {
                return false;
            }

            if (field?.Equals(value) == true)
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetNotifyingProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "") where T: class
        {
            if (ReferenceEquals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "") => OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
    }
}