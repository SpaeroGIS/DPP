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
            var localizationFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                @"Resources\Visualization3DLocalization.xml");

            if (File.Exists(localizationFilePath))
            {
                localizationDoc.Load(localizationFilePath);
                _root = localizationDoc.SelectSingleNode("Visualization3D");
            }
        }

        //Captions
        public string WindowCaption => FindLocalizedElement("WindowCaption", "Prepare 3D Visualization");
         
        //Buttons
        public string GenerateButton => FindLocalizedElement("GenerateButton", "Generate");
        public string GenerateProfileTabHeader => FindLocalizedElement("GenerateButton", "Generate");
        public string Generate3DSceneTabHeader => FindLocalizedElement("Generat3DTabHeader", "Generat 3D Scene");
        public string GenerateImageTabHeader => FindLocalizedElement("ImageTabHeader", "Generate Image");

        //Labels
        public string SurfaceLabel => FindLocalizedElement("SurfaceLabel", "Surface");
        public string ArcSceneParamsLabel => FindLocalizedElement("ArcSceneParams", "ArcScene Params"); 
        public string PlantsLayerLabel => FindLocalizedElement("PlantsLayerLabel", "Plants Layer");
        public string HightLablel => FindLocalizedElement("HightLablel", "Hight");
        public string BuildingsLayerLabel => FindLocalizedElement("BuildingsLayerLabel", "Buildings Layer");
        public string TransportLayerLabel => FindLocalizedElement("TransportLayerLabel", "Transport Layer");
        public string HydroLayerLabel => FindLocalizedElement("HydroLayerLabel", "Hydro Layer");
        public string ProfilesLabel => FindLocalizedElement("ProfilesLabel", "Profiles");
        public string SurfacessLabel => FindLocalizedElement("SurfacesLabel", "Surfaces");
        public string lblZFactor => FindLocalizedElement("lblZFactor", "Height conversion factor");

        //TreeView Strings
        public string Section => FindLocalizedElement("Section", "Section");
        public string Fun => FindLocalizedElement("Fun", "Fun");
        public string Primitive => FindLocalizedElement("Primitive", "Primitive");
        public object Profile => FindLocalizedElement("Profile", "Profile");

        //Errors        

        //Form visibility select
        public string WindowCaptionVisibilites => FindLocalizedElement("WindowCaptionVisibilites", "Visibilites");
        public string labelHeaderSessionVisibilites => FindLocalizedElement("labelHeaderSessionVisibilites", "Visibilites for Visualization");
        public string buttonAddProfilesTo3D => FindLocalizedElement("buttonAddProfilesTo3D", "Add");

        //Form Profiles select
        public string WindowCaptionProfiles => FindLocalizedElement("WindowCaptionProfiles", "Profiles");
        public string LabelProfileTreeHeader => FindLocalizedElement("LabelProfileTreeHeader", "Profiles for Visualization");
        public string ButtonAddSessionProfileTree => FindLocalizedElement("ButtonAddSessionProfileTree", "Add");

        //ToolTip
        public string ButtonUpdateToolTip => FindLocalizedElement("ButtonUpdateToolTip", "Update data layers");
        public string ButtonAddSurfaceToolTip => FindLocalizedElement("ButtonAddSurfaceToolTip", "Add surfaces");
        public string ButtonRemoveSurfaceToolTip => FindLocalizedElement("ButtonRemoveSurfaceToolTip", "Remove selected surfaces");
        public string ButtonAddProfileToolTip => FindLocalizedElement("ButtonAddProfileToolTip", "Add profiles");
        public string ButtonRemoveProfileToolTip => FindLocalizedElement("ButtonRemoveProfileToolTip", "Remove selected profiles");

        private string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
