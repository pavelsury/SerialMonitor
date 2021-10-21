using SerialMonitor.Win.Business.Factories;

namespace SerialMonitor.Win.Ui.Factories
{
    public class UiFactory
    {
        public UiFactory(ModelFactory modelFactory)
        {
            _modelFactory = modelFactory;
        }

        public SerialMonitorControl SerialMonitorControl
        {
            get
            {
                if (_serialMonitorControl == null)
                {
                    _serialMonitorControl = new SerialMonitorControl(_modelFactory);
                    _modelFactory.SetConsoleWriter(_serialMonitorControl);
                }

                return _serialMonitorControl;
            }
        }
        
        private readonly ModelFactory _modelFactory;
        private SerialMonitorControl _serialMonitorControl;
    }
}