//#define XPS
using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Tenor.Mobile.Location
{
#if XPS
    public static class Xps
    {
        /*
        WPSAPI_EXPORT WPS_ReturnCode WPSAPI_CALL
WPS_location(const WPS_SimpleAuthentication* authentication,
             WPS_StreetAddressLookup street_address_lookup,
             WPS_Location** location);
         */

        [DllImport("wpsapi.dll")]
        private static extern WPS_ReturnCode WPS_location(
            ref WPS_SimpleAuthentication authentication, 
            WPS_StreetAddressLookup street_address_lookup,
            ref WPS_Location location);

        [DllImport("wpsapi.dll")]
        private static extern WPS_ReturnCode WPS_ip_location(ref
WPS_SimpleAuthentication
        authentication, WPS_StreetAddressLookup street_address_lookup,
[Out]IntPtr buffer);


        public static WPS_ReturnCode WPSLocation(String realm, String userName,
            out Double latitude, out Double longitude)
        {
            WPS_ReturnCode result = WPS_ReturnCode.WPS_ERROR;

            //create and initialize the authentication structure
            WPS_SimpleAuthentication authentication = new
WPS_SimpleAuthentication();
            authentication.realm = realm;
            authentication.username = userName;

            //set to no address lookup
            WPS_StreetAddressLookup streetAddressLookup =
WPS_StreetAddressLookup.WPS_NO_STREET_ADDRESS_LOOKUP;

            //Create location and pointer to it.
            WPS_IPLocation location = new WPS_IPLocation();
            IntPtr buffer =
Marshal.AllocCoTaskMem(Marshal.SizeOf(location));
            Marshal.StructureToPtr(location, buffer, false);

            try
            {
                result = WPS_ip_location(ref authentication,
streetAddressLookup, buffer);
            }
            catch (Exception)
            { }
            finally
            {
                Marshal.FreeCoTaskMem(buffer);
            }

            //these are 'out' variables from the custom function. so they need to be set
            latitude = 0;
            longitude = 0;

            if (result == WPS_ReturnCode.WPS_OK)
            {
                latitude = location.latitude;
                longitude = location.longitude;
            }
            return result;
        }
    }

    public enum WPS_ReturnCode
    {
        WPS_OK = 0,
        WPS_ERROR_SCANNER_NOT_FOUND = 1,
        WPS_ERROR_WIFI_NOT_AVAILABLE = 2,
        WPS_ERROR_NO_WIFI_IN_RANGE = 3,
        WPS_ERROR_UNAUTHORIZED = 4,
        WPS_ERROR_SERVER_UNAVAILABLE = 5,
        WPS_ERROR_LOCATION_CANNOT_BE_DETERMINED = 6,
        WPS_ERROR_PROXY_UNAUTHORIZED = 7,
        WPS_ERROR_FILE_IO = 8,
        WPS_ERROR_INVALID_FILE_FORMAT = 9,
        WPS_NOMEM = 98,
        WPS_ERROR = 99
    }

    enum WPS_Continuation
    {
        WPS_STOP = 0,
        WPS_CONTINUE = 1
    }

    enum WPS_StreetAddressLookup
    {
        WPS_NO_STREET_ADDRESS_LOOKUP,
        WPS_LIMITED_STREET_ADDRESS_LOOKUP,
        WPS_FULL_STREET_ADDRESS_LOOKUP
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WPS_SimpleAuthentication
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String username;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String realm;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WPS_NameCode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String code;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WPS_StreetAddress
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String street_number;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String address_line;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String city;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String postal_code;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String county;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String province;
        public WPS_NameCode state;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String region;
        public WPS_NameCode country;

    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WPS_Location
    {
        public Double latitude;
        public Double longitude;
        public Double hpe;
        public UInt16 nap;
        public Double speed;
        public Double bearing;
        public WPS_StreetAddress street_address;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WPS_IPLocation
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String ip;
        public Double latitude;
        public Double longitude;
        public IntPtr street_address;
    }
#endif
}