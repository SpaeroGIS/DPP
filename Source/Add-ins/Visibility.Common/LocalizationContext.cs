using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace MilSpace.Visibility
{
    internal class LocalizationContext
    {
        private readonly XmlNode _root;

        public LocalizationContext()
        {         
            var localizationDoc = new XmlDocument();            
            var localizationFilePath = 
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\SP_VisibilityLocalization.xml");
            if (File.Exists(localizationFilePath))
            {
                localizationDoc.Load(localizationFilePath);
                _root = localizationDoc.SelectSingleNode("SP_Visibility");
            }
        }

        //Captions
        public string WindowCaption => FindLocalizedElement("WindowCaption", "Module Visibility");
         
        //Buttons
        public string GenerateButton => FindLocalizedElement("GenerateButton", "Generate");

        //Labels
        public string SurfaceLabel => FindLocalizedElement("SurfaceLabel", "Surface");

        //Errors        

        private string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
