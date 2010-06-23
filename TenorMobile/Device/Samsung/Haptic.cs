using System;

using System.Collections.Generic;
using System.Text;
using SamsungMobileSdk;
using System.Threading;

namespace Tenor.Mobile.Device.Samsung
{
    class Haptic : IHaptic
    {
        private static int handle = 0;
        public Haptic()
        {
            if (handle == 0)
            {
                SmiResultCode result;
                try
                {
                    byte[] key = Encoding.ASCII.GetBytes("XKNS7KYGREPRY2MATZ7VRFLRSZK74DT2");
                    SamsungMobileSdk.Haptics.SetKey(key, key.Length);
                    result = SamsungMobileSdk.Haptics.Open(ref handle);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Cannot open Haptic Hardware, " + ex.Message, ex);
                }
                if (handle == 0)
                    throw new InvalidOperationException("Cannot open Haptic Hardware: " + result.ToString());
            }
        }

        ~Haptic()
        {
            try
            {
                if (handle > 0)
                    SamsungMobileSdk.Haptics.Close(handle);
            }
            catch { }
        }

        public void Soft()
        {
            try
            {

                Thread t = new Thread(new ThreadStart(delegate()
                 {

                     Haptics.HapticsNote[] _hapticsNotes = new Haptics.HapticsNote[1];
                     _hapticsNotes[0].magnitude = 20;

                     _hapticsNotes[0].startingMagnitude = 0;
                     _hapticsNotes[0].startTimeDuration = 0;

                     _hapticsNotes[0].duration = 100;

                     _hapticsNotes[0].endTimeDuration = 0;
                     _hapticsNotes[0].endingMagnitude = 0;

                     _hapticsNotes[0].style = Haptics.NoteStyle.Sharp;
                     _hapticsNotes[0].period = 2;

                     Haptics.PlayNotes(handle, 1, _hapticsNotes, false, null);


                 }));
                t.Start();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "Haptic.Soft");
            }
        }

        Timer timer = null;
        public void Soft(uint period)
        {
            try
            {
                if (period == 0 && timer != null)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    timer = null;
                }
                else if (period > 0 && timer == null)
                {
                    timer = new Timer(new TimerCallback(TimerHit), null, 0, period);
                }
                else if (period > 0)
                {
                    timer.Change(0, period);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "Haptic.Soft");
            }
        }

        private void TimerHit(object state)
        {
            Haptics.HapticsNote[] _hapticsNotes = new Haptics.HapticsNote[1];
            _hapticsNotes[0].magnitude = 20;

            _hapticsNotes[0].startingMagnitude = 0;
            _hapticsNotes[0].startTimeDuration = 0;

            _hapticsNotes[0].duration = 50;

            _hapticsNotes[0].endTimeDuration = 0;
            _hapticsNotes[0].endingMagnitude = 0;

            _hapticsNotes[0].style = Haptics.NoteStyle.Sharp;
            _hapticsNotes[0].period = 50;

            Haptics.PlayNotes(handle, 1, _hapticsNotes, false, null);

        }

    }
}
