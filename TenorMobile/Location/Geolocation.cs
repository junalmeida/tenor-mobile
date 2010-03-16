using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace Tenor.Mobile.Location
{
    /// <summary>
    /// Provides location information based on The Google Geocoding Web Service.
    /// http://code.google.com/apis/maps/documentation/geocoding/
    /// </summary>
    public class Geolocation
    {


        //const string key = "ABQIAAAADIubaBFn17TT-9Qlq83q3RRABhB3JDiBIx8yuapGDhWQqPAk6BQAtqQOGWpmfDdTeA1cwPEJwGx9TA";
        const string baseUrl = "http://maps.google.com/maps/api/geocode/xml?latlng={0},{1}&sensor=true&oe=utf-8";

        private Geolocation(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Gets the latitude position part of this Geolocation instance.
        /// </summary>
        public double Latitude
        { get; private set; }

        /// <summary>
        /// Gets the longitude position part of this Geolocation instance.
        /// </summary>
        public double Longitude
        { get; private set; }

        /// <summary>
        /// Gets the Neighborhood or sublocality. Usually small city divisions.
        /// </summary>
        public string Neighborhood
        { get; private set; }

        /// <summary>
        /// Gets the full formatted address.
        /// </summary>
        public string Address
        { get; private set; }

        /// <summary>
        /// Gets the city name.
        /// </summary>
        public string City
        { get; private set; }

        /// <summary>
        /// The province or state, usually the first administrative level.
        /// </summary>
        public string Province
        { get; private set; }

        /// <summary>
        /// Gets the postal code.
        /// </summary>
        public string ZipCode
        { get; private set; }

        /// <summary>
        /// Gets the political country name.
        /// </summary>
        public string Country
        { get; private set; }

        /// <summary>
        /// Gets only the route. Usually, the street name.
        /// </summary>
        public string Route
        { get; private set; }

#if DEBUG
        private string rawData;
#endif 

        private void ParseData(string data)
        {
            if (data.StartsWith("<?xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);
                ParseData(doc.DocumentElement);
            }
            else
            {
                //parses csv
                string[] rawData = data.Split('\n');

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
                                values = values[0].Replace(" - ", ",").Trim().Split(',');

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
                        //if (string.IsNullOrEmpty(Neighborhood) && values.Length > 0)
                        //{
                        //    Neighborhood = values[0].Trim();
                        //}
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

        }

        private void ParseData(XmlElement xmlElement)
        {
            foreach (XmlNode e in xmlElement.ChildNodes)
            {
                if (e is XmlElement)
                {
                    XmlElement el = (XmlElement)e;

                    switch (el.Name)
                    {
                        case "formatted_address":
                            Address = el.InnerText;
                            break;
                        case "address_component":
                            ParseAddressComponent(el);
                            break;
                        default:
                            ParseData(el);
                            break;
                    }
                }
            }
        }

        private void ParseAddressComponent(XmlElement el)
        {
            XmlElement type = (XmlElement)el.SelectSingleNode("type");
            switch (type.InnerText)
            {
                case "postal_code":
                    {
                        XmlElement long_name = (XmlElement)el.SelectSingleNode("long_name");
                        ZipCode = long_name.InnerText;
                    }
                    break;
                case "route":
                    {
                        XmlElement long_name = (XmlElement)el.SelectSingleNode("long_name");
                        if (Route == null)
                            Route = long_name.InnerText;
                    }
                    break;
                case "administrative_area_level_2":
                case "administrative_area_level_3":
                case "sublocality":
                case "neighborhood":
                    {
                        XmlElement long_name = (XmlElement)el.SelectSingleNode("long_name");
                        if (Neighborhood == null && long_name.InnerText != City)
                            Neighborhood = long_name.InnerText;
                    }
                    break;
                case "locality":
                    {
                        XmlElement long_name = (XmlElement)el.SelectSingleNode("long_name");
                        City = long_name.InnerText;
                    }
                    break;
                case "administrative_area_level_1":
                    {
                        XmlElement long_name = (XmlElement)el.SelectSingleNode("long_name");
                        Province = long_name.InnerText;
                    }
                    break;          
                case "country":
                    {
                        XmlElement long_name = (XmlElement)el.SelectSingleNode("long_name");
                        Country = long_name.InnerText;
                    }
                    break;
            }
        }

        /// <summary>
        /// Return a System.String that represents this Geolocation.
        /// </summary>
        public override string ToString()
        {
            if (Address != null)
                return Address;
            else
                return Country;
        }


        private bool GetAddress()
        {

            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(
                    new Uri(string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-us")
                              , baseUrl, Latitude, Longitude)));

                req.Method = "GET";

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(res.GetResponseStream());
                StringBuilder xml = new StringBuilder();
                string rawData = null;
                do
                {
                    rawData = reader.ReadLine();
                    xml.Append(rawData);
                    xml.Append("\r\n");
                } while (rawData.IndexOf("<geometry>") == -1);
                xml.Append("</geometry></result></GeocodeResponse>");
                Tenor.Mobile.Network.WebRequest.Abort(req);

                reader.Close();
#if DEBUG
                this.rawData = xml.ToString();
#endif
                ParseData(xml.ToString());
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
        /// <summary>
        /// Gets an instance of Geolocation based on latitude and longitude values. 
        /// This will consume the data plan.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Determines whether the speficied object is equal to this instance.
        /// </summary>
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


        /// <summary>
        /// Retruns a hash that represents this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = 57;
            hash = 27 * hash * Latitude.GetHashCode();
            hash = 27 * hash * Longitude.GetHashCode();
            return hash;
        }

    }
}
