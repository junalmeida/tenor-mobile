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

        private static bool hapticFailed = false;
        private static IHaptic CreateHaptics()
        {
            if (!HapticFeedback)
                return null;
            else if (Manufacturer.IndexOf("samsung", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                try
                {
                    return new Samsung.Haptic();
                }
                catch (InvalidOperationException)
                {
                    //do we really need to fail silently?
                    hapticFailed = true;
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static IHaptic haptic;

        public static void HapticSoft()
        {
            if (haptic == null)
                haptic = CreateHaptics();
            if (haptic != null)
                haptic.Soft();

        }


        public static void HapticSoft(uint period)
        {
            if (haptic == null)
                haptic = CreateHaptics();

            if (haptic != null)
                haptic.Soft(period);
        }



        public static bool HapticFeedback
        {
            get
            {
                if (hapticFailed)
                    return false;
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
