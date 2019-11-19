using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace MilSpace.Visibility.Localization
{
    internal class LocalizationContext
    {
        private readonly XmlNode _root;
        private static readonly LocalizationContext instance = new LocalizationContext();
        private Dictionary<VisibilityCalcTypeEnum, string> calcTypeLocalisation = new Dictionary<VisibilityCalcTypeEnum, string>();
        private Dictionary<VisibilityCalcTypeEnum, string> calcTypeLocalisationShort = new Dictionary<VisibilityCalcTypeEnum, string>();

        

        private LocalizationContext()
        {         
            var localizationDoc = new XmlDocument();            
            var localizationFilePath = 
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\SP_VisibilityLocalization.xml");
            if (File.Exists(localizationFilePath))
            {
                localizationDoc.Load(localizationFilePath);
                _root = localizationDoc.SelectSingleNode("SP_Visibility");
            }
            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.None, string.Empty);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.None, string.Empty);

            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.OpservationPoints, CalcFirstTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.OpservationPoints, CalcFirstTypeDescriptionShort);
            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.ObservationObjects, CalcTherdTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.ObservationObjects, CalcTherdTypeDescriptionShort);
            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.BestObservationParameters, CalcTherdTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.BestObservationParameters, CalcTherdTypeDescriptionShort);
            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.ResultsAnalize, CalcFourthTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.ResultsAnalize, CalcFourthTypeDescriptionShort);
        }


        internal static LocalizationContext Instance => instance;


        internal Dictionary<VisibilityCalcTypeEnum, string> CalcTypeLocalisation => calcTypeLocalisation;
        internal Dictionary<VisibilityCalcTypeEnum, string> CalcTypeLocalisationShort => calcTypeLocalisationShort;
        internal string CalcFirstTypeDescriptionShort => FindLocalizedElement("CalcFirstTypeDescriptionShort", "VS");
        internal string CalcFirstTypeDescription => FindLocalizedElement("CalcFirstTypeDescription","Визначення областей видимості на обраної поверхні в цілому.");
        internal string CalcSecondTypeDescriptionShort => "VA";
        internal string CalcSecondTypeDescription => "Визначення видимості в заданих ОН.";

        internal string CalcTherdTypeDescriptionShort => "VO";
        internal string CalcTherdTypeDescription => "Визначення параметрів пунктів спостереження (ПН).";
        internal string CalcFourthTypeDescriptionShort => "VP";
        internal string CalcFourthTypeDescription => "Аналіз результатів спостереження.";


        public string YesWord => FindLocalizedElement("YesWord", "Yes");

        public string NoWord => FindLocalizedElement("NoWord", "No");

        public string PlaceLayerAbove => FindLocalizedElement("PlaceLayerAbove", "Above");
        public string PlaceLayerBelow => FindLocalizedElement("PlaceLayerBelow", "Below");

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
