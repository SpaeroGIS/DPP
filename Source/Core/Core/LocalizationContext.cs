using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MilSpace.Core
{
    internal class LocalizationContext
    {
        private readonly XmlNode _root;

        private static readonly LocalizationContext instance = new LocalizationContext();

        private LocalizationContext()
        {
            var localizationDoc = new XmlDocument();
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var localizationFilePath = directory + @"\Resources\SP_CoreLocalization.xml";

            if (!File.Exists(localizationFilePath))
            {
                throw new FileNotFoundException(localizationFilePath);
            }

            localizationDoc.Load(localizationFilePath);
            _root = localizationDoc.SelectSingleNode("SP_CoreLocalization");
        }

        internal static LocalizationContext Instance => instance;

        internal string MessageBoxTitle => FindLocalizedElement("MessageBoxTitle", "Спостереження");
        internal string ChooseText => FindLocalizedElement("BtnChooseText", "Обрати");
        internal string ErrorHappendText => FindLocalizedElement("MsgErrorHappenedText", "Виникла помилка /n Більш детальна інформація знаходиться у журналі");
        internal string IdHeaderText => FindLocalizedElement("DgvObservPointsIdHeader", "Ідентифікатор");
        internal string TitleHeaderText => FindLocalizedElement("DgvObservPointsTitleHeader", "Назва");

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
