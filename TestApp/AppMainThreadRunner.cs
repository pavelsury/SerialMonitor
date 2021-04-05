using System;
using System.Windows.Threading;
using SerialMonitor.Business;

namespace TestApp
{
    public class AppMainThreadRunner : IMainThreadRunner
    {
        public AppMainThreadRunner(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Run(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }

        private readonly Dispatcher _dispatcher;
    }
}
