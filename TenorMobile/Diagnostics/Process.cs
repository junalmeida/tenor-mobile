using System;

using System.Collections.Generic;
using System.Text;

namespace Tenor.Mobile.Diagnostics
{
    /// <summary>
    /// Represents a running process.
    /// </summary>
    public class Process
    {
        private Process()
            : base()
        {
        }

        NativeMethods.PROCESSENTRY32 pEntry;
        private Process(NativeMethods.PROCESSENTRY32 pEntry)
        {
            this.pEntry = pEntry;
        }

        /// <summary>
        /// Immediately stops this process.
        /// </summary>
        public void Kill()
        {
            IntPtr hProcess = NativeMethods.OpenProcess(NativeMethods.PROCESS_TERMINATE, false, this.Id);
            if (hProcess.ToInt32() != -1)
            {
                NativeMethods.TerminateProcess(hProcess, 0);
                NativeMethods.CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// Starts a process by specifying the file name and a set of arguments.
        /// </summary>
        /// <param name="fileName">The full path of the application.</param>
        /// <param name="arguments">A set of arguments.</param>
        /// <returns>The Process that references this executable.</returns>
        public static Process Start(string fileName, string arguments)
        {
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(fileName, arguments);
            
            foreach (Process p in GetProcesses())
            {
                if (proc.Id == p.Id)
                    return p;
            }
            return null;
        }


        /// <summary>
        /// Gets the process id.
        /// </summary>
        public int Id
        {
            get { return (int)pEntry.ProcessID; }
        }

        /// <summary>
        /// Gets the full path of this process image.
        /// </summary>
        public string FileName
        {
            get
            {
                return pEntry.ExeFile;
            }
        }




        /// <summary>
        /// Lists current running process.
        /// </summary>
        /// <returns></returns>
        public static Process[] GetProcesses()
        {
            List<Process> list = new List<Process>();
            IntPtr handle = NativeMethods.CreateToolhelp32Snapshot(NativeMethods.TH32CS.SNAPPROCESS, 0);
            if (((int)handle) <= 0)
            {
                int error = Tenor.Mobile.NativeMethods.GetLastError();
                throw new Exception(string.Format("Unable to create snapshot. Error {0}.", error));
            }
            try
            {
                byte[] pe = new NativeMethods.PROCESSENTRY32().ToByteArray();
                byte[] array = new byte[pe.Length];
                pe.CopyTo(array, 0);
                for (int i = NativeMethods.Process32First(handle, pe); i == 1; i = NativeMethods.Process32Next(handle, pe))
                {
                    NativeMethods.PROCESSENTRY32 processentry = new NativeMethods.PROCESSENTRY32(pe);
                    Process proc = new Process(processentry);
                    list.Add(proc);
                }
            }
            finally
            {
                NativeMethods.CloseToolhelp32Snapshot(handle);
            }

            foreach (Process p in list)
            {
                try
                {
                    p.pEntry.szExeFile = GetProcessPath((uint)p.pEntry.ProcessID);
                }
                catch { }
            }

            return list.ToArray();
        }

        private static string GetProcessPath(uint processID)
        {
            string path = new string((char)0x20, 255);
            NativeMethods.GetModuleFileName((IntPtr)(int)processID, path, (uint)path.Length);
            int slash = path.LastIndexOf('\0');
            if (slash > -1)
                path = path.Remove(slash, path.Length - slash);
            return path;
        }

        private static string[] GetModules(uint processID)
        {
            List<string> list = new List<string>();
            IntPtr handle = NativeMethods.CreateToolhelp32Snapshot(NativeMethods.TH32CS.SNAPMODULE, processID);
            if (handle.ToInt32() <= 0)
            {
                throw new Exception("Unable to create snapshot");
            }
            try
            {
                byte[] me = new NativeMethods.MODULEENTRY32().ToByteArray();
                for (int i = NativeMethods.Module32First(handle, me); i == 1; i = NativeMethods.Module32Next(handle, me))
                {
                    NativeMethods.MODULEENTRY32 moduleentry = new NativeMethods.MODULEENTRY32(me);

                    string path = new string((char)0x20, 255);
                    NativeMethods.GetModuleFileName((IntPtr)(int)moduleentry.hModule, path, (uint)path.Length);
                    int slash = path.LastIndexOf('\0');
                    if (slash > -1)
                        path = path.Remove(slash, path.Length - slash);
                    moduleentry.szExePath = path;


                    list.Add(moduleentry.szExePath);
                }
            }
            finally
            {
                NativeMethods.CloseToolhelp32Snapshot(handle);
            }
            return list.ToArray();
        }

 


    }
}
