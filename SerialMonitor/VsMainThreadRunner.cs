using System;
using Microsoft.VisualStudio.Shell;
using SerialMonitor.Business;

namespace SerialMonitor
{
    public class VsMainThreadRunner : IMainThreadRunner
    {
        public void Run(Action action)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                action();
            });
        }
    }
}
