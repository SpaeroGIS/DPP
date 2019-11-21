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
        public string MessageBoxCaption => FindLocalizedElement("MessageBoxCaption", "Sposterezhennya");
        public string MessageBoxShareCaption => FindLocalizedElement("MessageBoxShareCaption", "Sposterezhennya. Set access");
        public string MessageBoxWarningCaption => FindLocalizedElement("MessageBoxWarningCaption", "Sposterezhennya. Warning");

        public string ObservPointsTabCaption => FindLocalizedElement("ObservPointsTabCaption", "Observ P");
        public string ObservObjTabCaption => FindLocalizedElement("ObservObjTabCaption", "Observ O");
        public string TasksTabCaption => FindLocalizedElement("TasksTabCaption", "Tasks");
        public string ResultsTabCaption => FindLocalizedElement("ResultsTabCaption", "Results");

        //Header
        public string NameHeaderText => FindLocalizedElement("NameHeaderText", "Name");
        public string StateHeaderText => FindLocalizedElement("StateHeaderText", "State");
        public string TypeHeaderText => FindLocalizedElement("TypeHeaderText", "Type");
        public string AffiliationHeaderText => FindLocalizedElement("AffiliationHeaderText", "Affiliation");
        public string DateHeaderText => FindLocalizedElement("DateHeaderText", "Date");
        public string GroupHeaderText => FindLocalizedElement("GroupHeaderText", "Group");
         
        //Buttons
        public string GenerateButton => FindLocalizedElement("GenerateButton", "Generate");

        //Labels
        public string SurfaceLabel => FindLocalizedElement("SurfaceLabel", "Surface");
        public string ObservPointsHeaderLabel => FindLocalizedElement("ObservPointsHeaderLabel", "Observation points (OP)");
        public string ObservObjHeaderLabel => FindLocalizedElement("ObservObjHeaderLabel", "Observation objects (OO)");
        public string TasksHeaderLabel => FindLocalizedElement("TasksHeaderLabel", "Calculation tasks");
        public string ResultsHeaderLabel => FindLocalizedElement("ResultsHeaderLabel", "Results list");

        //Errors        
        public string IncorrectCoordMessage => FindLocalizedElement("IncorrectCoordMessage", "Invalid data. \nThe required coordinates are represented in WGS-84 CS, decimal degrees");
        public string IncorrectRangeMessage => FindLocalizedElement("IncorrectRangeMessage", "Invalid data.\nThe value must be in range from {0} to {1}");
        public string ValueLessThenZeroMessage => FindLocalizedElement("ValueLessThenZeroMessage", "Invalid data.\nThe value must be greater than zero");
        public string EmptyValueMessage => FindLocalizedElement("EmptyValueMessage", "Invalid data.\nPlease enter a value");
        public string InvalidFormatMessage => FindLocalizedElement("InvalidFormatMessage", "Invalid format");
        public string UnableToRemoveTaskMessage => FindLocalizedElement("UnableToRemoveTaskMessage", "The result of the current session visibility calculation cannot be deleted");
        public string UnableToFullRemoveTaskMessage => FindLocalizedElement("UnableToFullRemoveTaskMessage", "The result of the visibility calculation cannot be completely deleted");
        public string CalculationErrorMessage => FindLocalizedElement("CalculationErrorMessage", "The calculation finished with error\nTo view complete information go to the work log");
        public string SomeResultsNotAddedMessage => FindLocalizedElement("SomeResultsNotAddedMessage", "Some results cannot be added to the current session");

        //Info
        public string ObservPointRemoveMessage => FindLocalizedElement("ObservPointRemoveMessage", "Are you sure you want to delete this point (OP)?");
        public string VisibilityTaskRemoveMessage => FindLocalizedElement("VisibilityTaskRemoveMessage", "Are you sure you want to delete the calculation result from the current session?");
        public string VisibilityResultsFullRemoveMessage => FindLocalizedElement("VisibilityResultsFullRemoveMessage", "Are you sure you want to completely delete the calculation result?");
        public string VisibilityResultsRemoveFromSessionMessage => FindLocalizedElement("VisibilityResultsRemoveFromSessionMessage", "Are you sure you want to delete the calculation result from the current session?");
        public string VisibilityResultLayersRemoveMessage => FindLocalizedElement("VisibilityResultLayersRemoveMessage", "Are you sure you want to delete the calculation result layers?");
        public string SuccessfullySharedMessage => FindLocalizedElement("SuccessfullySharedMessage", "Shared with all users");
        public string AlreadySharedMessage => FindLocalizedElement("AlreadySharedMessage", "Results are already shared");


        private string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
