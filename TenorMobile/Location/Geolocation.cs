using System;

using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Tenor.Mobile.Location
{
    public class Geolocation
    {
        const string key = "ABQIAAAADIubaBFn17TT-9Qlq83q3RRABhB3JDiBIx8yuapGDhWQqPAk6BQAtqQOGWpmfDdTeA1cwPEJwGx9TA";
        const string baseUrl = "http://maps.google.com/maps/geo?q={0},{1}&output=csv&sensor=true&key={2}";

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

        string rawAddress;

        public override string ToString()
        {
            return rawAddress;
        }


        private void GetAddress()
        {

            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(
new Uri(string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-us")
          , baseUrl, Latitude, Longitude, key)));
                req.Method = "GET";

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(res.GetResponseStream());
                rawAddress = reader.ReadToEnd();
                reader.Close();
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
        }

        private static List<Geolocation> cache = new List<Geolocation>();
        public static Geolocation Get(double latitude, double longitude)
        {
            Geolocation geo = new Geolocation(latitude, longitude);
            int i = cache.IndexOf(geo);
            if (i > -1)
                geo = cache[i];
            else
            {
                geo.GetAddress();
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
