namespace SerialMonitor.Business
{
    public class CustomButton : NotifyPropertyChanged
    {
        public string Label
        {
            get => _label;
            set => SetNotifyingProperty(ref _label, value ?? string.Empty);
        }

        public string Command
        {
            get => _command;
            set => SetNotifyingProperty(ref _command, value ?? string.Empty);
        }

        private string _label = string.Empty;
        private string _command = string.Empty;
    }
}
