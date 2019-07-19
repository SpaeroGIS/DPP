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
using Microsoft.Win32;

namespace MilSpace.Configurations
{
    public abstract class MilSpaceRootConfiguration
    {
        private const string rootSectionNane = "milspace";
        private const string registryPathToConfig = @"SOFTWARE\WOW6432Node\MilSpace\";
        private static readonly string configurationFileName = $"{typeof(MilSpaceRootConfiguration).Assembly.GetName().Name}.config";
        private static string configurationFilePath;

        private static Configuration currentConfig = null;


        public static string ConfigurationFilePath
        {
            get { return configurationFilePath; }
            set
            {

                if (!Directory.Exists(value))
                {
                    throw new DirectoryNotFoundException(value);
                }

                var assemblyName = Path.Combine(value, new FileInfo(Assembly.GetAssembly(typeof(MilSpaceRootConfiguration)).Location).Name);

                if (!File.Exists(assemblyName))
                {
                    throw new FileNotFoundException(assemblyName);
                }

                configurationFilePath = assemblyName;
            }
        }


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
                        var registryConfiguration = GetConfigurationPathFromRegistry();
                        if (string.IsNullOrWhiteSpace(registryConfiguration))
                        {
                            configurationFilePath = configurationFilePath ?? Assembly.GetExecutingAssembly().Location;
                            currentConfig = ConfigurationManager.OpenExeConfiguration(configurationFilePath); //ConfigurationUserLevel.PerUserRoamingAndLocal
                        }
                        else
                        {
                            configurationFilePath = Path.Combine(registryConfiguration, configurationFileName);
                            if (File.Exists(configurationFilePath))
                            {
                                ConfigurationFileMap fl = new ConfigurationFileMap(configurationFilePath);
                                currentConfig = ConfigurationManager.OpenMappedMachineConfiguration(fl);
                            }
                            else
                            {
                                throw new FileNotFoundException($"The configuration file {configurationFilePath} doen't exist.");
                            }
                        }

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

        private static string GetConfigurationPathFromRegistry()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPathToConfig))
            {
                if (key == null)
                {
                    return null;
                }

                var val = key.GetValue("Configuration");
                
                return val.ToString();
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
