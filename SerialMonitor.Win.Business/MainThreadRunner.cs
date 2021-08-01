using System;
using System.Windows.Threading;
using SerialMonitor.Business;

namespace SerialMonitor.Win.Business
{
    public class MainThreadRunner : IMainThreadRunner
    {
        public MainThreadRunner() => _dispatcher = Dispatcher.CurrentDispatcher;

        public void Run(Action action) => _dispatcher.InvokeAsync(action, DispatcherPriority.Background);

        private readonly Dispatcher _dispatcher;
    }
}