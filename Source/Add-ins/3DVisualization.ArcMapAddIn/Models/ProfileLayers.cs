using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace MilSpace.Visualization3D.Models
{
    static class ProfileLayers
    {

        private static readonly List<ILayer> Layers;

        private static esriGeometryType[] lineTypes = new esriGeometryType[] { esriGeometryType.esriGeometryLine, esriGeometryType.esriGeometryPolyline };
        private static esriGeometryType[] pointTypes = new esriGeometryType[] { esriGeometryType.esriGeometryPoint };
        private static esriGeometryType[] polygonTypes = new esriGeometryType[] { esriGeometryType.esriGeometryPolygon };

        static ProfileLayers()
        {
            Layers = GetAllLayers();
        }

        private static IEnumerable<IRasterLayer> GetRasterLayers(ILayer layer)
        {
            var result = new List<IRasterLayer>();
            

            if (layer is IRasterLayer fLayer)
            {
                result.Add(fLayer);
            }

            if (layer is ICompositeLayer cLayer)
            {

                for (int j = 0; j < cLayer.Count; j++)

                {
                    if ((layer is IRasterLayer cRastreLayer))
                    {
                        result.Add(cRastreLayer);
                    }
                }

            }

            return result;
        }



        internal static IEnumerable<ILayer> GetFeatureLayers(ILayer layer)
        {
            var result = new List<ILayer>();

            if (layer is IFeatureLayer fLayer)
            {
                var featureLayer = fLayer;
                var featureClass = featureLayer.FeatureClass;

                if (featureClass != null &&
                    ((featureClass.ShapeType == esriGeometryType.esriGeometryLine) ||
                    (featureClass.ShapeType == esriGeometryType.esriGeometryPolyline) ||
                    (featureClass.ShapeType == esriGeometryType.esriGeometryPoint) ||
                    (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon)))
                {
                    result.Add(fLayer);
                }
            }
            if (layer is ICompositeLayer cLayer)
            {
                for (int j = 0; j < cLayer.Count; j++)
                {
                    var curLauer = cLayer.Layer[j];
                    if ((curLauer is IFeatureLayer cfeatureLayer))
                    {
                        result.Add(cfeatureLayer);
                    }

                    if (curLauer is ICompositeLayer ccLayer)
                    {
                        // Here can be check by Tag in the Layer description
                        result.AddRange(GetFeatureLayers(curLauer as ILayer));
                    }
                }

            }

            return result;
        }

        internal static List<ILayer> GetAllLayers()
        {
            var layersToReturn = new List<ILayer>();
            try
            {
                var layers = ArcMap.Document.ActiveView.FocusMap.Layers;
                var layer = layers.Next();

                while (layer != null)
                {

                    layersToReturn.AddRange(GetRasterLayers(layer));
                    layersToReturn.AddRange(GetFeatureLayers(layer));

                    layer = layers.Next();
                }

                return layersToReturn;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        internal static IEnumerable<ILayer> RasterLayers => Layers.Where(layer => layer is IRasterLayer);

        internal static IEnumerable<ILayer> PointLayers => GetFeatureLayers(pointTypes);

        internal static IEnumerable<ILayer> LineLayers => GetFeatureLayers(lineTypes);

        internal static IEnumerable<ILayer> PolygonLayers => GetFeatureLayers(polygonTypes);


        private static IEnumerable<ILayer> GetFeatureLayers(IEnumerable<esriGeometryType> geomType)
        {

            return GetAllLayers().Where(l => l is IFeatureLayer && geomType.Any(g => g == ((IFeatureLayer)l).FeatureClass.ShapeType));

        }

    }
}
