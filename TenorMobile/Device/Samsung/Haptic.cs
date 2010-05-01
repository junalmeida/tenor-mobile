using System;

using System.Collections.Generic;
using System.Text;
using SamsungMobileSdk;
using System.Threading;

namespace Tenor.Mobile.Device.Samsung
{
    class Haptic : IHaptic
    {

        public void Soft()
        {
            try
            {

                int handle = 0;
                if (SamsungMobileSdk.Haptics.Open(ref handle) == SamsungMobileSdk.SmiResultCode.Success)
                {
                    Thread t = new Thread(new ThreadStart(delegate()
                    {
                        try
                        {
                            Haptics.HapticsNote[] _hapticsNotes = new Haptics.HapticsNote[1];
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, "VibrateSoft");
            }
        }

    }
}
