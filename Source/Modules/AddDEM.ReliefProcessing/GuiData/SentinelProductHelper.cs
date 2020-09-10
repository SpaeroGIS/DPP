using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;

namespace MilSpace.AddDem.ReliefProcessing.GuiData
{
    internal static class SentinelProductHelper
    {
        //TODO: Localization
        public static Dictionary<string, string> PropertyDescription = new Dictionary<string, string>
        {
            { "DateTime", "Дата" },
            { "RelativeOrbit", "Відносна орбіта" },
            { "PassDirection", "Напрямок руху" },
            { "SliceNumber", "Номер в нарізці"},
            { "Identifier", "Назва" },
            { "Coverage", "Покриття" }
            //{ "OrbitNumber", "Номер орбіти" }
        };

        private static Type sentinelProductType = typeof(SentinelProduct);

        public static List<string[]> GetProductProperies(SentinelProduct product, Tile srtmTile = null)
        {
            var resalt = new List<string[]>();
            if (product != null)
            {
                foreach (var propertyName in PropertyDescription.Keys)
                {
                    string propValue = string.Empty;
                    bool addProperty = false;
                    var filedIfo = sentinelProductType.GetField(propertyName);
                    if (filedIfo != null)
                    {
                        propValue = filedIfo.GetValue(product).ToString();
                        addProperty = true;
                    }
                    else
                    {
                        var propInfo = sentinelProductType.GetProperty(propertyName);
                        if (propInfo != null)
                        {
                            propValue = propInfo.GetValue(product).ToString();
                            addProperty = true;
                        }
                    }
                    if (addProperty)
                    {
                        resalt.Add(new string[] { PropertyDescription[propertyName], propValue.ToString() });
                    }
                }
                if (srtmTile != null)
                {
                    var persent = SantinelProcessing.GetTileCoverage(product, srtmTile);
                    resalt.Add(new string[] { PropertyDescription["Coverage"], $"{persent}%" });
                }
            }
            return resalt;
        }



    }
}
