using System;

using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Tenor.Mobile.Device
{
    public static class Leds
    {
        private static Timer timer;
        /// <summary>
        /// Vibrates for a specific time.
        /// </summary>
        /// <param name="seconds"></param>
        public static void Vibrate(int timeout)
        {
            if (timer == null)
                timer = new Timer(new TimerCallback(VibrateOff), null, 0, timeout);
            Vibrate(true);
        }

        private static void VibrateOff(object state)
        {
            timer.Dispose(); timer = null;
            Vibrate(false);
        }


        /// <summary>
        /// Vibrates forever or disables vibration.
        /// </summary>
        /// <param name="state"></param>
        public static void Vibrate(bool state)
        {
            NativeMethods.NLED_SETTINGS_INFO info = new NativeMethods.NLED_SETTINGS_INFO();  
            
            info.LedNum = 1; //Sets the LED number to be used, this is based on the LED device capabilities but in the case of vibration shouldn't matter.
            info.OffOnBlink = state ? NativeMethods.OffOnBlink.On : NativeMethods.OffOnBlink.Off;//sets the state of the LED, with vibrate it can only be on or off but a value of 2 for blink can base used in other cases
            NativeMethods.NLedSetDevice(NativeMethods.NLED_LEDS.Vibrate, ref info);// Actually sets the device to enable vibration or disable it. the 1 subsection is the DeviceID, in this case and most phone cases this will be device 1, the second subsection is passing the information from the info structure.
        }

    }
}
