using System;
using System.Windows.Threading;

namespace SerialMonitor.Business
{
    public class MainThreadRunner : IMainThreadRunner
    {
        public MainThreadRunner(Dispatcher dispatcher) => _dispatcher = dispatcher;

        public void Run(Action action) => _dispatcher.InvokeAsync(action, DispatcherPriority.Background);

        private readonly Dispatcher _dispatcher;
    }
}
