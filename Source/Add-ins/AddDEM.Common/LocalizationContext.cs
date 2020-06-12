using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sposterezhennya.AddDEM.ArcMapAddin
{
    internal class LocalizationContext
    {
        private readonly XmlNode _root;
        private static readonly LocalizationContext instance = new LocalizationContext();

        private LocalizationContext()
        {
            var localizationDoc = new XmlDocument();
            var localizationFilePath =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Resources\SP_AddDEMLocalization.xml");

            if (!File.Exists(localizationFilePath))
            {
                throw new FileNotFoundException(localizationFilePath);
            }

            localizationDoc.Load(localizationFilePath);
            _root = localizationDoc.SelectSingleNode("SP_AddDEM");
        }

        internal static LocalizationContext Instance => instance;

        internal string FindLocalizedElement(string xmlNodeName, string defaultValue)
        {
            var result = _root?.SelectSingleNode(xmlNodeName)?.InnerText ?? defaultValue;
            return result.Replace(@"\n", Environment.NewLine);
        }
    }
}
