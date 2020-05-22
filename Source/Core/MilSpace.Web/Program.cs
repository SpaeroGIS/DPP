using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MilSpace.Web
{
    class Program
    {
        static void Main(string[] args)
        {

            string url =   "https://scihub.copernicus.eu/dhus/api/stub/products?filter=(%20footprint:%22Intersects(POLYGON((33.0%2044.0,43.0%2044.0,43.0%2054.0,33.0%2054.0,33.0%2044.0)))%22)%20AND%20(beginPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z]%20AND%20endPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z])%20AND%20((platformname:Sentinel-1%20AND%20filename:S1A_*%20AND%20producttype:SLC%20AND%20sensoroperationalmode:IW))&offset=0&limit=150&sortedby=ingestiondate&order=desc";
            string url02 = "https://scihub.copernicus.eu/dhus/api/stub/products?filter=(%20footprint:%22Intersects(POLYGON((33.0%2043.0,33.0%2044.0,34.0%2044.0,34.0%2043.0,33.0%2043.0)))%22)%20AND%20(beginPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z]%20AND%20endPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z])%20AND%20(platformname:Sentinel-1%20AND%20filename:S1A_*%20AND%20producttype:SLC%20AND%20sensoroperationalmode:IW)&offset=0&limit=150&sortedby=ingestiondate&order=desc";
            string url03 = "https://scihub.copernicus.eu/dhus/api/stub/products?filter=(footprint:%22Intersects(POLYGON%20(33.0%2043.0,33.0%2044.0,34.0%2044.0,34.0%2043.0,33.0%2043.0))%22)%20AND%20(beginPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z]%20AND%20endPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z])%20AND%20(platformname:Sentinel-1%20AND%20filename:S1A_*%20AND%20producttype:SLC%20AND%20sensoroperationalmode:IW)&offset=0&limit=150&sortedby=ingestiondate&order=desc";
            string url2 = "https://scihub.copernicus.eu/dhus/api/stub/products?filter=(footprint:%22Intersects(POLYGON((33.0%2043.0,33.0%2044.0,34.0%2044.0,34.0%2043.0,33.0%2043.0)))%22)AND%20(%20beginPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z]%20AND%20endPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z]%20)%20AND%20(%20%20(platformname:Sentinel-1%20AND%20filename:S1A_*%20AND%20producttype:SLC%20AND%20sensoroperationalmode:IW))&offset=0&limit=150&sortedby=ingestiondate&order=desc%22";
            var ttyy = HttpUtility.UrlDecode(url2);

            //            filter=(%20footprint:%22Intersects(POLYGON((20.0%2044.0,43.0%2044.0,43.0%2054.0,20.0%2054.0,20.0%2044.0)))%22%20)%20AND%20(%20beginPosition:[2020-03-27T00:00:00.000Z%20TO%202020-04-08T23:59:59.999Z]%20AND%20endPosition:[2020-03-27T00:00:00.000Z%20TO%20]%20)%20AND%20(%20%20(platformname:Sentinel-1%20AND%20filename:S1A_*%20AND%20producttype:SLC%20AND%20sensoroperationalmode:IW))&amp;offset=0&amp;limit=150&amp;sortedby=ingestiondate&amp;order=desc"/>

            string text;
            //WebRequest myWebRequest = WebRequest.Create(url); //"https://scihub.copernicus.eu/dhus/odata/v1/Products?$format=json"
            ////            WebRequest myWebRequest = WebRequest.Create("https://scihub.copernicus.eu/dhus/odata/v1/Products('ad83ef2b-4f12-4bca-8df9-f7026c63e90c')$format=json");



            SentineWeblRequestBuilder bn = new SentineWeblRequestBuilder()
            {
                Position = DateTime.Parse("2020-04-08"),
            };

            bn.AddTile(new DataAccess.DataTransfer.Sentinel.Tile { Lat = 33, Lon = 43 });

            url = bn.GetMetadataUrl;
            WebRequest myWebRequest = WebRequest.Create(url);


            myWebRequest.Credentials = new NetworkCredential("spaero", "spaero3404558");

            // Send the 'WebRequest' and wait for response.
            using (WebResponse myWebResponse = myWebRequest.GetResponse())
            {
                using (var reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    text = reader.ReadToEnd();
                }

                myWebResponse.Close();
            }

            var itms = SentinelImportManager.ReadJsonFromFile();

        }
    }
}
