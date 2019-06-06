using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ArcMapAddin
{
    internal class LocalizationContext
    {
        private readonly XmlNode _root;

        public LocalizationContext()
        {         
            var localizationDoc = new XmlDocument();
            var localizationFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Localization.xml");
            if (File.Exists(localizationFilePath))
            {
                localizationDoc.Load(localizationFilePath);
                _root = localizationDoc.SelectSingleNode("CoordinatesConverter");
            }
        }

        //Caption
        public string CoordinatesConverterWindowCaption => FindLocalizedElement("WindowCaption", "Coordinates Converter");

        //Labels
        public string CurrentMapLabel => FindLocalizedElement("CurrentMapLabel", "Current Map Coordinates");
        public string ProjectionsGroup => FindLocalizedElement("ProjectionsGroup", "Projections");
        public string PulkovoLabel => FindLocalizedElement("Pulkovo1942Label", "Pulkovo1942");
        public string WgsLabel => FindLocalizedElement("Wgs1984Label", "WGS1984");
        public string UkraineLabel => FindLocalizedElement("Ukraine2000Label", "Ukraine2000");
        public string MgrsLabel => FindLocalizedElement("MgrsLabel", "MGRS Representation");

        //Buttons
        public string CopyButton => FindLocalizedElement("CopyButton", "Copy");
        public string SaveButton => FindLocalizedElement("SaveButton", "Save to XML");
        public string MoveToCenterButton => FindLocalizedElement("MoveToCenterButton", "Map Center");

        public string WrongUtmFormatMessage { get; internal set; }
        public string ErrorString { get; internal set; }
        public string WrongMgrsFormatMessage { get; internal set; }

        private string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
