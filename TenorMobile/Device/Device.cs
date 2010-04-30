using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SamsungMobileSdk;
using System.Threading;

namespace Tenor.Mobile.Device
{
    public static class Device
    {
        /// <summary>
        /// Gets the string os OEM.
        /// </summary>
        public static string OemInfo
        {
            get
            {
                return GetSystemParameter(SPI_GETOEMINFO);
            }
        }

        public static string Manufacturer
        {
            get
            {
                return GetSystemParameter(SPI_GETPLATFORMMANUFACTURER);
            }
        }

        [DllImport("coredll.dll")]
        private static extern int SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWiniIni);
        private const uint SPI_GETPLATFORMTYPE = 257;
        private const uint SPI_GETPLATFORMMANUFACTURER = 262;
        private const uint SPI_GETOEMINFO = 258;
        private static string GetSystemParameter(uint uiParam)
        {
            StringBuilder sb = new StringBuilder(128);
            if (SystemParametersInfo(uiParam, (uint)sb.Capacity, sb, 0) == 0)
                throw new ApplicationException("Failed to get system parameter");
            return sb.ToString();
        }

        /// <summary>
        /// Vibrates the device for milliseconds on <paramref name="timeout"/>.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds.</param>
        public static void Vibrate(int timeout)
        {
            Leds.Vibrate(timeout);
        }

        public static void HapticSoft()
        {
            if (!HapticFeedback)
                return;

            try
            {
                if (Manufacturer.IndexOf("samsung", StringComparison.InvariantCultureIgnoreCase) > -1)
                {

                    int handle = 0;
                    if (SamsungMobileSdk.Haptics.Open(ref handle) == SamsungMobileSdk.SmiResultCode.Success)
                    {
                        Thread t = new Thread(new ThreadStart(delegate()
                        {
                            try
                            {
                                SamsungMobileSdk.Haptics.HapticsNote[] _hapticsNotes = new Haptics.HapticsNote[1];
                                _hapticsNotes[0].magnitude = 255;
                                _hapticsNotes[0].startingMagnitude = 0;
                                _hapticsNotes[0].endingMagnitude = 0;
                                _hapticsNotes[0].duration = 100;
                                _hapticsNotes[0].endTimeDuration = 0;
                                _hapticsNotes[0].startTimeDuration = 0;
                                _hapticsNotes[0].style = Haptics.NoteStyle.Sharp;
                                _hapticsNotes[0].period = 0;

                                Haptics.PlayNotes(handle, 1, _hapticsNotes, false, null);

                            }
                            finally
                            {
                                SamsungMobileSdk.Haptics.Close(handle);
                            }
                        }));
                        t.Start();
                    }
                    else
                    {
                        Vibrate(40);
                    }
                }
                else
                {
                    Vibrate(40);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "VibrateSoft");
                Vibrate(40);
            }
        }


        public static bool HapticFeedback
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("ControlPanel\\TouchVibration"))
                {
                    if (key == null)
                        return false;
                    else
                        return ((int)key.GetValue("TouchVibrateEnabled", 0)) > 0;
                }
            }
        }

    }
}
