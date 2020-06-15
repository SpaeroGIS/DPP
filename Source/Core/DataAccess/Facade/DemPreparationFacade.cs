using ESRI.ArcGIS.Geodatabase;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;

namespace MilSpace.DataAccess.Facade
{
    public class DemPreparationFacade
    {
        private static readonly Logger log = Logger.GetLoggerEx("DemPreparationFacade");
        public static SentinelSource GetSourceProductByName(string productName)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var probuct = accessor.GetSoureceByName(productName);
                if (probuct == null)
                {
                    log.InfoEx($"Sentinel product {productName} was not found");
                    return null;
                }

                return probuct.Get();
            }
        }
        public SentinelSource AddSentinelSource(SentinelSource source)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var newSource =  accessor.AddSource(source.Get());

                if (newSource == null)
                {
                    log.ErrorEx($"There was an error on adding Sentinel source {source.SceneId}");
                    return null;
                }
                return newSource.Get();
            }
        }
    }
}
