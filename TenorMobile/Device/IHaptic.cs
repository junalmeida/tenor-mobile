using System;
using System.Collections.Generic;
using System.Text;

namespace Tenor.Mobile.Device
{
    interface IHaptic 
    {
        void Soft();

        void Soft(uint period);
    }
}
