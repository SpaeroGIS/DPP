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
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                    @"Resources\SP_VisibilityLocalization.xml");

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
            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.ResultsObservationAnalize, CalcFourthTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.ResultsObservationAnalize, CalcFourthTypeDescriptionShort);
        }


        internal static LocalizationContext Instance => instance;
        //FindLocalizedElement("", "");

        internal Dictionary<VisibilityCalcTypeEnum, string> CalcTypeLocalisation => calcTypeLocalisation;
        internal Dictionary<VisibilityCalcTypeEnum, string> CalcTypeLocalisationShort => calcTypeLocalisationShort;

        internal string CalcFirstTypeDescriptionShort => 
            FindLocalizedElement("CalcFirstTypeDescriptionShort", "VS");
        internal string CalcFirstTypeDescription => 
            FindLocalizedElement("CalcFirstTypeDescription", "Визначення видимості в цілому");

        internal string CalcSecondTypeDescriptionShort =>
            FindLocalizedElement("CalcSecondTypeDescriptionShort", "VA");
        internal string CalcSecondTypeDescription =>
            FindLocalizedElement("CalcSecondTypeDescription", "Визначення видимості в заданих ОН");

        internal string CalcTherdTypeDescriptionShort =>
            FindLocalizedElement("CalcTherdTypeDescriptionShort", "VO");
        internal string CalcTherdTypeDescription =>
            FindLocalizedElement("CalcTherdTypeDescription", "Визначення параметрів пунктів спостереження");

        internal string CalcFourthTypeDescriptionShort =>
            FindLocalizedElement("CalcFourthTypeDescriptionShort", "VP");
        internal string CalcFourthTypeDescription =>
            FindLocalizedElement("CalcFourthTypeDescription", "Аналіз результатів спостереження");


        public string YesWord => 
            FindLocalizedElement("YesWord", "Yes");
        public string NoWord => 
            FindLocalizedElement("NoWord", "No");

        public string PlaceLayerAbove => 
            FindLocalizedElement("PlaceLayerAbove", "Above");
        public string PlaceLayerBelow => 
            FindLocalizedElement("PlaceLayerBelow", "Below");

        //Captions
        public string WindowCaption => 
            FindLocalizedElement("WindowCaption", "Module Visibility");
         
        //Buttons
        public string GenerateButton => 
            FindLocalizedElement("GenerateButton", "Generate");

        //Labels
        public string SurfaceLabel => 
            FindLocalizedElement("SurfaceLabel", "Surface");

        public string MsgBoxWarningHeader => FindLocalizedElement("MsgBoxWarningHeader", "Спостереження. Попередження");
        public string MsgBoxInfoHeader => FindLocalizedElement("MsgBoxInfoHeader", "Спостереження. Інфо");
        public string MsgBoxQueryHeader => FindLocalizedElement("MsgBoxQueryHeader", "Спостереження. Запит");
        public string MsgBoxErrorHeader => FindLocalizedElement("MsgBoxErrorHeader", "Спостереження. Помилка");

        //<MsgTextNoLocalizationXML>"Не знайдено файл локалізації, або виникла ошибка при його завантаженні/nВікно розрахунку видимоста може бути локалізовано не повністью"</MsgTextNoLocalizationXML>



        //Errors        

        ////---------------------------------------------------------------------------------------

        //            this.cmbAffiliationEdit.Items.AddRange(new object[] { "свои", "чужие ", "нейтральные", "неизвкчтно"});
        //            this.cmbStateFilter.AutoCompleteCustomSource.AddRange(new string[] {"у черзі", "виконується", "закінчено","зупинено"});
        //            this.cmbStateFilter.Items.AddRange(new object[] {"своі", "чужі ", "нейтральні", "невідомо"});

        //----------------------------------------------------------------------------------------

        internal string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
