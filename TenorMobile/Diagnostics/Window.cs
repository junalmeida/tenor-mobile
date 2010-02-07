using System;

using System.Collections.Generic;
using System.Text;

namespace Tenor.Mobile.Diagnostics
{
    /// <summary>
    /// Represents a window.
    /// </summary>
    public class Window
    {
        private Window() { }

        private bool visible;
        private string text;
        private string className;
        private uint pid;

        /// <summary>
        /// Gets whether this window is visible.
        /// </summary>
        public bool Visible { get { return visible; } }
        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string Text { get { return text; } }
        /// <summary>
        /// Gets the window class name.
        /// </summary>
        public string ClassName { get { return className; } }

        /// <summary>
        /// Gets the process id that created this window.
        /// </summary>
        public int ProcessId { get { return (int)pid; } }

        private static Dictionary<int, List<Window>> getWindows;

        /// <summary>
        /// Lists all windows currently created.
        /// </summary>
        /// <returns></returns>
        public static Window[] GetWindows()
        {
            if (getWindows == null) getWindows = new Dictionary<int, List<Window>>();

            Random r = new Random();
            int id = (int)r.Next(0, 32000);
            getWindows.Add(id, new List<Window>());
            NativeMethods.ListWindows(new NativeMethods.EnumWindowsProc(Proc), (uint)id);

            List<Window> list = getWindows[id];
            getWindows.Remove(id);

            //Process[] processes = Process.GetProcesses();
            //foreach (Window w in list)
            //{
            //    foreach (Process p in processes)
            //    {
            //        if (p.Id == (int)w.pid)
            //        {
            //            w.process = p;
            //            break;
            //        }
            //    }
            //}

            return list.ToArray();
        }


        private static int Proc(IntPtr hwnd, IntPtr lParam)
        {
            IntPtr pid = IntPtr.Zero;
            NativeMethods.GetWindowThreadProcessId(hwnd, ref pid);

            getWindows[lParam.ToInt32()].Add(
                    new Window()
                    {
                        visible = NativeMethods.IsWindowVisible(hwnd),
                        text = NativeMethods.GetWindowText(hwnd),
                        className = NativeMethods.GetWindowClass(hwnd),
                        pid = (uint)pid.ToInt32()
                    }
                );
            return 1;
        }
    }
}
