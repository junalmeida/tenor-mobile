﻿using System;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace Tenor.Mobile.Location
{

    //---Based on code written by Dale Lane
    //---Source: http://dalelane.co.uk/blog/?p=241
    /// <summary>
    /// Provides location services based on cell id and gps information.
    /// </summary>
    public class WorldPosition
    {
        private const string GoogleMobileServiceUri = "http://www.google.com/glm/mmap";
        private const string OpenCellServiceUri = "http://www.opencellid.org/cell/get?key=7e93ae41e81f11b986a6e99adb691997&mcc={0}&mnc={1}&lac={2}&cellid={3}";
        Gps.Gps gps;

        #region Events
        public event EventHandler TowerChanged;
        protected virtual void OnTowerChanged(EventArgs e)
        {
            if (TowerChanged != null)
                TowerChanged(this, e);
        }

        public event EventHandler LocationChanged;
        protected virtual void OnLocationChanged(EventArgs e)
        {
            if (LocationChanged != null)
                LocationChanged(this, e);
        }
        #endregion

        #region Data
        public int Id
        { get; private set; }

        public int CountryCode
        { get; private set; }

        public int AreaCode
        { get; private set; }

        public double? Latitude
        { get; private set; }

        public double? Longitude
        { get; private set; }

        public FixType FixType
        { get; private set; }

        public int NetworkCode
        { get; private set; }

        public double? Altitude
        { get; private set; }

        public DateTime LastFix
        { get; private set; }

        

        private int pollingInterval;
        /// <summary>
        /// The interval, in miliseconds we will pool cell location. Set zero to disable automatic polling.
        /// </summary>
        public int PollingInterval
        {
            get { return pollingInterval; }
            set
            {
                if (timer != null && pollingInterval <= 0 && value > 0)
                    timer.Change(value, Timeout.Infinite);

                pollingInterval = value;
            }
        }

        /// <summary>
        /// Determines whether to poll location from network when using cell id.
        /// </summary>
        public bool PollLocation { get; set; }

        /// <summary>
        /// Determines whether to use gps to get latitude and longitude
        /// </summary>
        public bool UseGps { get; set; }

        /// <summary>
        /// Returns a string representation of this WorldPosition instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", Id, AreaCode, CountryCode);
        }



        #endregion

        Thread baseThread;
        Thread getLocation;
        private Timer timer;
        /// <summary>
        /// Creates an instance of WorldPosition.
        /// </summary>
        public WorldPosition()
        {
            baseThread = Thread.CurrentThread;
            PollingInterval = 5000;
            PollLocation = false;
            UseGps = true;
            FixType = FixType.Network;
            timer = new Timer(new TimerCallback(PeriodicPoll), null, 500, Timeout.Infinite);
        }

        /// <summary>
        /// Finalizes the gps instance.
        /// </summary>
        ~WorldPosition()
        {
            if (gps != null && gps.Opened)
                gps.Close();
                
        }

        /// <summary>
        /// Creates an instance of WorldPosition.
        /// </summary>
        /// <param name="pollLocation">Indicates if it will try to find latitude and longitude when using cell id.
        /// This requires an internet connection.</param>
        public WorldPosition(bool pollLocation)
            : this()
        {
            this.PollLocation = pollLocation;
        }

        public WorldPosition(bool pollLocation, bool useGps)
            : this()
        {
            this.PollLocation = pollLocation;
            this.UseGps = useGps;
        }

        /// <summary>
        /// Manually poll location.
        /// </summary>
        public void Poll()
        {
            timer.Change(500, Timeout.Infinite);
        }

        private void PeriodicPoll(object state)
        {
            bool cellChanged = GetCellTowerInfo();

            if (UseGps)
                PollGps();
            else
            {
                FixType = FixType.Network;
                if (gps != null && gps.Opened)
                    gps.Close();
            }

            if (cellChanged && PollLocation && FixType == FixType.Network)
                PollCell();

            if (PollingInterval > 0)
            {
                //reset timer to the next pooling
                timer.Change(PollingInterval, Timeout.Infinite);
            }
        }



        #region Cell Functions




        bool idChanged = false;
        /*
         * Uses RIL to get CellID from the phone.
         */
        private bool GetCellTowerInfo()
        {
            // initialise handles
            IntPtr hRil = IntPtr.Zero;
            IntPtr hRes = IntPtr.Zero;

            // initialise RIL
            hRes = RIL_Initialize(1,                      // RIL port 1
                new RILRESULTCALLBACK(rilResultCallback), // function to call with result
                null,                                     // function to call with notify
                0,                                        // classes of notification to enable
                0,                                        // RIL parameters
                out hRil);                                // RIL handle returned

            if (hRes != IntPtr.Zero)
            {
                throw new Exception("Cannot connect to GSM chip.");
            }

            // initialised successfully

            // use RIL to get cell tower info with the RIL handle just created
            hRes = RIL_GetCellTowerInfo(hRil);

            // wait for cell tower info to be returned
            waithandle.WaitOne();

            // finished - release the RIL handle
            RIL_Deinitialize(hRil);

            //celltower info finished

            if (idChanged)
            {
                idChanged = false;
                OnTowerChanged(new EventArgs());
                return true;
            }
            else
                return false;   

        }




        // event used to notify user function that a response has
        //  been received from RIL
        private AutoResetEvent waithandle = new AutoResetEvent(false);

        private void rilResultCallback(uint dwCode,
                                             IntPtr hrCmdID,
                                             IntPtr lpData,
                                             uint cbData,
                                             uint dwParam)
        {
            // create empty structure to store cell tower info in
            RILCELLTOWERINFO rilCellTowerInfo = new RILCELLTOWERINFO();

            // copy result returned from RIL into structure
            Marshal.PtrToStructure(lpData, rilCellTowerInfo);

            // get the bits out of the RIL cell tower response that we want
            uint id = rilCellTowerInfo.dwCellID;
            if (id != Id)
            {
                Id = Convert.ToInt32(rilCellTowerInfo.dwCellID);
                AreaCode = Convert.ToInt32(rilCellTowerInfo.dwLocationAreaCode);
                CountryCode = Convert.ToInt32(rilCellTowerInfo.dwMobileCountryCode);
                NetworkCode = Convert.ToInt32(rilCellTowerInfo.dwMobileNetworkCode);
                idChanged = true;
            }
            // notify caller function that we have a result
            waithandle.Set();
        }

        #endregion

        #region Native Methods
        // -------------------------------------------------------------------
        //  RIL function definitions
        // -------------------------------------------------------------------
        /* 
         * Function definition converted from the definition 
         *  RILRESULTCALLBACK from MSDN:
         * 
         * http://msdn2.microsoft.com/en-us/library/aa920069.aspx
         */
        private delegate void RILRESULTCALLBACK(uint dwCode,
                                               IntPtr hrCmdID,
                                               IntPtr lpData,
                                               uint cbData,
                                               uint dwParam);

        /*
         * Function definition converted from the definition 
         *  RILNOTIFYCALLBACK from MSDN:
         * 
         * http://msdn2.microsoft.com/en-us/library/aa922465.aspx
         */
        private delegate void RILNOTIFYCALLBACK(uint dwCode,
                                               IntPtr lpData,
                                               uint cbData,
                                               uint dwParam);

        /*
         * Class definition converted from the struct definition 
         *  RILCELLTOWERINFO from MSDN:
         * 
         * http://msdn2.microsoft.com/en-us/library/aa921533.aspx
         */
        private class RILCELLTOWERINFO
        {
            public uint cbSize;
            public uint dwParams;
            public uint dwMobileCountryCode;
            public uint dwMobileNetworkCode;
            public uint dwLocationAreaCode;
            public uint dwCellID;
            public uint dwBaseStationID;
            public uint dwBroadcastControlChannel;
            public uint dwRxLevel;
            public uint dwRxLevelFull;
            public uint dwRxLevelSub;
            public uint dwRxQuality;
            public uint dwRxQualityFull;
            public uint dwRxQualitySub;
            public uint dwIdleTimeSlot;
            public uint dwTimingAdvance;
            public uint dwGPRSCellID;
            public uint dwGPRSBaseStationID;
            public uint dwNumBCCH;
        }

        // -------------------------------------------------------------------
        //  RIL DLL functions 
        // -------------------------------------------------------------------

        /* Definition from: http://msdn2.microsoft.com/en-us/library/aa919106.aspx */
        [DllImport("ril.dll")]
        private static extern IntPtr RIL_Initialize(
            uint dwIndex,
            RILRESULTCALLBACK pfnResult,
            RILNOTIFYCALLBACK pfnNotify,
            uint dwNotificationClasses,
            uint dwParam,
            out IntPtr lphRil);

        /* Definition from: http://msdn2.microsoft.com/en-us/library/aa923065.aspx */
        [DllImport("ril.dll")]
        private static extern IntPtr RIL_GetCellTowerInfo(IntPtr hRil);

        /* Definition from: http://msdn2.microsoft.com/en-us/library/aa919624.aspx */
        [DllImport("ril.dll")]
        private static extern IntPtr RIL_Deinitialize(IntPtr hRil);
        #endregion

        #region Gps Fix

        private bool CheckForGps()
        {
            if (gps == null)
                try
                {
                    gps = new Tenor.Mobile.Location.Gps.Gps();
                }
                catch { }
            if (gps != null && !gps.Opened)
                try
                {
                    gps.Open();
                }
                catch { }

            return (gps != null && gps.Opened);
        }


        private void PollGps()
        {
            FixType = FixType.Network;
            double? latitude = Latitude;
            double? longitude = Longitude;
            try
            {
                if (CheckForGps())
                {
                    Gps.GpsPosition pos = gps.GetPosition(new TimeSpan(0, 0, 0, 0, PollingInterval));
                    if (pos != null && pos.LongitudeValid && pos.LatitudeValid)
                    {

                        Latitude = pos.Latitude;
                        Longitude = pos.Longitude;
                        if (pos.SeaLevelAltitudeValid)
                            Altitude = pos.SeaLevelAltitude;
                        else
                            Altitude = null;

                        LastFix = pos.Time;

                        FixType = FixType.Gps;
                    }
                }
            }
            catch { }
            if (!object.Equals(latitude, Latitude) || !object.Equals(longitude, Longitude))
                OnLocationChanged(new EventArgs());

        }

        #endregion

        #region GeoLocation

        public void PollCell()
        {

            if (getLocation != null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Aborting current polling.");
#endif
                getLocation.Abort(); //If we have threadabortexception, it will abort the requests.
            }

            getLocation = new Thread(new ThreadStart(delegate()
            {
                double? latitude = Latitude;
                double? longitude = Longitude;

                try
                {
                    if (cellCache == null)
                        cellCache = new List<CellInfo>();

                    CellInfo info = new CellInfo() { MCC = CountryCode, MNC = NetworkCode, LAC = AreaCode, CID = Id };
                    int index = cellCache.IndexOf(info);
                    if (index > -1)
                    {
                        info = cellCache[index];
                    }
                    else
                    {
                        bool ok = TranslateCellIdWithGoogle(info, false);
                        if (!ok)
                            ok = TranslateCellIdWithOpenCellId(info);
                        if (ok)
                            cellCache.Add(info);
                        else
                            info = null;
                        
                    }
                    if (info != null)
                    {
                        Latitude = info.Lat;
                        Longitude = info.Lng;
                        LastFix = DateTime.UtcNow;
                    }
                }
#if DEBUG
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Status.ToString());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
#else
                catch { }
#endif
                getLocation = null;

                if (!object.Equals(latitude, Latitude) || !object.Equals(longitude, Longitude))
                    OnLocationChanged(new EventArgs());

            }));
            getLocation.Start();
        }


        private bool TranslateCellIdWithOpenCellId(CellInfo info)
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                string uri = string.Format(
                    OpenCellServiceUri, 
                    info.MCC,
                    info.MNC, 
                    info.LAC, 
                    info.CID);

                req = (HttpWebRequest)WebRequest.Create(
                    new Uri(uri));

                req.Timeout = 50000;
                req.Method = "GET";

                res = (HttpWebResponse)req.GetResponse();
                //StreamReader reader = new StreamReader(res);
                //string xml = reader.ReadToEnd();

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(res.GetResponseStream());

                    info.Lat = double.Parse(doc.DocumentElement.FirstChild.Attributes["lat"].Value, System.Globalization.CultureInfo.GetCultureInfo("en-us"));
                    info.Lng = double.Parse(doc.DocumentElement.FirstChild.Attributes["lon"].Value, System.Globalization.CultureInfo.GetCultureInfo("en-us"));
                    return true;
                }
            }
            catch (Exception)
            {
                if (req != null)
                    Tenor.Mobile.Network.WebRequest.Abort(req);

                throw;
            }
            finally
            {
                if (req != null)
                    req = null;

                if (res != null)
                {
                    res.Close();
                    res = null;
                }
            }
            return false;

        }


        private class CellInfo
        {
            public int MCC { get; set; }
            public int MNC { get; set; }
            public int LAC { get; set; }
            public int CID { get; set; }
            public double? Lat { get; set; }
            public double? Lng { get; set; }


            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                    return true;
                else if (obj == null)
                    return false;
                else if (obj.GetType() != this.GetType())
                    return false;
                else
                {
                    CellInfo other = (CellInfo)obj;
                    return
                        object.Equals(this.MCC, other.MCC) &&
                        object.Equals(this.MNC, other.MNC) &&
                        object.Equals(this.LAC, other.LAC) &&
                        object.Equals(this.CID, other.CID);
                }
            }
            public override int GetHashCode()
            {
                int hash = 57;
                hash = 27 * hash * MCC.GetHashCode();
                hash = 27 * hash * MNC.GetHashCode();
                hash = 27 * hash * LAC.GetHashCode();
                hash = 27 * hash * CID.GetHashCode();
                return hash;
            }
        }
        private static List<CellInfo> cellCache;


        private byte[] PostDataForGoogle(int MCC, int MNC, int LAC, int CID,
                       bool shortCID)
        {
            /* The shortCID parameter follows heuristic experiences:
             * Sometimes UMTS CIDs are build up from the original GSM CID (lower 4 hex digits)
             * and the RNC-ID left shifted into the upper 4 digits.
             */
            byte[] pd = new byte[] {
                0x00, 0x0e,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00,
                0x00, 0x00,
                0x00, 0x00,

                0x1b, 
                0x00, 0x00, 0x00, 0x00, // Offset 0x11
                0x00, 0x00, 0x00, 0x00, // Offset 0x15
                0x00, 0x00, 0x00, 0x00, // Offset 0x19
                0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, // Offset 0x1f
                0x00, 0x00, 0x00, 0x00, // Offset 0x23
                0x00, 0x00, 0x00, 0x00, // Offset 0x27
                0x00, 0x00, 0x00, 0x00, // Offset 0x2b
                0xff, 0xff, 0xff, 0xff,
                0x00, 0x00, 0x00, 0x00
            };

            bool isUMTSCell = ((Int64)CID > 65535);

            //if (isUMTSCell)
            //    Console.WriteLine("UMTS CID. {0}", shortCID ?
            //        "Using short CID to resolve." : "");
            //else
            //    Console.WriteLine("GSM CID given.");

            if (shortCID)
                CID &= 0xFFFF;      /* Attempt to resolve the cell using the 
                                    GSM CID part */

            if ((Int64)CID > 65536) /* GSM: 4 hex digits, UTMS: 6 hex 
                                    digits */
                pd[0x1c] = 5;
            else
                pd[0x1c] = 3;

            pd[0x11] = (byte)((MNC >> 24) & 0xFF);
            pd[0x12] = (byte)((MNC >> 16) & 0xFF);
            pd[0x13] = (byte)((MNC >> 8) & 0xFF);
            pd[0x14] = (byte)((MNC >> 0) & 0xFF);

            pd[0x15] = (byte)((MCC >> 24) & 0xFF);
            pd[0x16] = (byte)((MCC >> 16) & 0xFF);
            pd[0x17] = (byte)((MCC >> 8) & 0xFF);
            pd[0x18] = (byte)((MCC >> 0) & 0xFF);

            pd[0x27] = (byte)((MNC >> 24) & 0xFF);
            pd[0x28] = (byte)((MNC >> 16) & 0xFF);
            pd[0x29] = (byte)((MNC >> 8) & 0xFF);
            pd[0x2a] = (byte)((MNC >> 0) & 0xFF);

            pd[0x2b] = (byte)((MCC >> 24) & 0xFF);
            pd[0x2c] = (byte)((MCC >> 16) & 0xFF);
            pd[0x2d] = (byte)((MCC >> 8) & 0xFF);
            pd[0x2e] = (byte)((MCC >> 0) & 0xFF);

            pd[0x1f] = (byte)((CID >> 24) & 0xFF);
            pd[0x20] = (byte)((CID >> 16) & 0xFF);
            pd[0x21] = (byte)((CID >> 8) & 0xFF);
            pd[0x22] = (byte)((CID >> 0) & 0xFF);

            pd[0x23] = (byte)((LAC >> 24) & 0xFF);
            pd[0x24] = (byte)((LAC >> 16) & 0xFF);
            pd[0x25] = (byte)((LAC >> 8) & 0xFF);
            pd[0x26] = (byte)((LAC >> 0) & 0xFF);

            return pd;
        }

        private bool TranslateCellIdWithGoogle(CellInfo info, bool shortCID)
        {


            HttpWebResponse res = null;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(
                    new Uri(GoogleMobileServiceUri));

                req.Timeout = 50000;
                req.Method = "POST";

                byte[] pd = PostDataForGoogle(info.MCC, info.MNC, info.LAC, info.CID,
                    shortCID);

                req.ContentLength = pd.Length;
                req.ContentType = "application/binary";
                Stream outputStream = req.GetRequestStream();
                outputStream.Write(pd, 0, pd.Length);
                outputStream.Close();
                res = (HttpWebResponse)req.GetResponse();
                byte[] ps = new byte[res.ContentLength];
                int totalBytesRead = 0;
                while (totalBytesRead < ps.Length)
                {
                    totalBytesRead += res.GetResponseStream().Read(
                        ps, totalBytesRead, ps.Length - totalBytesRead);
                }

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    short opcode1 = (short)(ps[0] << 8 | ps[1]);
                    byte opcode2 = ps[2];
                    int ret_code = (int)((ps[3] << 24) | (ps[4] << 16) |
                                   (ps[5] << 8) | (ps[6]));
                    if (ret_code == 0)
                    {
                        info.Lat = ((double)((ps[7] << 24) | (ps[8] << 16)
                                     | (ps[9] << 8) | (ps[10]))) / 1000000;
                        info.Lng = ((double)((ps[11] << 24) | (ps[12] <<
                                     16) | (ps[13] << 8) | (ps[14]))) /
                                     1000000;

                        return true;
                    }
                }
            }
            catch (Exception)
            {
                //Latitude = null;
                //Longitude = null;

                if (req != null)
                    Tenor.Mobile.Network.WebRequest.Abort(req);

                throw;
            }
            finally
            {

                if (req != null)
                    req = null;

                if (res != null)
                {
                    res.Close();
                    res = null;
                }
            }
            return false;
        }
        #endregion


        /// <summary>
        /// Try to find the geolocation of the current fixed position.
        /// </summary>
        /// <returns></returns>
        public Geolocation GetGeoLocation()
        {
            if (Latitude.HasValue && Longitude.HasValue)
                return Geolocation.Get(Latitude.Value, Longitude.Value);
            else
                return null;
        }
    }


    public enum FixType
    {
        /// <summary>
        /// The last position cames from the internet.
        /// </summary>
        Network,
        /// <summary>
        /// The last position cames from the gps anthenna.
        /// </summary>
        Gps
    }

}