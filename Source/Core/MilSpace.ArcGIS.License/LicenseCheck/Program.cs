using Spaero.ArcGIS.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace LicenseCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isInit = SpaerLicenseInitializer.Instance.InitializeApplication();
            Console.WriteLine("");

               Console.WriteLine(isInit?"Initialising successful": "Initialising failed");
        }
    }
}
