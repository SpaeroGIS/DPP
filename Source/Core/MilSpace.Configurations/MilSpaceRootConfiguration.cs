using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.Web.Configuration;
using System.Reflection;
using System.IO;

namespace MilSpace.Configurations
{
    public abstract class MilSpaceRootConfiguration
    {
        private const string rootSectionNane = "spaero.gis";

        private static Configuration currentConfig = null;

        protected static Configuration GetCurrentConfiguration()
        {
            if (currentConfig == null)
            {
                //               
                if (GetIsWeb())
                {
                    currentConfig = WebConfigurationManager.OpenWebConfiguration("~/");
                }
                else
                {
                    if (Assembly.GetEntryAssembly() == null || Assembly.GetEntryAssembly().EntryPoint == null) // If the entry point in DLL (it was called from an external programm)
                    {
                        currentConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location); //ConfigurationUserLevel.PerUserRoamingAndLocal
                    }
                    else
                    {
                        currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    }
                }
            }

            return currentConfig;
        }


        protected static ConfigurationSectionGroup GetRootSectionGroup
        {

            get
            {
                var c = GetCurrentConfiguration();
                return c.GetSectionGroup(rootSectionNane);
            }
        }

        protected static bool GetIsWeb()
        {
            if (HttpContext.Current == null)
            {
                // NON-WEB

                // Check for WCF hosted in IIS - if you have a better way, please email me!
                String processName = Process.GetCurrentProcess().ProcessName;
                if (processName.Equals("w3wp", StringComparison.InvariantCultureIgnoreCase) // IIS process
                    || processName.Equals("WebDev.WebServer", StringComparison.InvariantCultureIgnoreCase) // Cassini: .NET 2.0-3.5
                    || processName.Equals("WebDev.WebServer40", StringComparison.InvariantCultureIgnoreCase)) // Cassini: .NET 4.0
                {
                    // This is WCF running in IIS/Cassini
                    return true;
                }
                else
                {
                    // Regular non-web app: winforms, console, windows service, etc
                    return false;
                }

            }
            else
            {
                // WEB
                return true;
            }

        }

        public static void LogText(string text)
        {
            string fileName = Path.Combine(Assembly.GetExecutingAssembly().Location, "Testlog.log");

            StreamWriter sw;
            FileStream fs = null;

            if (File.Exists(fileName))
            {
                fs = File.OpenWrite(fileName);
                sw = new StreamWriter(fs);
            }
            else
            {
                sw = File.CreateText(fileName);
            }
            sw.WriteLine(text);
            sw.Flush();
            sw.Close();
            sw.Dispose();

            if (fs != null)
            {
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }
    }

    public class LoggingConfiguration : MilSpaceRootConfiguration
    {
        private static LoggingConfiguration config;

        public static LoggingConfiguration Configuration
        {
            get
            {
                if (config == null)
                {
                    config = new LoggingConfiguration();
                }
                return config;
            }
        }

        public string ConfigFilePath
        {
            get
            {
                return GetCurrentConfiguration().FilePath;
            }
        }
    }
}
