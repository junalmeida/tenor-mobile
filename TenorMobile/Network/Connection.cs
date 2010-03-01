using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;

namespace Tenor.Mobile.Network
{

    /// <summary>
    /// Manages a network connection.
    /// </summary>
    public class Connection : IDisposable //: Component
    {
        #region "Native Connection Manager APIs"
        [DllImport("CellCore.dll", EntryPoint = "ConnMgrMapURL", SetLastError = true)]
        private static extern int ConnMgrMapURL(string url, ref Guid networkGuid, int passZero);

        [DllImport("CellCore.dll", EntryPoint = "ConnMgrEstablishConnection", SetLastError = true)]
        private static extern int ConnMgrEstablishConnection(ConnMgrConnectionInfo connectionInfo, ref IntPtr connectionHandle);

        [DllImport("CellCore.dll", EntryPoint = "ConnMgrEstablishConnectionSync", SetLastError = true)]
        private static extern int ConnMgrEstablishConnectionSync(ConnMgrConnectionInfo connectionInfo, ref IntPtr connectionHandle, uint dwTimeout, ref Status dwStatus);

        [DllImport("CellCore.dll", EntryPoint = "ConnMgrReleaseConnection", SetLastError = true)]
        private static extern int ConnMgrReleaseConnection(IntPtr connectionHandle, int cache);

        [DllImport("CellCore.dll", EntryPoint = "ConnMgrConnectionStatus", SetLastError = true)]
        private static extern int ConnMgrConnectionStatus(IntPtr connectionHandle, ref Status status);

        [DllImport("CellCore.dll", EntryPoint = "ConnMgrMapConRef", SetLastError = true)]
        private static extern int ConnMgrMapConRef(ConnMgrConRefTypeEnum e, string szConRef, ref Guid pGUID);

        #endregion

        #region "Constants & Structures"
        const int syncConnectTimeout = 60000; // 60 seconds

        [Flags]
        private enum ConnMgrParam : int
        {
            GuidDestNet = 0x1,
            MaxCost = 0x2,
            MinRcvBw = 0x4,
            MaxConnLatency = 0x8
        }

        [Flags]
        private enum ConnMgrProxy : int
        {
            NoProxy = 0x0,
            Http = 0x1,
            Wap = 0x2,
            Socks4 = 0x4,
            Socks5 = 0x8
        }

        private enum ConnMgrPriority
        {
            UserInteractive = 0x8000,
            HighPriorityBackground = 0x0200,
            LowPriorityBackground = 0x0008
        }



        private enum ConnMgrConRefTypeEnum
        {
            ConRefType_NAP = 0,
            ConRefType_PROXY
        }


        [StructLayout(LayoutKind.Sequential)]
        private class ConnMgrConnectionInfo
        {
            Int32 cbSize;                          // DWORD
            public ConnMgrParam dwParams = 0;      // DWORD
            public ConnMgrProxy dwFlags = 0;       // DWORD
            public ConnMgrPriority dwPriority = 0; // DWORD
            public Int32 bExclusive = 0;           // BOOL
            public Int32 bDisabled = 0;            // BOOL
            public Guid guidDestNet = Guid.Empty;  // GUID
            public IntPtr hWnd = IntPtr.Zero;      // HWND
            public UInt32 uMsg = 0;                // UINT
            public Int32 lParam = 0;               // LPARAM
            public UInt32 ulMaxCost = 0;           // ULONG
            public UInt32 ulMinRcvBw = 0;          // ULONG
            public UInt32 ulMaxConnLatency = 0;    // ULONG 

            public ConnMgrConnectionInfo()
            {
                cbSize = Marshal.SizeOf(typeof(ConnMgrConnectionInfo));
            }

            public ConnMgrConnectionInfo(Guid destination, ConnMgrPriority priority, ConnMgrProxy proxy)
                : this()
            {
                guidDestNet = destination;
                dwParams = ConnMgrParam.GuidDestNet;
                dwPriority = priority;
                dwFlags = proxy;
            }

            public ConnMgrConnectionInfo(Guid destination, ConnMgrPriority priority)
                : this(destination, priority, ConnMgrProxy.NoProxy) { }

            public ConnMgrConnectionInfo(Guid destination)
                : this(destination, ConnMgrPriority.UserInteractive) { }
        } ;


        #endregion


        string url;
        Guid id = new Guid("436EF144-B4FB-4863-A041-8F905A62C572"); //the internet
        Status status = Status.Unknown;

        /// <summary>
        /// Gets the current status of this connection.
        /// </summary>
        public Status Status
        {
            get { return status; }
        }

