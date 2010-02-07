using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Tenor.Mobile.Diagnostics
{
    internal static class NativeMethods
    {
        internal const int TH32CS_SNAPPROCESS = 0x00000002;
        internal const int TH32CS_SNAPMODULE = 8;
 

        [DllImport("toolhelp.dll")]
        internal static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);
        [DllImport("toolhelp.dll")]
        internal static extern int CloseToolhelp32Snapshot(IntPtr handle);
        [DllImport("toolhelp.dll")]
        internal static extern int Process32First(IntPtr handle, byte[] pe);
        [DllImport("toolhelp.dll")]
        internal static extern int Process32Next(IntPtr handle, byte[] pe);
        [DllImport("coredll.dll")]
        internal static extern IntPtr OpenProcess(int flags, bool fInherit, int PID);
        internal const int PROCESS_TERMINATE = 1;
        [DllImport("coredll.dll")]
        internal static extern bool TerminateProcess(IntPtr hProcess, uint ExitCode);
        [DllImport("coredll.dll")]
        internal static extern bool CloseHandle(IntPtr handle);
        internal const int INVALID_HANDLE_VALUE = -1;

        [DllImport("toolhelp.dll", SetLastError = true)]
        internal static extern int Module32Next(IntPtr handle, byte[] me);
        [DllImport("toolhelp.dll", SetLastError = true)]
        internal static extern int Module32First(IntPtr handle, byte[] me);
 
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern uint GetModuleFileName(IntPtr hModule,
           [MarshalAs(UnmanagedType.LPWStr)]
           string lpFileName,
           uint nSize);

        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(uint fdwAccess, bool fInherit, uint IDProcess);

 



        internal class PROCESSENTRY32
        {
            private const int AccessKeyOffset = 560;
            public uint cntThreads;
            public uint cntUsage;
            private const int DefaultHeapIDOffset = 12;
            public uint dwFlags;
            private const int dwFlagsOffset = 0x20;
            public uint dwSize;
            private const int ExeFileOffset = 0x24;
            private const int MAX_PATH = 260;
            private const int MemoryBaseOffset = 0x22c;
            private const int ModuleIDOffset = 0x10;
            private const int ParentProcessIDOffset = 0x18;
            public long pcPriClassBase;
            private const int PriClassBaseOffset = 0x1c;
            private const int ProcessIDOffset = 8;
            private const uint SizeCE = 0x234;
            private const int SizeOffset = 0;
            private const uint SizeWin32 = 300;
            public string szExeFile;
            public uint th32AccessKey;
            public uint th32DefaultHeapID;
            public uint th32MemoryBase;
            public uint th32ModuleID;
            public uint th32ParentProcessID;
            public uint th32ProcessID;
            private const int ThreadsOffset = 20;
            private const int UsageOffset = 4;

            public PROCESSENTRY32()
            {
                this.dwSize = (Environment.OSVersion.Platform == PlatformID.WinCE) ? (uint)0x234 : (uint)300;
            }

            public PROCESSENTRY32(byte[] aData)
            {
                this.dwSize = GetUInt(aData, 0);
                this.cntUsage = GetUInt(aData, 4);
                this.th32ProcessID = GetUInt(aData, 8);
                this.th32DefaultHeapID = GetUInt(aData, 12);
                this.th32ModuleID = GetUInt(aData, 0x10);
                this.cntThreads = GetUInt(aData, 20);
                this.th32ParentProcessID = GetUInt(aData, 0x18);
                this.pcPriClassBase = GetUInt(aData, 0x1c);
                this.dwFlags = GetUInt(aData, 0x20);
                this.szExeFile = GetString(aData, 0x24, 260).TrimEnd(new char[1]);
                if (Environment.OSVersion.Platform == PlatformID.WinCE)
                {
                    this.th32MemoryBase = GetUInt(aData, 0x22c);
                    this.th32AccessKey = GetUInt(aData, 560);
                }
            }

            public byte[] ToByteArray()
            {
                byte[] buffer;
                if (Environment.OSVersion.Platform == PlatformID.WinCE)
                {
                    buffer = new byte[0x234];
                }
                else
                {
                    buffer = new byte[300];
                }
                SetUInt(buffer, 0, (Environment.OSVersion.Platform == PlatformID.WinCE) ? 0x234 : 300);
                return buffer;
            }

            public uint BaseAddress
            {
                get
                {
                    return this.th32MemoryBase;
                }
            }

            public string ExeFile
            {
                get
                {
                    return this.szExeFile;
                }
            }

            public ulong ProcessID
            {
                get
                {
                    return (ulong)this.th32ProcessID;
                }
            }

            public ulong ThreadCount
            {
                get
                {
                    return (ulong)this.cntThreads;
                }
            }
        }

        internal class MODULEENTRY32
        {
            private const int BASE_ADDR_LEN = 4;
            private const int BASE_ADDR_OFFSET = 20;
            private const int BASE_SIZE_LEN = 4;
            private const int BASE_SIZE_OFFSET = 0x18;
            public uint dwFlags;
            public uint dwSize;
            private const int EXE_PATH_LEN = 520;
            private const int EXE_PATH_OFFSET = 0x228;
            private const int FLAGS_LEN = 4;
            private const int FLAGS_OFFSET = 0x430;
            public uint GlblcntUsage;
            private const int GLOBAL_COUNT_LEN = 4;
            private const int GLOBAL_COUNT_OFFSET = 12;
            public uint hModule;
            private const int HMODULE_LEN = 4;
            private const int HMODULE_OFFSET = 0x1c;
            private const int MAX_PATH = 260;
            public uint modBaseAddr;
            public uint modBaseSize;
            private const int MODULE_ID_LEN = 4;
            private const int MODULE_ID_OFFSET = 4;
            private const int MODULE_NAME_LEN = 520;
            private const int MODULE_NAME_OFFSET = 0x20;
            public uint ProccntUsage;
            private const int PROCESS_COUNT_LEN = 4;
            private const int PROCESS_COUNT_OFFSET = 0x10;
            private const int PROCESS_ID_LEN = 4;
            private const int PROCESS_ID_OFFSET = 8;
            private const int Size = 0x434;
            private const int SIZE_LEN = 4;
            private const int SIZE_OFFSET = 0;
            public string szExePath;
            public string szModule;
            public uint th32ModuleID;
            public uint th32ProcessID;

  
            public MODULEENTRY32()
            {
            }

            public MODULEENTRY32(byte[] aData)
            {
                this.dwSize = GetUInt(aData, 0);
                this.th32ModuleID = GetUInt(aData, 4);
                this.th32ProcessID = GetUInt(aData, 8);
                this.GlblcntUsage = GetUInt(aData, 12);
                this.ProccntUsage = GetUInt(aData, 0x10);
                this.modBaseAddr = GetUInt(aData, 20);
                this.modBaseSize = GetUInt(aData, 0);
                this.hModule = GetUInt(aData, 0x1c);
                this.szModule = GetString(aData, 0x20, 520).TrimEnd(new char[1]);
                this.szExePath = GetString(aData, 0x228, 520).TrimEnd(new char[1]);
                this.dwFlags = GetUInt(aData, 0x1c);
            }

            public byte[] ToByteArray()
            {
                byte[] aData = new byte[0x434];
                SetUInt(aData, 0, 0x434);
                return aData;
            }
        }

 


       
        internal static string GetString(byte[] aData, int Offset, int Length)
        {
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                return Encoding.Unicode.GetString(aData, Offset, Length);
            }
            return Encoding.ASCII.GetString(aData, Offset, Length);
        }

        internal static uint GetUInt(byte[] aData, int Offset)
        {
            return BitConverter.ToUInt32(aData, Offset);
        }

        internal static ulong GetULng(byte[] aData, int Offset)
        {
            return BitConverter.ToUInt64(aData, Offset);
        }

        internal static ushort GetUShort(byte[] aData, int Offset)
        {
            return BitConverter.ToUInt16(aData, Offset);
        }

        internal static void SetString(byte[] aData, int Offset, string Value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(Value);
            Buffer.BlockCopy(bytes, 0, aData, Offset, bytes.Length);
        }

        internal static void SetUInt(byte[] aData, int Offset, int Value)
        {
            byte[] bytes = BitConverter.GetBytes(Value);
            Buffer.BlockCopy(bytes, 0, aData, Offset, bytes.Length);
        }

        internal static void SetUShort(byte[] aData, int Offset, int Value)
        {
            byte[] bytes = BitConverter.GetBytes((short)Value);
            Buffer.BlockCopy(bytes, 0, aData, Offset, bytes.Length);
        }



        #region Window Enumerator

        [DllImport("coredll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("coredll.dll")]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder buf, int nMaxCount);

        [DllImport("coredll.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool EnumWindows(IntPtr lpEnumFunc, uint lParam);

        /*
         DWORD GetWindowThreadProcessId(
HWND hWnd, 
LPDWORD lpdwProcessId );
         */
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, ref IntPtr lpdwProcessId);

        internal delegate int EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        internal static void ListWindows(EnumWindowsProc callback, uint id)
        {
            IntPtr callbackDelegatePointer;
            callbackDelegatePointer = Marshal.GetFunctionPointerForDelegate(callback);

            EnumWindows(callbackDelegatePointer, id);
        }

        internal static string GetWindowText(IntPtr handle)
        {
            StringBuilder sb = new StringBuilder(255);
            GetWindowText(handle, sb, sb.Capacity);
            return sb.ToString().Trim();
        }

        internal static string GetWindowClass(IntPtr handle)
        {
            StringBuilder sb = new StringBuilder(255);
            GetClassName(handle, sb, sb.Capacity);
            return sb.ToString().Trim();
        }
        #endregion

    }
}
