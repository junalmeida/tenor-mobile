//#define PROXY
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Tenor.Mobile.Location
{
    /// <summary>
    /// Provides location information based on The Google Geocoding Web Service.
    /// http://code.google.com/apis/maps/documentation/geocoding/
    /// </summary>
    public class Geolocation
    {


        //const string key = "ABQIAAAADIubaBFn17TT-9Qlq83q3RRABhB3JDiBIx8yuapGDhWQqPAk6BQAtqQOGWpmfDdTeA1cwPEJwGx9TA";
        const string baseUrl = "http://maps.google.com/maps/api/geocode/csv?latlng={0},{1}&sensor=true&oe=utf-8";

        private Geolocation(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public double Latitude
        { get; private set; }

        public double Longitude
        { get; private set; }

        public string Neighborhood
        { get; private set; }

        public string Address
        { get; private set; }

        public string City
        { get; private set; }

        public string Province
        { get; private set; }

        public string ZipCode
        { get; private set; }

        public string Country
        { get; private set; }

        string[] rawData;
        private void ParseData(string data)
        {
            rawData = data.Split('\n');

            foreach (string s in rawData)
            {
                if (s.StartsWith("OK,,") || s.StartsWith("OK,street_address"))
                {
                    //the street address.
                    int pos = s.IndexOf("\"");
                    int pos2 = s.IndexOf("\"", pos + 1);

                    Address = s.Substring(pos + 1, pos2 - pos - 1).Trim();

                    //if (pos2 == -1)
                    //    pos2 = s.IndexOf(",", pos);
                    //if (pos2 == -1)
                    //    pos2 = s.IndexOf("\"", pos);
                    //Address = s.Substring(pos + 1, pos2 - (pos + 1)).Trim();
                }
                else if (s.StartsWith("OK,locality"))
                {
                    //city
                    int pos = s.IndexOf("\"");
                    int pos2 = s.IndexOf("\"", pos + 1);

                    string[] values = s.Substring(pos + 1, pos2 - pos - 1).Split(',');
                    if (values.Length > 0)
                    {
                        if (values.Length == 2)
                            values = values[0].Replace(" - ",",").Trim().Split(',');

                        if (values.Length > 0)
                            City = values[0].Trim();
                        if (values.Length > 1 && string.IsNullOrEmpty(Province))
                            Province = values[1].Trim();
                    }
                }
                else if (s.StartsWith("OK,neighborhood") || s.StartsWith("OK,sublocality"))
                {
                    //neighborhood
                    int pos = s.IndexOf("\"");
                    int pos2 = s.IndexOf("\"", pos + 1);

                    string[] values = s.Substring(pos + 1, pos2 - pos - 1).Split(',');
                    if (values.Length > 0)
                        Neighborhood = values[0].Trim();
                }
                else if (string.IsNullOrEmpty(ZipCode) && s.StartsWith("OK,postal_code"))
                {
                    //zip
                    int pos = s.IndexOf("\"");
                    int pos2 = s.IndexOf("\"", pos + 1);

                    string[] values = s.Substring(pos + 1, pos2 - pos - 1).Split(',');
                    if (string.IsNullOrEmpty(Neighborhood) && values.Length > 0)
                    {
                        Neighborhood = values[0].Trim();
                    }
                    foreach (string v in values)
                    {
                        foreach (char c in v)
                        {
                            if (c >= '0' && c <= '9')
                            {
                                ZipCode = v.Trim();
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(ZipCode)) break;
                    }
                }
                else if (string.IsNullOrEmpty(Country) && s.StartsWith("OK,country"))
                {
                    string[] values = s.Split(',');

                    if (values.Length > 1)
                        Country = values[2].Trim();
                }
                else if (s.StartsWith("OK,administrative_area_level_1"))
                {
                    int pos = s.IndexOf("\"");
                    int pos2 = s.IndexOf("\"", pos + 1);

                    string[] values = s.Substring(pos + 1, pos2 - pos - 1).Split(',');
                    if (values.Length > 0)
                        Province = values[0].Trim();
                }

            }

        }


        public override string ToString()
        {
            return Address;
        }


        private bool GetAddress()
        {

            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(
                    new Uri(string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-us")
                              , baseUrl, Latitude, Longitude)));

#if PROXY
                WebProxy proxy = new WebProxy("10.2.108.25", 8080);
                proxy.Credentials = new NetworkCredential("y3tr", "htc9377I");
                req.Proxy = proxy;
#endif

                req.Method = "GET";

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(res.GetResponseStream());
                string rawData = reader.ReadToEnd();
                reader.Close();
                ParseData(rawData);
                return true;
            }
            catch (Exception)
            {
                if (req != null)
                    Tenor.Mobile.Network.WebRequest.Abort(req);
            }
            finally
            {
                req = null;
            }
            return false;
        }

        private static List<Geolocation> cache = null;
        public static Geolocation Get(double latitude, double longitude)
        {
            if (cache == null)
                cache = new List<Geolocation>();
            Geolocation geo = new Geolocation(latitude, longitude);
            int i = cache.IndexOf(geo);
            if (i > -1)
                geo = cache[i];
            else
            {
                bool got = geo.GetAddress();
                if (!got)
                    return null;
                cache.Add(geo);
                if (cache.Count > 10)
                    cache.RemoveAt(0);
            }
            return geo;
        }



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
                Geolocation other = (Geolocation)obj;
                return object.Equals(this.Latitude, other.Latitude) && object.Equals(this.Longitude, other.Longitude);
            }
        }
        public override int GetHashCode()
        {
            int hash = 57;
            hash = 27 * hash * Latitude.GetHashCode();
            hash = 27 * hash * Longitude.GetHashCode();
            return hash;
        }

    }
}
