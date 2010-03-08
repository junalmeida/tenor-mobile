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

        public Geolocation(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            GetAddress();
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
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
                    new Uri(string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-us")
                      , baseUrl, Latitude,Longitude, key)));
                req.Method = "GET";

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(res.GetResponseStream());
                rawAddress = reader.ReadToEnd();
                reader.Close();


            }
            catch (Exception ex)
            {
            }
        }
    }
}
