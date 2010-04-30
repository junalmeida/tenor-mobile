using System;

using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Tenor.Mobile.Device
{
    public enum LedStatus
    {
        Off = 0,
        On,
        Blink
    }

    internal static class Leds
    {
        class NLED_SETTINGS_INFO
        {
            public uint LedNum;
            public uint OffOnBlink;
            public int TotalCycleTime;
            public int OnTime;
            public int OffTime;
            public int MetaCycleOn;
            public int MetaCycleOff;
        }

        class NLED_COUNT_INFO
        {
            public int cLeds;
        }

        const int NLED_COUNT_INFO_ID = 0;
        const int NLED_SETTINGS_INFO_ID = 2;



        [DllImport("coredll.dll")]
        private static extern bool NLedGetDeviceInfo(uint nID, NLED_COUNT_INFO pOutput);

        [DllImport("coredll.dll")]
        private static extern bool NLedSetDevice(uint nID, NLED_SETTINGS_INFO pOutput);

        private static int _ledCount;

        static Leds()
        {
            _ledCount = GetLedCount();
        }

        private static void SetLedStatus(LedStatus status)
        {
            NLED_SETTINGS_INFO nsi = new NLED_SETTINGS_INFO();
            nsi.OffOnBlink = (uint)status;
            for (int i = 0; i < _ledCount; i++)
            {
                nsi.LedNum = (uint)i;
                NLedSetDevice(NLED_SETTINGS_INFO_ID, nsi);
            }
        }

        public static void Vibrate(int millisecondsTimeout)
        {
            Thread t = new Thread(new ThreadStart(delegate()
            {
                SetLedStatus(LedStatus.On);
                Thread.Sleep(millisecondsTimeout);
                SetLedStatus(LedStatus.Off);
            }));
            t.Start();
        }


        private static int GetLedCount()
        {
            int count = 0;
            NLED_COUNT_INFO nci = new NLED_COUNT_INFO();
            if (NLedGetDeviceInfo(NLED_COUNT_INFO_ID, nci))
            {
                count = nci.cLeds;
            }
            return count;
        }


    }
}
