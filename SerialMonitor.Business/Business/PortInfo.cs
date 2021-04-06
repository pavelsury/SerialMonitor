using System;

namespace SerialMonitor.Business
{
    public class PortInfo : NotifyPropertyChanged, IComparable<PortInfo>
    {
        public string Name { get; set; }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetNotifyingValueProperty(ref _isAvailable, value);
        }

        public PortSettings Settings
        {
            get => _settings;
            set => SetNotifyingProperty(ref _settings, value);
        }

        public int CompareTo(PortInfo other) => string.Compare(Name, other.Name, StringComparison.CurrentCulture);

        private bool _isAvailable;
        private PortSettings _settings;
    }
}