using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Tools;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    internal class GeometriesFromLayerController
    {
        private MapLayersManager _mapLayersManager = new MapLayersManager(ArcMap.Document.ActiveView);
        private Logger _log = Logger.GetLoggerEx("MilSpace.Profile.ModalWindows.GeometriesFromLayerController");

        public List<FromLayerGeometry> GetGeometries(string layerName, string displayedFieldName)
        {
            var layer = _mapLayersManager.GetLayer(layerName) as IFeatureLayer;
            var geometries = new List<FromLayerGeometry>();

            var featureClass = layer.FeatureClass;
            var idFieldIndex = featureClass.FindField("OBJECTID");
            var selectedFieldIndex = (!string.IsNullOrEmpty(displayedFieldName)) ? featureClass.FindField(displayedFieldName) : -2;

            if(idFieldIndex == -1)
            {
                _log.WarnEx($"> GetGeometries. Warning: Cannot find fild \"OBJECTID\" in featureClass {featureClass.AliasName}");
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле OBJECTID"),
                                    LocalizationContext.Instance.MessageBoxTitle);

                return null;
            }

            if(selectedFieldIndex == -1)
            {
                _log.WarnEx($"> GetGeometries. Warning: Cannot find fild {displayedFieldName} in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = "OBJECTID > 0";

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

        public List<string> GetNotPointFeatureLayers()
        {
            var layers = _mapLayersManager.PolygonLayers.Where(l => !(l as IFeatureLayer).FeatureClass.AliasName.EndsWith("MilSp_Visible_ObjectsObservation_R")).Select(layer => layer.Name).ToList();
            layers.AddRange(_mapLayersManager.LineLayers.Select(layer => layer.Name));

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
