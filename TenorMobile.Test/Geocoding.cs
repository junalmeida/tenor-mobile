#define PROXY

using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenor.Mobile.Location;

namespace TenorMobile.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Geocoding
    {


        [TestMethod]
        public void GeoCodingTest()
        {
            List<KeyValuePair<double, double>> latlngList = new List<KeyValuePair<double, double>>();
            latlngList.Add(new KeyValuePair<double, double>(-22.909667, -43.179656)); //Rio de Janeiro
            latlngList.Add(new KeyValuePair<double, double>(42.983202, -108.627434)); //Wyoming
            latlngList.Add(new KeyValuePair<double, double>(19.404819, -72.367172)); //Haiti, unknown address
            latlngList.Add(new KeyValuePair<double, double>(18.555136, -72.319565)); //Haiti, unknown address
            latlngList.Add(new KeyValuePair<double, double>(31.248382, 121.243515)); //Shanghai, China
            latlngList.Add(new KeyValuePair<double, double>(43.665674, -79.40938)); //Toronto, Canada

            List<Geolocation> list = new List<Geolocation>();
            foreach (KeyValuePair<double, double> latlng in latlngList)
            {
                Geolocation loc = Geolocation.Get(latlng.Key, latlng.Value);
                if (loc == null)
                    Assert.Inconclusive("Either we don't have network or an error occured");
                list.Add(loc);
            }

            

        }
    }
}
