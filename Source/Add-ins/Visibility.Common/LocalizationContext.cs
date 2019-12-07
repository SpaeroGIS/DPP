using MilSpace.DataAccess.DataTransfer;
using System;
using System.Linq;
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

        private Dictionary<VisibilityCalcTypeEnum, string> calcTypeLocalisation = 
            new Dictionary<VisibilityCalcTypeEnum, string>();

        private Dictionary<VisibilityCalcTypeEnum, string> calcTypeLocalisationShort = 
            new Dictionary<VisibilityCalcTypeEnum, string>();

        private Dictionary<VisibilityresultSummaryItemsEnum, string> summaryItems = 
            Enum.GetValues(typeof(VisibilityresultSummaryItemsEnum))
            .Cast<VisibilityresultSummaryItemsEnum>()
            .ToDictionary(t => t, t => t.ToString());

        private Dictionary<VisibilityTaskStateEnum, string> states = 
            Enum.GetValues(typeof(VisibilityTaskStateEnum)).
            Cast<VisibilityTaskStateEnum>().
            ToDictionary(t => t, ts => ts.ToString());

        private  Dictionary<ObservationPointMobilityTypesEnum, string> mobilityTypes = 
            Enum.GetValues(typeof(ObservationPointMobilityTypesEnum)).
            Cast<ObservationPointMobilityTypesEnum>().
            ToDictionary(t => t, ts => ts.ToString());

        private  Dictionary<ObservationPointTypesEnum, string> affiliationTypes = 
            Enum.GetValues(typeof(ObservationPointTypesEnum)).
            Cast<ObservationPointTypesEnum>().
            ToDictionary(t => t, ts => ts.ToString());

        private  Dictionary<ObservationObjectTypesEnum, string> observObjectsTypes = 
            Enum.GetValues(typeof(ObservationObjectTypesEnum)).
            Cast<ObservationObjectTypesEnum>().
            ToDictionary(t => t, ts => ts.ToString());


        private LocalizationContext()
        {
            var localizationDoc = new XmlDocument();
            var localizationFilePath =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Resources\SP_VisibilityLocalization.xml");

            if (!File.Exists(localizationFilePath))
            {
                throw new FileNotFoundException(localizationFilePath);
            }

            localizationDoc.Load(localizationFilePath);
            _root = localizationDoc.SelectSingleNode("SP_Visibility");

            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.None, string.Empty);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.None, string.Empty);

            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.OpservationPoints, CalcFirstTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.OpservationPoints, CalcFirstTypeDescriptionShort);

            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.ObservationObjects, CalcSecondTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.ObservationObjects, CalcSecondTypeDescriptionShort);

            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.BestObservationParameters, CalcTherdTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.BestObservationParameters, CalcTherdTypeDescriptionShort);

            calcTypeLocalisation.Add(VisibilityCalcTypeEnum.ResultsObservationAnalize, CalcFourthTypeDescription);
            calcTypeLocalisationShort.Add(VisibilityCalcTypeEnum.ResultsObservationAnalize, CalcFourthTypeDescriptionShort);

            summaryItems[VisibilityresultSummaryItemsEnum.Name] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.Name", "Назва результату розрахунку");
            summaryItems[VisibilityresultSummaryItemsEnum.CalculateCommonVisibilityResult] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.CalculateCommonVisibilityResult", "Розрахувати загальну ОВ");
            summaryItems[VisibilityresultSummaryItemsEnum.CalculateSeparatedPoints] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.CalculateSeparatedPoints", "Розрахувати поверхні всіх ПН");
            summaryItems[VisibilityresultSummaryItemsEnum.ConvertToPolygon] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.ConvertToPolygon", "Конвертувати у полігони");
            summaryItems[VisibilityresultSummaryItemsEnum.ObservationObjects] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.ObservationObjects", "Обрані області нагляду (ОН)");
            summaryItems[VisibilityresultSummaryItemsEnum.ObservationPoints] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.ObservationPoints", "Обрані пункти спостереження (ПН)");
            summaryItems[VisibilityresultSummaryItemsEnum.Surface] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.Surface", "Поверхня для розпрахунку");
            summaryItems[VisibilityresultSummaryItemsEnum.TrimCalculatedSurface] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.TrimCalculatedSurface", "Обрізати розраховані поверхні");
            summaryItems[VisibilityresultSummaryItemsEnum.Type] =
                FindLocalizedElement("VisibilityresultSummaryItemsEnum.Type", "Тип розрахунку");

            states[VisibilityTaskStateEnum.All] = 
                FindLocalizedElement("State_Calculate_all", states[VisibilityTaskStateEnum.All]);
            states[VisibilityTaskStateEnum.Calculated] = 
                FindLocalizedElement("State_Calculate_queue", states[VisibilityTaskStateEnum.Calculated]);
            states[VisibilityTaskStateEnum.Finished] = 
                FindLocalizedElement("StateFilter_Calculate_finished", states[VisibilityTaskStateEnum.Finished]);
            states[VisibilityTaskStateEnum.Pending] = 
                FindLocalizedElement("StateFilter_Calculate_run", states[VisibilityTaskStateEnum.Pending]);

            mobilityTypes[ObservationPointMobilityTypesEnum.All] =
                FindLocalizedElement("mobilityTypes_All", mobilityTypes[ObservationPointMobilityTypesEnum.All]);
            mobilityTypes[ObservationPointMobilityTypesEnum.Mobile] =
                FindLocalizedElement("mobilityTypes_Mobile", mobilityTypes[ObservationPointMobilityTypesEnum.Mobile]);
            mobilityTypes[ObservationPointMobilityTypesEnum.Stationary] =
                FindLocalizedElement("mobilityTypes_Stationary", mobilityTypes[ObservationPointMobilityTypesEnum.Stationary]);

            affiliationTypes[ObservationPointTypesEnum.All] = 
                FindLocalizedElement("Affiliation_All", affiliationTypes[ObservationPointTypesEnum.All]);
            affiliationTypes[ObservationPointTypesEnum.Enemy] = 
                FindLocalizedElement("AffiliationEdit_strangers", affiliationTypes[ObservationPointTypesEnum.Enemy]);
            affiliationTypes[ObservationPointTypesEnum.Neutrality] = 
                FindLocalizedElement("AffiliationEdit_neutral", affiliationTypes[ObservationPointTypesEnum.Neutrality]);
            affiliationTypes[ObservationPointTypesEnum.Our] = 
                FindLocalizedElement("Affiliation_Own", affiliationTypes[ObservationPointTypesEnum.Our]);
            affiliationTypes[ObservationPointTypesEnum.Undefined] = 
                FindLocalizedElement("AffiliationEdit_unknown", affiliationTypes[ObservationPointTypesEnum.Undefined]);

            observObjectsTypes[ObservationObjectTypesEnum.All] = 
                FindLocalizedElement("Affiliation_All", observObjectsTypes[ObservationObjectTypesEnum.All]);
            observObjectsTypes[ObservationObjectTypesEnum.Enemy] = 
                FindLocalizedElement("AffiliationEdit_strangers", observObjectsTypes[ObservationObjectTypesEnum.Enemy]);
            observObjectsTypes[ObservationObjectTypesEnum.Neutrality] = 
                FindLocalizedElement("AffiliationEdit_neutral", observObjectsTypes[ObservationObjectTypesEnum.Neutrality]);
            observObjectsTypes[ObservationObjectTypesEnum.Our] = 
                FindLocalizedElement("Affiliation_Own", observObjectsTypes[ObservationObjectTypesEnum.Our]);
            observObjectsTypes[ObservationObjectTypesEnum.Undefined] = 
                FindLocalizedElement("AffiliationEdit_unknown", observObjectsTypes[ObservationObjectTypesEnum.Undefined]);
        }

        internal static LocalizationContext Instance => instance;

        internal Dictionary<VisibilityCalcTypeEnum, string> CalcTypeLocalisation => calcTypeLocalisation;
        internal Dictionary<VisibilityCalcTypeEnum, string> CalcTypeLocalisationShort => calcTypeLocalisationShort;
        internal Dictionary<VisibilityresultSummaryItemsEnum, string> SummaryItems => summaryItems;
        internal Dictionary<VisibilityTaskStateEnum, string> CalculationStates => states;

        internal Dictionary<ObservationPointMobilityTypesEnum, string> MobilityTypes => mobilityTypes;
        internal Dictionary<ObservationPointTypesEnum, string> AffiliationTypes => affiliationTypes;
        internal Dictionary<ObservationObjectTypesEnum, string> ObservObjectsTypes => observObjectsTypes;


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


        public string YesWord => FindLocalizedElement("YesWord", "Yes");
        public string NoWord => FindLocalizedElement("NoWord", "No");

        public string DeafultObservationpointTitle => FindLocalizedElement("NewObservationPointName", "New point");

        public string PlaceLayerAbove => FindLocalizedElement("PlaceLayerAbove", "Above");
        public string PlaceLayerBelow => FindLocalizedElement("PlaceLayerBelow", "Below");

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

        public string WindowCaptionMaster => 
            FindLocalizedElement("WindowCaptionMaster", "Спостереження. Майстер розрахунку видимостей");

        //public string GenerateButton => FindLocalizedElement("GenerateButton", "Generate");
        //public string SurfaceLabel => FindLocalizedElement("SurfaceLabel", "Surface");

        public string MsgBoxWarningHeader => FindLocalizedElement("MsgBoxWarningHeader", "Спостереження. Попередження");
        public string MsgBoxInfoHeader => FindLocalizedElement("MsgBoxInfoHeader", "Спостереження. Інфо");
        public string MsgBoxQueryHeader => FindLocalizedElement("MsgBoxQueryHeader", "Спостереження. Запит");
        public string MsgBoxErrorHeader => FindLocalizedElement("MsgBoxErrorHeader", "Спостереження. Помилка");

        //<MsgTextNoLocalizationXML>"Не знайдено файл локалізації, або виникла ошибка при його завантаженні/nВікно розрахунку видимоста може бути локалізовано не повністью"</MsgTextNoLocalizationXML>

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


        ////---------------------------------------------------------------------------------------

        //            this.cmbAffiliationEdit.Items.AddRange(new object[] { "свои", "чужие ", "нейтральные", "неизвкчтно"});
        //            this.cmbStateFilter.AutoCompleteCustomSource.AddRange(new string[] {"у черзі", "виконується", "закінчено","зупинено"});
        //            this.cmbStateFilter.Items.AddRange(new object[] {"своі", "чужі ", "нейтральні", "невідомо"});

        //----------------------------------------------------------------------------------------

        internal bool HasLocalizedElement(string xmlNodeName)
        {
            try
            {
                return _root.SelectSingleNode(xmlNodeName) != null;
            }
            catch
            {
                return false;
            }
        }

        internal string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
