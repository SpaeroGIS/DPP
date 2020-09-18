using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MilSpace.AddDem.ReliefProcessing
{
    internal class LocalizationContext
    {
        private readonly XmlNode _root;
        private static readonly LocalizationContext instance = new LocalizationContext();

        private LocalizationContext()
        {
            var localizationDoc = new XmlDocument();
            var localizationFilePath =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Resources\SP_AddDEMLocalization.xml");

            if (!File.Exists(localizationFilePath))
            {
                throw new FileNotFoundException(localizationFilePath);
            }

            localizationDoc.Load(localizationFilePath);
            _root = localizationDoc.SelectSingleNode("SP_AddDEM");
        }

        internal static LocalizationContext Instance => instance;

        internal string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            var result = _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
            return result.Replace(@"\n", Environment.NewLine);
        }

        internal string MessageBoxTitle => FindLocalizedElement("MessageBoxTitle", "Спостереження ЦММ");
    }
}
