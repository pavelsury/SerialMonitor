using System;

namespace SerialMonitor.Business
{
    public interface IMainThreadRunner
    {
        void Run(Action action);
    }
}