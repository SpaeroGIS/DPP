using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.DTO
{
    internal class TemporaryFeatureClassInfo
    {
        public IFeatureClass TemporaryFeatureClass;
        public bool IsIdsInSourceFromZero;
        public List<int> PointsNotOnTheRaster;
    }
}
