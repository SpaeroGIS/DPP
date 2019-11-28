using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MilSpace.Core.Tools
{
    public class MapLayersManager
    {
        private readonly List<ILayer> Layers;

        private esriGeometryType[] lineTypes = new esriGeometryType[] { esriGeometryType.esriGeometryLine, esriGeometryType.esriGeometryPolyline };
        private esriGeometryType[] pointTypes = new esriGeometryType[] { esriGeometryType.esriGeometryPoint };
        private esriGeometryType[] polygonTypes = new esriGeometryType[] { esriGeometryType.esriGeometryPolygon };
        private readonly IActiveView activeView;
        private static Logger logger = Logger.GetLoggerEx("MapLayersManager");

        public MapLayersManager(IActiveView activeView)
        {
            this.activeView = activeView;
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

        private static IEnumerable<ILayer> GetFeatureLayers(ILayer layer)
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

        internal List<ILayer> GetAllLayers()
        {
            var layersToReturn = new List<ILayer>();
            try
            {
                var layers = activeView.FocusMap.Layers;
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
                logger.ErrorEx("Error: " + ex.ToString());
                return null;
            }
        }

        internal List<ILayer> GetAllFirstLevelLayers()
        {
            var layersToReturn = new List<ILayer>();
            try
            {
                for(int i = 0; i < activeView.FocusMap.LayerCount; i++)
                {
                   layersToReturn.Add(activeView.FocusMap.Layer[i]);
                }
                    return layersToReturn;
            }
            catch(Exception ex)
            {
                logger.ErrorEx("Error: " + ex.ToString());
                return null;
            }
        }

        public string GetLayerAliasByFeatureClass(string featureClassName)
        {
            string result = null;
            var pattern = @"^[A-Za-z0-9]+\.[A-Za-z0-9]+\." + featureClassName + "$";

            var layer = GetAllLayers().Where(l => l is IFeatureLayer).Cast<IFeatureLayer>().
                FirstOrDefault( fl => fl.FeatureClass.AliasName.Equals(featureClassName, StringComparison.InvariantCultureIgnoreCase) || 
                                                  Regex.IsMatch(fl.FeatureClass.AliasName, pattern));

            if (layer != null)
            {
                result = layer.Name;
            }
            return result;
        }

        public IEnumerable<IRasterLayer> RasterLayers => Layers.Where(layer => layer is IRasterLayer).Cast<IRasterLayer>();

        public IEnumerable<ILayer> PointLayers => GetFeatureLayers(pointTypes);

        public IEnumerable<ILayer> LineLayers => GetFeatureLayers(lineTypes);

        public IEnumerable<ILayer> PolygonLayers => GetFeatureLayers(polygonTypes);

        public IEnumerable<ILayer> FirstLevelLayers => GetAllFirstLevelLayers();

        private IEnumerable<ILayer> GetFeatureLayers(IEnumerable<esriGeometryType> geomType)
        {
            return GetAllLayers().Where(l => l != null && l is IFeatureLayer  && ((IFeatureLayer)l).FeatureClass != null 
            && geomType.Any(g => g == ((IFeatureLayer)l).FeatureClass.ShapeType));
        }
    }
}
