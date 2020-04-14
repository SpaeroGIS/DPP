using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Localization;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    internal class PointsFromLayerController
    {
        private MapLayersManager _mapLayersManager;
        private Logger _log = Logger.GetLoggerEx("MilSpace.Profile.ModalWindows.PointsFromLayerController");

        internal PointsFromLayerController(IActiveView activeView)
        {
            _mapLayersManager = new MapLayersManager(activeView);
        }

        public List<FromLayerPointModel> GetPoints(string layerName, string displayedFieldName)
        {
            var layer = _mapLayersManager.GetLayer(layerName) as IFeatureLayer;
            var points = new List<FromLayerPointModel>();

            var featureClass = layer.FeatureClass;
            var idFieldIndex = featureClass.FindField(featureClass.OIDFieldName);
            var selectedFieldIndex = (!string.IsNullOrEmpty(displayedFieldName)) ? featureClass.FindField(displayedFieldName) : -2;

            if(idFieldIndex == -1)
            {
                _log.WarnEx($"> GetPoints. Warning: Cannot find fild {featureClass.OIDFieldName} in featureClass {featureClass.AliasName}");
                MessageBox.Show(String.Format(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле {0}"), featureClass.OIDFieldName),
                                    LocalizationContext.Instance.MessageBoxTitle);

                return null;
            }

            if(selectedFieldIndex == -1)
            {
                _log.WarnEx($"> GetPoints. Warning: Cannot find fild {displayedFieldName} in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = $"{featureClass.OIDFieldName} >= 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while(feature != null)
                {
                    var shape = feature.Shape;

                    if(featureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        var point = shape as IPoint;
                        var pointCopy = point.Clone();
                        pointCopy.Project(EsriTools.Wgs84Spatialreference);

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

                        points.Add(new FromLayerPointModel { Point = pointCopy, ObjId = id, DisplayedField = displayedField });
                    }
                    else
                    {
                        throw new Exception($"> GetPoints. Exception: Layer {layerName} doesn`t have a feature class with type esriGeometryPoint");
                    }

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

            return points;
        }

        public string[] GetPointLayers()
        {

            //TODO: The OP layer should be taken form the controller as an instance and then compared. The layer nmae property can be changed (e.g. localized)
            //TOFix
            var layers = _mapLayersManager.PointLayers.Where(layer => !layer.Name.Equals("MilSp_Visible_ObservPoints")).Select(l => l.Name).ToArray();

            //TODO: If the exception is thrown then it should be catch! Or return a value to be recognized as an exceptional point
            //if (layers.Length == 0)
            //{
                //throw new ArgumentNullException("Required layers are missing in the project");
            //}

            return layers;
            
        }

        public string[] GetLayerFields(string layerName)
        {
            try
            {
                var layer = _mapLayersManager.GetLayer(layerName);
                return  _mapLayersManager.GetFeatureLayerFields(layer as IFeatureLayer).ToArray();
            }
            catch(Exception ex)
            {
                _log.ErrorEx($"> GetLayerFields. Exception: {ex}");
                return null;
            }
        }
    }
}
