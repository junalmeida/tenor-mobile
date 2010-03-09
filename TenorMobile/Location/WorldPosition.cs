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
            try
            {
                gps = new Tenor.Mobile.Location.Gps.Gps();
                gps.Open();
            }
            catch { gps = null; }

            PollingInterval = 5000;
            PollLocation = false;
            FixType = FixType.Network;
            timer = new Timer(new TimerCallback(GetCellTowerInfo), null, 500, Timeout.Infinite);
        }

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


        #region Cell Functions

        /// <summary>
        /// Manually poll cellid and geolocation.
        /// </summary>
        public void Poll()
        {
            timer.Change(500, Timeout.Infinite);
        }


        bool idChanged = false;
        /*
         * Uses RIL to get CellID from the phone.
         */
        private void GetCellTowerInfo(object state)
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



            FixType = FixType.Network;
            double? latitude = Latitude;
            double? longitude = Longitude;
            try
            {
                if (gps != null && gps.Opened)
                {
                    Gps.GpsPosition pos = gps.GetPosition(new TimeSpan(0, 0, 0, 0, PollingInterval));
                    if (pos != null && pos.LongitudeValid && pos.LatitudeValid)
                    {

                        Latitude = pos.Latitude;
                        Longitude = pos.Longitude;
                        FixType = FixType.Gps;
                    }
                }
            }
            catch { }
            if (!object.Equals(latitude, Latitude) || !object.Equals(longitude, Longitude))
                OnLocationChanged(new EventArgs());

            if (idChanged)
            {
                if (PollLocation && FixType == FixType.Network)
                {
                    if (req != null)
                        Tenor.Mobile.Network.WebRequest.Abort(req);

                    if (getLocation != null)
                        getLocation.Abort();

                    getLocation = new Thread(new ThreadStart(delegate()
                    {
                        latitude = Latitude;
                        longitude = Longitude;

                        TranslateCellIdWithGoogle(CountryCode, 0, AreaCode, Id, false);
                        if (!latitude.HasValue)
                            TranslateCellIdWithOpenCellId(CountryCode, NetworkCode, AreaCode, Id);

                        getLocation = null;
                        req = null;

                        if (!object.Equals(latitude, Latitude) || !object.Equals(longitude, Longitude))
                            OnLocationChanged(new EventArgs());

                    }));
                    getLocation.Start();
                }

                OnTowerChanged(new EventArgs());
            }

            idChanged = false;

            if (PollingInterval > 0)
            {
                //reset timer to the next pooling
                timer.Change(PollingInterval, Timeout.Infinite);
            }
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


        #region GeoLocation
        private void TranslateCellIdWithOpenCellId(int MCC, int MNC, int LAC, int CID)
        {
            HttpWebResponse res = null;
            try
            {
                string uri = string.Format(OpenCellServiceUri, MCC, MNC, LAC, CID);
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

                    Latitude = double.Parse(doc.DocumentElement.FirstChild.Attributes["lat"].Value, System.Globalization.CultureInfo.GetCultureInfo("en-us"));
                    Longitude = double.Parse(doc.DocumentElement.FirstChild.Attributes["lon"].Value, System.Globalization.CultureInfo.GetCultureInfo("en-us"));
                }
                else
                {
                    Latitude = null;
                    Longitude = null;
                }
            }
            catch (Exception)
            {
                //Latitude = null;
                //Longitude = null;
                Tenor.Mobile.Network.WebRequest.Abort(req);
            }
            finally
            {
                if (req != null)
                {
                    req = null;
                }
                if (res != null)
                {
                    res.Close();
                    res = null;
                }
            }


        }



        HttpWebRequest req = null;
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

        private void TranslateCellIdWithGoogle(int MCC, int MNC, int LAC, int CID, bool shortCID)
        {
            HttpWebResponse res = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(
                    new Uri(GoogleMobileServiceUri));

                req.Timeout = 50000;
                req.Method = "POST";

                //int MCC = Convert.ToInt32(args[0]);
                //int MNC = Convert.ToInt32(args[1]);
                //int LAC = Convert.ToInt32(args[2]);
                //int CID = Convert.ToInt32(args[3]);
                byte[] pd = PostDataForGoogle(MCC, MNC, LAC, CID,
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
                        Latitude = ((double)((ps[7] << 24) | (ps[8] << 16)
                                     | (ps[9] << 8) | (ps[10]))) / 1000000;
                        Longitude = ((double)((ps[11] << 24) | (ps[12] <<
                                     16) | (ps[13] << 8) | (ps[14]))) /
                                     1000000;
                    }
                    else
                    {
                        Latitude = null;
                        Longitude = null;
                    }
                }
                else
                {
                    Latitude = null;
                    Longitude = null;
                }
            }
            catch (Exception)
            {
                Latitude = null;
                Longitude = null;
                Tenor.Mobile.Network.WebRequest.Abort(req);
            }
            finally
            {
                if (req != null)
                {
                    req = null;
                }
                if (res != null)
                {
                    res.Close();
                    res = null;
                }
            }

        }
        #endregion
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
