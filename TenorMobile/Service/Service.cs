using System;

using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.WindowsCE.Forms;

namespace Tenor.Mobile
{
    public static class Service
    {
        private static bool messageLoop;
        public static void Run()
        {
            // start up the message pump
            messageLoop = true;
            while (Pump()) { };
            messageLoop = false;
        }

        private static ArrayList messageFilters = new ArrayList();
        private static bool process;
        private static NativeMethods.MSG msg;
        private static bool Pump()
        {
            ArrayList MyMessageFilters = new ArrayList();
            // there are, so get the top one
            if (NativeMethods.GetMessage(out msg, IntPtr.Zero, 0, 0))
            {
                process = true;
                MyMessageFilters = (ArrayList)(messageFilters.Clone());
                // iterate any filters
                lock (messageFilters.SyncRoot)
                {
                    foreach (NativeMethods.IMessageFilter mf in MyMessageFilters)
                    {
#if !NDOC && !DESIGN
                        Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

                        // if *any* filter says not to process, we won't
                        process = process ? !(mf.PreFilterMessage(ref m)) : false;
#endif
                    }

                    // if we're supposed to process the message, do so
                    if (process)
                    {
                        NativeMethods.TranslateMessage(out msg);
                        NativeMethods.DispatchMessage(ref msg);
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
