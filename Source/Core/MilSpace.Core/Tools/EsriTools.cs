using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.Tools
{
    public class EsriTools
    {

        private static ISpatialReference wgs84 = null;

        public static void ProjectToWgs84(IGeometry geometry)
        {
            try
            {
                geometry.Project(Wgs84Spatialreference);
            }
            catch(Exception ex)
            {
                //ToDO: Loggig
            }

        }

        public static void ProjectToMapSpatialReference(IGeometry geometry, ISpatialReference mapSpatialReference)
        {

            try
            {
                geometry.Project(mapSpatialReference);
            }
            catch (Exception ex)
            {
                //ToDO: Loggig
            }
        }
            

        public static ISpatialReference Wgs84Spatialreference
        {
            get
            {
                if (wgs84 == null)
                {
                    SpatialReferenceEnvironmentClass factory = new SpatialReferenceEnvironmentClass();
                    wgs84 = factory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
                }

                return wgs84;
            }
        }

    }
}
