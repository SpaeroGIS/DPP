﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace MilSpace.GeoCalculator
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
        public string UtmLabel => FindLocalizedElement("UtmLabel", "UTM Representation");

        //Buttons
        public string CopyButton => FindLocalizedElement("CopyButton", "Copy");
        public string SaveButton => FindLocalizedElement("SaveButton", "Save to XML");
        public string MoveToCenterButton => FindLocalizedElement("MoveToCenterButton", "Map Center");
        public string ToolButton => FindLocalizedElement("ToolButton", "Turn Map Interaction Off");
        public string ClearGridButton => FindLocalizedElement("ClearGridButton", "Clear list");

        //TextBoxes
        public string AltRightToMove => FindLocalizedElement("AltRightToMove", "Alt+Right to move between parts");

        //Errors
        public string WrongUtmFormatMessage => FindLocalizedElement("WrongUtmFormatMessage", "Wrong UTM String Format");
        public string ErrorString => FindLocalizedElement("ErrorString", "Error");
        public string WrongMgrsFormatMessage => FindLocalizedElement("WrongMgrsFormatMessage", "Wrong MGRS String Format");
        public string WrongFormatMessage => FindLocalizedElement("WrongFormatMessage", "Wrong Format");
        public string NoSelectedPointError => FindLocalizedElement("NoSelectedPointMessage", "Please point somewhere on the map.");

        private string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
