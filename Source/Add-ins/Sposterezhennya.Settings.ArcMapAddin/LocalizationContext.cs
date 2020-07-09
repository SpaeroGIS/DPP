using MilSpace.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MilSpace.Settings
{
    class LocalizationContext
    {
        private readonly XmlNode _root;

        private static readonly LocalizationContext instance = new LocalizationContext();

        private Dictionary<GraphicsTypesEnum, string> _clearGraphicsLocalisation;
        private Dictionary<GraphicsTypesEnum, string> _showGraphicsLocalisation;

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

            _clearGraphicsLocalisation = new Dictionary<GraphicsTypesEnum, string>()
            {
                {GraphicsTypesEnum.All, AllGraphicsText },
                {GraphicsTypesEnum.Solution, SolutionGraphicsText },
                {GraphicsTypesEnum.AllButSolution, AllGraphicsButSolutionText },
                {GraphicsTypesEnum.Geocalculator, GeocalculatorGraphicsText },
                {GraphicsTypesEnum.Profile, ProfileGraphicsText },
                {GraphicsTypesEnum.Visibility, VisibiltyGraphicsText },
            };

            _showGraphicsLocalisation = new Dictionary<GraphicsTypesEnum, string>()
            {
                {GraphicsTypesEnum.Solution, SolutionGraphicsText },
                {GraphicsTypesEnum.Geocalculator, GeocalculatorGraphicsText },
                {GraphicsTypesEnum.Profile, ProfileGraphicsText },
                {GraphicsTypesEnum.Visibility, VisibiltyGraphicsText },
            };
        }

        internal static LocalizationContext Instance => instance;

        internal string MessageBoxTitle => FindLocalizedElement("MessageBoxTitle", "Спостереження");
        internal string ErrorHappendText => FindLocalizedElement("MsgErrorHappenedText", "Виникла помилка /n Більш детальна інформація знаходиться у журналі");
        internal string VisibilityModuleNotConnectedMessage => FindLocalizedElement("ObservPointsModuleDoesnotExistMessage", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним");
        internal string ProfileModuleNotConnectedMessage => FindLocalizedElement("ProfileModuleDoesnotExistMessage", "Модуль \"Профілі\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним");
        internal string GeocalcModuleNotConnectedMessage => FindLocalizedElement("GeoCalcModuleDoesnotExistMessage", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним");

        internal string AllGraphicsText => FindLocalizedElement("SolutionSettingsWindow_chckListBoxGraphicsAllGraphicsText", "вся графіка поекту");
        internal string SolutionGraphicsText => FindLocalizedElement("SolutionSettingsWindow_chckListBoxGraphicSolutionGraphicsText", "вся графіка рішення");
        internal string AllGraphicsButSolutionText => FindLocalizedElement("SolutionSettingsWindow_chckListBoxGraphicAllButSolutionGraphicsText", "вся графіка проекту, але не рішення");
        internal string GeocalculatorGraphicsText => FindLocalizedElement("SolutionSettingsWindow_chckListBoxGraphicGeocalculatorGraphicsText", "графіка Калькулятора Координат");
        internal string ProfileGraphicsText => FindLocalizedElement("SolutionSettingsWindow_chckListBoxGraphicProfileGraphicsText", "графіка Профілів");
        internal string VisibiltyGraphicsText => FindLocalizedElement("SolutionSettingsWindow_chckListBoxGraphicVisibilityGraphicsText", "графіка розрахунку Видимості");

        internal Dictionary<GraphicsTypesEnum, string> ClearGraphicsLocalisation => _clearGraphicsLocalisation;
        internal Dictionary<GraphicsTypesEnum, string> ShowGraphicsLocalisation => _showGraphicsLocalisation;

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
            var result = _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
            return result.Replace(@"\n", Environment.NewLine);
        }
    }
}
