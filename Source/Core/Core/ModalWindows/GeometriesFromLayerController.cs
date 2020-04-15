using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Localization;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    internal class GeometriesFromLayerController
    {
        private MapLayersManager _mapLayersManager;
        private Logger _log = Logger.GetLoggerEx("MilSpace.Core.ModalWindows.GeometriesFromLayerController");

        internal GeometriesFromLayerController(IActiveView activeView)
        {
            _mapLayersManager = new MapLayersManager(activeView);
        }

        public List<FromLayerGeometry> GetGeometries(string layerName, string displayedFieldName)
        {
            var layer = _mapLayersManager.GetLayer(layerName) as IFeatureLayer;
            var geometries = new List<FromLayerGeometry>();

            var featureClass = layer.FeatureClass;
            var idFieldIndex = featureClass.FindField(featureClass.OIDFieldName);
            var selectedFieldIndex = (!string.IsNullOrEmpty(displayedFieldName)) ? featureClass.FindField(displayedFieldName) : -2;

            if(idFieldIndex == -1)
            {
                _log.WarnEx($"> GetGeometries. Warning: Cannot find fild \"{featureClass.OIDFieldName}\" in featureClass {featureClass.AliasName}");
                MessageBox.Show(String.Format(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле {0}"), featureClass.OIDFieldName),
                                    LocalizationContext.Instance.MessageBoxTitle);

                return null;
            }

            if(selectedFieldIndex == -1)
            {
                _log.WarnEx($"> GetGeometries. Warning: Cannot find fild {displayedFieldName} in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = $"{featureClass.OIDFieldName} >= 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while(feature != null)
                {
                    var shape = feature.ShapeCopy;

                    var geometry = shape as IGeometry;
                    
                    int id = -1;
                    string displayedField = string.Empty;

                    if(idFieldIndex >= 0)
                    {
                        id = (int)feature.Value[idFieldIndex];
                    }

                    if(selectedFieldIndex >= 0)
                    {
                        displayedField = feature.Value[selectedFieldIndex].ToString();
                    }

                    geometries.Add(new FromLayerGeometry { Geometry = geometry, ObjId = id, Title = displayedField });

                    feature = featureCursor.NextFeature();
                }
            }
            catch(Exception ex)
            {
                _log.ErrorEx($"> GetPoints Exception. ex.Message:{ex.Message}");
                return null;
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            return geometries;
        }
        public IEnumerable<string> GetFeatureLayers()
        {
            return _mapLayersManager.GetFeatureLayersNames();
        }

        public List<string> GetFeatureLayers(bool withObservObj = false, bool withPointLayers = false)
        {
            var layers = new List<string>();

            if(withObservObj)
            {
                layers = _mapLayersManager.PolygonLayers.Select(layer => layer.Name).ToList();
            }
            else
            {
                var visibilityModule = ModuleInteraction.Instance.GetModuleInteraction<IVisibilityInteraction>(out bool changes);

                if (!changes && visibilityModule == null)
                {
                    layers = _mapLayersManager.PolygonLayers.Select(layer => layer.Name).ToList();
                }
                else
                {
                    var observObjFeatureClassName = visibilityModule.GetObservationStationFeatureClassName();
                    layers = _mapLayersManager.PolygonLayers.Where(l => !(l as IFeatureLayer).FeatureClass.AliasName.EndsWith(observObjFeatureClassName)).Select(layer => layer.Name).ToList();
                }
            }

            layers.AddRange(_mapLayersManager.LineLayers.Select(layer => layer.Name));

            if(withPointLayers)
            {
                layers.AddRange(_mapLayersManager.PointLayers.Select(layer => layer.Name));
            }

            if(layers.Count == 0)
            {
                throw new ArgumentNullException("Required layers are missing in the project");
            }

            return layers;
        }

        public string[] GetLayerFields(string layerName)
        {
            try
            {
                var layer = _mapLayersManager.GetLayer(layerName);
                return _mapLayersManager.GetFeatureLayerFields(layer as IFeatureLayer).ToArray();
            }
            catch(Exception ex)
            {
                _log.ErrorEx($"> GetLayerFields. Exception: {ex}");
                return null;
            }
        }
    }
}
