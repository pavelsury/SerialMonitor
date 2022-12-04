using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using SerialMonitor.Business;

namespace SerialMonitor.Win.Business
{
    public sealed class UsbNotification : IUsbNotification
    {
        public UsbNotification()
        {
            _window = new Window();
            _window.SourceInitialized += OnSourceInitialized;
            new WindowInteropHelper(_window).EnsureHandle();
        }

        public event EventHandler<string> PortRemoved;
        public event EventHandler<string> PortArrived;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            PortRemoved = null;
            PortArrived = null;

            if (_buffer != IntPtr.Zero)
            {
                UnregisterDeviceNotification(_buffer);
                Marshal.FreeHGlobal(_buffer);
            }
            _window.Close();
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(_window).Handle);
            if (source != null)
            {
                source.AddHook(HwndHandler);
                RegisterUsbDeviceNotification(source.Handle);
            }
        }

        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == WmDeviceChange && !_isDisposed)
            {
                EventHandler<string> portEvent = null;

                switch ((int)wparam)
                {
                    case DbtDeviceRemoveComplete: portEvent = PortRemoved; break;
                    case DbtDeviceArrival: portEvent = PortArrived; break;
                }

                var portName = GetPortName(lparam);
                if (portName != null)
                {
                    portEvent?.Invoke(this, portName);
                }
            }

            handled = false;
            return IntPtr.Zero;
        }

        private void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            var dbi = new DevBroadcastDeviceinterface
            {
                DeviceType = DbtDevtypDeviceinterface,
                Reserved = 0,
                ClassGuid = GuidDevinterfaceUsbDevice,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            _buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, _buffer, true);

            RegisterDeviceNotification(windowHandle, _buffer, 0);
        }

        private static string GetPortName(IntPtr lparam)
        {
            var dbh = (DevBroadcastHdr)Marshal.PtrToStructure(lparam, typeof(DevBroadcastHdr));
            if (dbh?.dbch_devicetype != 3)
            {
                return null;
            }

            const int sizeOfDbh = 12;
            var portNameBytes = new byte[dbh.dbch_size - sizeOfDbh];
            Marshal.Copy(lparam + sizeOfDbh, portNameBytes, 0, portNameBytes.Length);
            var portName = Encoding.Unicode.GetString(portNameBytes).TrimEnd('\0');
            return portName;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct DevBroadcastDeviceinterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DevBroadcastHdr
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }

        private readonly Window _window;
        private IntPtr _buffer;
        private bool _isDisposed;
        private const int DbtDevtypDeviceinterface = 5;
        private const int DbtDeviceArrival = 0x8000;
        private const int DbtDeviceRemoveComplete = 0x8004;
        private const int WmDeviceChange = 0x0219;
        private static readonly Guid GuidDevinterfaceUsbDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
    }
}
