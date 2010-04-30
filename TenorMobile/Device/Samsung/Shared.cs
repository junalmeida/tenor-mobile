using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SamsungMobileSdk
{
    internal enum SmiResultCode
    {
        Success              = 0x00000000,
        Unknown              = 0x00000001,
        DeviceNotFound       = 0x00000002,
        DeviceDisabled       = 0x00000003,
        PermissionDenied     = 0x00000004,
        InvalidParameter     = 0x00000005,
        CannotActivateServer = 0x00000006,

        HapticsInvalidHandle = 0x10020000,
        HapticsOutOfHandles  = 0x10020001,

        LedColorNotSupported = 0x10030000

    }


    internal struct SdkVersion
    {
        public uint major;
        public uint minor;
        public uint patch;
    }

    internal class Shared
    {
        public const string SamsungMobileSDKDllName = "SamsungMobileSDK_2.dll";

        [DllImport(SamsungMobileSDKDllName, EntryPoint = "SmiGetSdkVersion")]
        private static extern IntPtr SmiGetSdkVersion(ref SdkVersion version);

        public static string GetVersion(ref SdkVersion version)
        {
            IntPtr v = SmiGetSdkVersion(ref version);
            return Marshal.PtrToStringUni(v);
        }
    }
}