        IntPtr handle = IntPtr.Zero;
        /// <summary>
        /// Creates a new instance to manage a new connection.
        /// </summary>
        /// <param name="url"></param>
        public Connection(string url)
        {
            this.url = url;
            ConnMgrMapURL(url, ref id, 0);
        }



        private Timer timer;
        /// <summary>
        /// Asyncronously connects to network.
        /// </summary>
        public void Connect()
        {
            ConnMgrConnectionInfo info = new ConnMgrConnectionInfo(id, ConnMgrPriority.UserInteractive);
            ConnMgrEstablishConnection(info, ref handle);
            if (timer != null)
                timer.Dispose();
            timer = new Timer(new TimerCallback(CheckState), null, 100, 800);
        }

        private void CheckState(object state)
        {
            bool disableTimer = false;
            if (handle == IntPtr.Zero)
                disableTimer = true;

            else
            {
                Status newStatus = Status.Unknown;
                ConnMgrConnectionStatus(handle, ref newStatus);
                if (status != newStatus)
                {
                    status = newStatus;
                    switch (status)
                    {
                        case Status.Unknown:
                            break;
                        case Status.Connected:
                            OnConnected(new EventArgs());
                            break;
                        case Status.Suspended:
                            OnSuspended(new EventArgs());
                            break;
                        case Status.Disconnected:
                            OnDisconnected(new EventArgs());
                            disableTimer = true;
                            break;
                        case Status.ConnectionFailed:
                        case Status.ConnectionCanceled:
                        case Status.ConnectionDisabled:
                        case Status.NoPathToDestination:
                        case Status.PhoneOff:
                        case Status.ExclusiveConflict:
                        case Status.NoResources:
                        case Status.ConnectionLinkFailed:
                        case Status.NoPathWithProperty:
                        case Status.AuthenticationFailed:
                            OnFailed(new EventArgs());
                            disableTimer = true;
                            break;
                        case Status.WaitingForPath:
                        case Status.WaitingForPhone:
                        case Status.WaitingConnection:
                        case Status.WaitingForResource:
                        case Status.WaitingForNetwork:
                        case Status.WaitingDisconnection:
                        case Status.WaitingConnectionAbort:
                            break;
                        default:
                            break;
                    }
                }
            }
            if (disableTimer && timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        #region Events
        /// <summary>
        /// Occurs when the connection sucessfuly connects.
        /// </summary>
        public event EventHandler Connected;
        /// <summary>
        /// Occurs when the connection has gone.
        /// </summary>
        public event EventHandler Disconnected;
        /// <summary>
        /// Occurs when cannot connect;
        /// </summary>
        public event EventHandler Failed;
        /// <summary>
        /// Occurs when the connection is suspended due to a device operation such as a call.
        /// </summary>
        public event EventHandler Suspended;

        protected virtual void OnFailed(EventArgs eventArgs)
        {
            if (Failed != null)
                Failed(this, eventArgs);
        }

        protected virtual void OnDisconnected(EventArgs eventArgs)
        {
            if (Disconnected != null)
                Disconnected(this, eventArgs);
        }

        protected virtual void OnSuspended(EventArgs eventArgs)
        {
            if (Suspended != null)
                Suspended(this, eventArgs);
        }

        protected virtual void OnConnected(EventArgs eventArgs)
        {
            if (Connected != null)
                Connected(this, eventArgs);
        }

        #endregion

        /// <summary>
        /// Tries to disconnect this connection.
        /// </summary>
        public void Disconnect()
        {
            if (handle != IntPtr.Zero)
            {
                ConnMgrReleaseConnection(handle, 0);
                status = Status.Disconnected;
                OnDisconnected(new EventArgs());
            }
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

    }


    public enum Status
    {
        Unknown = 0x00,
        Connected = 0x10,
        Suspended = 0x11,
        Disconnected = 0x20,
        ConnectionFailed = 0x21,
        ConnectionCanceled = 0x22,
        ConnectionDisabled = 0x23,
        NoPathToDestination = 0x24,
        WaitingForPath = 0x25,
        WaitingForPhone = 0x26,
        PhoneOff = 0x27,
        ExclusiveConflict = 0x28,
        NoResources = 0x29,
        ConnectionLinkFailed = 0x2a,
        AuthenticationFailed = 0x2b,
        NoPathWithProperty = 0x2c,
        WaitingConnection = 0x40,
        WaitingForResource = 0x41,
        WaitingForNetwork = 0x42,
        WaitingDisconnection = 0x80,
        WaitingConnectionAbort = 0x81
    }
}
