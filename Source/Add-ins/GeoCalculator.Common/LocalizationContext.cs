using System;
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
            var localizationFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                @"Resources\Localization.xml");
            if (File.Exists(localizationFilePath))
            {
                localizationDoc.Load(localizationFilePath);
                _root = localizationDoc.SelectSingleNode("CoordinatesConverter");
            }
        }

        //Captions
        public string CoordinatesConverterWindowCaption => FindLocalizedElement("WindowCaption", "Coordinates Converter");
        public string SaveAs => FindLocalizedElement("SaveAs", "Save As");

        //Labels
        public string CurrentMapLabel => FindLocalizedElement("CurrentMapLabel", "Current Map Coordinates");
        public string ProjectionsGroup => FindLocalizedElement("ProjectionsGroup", "Projections");
        public string PulkovoLabel => FindLocalizedElement("Pulkovo1942Label", "Pulkovo1942");
        public string WgsLabel => FindLocalizedElement("Wgs1984Label", "WGS1984");
        public string UkraineLabel => FindLocalizedElement("Ukraine2000Label", "Ukraine2000");
        public string MgrsLabel => FindLocalizedElement("MgrsLabel", "MGRS Representation");
        public string UtmLabel => FindLocalizedElement("UtmLabel", "UTM Representation");
        public string MgrsName=> FindLocalizedElement("MgrsName", "MGRS Representation");
        public string UtmName => FindLocalizedElement("UtmName", "UTM Representation");

        //Buttons
        public string CopyButton => FindLocalizedElement("CopyButton", "Copy");
        public string SaveButton => FindLocalizedElement("SaveButton", "Save to File");
        public string OpenFileButton => FindLocalizedElement("OpenFileButton", "Open File");
        public string MoveToCenterButton => FindLocalizedElement("MoveToCenterButton", "Map Center");
        public string MoveToProjectedCoordButton => FindLocalizedElement("MoveToProjectedCoordButton", "Move To Projected Coordinate");
        public string ToolButton => FindLocalizedElement("ToolButton", "Turn Map Interaction Off");
        public string ClearGridButton => FindLocalizedElement("ClearGridButton", "Clear list");
        public string ShowPointOnMapButton => FindLocalizedElement("ShowPointOnMap", "Show on the map");
        public string DeletePointButton => FindLocalizedElement("DeletePoint", "Delete point");
        public string CopyCoordinateButton => FindLocalizedElement("CopyCoordinateButton", "Copy coordinate to clipboard");
        public string PasteCoordinateButton => FindLocalizedElement("PasteCoordinateButton", "Paste coordinates from clipboard");

        //TextBoxes
        public string AltRightToMove => FindLocalizedElement("AltRightToMove", "Alt+Right to move between parts");

        //Errors
        public string WrongUtmFormatMessage => FindLocalizedElement("WrongUtmFormatMessage", "Wrong UTM String Format");
        public string ErrorString => FindLocalizedElement("ErrorString", "Error");
        public string WarningString => FindLocalizedElement("WarningString", "Warning");
        public string WrongMgrsFormatMessage => FindLocalizedElement("WrongMgrsFormatMessage", "Wrong MGRS String Format");
        public string WrongFormatMessage => FindLocalizedElement("WrongFormatMessage", "Wrong Format");
        public string NoSelectedPointError => FindLocalizedElement("NoSelectedPointMessage", "Please point somewhere on the map");
        public string GridCleanWarningMessage => FindLocalizedElement("GridCleanWarningMessage", "Next Operation Will Clear Points List. Proceed?");

        public string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
