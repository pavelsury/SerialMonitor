﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SerialMonitor.Business
{
    public sealed class UsbNotification : IUsbNotification, IDisposable
    {
        public static UsbNotification Instance { get; private set; } = new UsbNotification();

        public event EventHandler<bool> DeviceChanged;

        public void Dispose()
        {
            DeviceChanged = null;

            if (_buffer != IntPtr.Zero)
            {
                UnregisterDeviceNotification(_buffer);
                Marshal.FreeHGlobal(_buffer);
            }
            _window.Close();
            Instance = null;
        }

        private UsbNotification()
        {
            _window = new Window();
            _window.SourceInitialized += OnSourceInitialized;
            new WindowInteropHelper(_window).EnsureHandle();
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
            if (msg == WmDeviceChange)
            {
                switch ((int)wparam)
                {
                    case DbtDeviceRemoveComplete:
                        DeviceChanged?.Invoke(this, false);
                        break;

                    case DbtDeviceArrival:
                        DeviceChanged?.Invoke(this, true);
                        break;
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

        private readonly Window _window;
        private IntPtr _buffer;
        private const int DbtDevtypDeviceinterface = 5;
        private const int DbtDeviceArrival = 0x8000;
        private const int DbtDeviceRemoveComplete = 0x8004;
        private const int WmDeviceChange = 0x0219;
        private static readonly Guid GuidDevinterfaceUsbDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
    }
}
