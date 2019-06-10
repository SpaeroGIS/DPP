using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using log4net;
using log4net.Config;
using MilSpace.Configurations;

namespace MilSpace.Core
{
    public class Logger
    {
        private static ILog logger = null;
        private static string log4NetSectionName = "log4net";
        private ILog instanceLogger = null;

        private Logger()
        {
        }

        #region Methods to create Logger
        public static Logger GetLoggerEx(Type type)
        {
            Initiate();
            Logger newInstance = new Logger();
            newInstance.instanceLogger = LogManager.GetLogger(type);
            return newInstance;
        }

        public static Logger GetLoggerEx(string loggerName)
        {
            Initiate();
            Logger newInstance = new Logger();
            newInstance.instanceLogger = LogManager.GetLogger(loggerName);
            return newInstance;
        }

        public static Logger GetLoggerByMethodNameEx()
        {
            Initiate();
            StackTrace stackTrace = new StackTrace();
            string loggerName = stackTrace.GetFrame(1).GetMethod().Name;


            return GetLoggerEx(string.Format("{0}.{1}", stackTrace.GetFrame(1).GetMethod().DeclaringType.FullName, loggerName));
        }
        #endregion

        public static ILog Instance
        {
            get
            {
                if (logger == null)
                {
                    XmlElement toConfig = null;
                    try
                    {
                        FileInfo fi = new FileInfo(MilSpaceConfiguration.ConfigurationFileName);
                            if (fi.Exists)
                        {
                            XDocument xdoc = XDocument.Load(fi.FullName);

                            XElement configSection = xdoc.Descendants("configuration").FirstOrDefault();
                            XElement l4nElement = null;

                            if (configSection != default(XNode))
                            {
                                if (configSection.Descendants("section").Any(elem => elem.Attributes("name").Any(a => a.Value.Equals(log4NetSectionName))))
                                {
                                    l4nElement = configSection.Descendants(log4NetSectionName).FirstOrDefault();
                                }
                            }

                            if (default(XElement) != l4nElement)
                            {
                                XmlDocument config = new XmlDocument();
                                config.LoadXml(l4nElement.ToString());
                                toConfig = config.DocumentElement;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ConfigurationException("Open configuration file error.", ex);

                    }

                    if (null != toConfig)
                    {
                        XmlConfigurator.Configure(toConfig);
                    }
                    else
                    {
                        XmlConfigurator.Configure();
                    }

                    logger = LogManager.GetLogger("Spaero Core");
                }

                return logger;
            }
        }

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        public static ILog GetLogger(string loggerName)
        {
            return LogManager.GetLogger(loggerName);
        }

        public static ILog GetLoggerByMethodName()
        {
            StackTrace stackTrace = new StackTrace();
            string loggerName = stackTrace.GetFrame(1).GetMethod().Name;


            return GetLogger(string.Format("{0}.{1}", stackTrace.GetFrame(1).GetMethod().DeclaringType.FullName, loggerName));
        }

        public static void InfoWithMethodName(string message)
        {

            Instance.Info(message);
        }

        public static void Info(string message)
        {
            Instance.Info(message);
        }

        public static void Info(string message, params object[] args)
        {
            logger.InfoFormat(message, args);
        }

        public static void Info(string message, object arg0)
        {
            logger.InfoFormat(message, arg0);
        }

        public static void Error(string message)
        {
            Instance.Error(message);
        }

        public static void Error(string message, params object[] args)
        {
            Instance.ErrorFormat(message, args);
        }

        public static void Error(string message, object arg0)
        {
            Instance.ErrorFormat(message, arg0);
        }

        public static void Fatal(string message)
        {
            Instance.Fatal(message);
        }

        public static void Fatal(string message, params object[] args)
        {
            Instance.FatalFormat(message, args);
        }

        public static void Fatal(string message, object arg0)
        {
            Instance.FatalFormat(message, arg0);
        }

        public static void Warn(string message)
        {
            Instance.Warn(message);
        }

        public static void Warn(string message, params object[] args)
        {
            Instance.WarnFormat(message, args);
        }

        public static void Warn(string message, object arg0)
        {
            Instance.WarnFormat(message, arg0);
        }

        public static void Debug(string message)
        {
            Instance.Debug(message);
        }
        public static void Debug(string message, params object[] args)
        {
            Instance.DebugFormat(message, args);
        }

        public static void Debug(string message, object arg0)
        {
            Instance.DebugFormat(message, arg0);
        }

        #region Instance methods
        public void InfoEx(string message, params object[] args)
        {
            this.instanceLogger.InfoFormat(message, args);
        }

        public void InfoEx(string message)
        {
            this.instanceLogger.Info(message);
        }

        public void WarnEx(string message, params object[] args)
        {
            this.instanceLogger.WarnFormat(message, args);
        }

        public void WarnEx(string message)
        {
            this.instanceLogger.Warn(message);
        }

        public void DebugEx(string message, params object[] args)
        {
            this.instanceLogger.DebugFormat(message, args);
        }

        public void DebugEx(string message)
        {
            this.instanceLogger.Debug(message);
        }

        public void ErrorEx(string message, params object[] args)
        {
            this.instanceLogger.ErrorFormat(message, args);
        }

        public void ErrorEx(string message)
        {
            this.instanceLogger.Error(message);
        }

        public void FatalEx(string message)
        {
            this.instanceLogger.Fatal(message);
        }

        public void FatalEx(string message, params object[] args)
        {
            this.instanceLogger.FatalFormat(message, args);
        }

        private static void Initiate()
        {
            if (logger == null)
            { var inst = Instance; }

        }
        #endregion
    }
}
