using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Web
{
    class Program
    {
        static void Main(string[] args)
        {

            string text;
            WebRequest myWebRequest = WebRequest.Create("https://scihub.copernicus.eu/dhus/odata/v1/Products?$format=json");
//            WebRequest myWebRequest = WebRequest.Create("https://scihub.copernicus.eu/dhus/odata/v1/Products('ad83ef2b-4f12-4bca-8df9-f7026c63e90c')$format=json");
        
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

            ImportManager.ReadJson(text);

        }
    }
}
