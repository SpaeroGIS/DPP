using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
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

        private readonly esriGeometryType[] lineTypes = new esriGeometryType[] { esriGeometryType.esriGeometryLine, esriGeometryType.esriGeometryPolyline };
        private readonly esriGeometryType[] pointTypes = new esriGeometryType[] { esriGeometryType.esriGeometryPoint };
        private readonly esriGeometryType[] polygonTypes = new esriGeometryType[] { esriGeometryType.esriGeometryPolygon };
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
                for (int i = 0; i < activeView.FocusMap.LayerCount; i++)
                {
                    layersToReturn.Add(activeView.FocusMap.Layer[i]);
                }
                return layersToReturn;
            }
            catch (Exception ex)
            {
                logger.ErrorEx("Error: " + ex.ToString());
                return null;
            }
        }


        public ILayer GetLayer(string layerName)
        {
            return GetAllLayers().FirstOrDefault(layer => layer.Name.Equals(layerName));
        }
        
        public List<string> GetFeatureLayerFields(IFeatureLayer layer)
        {
            var fieldsNames = new List<string>();

            var featureClass = layer.FeatureClass;
            var fields = featureClass.Fields;

            for(int i = 0; i < fields.FieldCount; i++)
            {
                if(!fields.Field[i].Name.Equals(featureClass.ShapeFieldName) && !(fields.Field[i].Name.Equals(featureClass.OIDFieldName)))
                {
                    fieldsNames.Add(fields.Field[i].AliasName);
                }
            }

            return fieldsNames;
        }

        public string GetLayerAliasByFeatureClass(string featureClassName)
        {
            string result = null;
            var pattern = @"^[A-Za-z0-9]+\.[A-Za-z0-9]+\." + featureClassName + "$";

            var layer = GetAllLayers().Where(l => l is IFeatureLayer).Cast<IFeatureLayer>().
                FirstOrDefault(fl => fl.FeatureClass.AliasName.Equals(featureClassName, StringComparison.InvariantCultureIgnoreCase) ||
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

        public IFeatureLayer FindFeatureLayer(string layerNameOrAlias)
        {
            return GetAllLayers().FirstOrDefault(l => l != null && l is IFeatureLayer  && ((IFeatureLayer)l).FeatureClass != null
            && ((IFeatureLayer)l).FeatureClass.AliasName.EndsWith(layerNameOrAlias, StringComparison.InvariantCultureIgnoreCase)) as IFeatureLayer;
            
        }

        public ILayer LastLayer
        {
            get
            {
                var map = activeView.FocusMap;
                return map.LayerCount == 0 ? null : map.Layer[map.LayerCount - 1];
            }
        }

        public bool InserLayer(ILayer layerToAdd, string layerName, bool before = false)
        {
            var mapLayers = activeView.FocusMap as IMapLayers2;

            var i = 0;

            if (!string.IsNullOrWhiteSpace(layerName))
            {
                var lr = GetAllLayers().FirstOrDefault(l => l.Name == layerName);
                if (lr != null)
                {
                    for (int index = 0; index < activeView.FocusMap.LayerCount; index++)
                    {
                        ILayer layerAtIndex = activeView.FocusMap.get_Layer(index);
                        if (layerAtIndex == lr)
                        {
                            i = before ? index : index++;
                            break;
                        }
                    }
                }
            }

            mapLayers.InsertLayer(layerToAdd, false, i);
            return true;
        }


        public bool InserLayerBefore(ILayer layerToAdd, string layerName)
        {
            var lr = GetAllLayers().FirstOrDefault(l => l.Name == layerName);
            if (lr == null)
            {
                return false;
            }
            var mapLayers = activeView.FocusMap as IMapLayers2;

            for (int index = 0; index < activeView.FocusMap.LayerCount; index++)
            {
                ILayer layerAtIndex = activeView.FocusMap.get_Layer(index);
                if (layerAtIndex == lr)
                {
                    mapLayers.InsertLayer(layerToAdd, false, index);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> GetFeatureLayersNames()
        {
            var types = new esriGeometryType[3] { esriGeometryType.esriGeometryPoint, esriGeometryType.esriGeometryPolygon, esriGeometryType.esriGeometryPolyline };
            var layers = GetFeatureLayers(types);
            var layesStrings = layers.Select(l => l.Name);

            return layesStrings;
        }

        public IEnumerable<string> GetObservPointsAppropriateLayers()
        {
            var appropriateLayers = new List<string>();

            try
            {
                foreach (var layer in PointLayers)
                {
                    var featureLayer = layer as IFeatureLayer;
                    var featureClass = featureLayer.FeatureClass;

                    if (!HasFeatureClassObserverPointFields(featureClass)) 
                    {
                        continue;
                    }

                    appropriateLayers.Add(layer.Name);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"> GetObservPointsAppropriateLayers Exception. ex.Message:{ex.Message}");
            }

            return appropriateLayers;
        }

        public bool HasFeatureClassObserverPointFields(IFeatureClass featureClass)
        {
            return featureClass.FindField(featureClass.OIDFieldName) > -1 
                    && featureClass.FindField("AzimuthB") > -1
                    && featureClass.FindField("AzimuthE") > -1 
                    && featureClass.FindField("AnglMinH") > -1 
                    && featureClass.FindField("AnglMaxH") > -1;
        }

        public bool InserLayerAfter(ILayer layerToAdd, string layerName)
        {
            var lr = GetAllLayers().FirstOrDefault(l => l.Name == layerName);
            if (lr == null)
            {
                return false;
            }
            var mapLayers = activeView.FocusMap as IMapLayers2;

            for (int index = 0; index < activeView.FocusMap.LayerCount; index++)
            {
                ILayer layerAtIndex = activeView.FocusMap.get_Layer(index);
                if (layerAtIndex == lr)
                {
                    mapLayers.InsertLayer(layerToAdd, false, index + 1);
                    return true;
                }
            }

            return false;
        }
        private IEnumerable<ILayer> GetFeatureLayers(IEnumerable<esriGeometryType> geomType)
        {
            return GetAllLayers().Where(l => l != null && l is IFeatureLayer && ((IFeatureLayer)l).FeatureClass != null
            && geomType.Any(g => g == ((IFeatureLayer)l).FeatureClass.ShapeType));
        }
    }
}
