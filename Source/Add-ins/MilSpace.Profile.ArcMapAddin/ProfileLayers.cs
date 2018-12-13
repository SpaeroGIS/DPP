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

namespace MilSpace.Profile
{
    static class ProfileLayers
    {
      
        private static readonly List<ILayer> Layers;

        static ProfileLayers()
        {
            Layers = GetAllLayers();
        }

        private static List<ILayer> GetAllLayers()
        {
            var layersToReturn = new List<ILayer>();
            try
            {
                var layers = ArcMap.Document.ActiveView.FocusMap.Layers;
                var layer = layers.Next();

                while (layer != null)
                {
                    if ((layer is IRasterLayer)) 
                    {
                        layersToReturn.Add(layer);
                        layer = layers.Next();
                        continue;
                    }

                    if (layer is IFeatureLayer fLayer)
                    {
                        var featureLayer = fLayer;
                        var featureClass = featureLayer.FeatureClass;
                        if ((featureClass.ShapeType == esriGeometryType.esriGeometryLine) ||
                            (featureClass.ShapeType == esriGeometryType.esriGeometryPoint) ||
                            (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon))
                        {
                            layersToReturn.Add(layer);
                        }
                    }
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

        internal static List<ILayer> RasterLayers => Layers.Where(layer => layer is IRasterLayer).ToList();

        internal static List<ILayer> PointLayers => GetFeatureLayers(esriGeometryType.esriGeometryPoint);
        internal static List<ILayer> LineLayers => GetFeatureLayers(esriGeometryType.esriGeometryLine);

        internal static List<ILayer> PolygonLayers => GetFeatureLayers(esriGeometryType.esriGeometryPolygon);
       

        private static List<ILayer> GetFeatureLayers(esriGeometryType geomType)
        {
            List<ILayer> pointLayers = new List<ILayer>();
            foreach (var layer in Layers)
            {
                if (!(layer is IFeatureLayer featureLayer)) continue;
                if (featureLayer.FeatureClass.ShapeType == geomType)
                    pointLayers.Add(layer);

                IGroupLayer a;
                
            }

            return pointLayers;
        }

       }
}
