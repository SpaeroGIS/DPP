using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace MilSpace.Visualization3D
{
    internal class LocalizationContext
    {
        private readonly XmlNode _root;

        public LocalizationContext()
        {         
            var localizationDoc = new XmlDocument();            
            var localizationFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Visualization3DLocalization.xml");
            if (File.Exists(localizationFilePath))
            {
                localizationDoc.Load(localizationFilePath);
                _root = localizationDoc.SelectSingleNode("Visualization3D");
            }
        }

        //Captions
        public string WindowCaption => FindLocalizedElement("WindowCaption", "Profiles 3D Visualization");
         
        //Buttons
        public string GenerateButton => FindLocalizedElement("GenerateButton", "Generate");

        //Labels
        public string SurfaceLabel => FindLocalizedElement("SurfaceLabel", "Surface");
        public string PlantsLayerLabel => FindLocalizedElement("PlantsLayerLabel", "Plants Layer");
        public string HightLablel => FindLocalizedElement("HightLablel", "Hight");
        public string BuildingsLayerLabel => FindLocalizedElement("BuildingsLayerLabel", "Buildings Layer");
        public string TransportLayerLabel => FindLocalizedElement("TransportLayerLabel", "Transport Layer");
        public string HydroLayerLabel => FindLocalizedElement("HydroLayerLabel", "Hydro Layer");
        public string ProfilesLabel => FindLocalizedElement("ProfilesLabel", "Profiles");
       
        //Errors        

        private string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
