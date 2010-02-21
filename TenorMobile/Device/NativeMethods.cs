using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Tenor.Mobile.Device
{
    internal static class NativeMethods
    {
        internal enum NLED_LEDS : int
        {
            Vibrate = 1
        }

        internal enum OffOnBlink : int
        {
            Off =0,
            On = 1,
            Blink = 2
        }

        internal struct NLED_SETTINGS_INFO
        {
            /// <summary>
            /// LED number. The first LED is zero (0).
            /// </summary>
            public int LedNum;

            /// <summary>
            /// Current setting. The following table shows the defined values. 
            /// </summary>
            public OffOnBlink OffOnBlink;

            /// <summary>
            /// Total cycle time of a blink, in microseconds.
            /// </summary>
            public int TotalCycleTime;

            /// <summary>
            /// On time of the cycle, in microseconds.
            /// </summary>
            public int OnTime;

            /// <summary>
            /// Off time of the cycle, in microseconds.
            /// </summary>
            public int OffTime;

            /// <summary>
            /// Number of on blink cycles.
            /// </summary>
            public int MetaCycleOn;

            /// <summary>
            /// Number of off blink cycles.
            /// </summary>
            public int MetaCycleOff;
        }

        /// <summary>
        /// For vibrate it is generally ID 1.
        /// </summary>
        /// <param name="deviceId">DeviceID is the physical ID of the LED you wish to access.</param>
        /// <param name="info"></param>
        /// <returns></returns>
        [DllImport("coredll")]
        internal extern static bool NLedSetDevice(NLED_LEDS led, ref NLED_SETTINGS_INFO info); 

    }
}
