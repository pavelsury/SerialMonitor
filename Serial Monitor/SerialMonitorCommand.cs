using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace Serial_Monitor
{
    internal sealed class SerialMonitorCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("edf12506-fbfd-4d38-99e4-f64c7ac9d468");

        public static SerialMonitorCommand Instance { get; private set; }

        public static void Initialize(Package package) => Instance = new SerialMonitorCommand(package);

        private IServiceProvider ServiceProvider => _package;

        private SerialMonitorCommand(Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandId = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(ShowToolWindow, menuCommandId);
                commandService.AddCommand(menuItem);
            }
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var window = _package.FindToolWindow(typeof(SerialMonitor), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private readonly Package _package;
    }
}
