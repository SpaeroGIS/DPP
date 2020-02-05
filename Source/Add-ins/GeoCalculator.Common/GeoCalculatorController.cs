using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.GeoCalculator
{
    public class GeoCalculatorController
    {
        private LocalizationContext _context = new LocalizationContext();
        private static Logger _log = Logger.GetLoggerEx("MilSpace.GeoCalculator.GeoCalculatorController");

        internal void SetView(IGeoCalculatorView view)
        {
            View = view;
        }

        internal IGeoCalculatorView View { get; private set; }


        internal void ExportToLayer(IEnumerable<IPoint> points)
        {
            _log.DebugEx("> ExportToLayer START.");

            var fClassName = $"GCP_{DateTime.Now.ToString("yyyyMMddHHmmss")}_P";

            var featureClass = GdbAccess.Instance.AddCalcPointsFeature(points, fClassName, EsriTools.Wgs84Spatialreference);
            ArcMapHelper.AddFeatureClassToMap(featureClass);

            _log.DebugEx("> ExportToLayer END.");
        }

        internal void ImportFromLayer(string layerName)
        {
            _log.DebugEx("> ImportFromLayer START.");

            var spatialReference = ArcMap.Document.ActiveView.FocusMap.SpatialReference;
            var layer = ArcMapHelper.GetLayer(layerName) as IFeatureLayer;
            var points = new List<IPoint>();

            var featureClass = layer.FeatureClass;


            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = "OBJECTID > 0";

           

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
                        points.Add(point);
                    }
                    else
                    {
                        var path = shape as IPointCollection;

                        for(int i = 0; i < path.PointCount; i++)
                        {
                            points.Add(path.Point[i]);
                        }
                    }

                    feature = featureCursor.NextFeature();
                }

            }
            catch(Exception ex)
            {
                _log.ErrorEx($"> ImportFromLayer Exception. ex.Message:{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            if(points.Count > 500)
            {
                var dialogResult = MessageBox.Show(String.Format(_context.FindLocalizedElement("BigFeatureClassMessage", "Отримано {0} точок. Імпорт такого об'єму може зайняти багато часу.\nВи бажаєте продовжити?"), points.Count),
                                                        _context.WarningString, MessageBoxButtons.OKCancel);

                if(dialogResult != DialogResult.OK)
                {
                    _log.DebugEx("> ImportFromLayer CANCELED.");
                    return;
                }
            }

            View.AddPointsToGrid(points);

            _log.DebugEx("> ImportFromLayer END.");
        }
    }
}
