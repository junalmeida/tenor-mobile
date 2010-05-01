using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
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

        private static IHaptic CreateHaptics()
        {
            if (!HapticFeedback)
                return null;
            else if (Manufacturer.IndexOf("samsung", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return new Samsung.Haptic();
            }
            else
            {
                return null;
            }
        }


        public static void HapticSoft()
        {
            IHaptic haptic = CreateHaptics();
            if (haptic != null)
                haptic.Soft();
        }


        public static bool HapticFeedback
        {
            get
            {
                return false; //since we don't have samsung api key.
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
