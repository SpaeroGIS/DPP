using MilSpace.Core.DataAccess;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MilSpace.Core
{
    internal class LocalizationContext
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
                {GraphicsTypesEnum.AllGraphics, AllGraphicsText },
                {GraphicsTypesEnum.SolutionGraphics, SolutionGraphicsText },
                {GraphicsTypesEnum.AllButSolutionGraphics, AllGraphicsButSolutionText },
                {GraphicsTypesEnum.GeocalculatorGraphics, GeocalculatorGraphicsText },
                {GraphicsTypesEnum.ProfileGraphics, ProfileGraphicsText },
                {GraphicsTypesEnum.VisibilityGraphics, VisibiltyGraphicsText },
            };

            _showGraphicsLocalisation = new Dictionary<GraphicsTypesEnum, string>()
            {
                {GraphicsTypesEnum.SolutionGraphics, SolutionGraphicsText },
                {GraphicsTypesEnum.GeocalculatorGraphics, GeocalculatorGraphicsText },
                {GraphicsTypesEnum.ProfileGraphics, ProfileGraphicsText },
                {GraphicsTypesEnum.VisibilityGraphics, VisibiltyGraphicsText },
            };
        }

        internal static LocalizationContext Instance => instance;

        internal string MessageBoxTitle => FindLocalizedElement("MessageBoxTitle", "Спостереження");
        internal string ChooseText => FindLocalizedElement("BtnChooseText", "Обрати");
        internal string ErrorHappendText => FindLocalizedElement("MsgErrorHappenedText", "Виникла помилка /n Більш детальна інформація знаходиться у журналі");
        internal string IdHeaderText => FindLocalizedElement("DgvObservPointsIdHeader", "Ідентифікатор");
        internal string TitleHeaderText => FindLocalizedElement("DgvObservPointsTitleHeader", "Назва");

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
            return _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
        }
    }
}
