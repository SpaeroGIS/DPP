using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace MilSpace.Core.MilSpaceResourceManager
{
    public class MilSpaceResourceManager
    {

        private static readonly string localizationregistryKey = "Localization";
        private static Logger logger = Logger.GetLoggerEx("MilSpaceResourceManager");
        private static string defailtValueNotLocalized = "<not localized>";
        private static CultureInfo cultoreToLocalize;

        ResourceManager innerObject;

        public MilSpaceResourceManager(string sourceName, CultureInfo cultireInfo)
        {
            var pathToAssembly = Helper.GetRegistryValue(localizationregistryKey);
            pathToAssembly = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Localization");
            innerObject = ResourceManager.CreateFileBasedResourceManager(sourceName, $"{pathToAssembly}", null);

            cultoreToLocalize = cultireInfo;
        }

        public string GetLocalization(string key, string defailtcvalue = null)
        {
            try
            {
                var result = innerObject.GetString(key, cultoreToLocalize);
                return result;
            }
            catch
            {
                defailtcvalue = string.IsNullOrEmpty(defailtcvalue) ? defailtValueNotLocalized : defailtcvalue;
                logger.ErrorEx($"There is no localization key \"{key}\". Default value {defailtcvalue} was returned.");
                return defailtcvalue;
            }
        }
    }
}
