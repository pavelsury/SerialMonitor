using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace SerialMonitor
{
    internal static class CommandHelper
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            var commandService = (IMenuCommandService)await package.GetServiceAsync(typeof(IMenuCommandService));
            if (commandService == null)
            {
                throw new ArgumentNullException(nameof(commandService));
            }

            const int commandIdValue = 0x0100;
            var commandId = new CommandID(Guid.Parse("edf12506-fbfd-4d38-99e4-f64c7ac9d468"), commandIdValue);
            var menuCommand = new MenuCommand((s, e) => ThreadHelper.JoinableTaskFactory.Run(async () => await ShowToolWindowAsync(package)), commandId);
            commandService.AddCommand(menuCommand);
        }

        private static async Task ShowToolWindowAsync(Package package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            
            var window = (ToolWindow)package.FindToolWindow(typeof(ToolWindow), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
