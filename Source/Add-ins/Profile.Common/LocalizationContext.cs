using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
namespace MilSpace.Profile.Localization
{
    public class LocalizationContext
    {
        private readonly XmlNode _root;

        private static readonly LocalizationContext instance = new LocalizationContext();

       
        private LocalizationContext()
        {
            var localizationDoc = new XmlDocument();
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var localizationFilePath = directory + @"\Resources\SP_ProfileLocalization.xml";
              
            if(!File.Exists(localizationFilePath))
            {
                throw new FileNotFoundException(localizationFilePath);
            }

            localizationDoc.Load(localizationFilePath);
            _root = localizationDoc.SelectSingleNode("SP_ProfileLocalization");
        }

        internal static LocalizationContext Instance => instance;

        internal string NamePlaceholder => FindLocalizedElement("TxtNamePlaceholder", "Назва профілю");
        internal string CreatorPlaceholder => FindLocalizedElement("TxtCreatorPlaceholder", "Автор");
        internal string PointsTypeText => FindLocalizedElement("PointsTypeText", "Відрізки");
        internal string FunTypeText => FindLocalizedElement("FunTypeText", "\"Віяло\"");
        internal string PrimitiveTypeText => FindLocalizedElement("PrimitiveTypeText", "Графіка");
        internal string MessageBoxTitle => FindLocalizedElement("MessageBoxTitle", "Спостереження");
        internal string AzimuthInfoText => FindLocalizedElement("LblAzimuthInfoText", "Азимут:");
        internal string LengthInfoText => FindLocalizedElement("LblLengthInfoText", "Довжина:");
        internal string DimensionText => FindLocalizedElement("LblDimensionText", "м");
        internal string ChooseText => FindLocalizedElement("BtnChooseText", "Обрати");
        internal string AssignmentMethodText => FindLocalizedElement("LblAssignmentMethodText", "Спосіб призначення");
        internal string ErrorHappendText => FindLocalizedElement("MsgErrorHappenedText", "Виникла помилка /n Більш детальна інформація знаходиться у журналі");
        internal string ReturnPointValueText => FindLocalizedElement("BtnReturnPointValueToolTip", "Повернутися до дійсного значення точки");
        internal string AssignmentMethodFromMapItem => FindLocalizedElement("CmbAssignmentMethodFromMapTypeText", "Мапа");
        internal string TargetAssignmentMethodInSector => FindLocalizedElement("CmbTargetAssignmentMethodInSectorText", "У вказаному секторі");
        internal string ToPointsCreationMethodAzimuthsLines => FindLocalizedElement("CmbToPointsCreationMethodLineNumberText", "К-ть ліній від мін до макс азимуту");
        internal string DemLayerNotChosenText => FindLocalizedElement("MsgDemLayerNotChosen", "Будь ласка оберіть шар ЦМР/ЦММ, щоб отримати Z координату");

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
