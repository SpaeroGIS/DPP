using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing.GuiData
{
    internal static class SentinelProductHelper
    {
        public static Dictionary<string, string> PropertyDescription = new Dictionary<string, string>
        {
            { "DateTime", "Дата" },
            { "OrbitNumber", "Номер орбіти" },
            { "PassDirection", "Напрямок руху" },
            { "RelativeOrbit", "Відносна орбіта" },
            { "SliceNumber", "Номер в нарізці"},
            { "Identifier", "Назва" },
            { "Instrument", "Іструмент" }
        };

        private static Type sentinelProductType = typeof(SentinelProduct);

        public static List<string[]> GetProductProperies(SentinelProduct product)
        {


            var resalt = new List<string[]>();
            if (product != null)
                foreach (var propertyName in PropertyDescription.Keys)
                {
                    string propValue = string.Empty;
                    var filedIfo = sentinelProductType.GetField(propertyName);
                    if (filedIfo != null)
                    {
                        propValue = filedIfo.GetValue(product).ToString();
                    }
                    else
                    {
                        var propInfo = sentinelProductType.GetProperty(propertyName);
                        if (propInfo != null)
                        {
                            propValue = propInfo.GetValue(product).ToString();
                        }
                    }
                    resalt.Add(new string[] { PropertyDescription[propertyName], propValue.ToString() });
                }

            return resalt;
        }



    }
}
