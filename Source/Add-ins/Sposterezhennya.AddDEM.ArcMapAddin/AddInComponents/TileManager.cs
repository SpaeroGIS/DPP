using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents
{
    internal class TileManager
    {
        private static string srtmGrigsFeatureClassName = "MILSP_SRTM_GRID";
        private static string s1GrigsFeatureClassName = "MILSP_S1_GRID";

        static IFeatureClass srtmGridFeatureClass;
        static IFeatureClass s1GridFeatureClass;

        public static IFeatureClass SrtmGridFeatureClass
        {
            get
            {
                if (srtmGridFeatureClass == null)
                {
                    srtmGridFeatureClass = GdbAccess.Instance.GetFeatureFromWorkingWorkspace(srtmGrigsFeatureClassName);
                }

                return srtmGridFeatureClass;
            }
        }

        public static IFeatureClass S1GridFeatureClass
        {
            get
            {
                if (s1GridFeatureClass == null)
                {
                    s1GridFeatureClass = GdbAccess.Instance.GetFeatureFromWorkingWorkspace(s1GrigsFeatureClassName);
                }

                return s1GridFeatureClass;
            }
        }

        public static IEnumerable<S1Grid> GetS1GridByArea(IGeometry area)
        {
            return AddDemFacade.GetS1GridsByIds(Intersect(area, S1GridFeatureClass));
        }
        public static IEnumerable<SrtmGrid> GetSrtmGridByArea(IGeometry area)
        {
            return AddDemFacade.GetSrtmGridsByIds(Intersect(area, SrtmGridFeatureClass));
        }

        private static IEnumerable<int> Intersect(IGeometry area, IFeatureClass featureClass)
        {
            if (featureClass == null)
            {
                throw new Exception("Cannot find Grid Feature class");
            }
            ISpatialFilter spatialFilter = new SpatialFilterClass
            {
                Geometry = area,
                GeometryField = featureClass.ShapeFieldName,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
            };

            IFeatureCursor cursor = featureClass.Search(spatialFilter, false);
            IFeature grid = null;
            var results = new List<int>();
            while ((grid = cursor.NextFeature()) != null)
            {
                results.Add(Convert.ToInt32(grid.get_Value(0)));
            }

            // Discard the cursors as they are no longer needed.
            Marshal.ReleaseComObject(cursor);

            return results;



        }

    }
}
