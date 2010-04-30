using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace SamsungMobileSdk
{
    internal class Accelerometer
    {
        public struct Vector
        {
            public float x;
            public float y;
            public float z;
        }

        public struct Capabilities
        {
	        public uint callbackPeriod; // in milliseconds
        }

        public delegate void EventHandler(Vector v);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiAccelerometerGetVector")]
        public static extern SmiResultCode GetVector(ref Vector accel);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiAccelerometerGetCapabilities")]
        public static extern SmiResultCode GetCapabilities(ref Capabilities cap);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiAccelerometerRegisterHandler")]
        public static extern SmiResultCode RegisterHandler(uint period, EventHandler handler);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiAccelerometerUnregisterHandler")]
        public static extern SmiResultCode UnregisterHandler();
    }
}
