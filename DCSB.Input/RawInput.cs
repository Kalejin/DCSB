using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DCSB.Input
{
    public class RawInput : NativeWindow
    {
        static RawKeyboard _keyboardDriver;
        readonly IntPtr _devNotifyHandle;
        static readonly Guid DeviceInterfaceHid = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        private PreMessageFilter _filter;

        public  event RawKeyboard.DeviceEventHandler KeyPressed
        {
            add { _keyboardDriver.KeyPressed += value; }
            remove { _keyboardDriver.KeyPressed -= value;}
        }

        public int NumberOfKeyboards
        {
            get { return _keyboardDriver.NumberOfKeyboards; } 
        }

        public bool CaptureOnlyIfTopMostWindow
        {
            get { return _keyboardDriver.CaptureOnlyIfTopMostWindow; }
            set { _keyboardDriver.CaptureOnlyIfTopMostWindow = value; }
        }

        public void AddMessageFilter()
        {
            if (null != _filter) return;

            _filter = new PreMessageFilter();
            Application.AddMessageFilter(_filter);
        }

        public void RemoveMessageFilter()
        {
            if (null == _filter) return;

            Application.RemoveMessageFilter(_filter);
        }

        public RawInput()
        {
            throw new NotSupportedException("Call the overloaded contructor with a Window handle.");
        }

        public RawInput(IntPtr parentHandle)
        {
            AssignHandle(parentHandle); 

            _keyboardDriver = new RawKeyboard(parentHandle);
            _keyboardDriver.EnumerateDevices();
            _devNotifyHandle = RegisterForDeviceNotifications(parentHandle);
        }

        static IntPtr RegisterForDeviceNotifications(IntPtr parent)
        {
            var usbNotifyHandle = IntPtr.Zero;
            var bdi = new BroadcastDeviceInterface();
            bdi.dbcc_size = Marshal.SizeOf(bdi);
            bdi.BroadcastDeviceType = BroadcastDeviceType.DBT_DEVTYP_DEVICEINTERFACE;
            bdi.dbcc_classguid = DeviceInterfaceHid;

            var mem = IntPtr.Zero;
            try
            {
                mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BroadcastDeviceInterface)));
                Marshal.StructureToPtr(bdi, mem, false);
                usbNotifyHandle = Win32.RegisterDeviceNotification(parent, mem, DeviceNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            catch (Exception e)
            {
                Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
                Debug.Print(e.StackTrace);
            }
            finally
            {
                Marshal.FreeHGlobal(mem);
            }

            if (usbNotifyHandle == IntPtr.Zero)
            {
                Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
            }
            
            return usbNotifyHandle;
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case Win32.WM_INPUT:
                    {
                        // Should never get here if you are using PreMessageFiltering
                        _keyboardDriver.ProcessRawInput(message.LParam);
                    }
                    break;

                case Win32.WM_USB_DEVICECHANGE:
                    {
                        Debug.WriteLine("USB Device Arrival / Removal");
                        _keyboardDriver.EnumerateDevices();
                    }
                    break;
            }

            base.WndProc(ref message);
        }
        
        private class PreMessageFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg != Win32.WM_INPUT)
                {
                    // Allow any non WM_INPUT message to pass through
                    return false;
                }
                
                return _keyboardDriver.ProcessRawInput(m.LParam);
            }
        }
        
        ~RawInput()
        {
            Win32.UnregisterDeviceNotification(_devNotifyHandle);
        }
    }
}
