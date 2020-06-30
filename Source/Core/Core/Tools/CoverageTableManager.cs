using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.SurfaceProfile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.Tools
{
    public class CoverageTableManager
    {
        private List<CoverageAreaData> _coverageAreaData = new List<CoverageAreaData>();
        private List<CoverageTableRowModel> _coverageTableModel = new List<CoverageTableRowModel>();
        private Dictionary<int, IPolygon> _objPolygons = new Dictionary<int, IPolygon>();
        private double _totalVisibleArea;
        private IFeatureClass _observationStationFeatureClass = null;
        private VisibilityCalcTypeEnum _calcType;

        private Logger _logger = Logger.GetLoggerEx("MilSpace.Tools.CoverageTableManager");

        private readonly string _gdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;
        private const string _allTitle = "All";

        public void SetCalculateAreas(
            string observerPointsFeatureClassName,
            string observatoinStationFeatureClassName)
        {
            _logger.InfoEx("> SetCalculateAreas START. observPointFC:{0} observObjFC:{1}", observerPointsFeatureClassName,
                                                                                           observatoinStationFeatureClassName);

            if (!String.IsNullOrEmpty(observatoinStationFeatureClassName))
            {
                _calcType = VisibilityCalcTypeEnum.ObservationObjects;
                _observationStationFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, observatoinStationFeatureClassName);

                SetObjPolygons(observatoinStationFeatureClassName);
            }
            else
            {
                _calcType = VisibilityCalcTypeEnum.OpservationPoints;
            }

            //_observerPointsFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, observerPointsFeatureClassName);
            SetCoverageAreas(observerPointsFeatureClassName);


            var totalExpectedPolygon = _coverageAreaData.FirstOrDefault(area => area.PointId == -1);
            if (totalExpectedPolygon != null)
            {
                var totalArea = (IArea)(totalExpectedPolygon.Polygon);
                _logger.InfoEx("> SetCalculateAreas END. calcType:{0} totalExpectedArea:{1}", _calcType.ToString(), totalArea.Area);
            }
            else
            {
                _logger.WarnEx($"> There is no Coverege on {observerPointsFeatureClassName} and {observerPointsFeatureClassName}");
            }
        }

        public void AddPotentialArea(string featureClassName, bool isTotal, int currPointId = 0)
        {
            _logger.InfoEx("> AddPotentialArea START featureClassName:{0} isTotal:{1} currPointId:{2}",
                featureClassName, isTotal.ToString(), currPointId);
            try
            {
                if (isTotal)
                {
                    currPointId = -1;
                }
                var pointAreas = _coverageAreaData.Where(area => area.PointId == currPointId);
                //var ExpectedPolygon = _coverageAreaData.First(area => area.PointId == currPointId).Polygon;
                if (pointAreas != null)
                {
                    foreach (var area in pointAreas)
                    {
                        GdbAccess.Instance.AddCoverageAreaFeature(
                            area.Polygon,
                            area.PointId,
                            featureClassName,
                            VisibilityManager.CurrentMap.SpatialReference);
                    }
                }
                _logger.InfoEx("> AddPotentialArea END");
            }
            catch (Exception ex)
            {
                _logger.ErrorEx("> AddPotentialArea EXCEPTION. ex.Message:{0}", ex.Message);
            }
        }

        private void SetObjPolygons(string observationStationFeatureClassName)
        {
            _logger.DebugEx("> SetObjPolygons START");

            var observationStationFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, observationStationFeatureClassName);

            IQueryFilter queryFilter = new QueryFilter
            {
                WhereClause = $"{observationStationFeatureClass.OIDFieldName} >= 0"
            };

            IFeatureCursor featureCursor = observationStationFeatureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();

            try
            {
                while (feature != null)
                {
                    var objId = feature.OID;
                    var objGeom = feature.ShapeCopy as IPolygon;
                    objGeom.Project(VisibilityManager.CurrentMap.SpatialReference);
                    _objPolygons.Add(objId, objGeom as IPolygon);

                    feature = featureCursor.NextFeature();
                }

                var totalObjArea = EsriTools.GetTotalPolygon(_objPolygons.Select(area => { return area.Value; }).ToList());

                _objPolygons.Add(-1, totalObjArea);
            }
            catch (Exception ex)
            {
                _logger.ErrorEx("> SetObjPolygons Exception: {0}", ex.Message);
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            _logger.DebugEx("> SetObjPolygons END");
        }

        private void SetCoverageAreas(string observerPointsFeatureClassName)
        {
            _logger.DebugEx("> SetCoverageAreas START");

            var observerPointsFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, observerPointsFeatureClassName);

            var observationPoints = GetObservationPoints(observerPointsFeatureClassName);

            if (observationPoints == null || observationPoints.Count() == 0)
            {
                _logger.ErrorEx($"> SetCoverageAreas END. Observation points are not found");
                return;
            }

            var observObjPolygons = new Dictionary<int, IPolygon>();

            foreach (var pointModel in observationPoints)
            {
                var point = observerPointsFeatureClass.GetFeature(pointModel.Objectid);
                IPoint pointGeom = point.Shape as IPoint;
                pointGeom.Project(VisibilityManager.CurrentMap.SpatialReference);

                var realMaxDistance = EsriTools.GetMaxDistance(pointModel.OuterRadius.Value, pointModel.AngelMaxH.Value, pointModel.RelativeHeight.Value);
                var realMinDistance = EsriTools.GetMinDistance(pointModel.InnerRadius.Value, pointModel.AngelMinH.Value, pointModel.RelativeHeight.Value);

                if (realMaxDistance < realMinDistance)
                {
                    _logger.WarnEx("> SetCoverageAreas END. Observation point doesn`t has a coverage area");
                    return;
                }

                var visibilityPolygon = EsriTools.GetCoverageArea(
                    pointGeom,
                    pointModel.AzimuthStart.Value,
                    pointModel.AzimuthEnd.Value,
                    realMinDistance,
                    realMaxDistance);

                _coverageAreaData.Add(new CoverageAreaData
                {
                    ObjId = -1,
                    PointId = pointModel.Objectid,
                    Polygon = visibilityPolygon
                });
            }

            var totalArea = EsriTools.GetTotalPolygon(_coverageAreaData.Select(area => { return area.Polygon; }).ToList());

            _coverageAreaData.Add(new CoverageAreaData
            {
                ObjId = -1,
                PointId = -1,
                Polygon = totalArea
            });

            _logger.DebugEx("> SetCoverageAreas END");
        }

        public void CalculateCoverageTableDataForPoint(bool isTotal, string visibilityAreasFCName,
                                                        int pointCount, string currentPointClassName,
                                                        int curPointId)
        {
            if (_calcType == VisibilityCalcTypeEnum.ObservationObjects)
            {
                CalculateCoverageTableVADataForPoint(isTotal, currentPointClassName, visibilityAreasFCName,
                                                        pointCount, curPointId);
            }
            else if (_calcType == VisibilityCalcTypeEnum.OpservationPoints)
            {
                CalculateCoverageTableVSDataForPoint(isTotal, currentPointClassName, visibilityAreasFCName,
                                                        pointCount, curPointId);
            }
        }

        public void SaveDataToCoverageTable(string tableName)
        {
            if (_calcType == VisibilityCalcTypeEnum.ObservationObjects)
            {
                GdbAccess.Instance.FillVACoverageTable(_coverageTableModel, tableName, _gdb);
            }
            else
            {
                GdbAccess.Instance.FillVSCoverageTable(_coverageTableModel, tableName, _gdb);
            }
        }

        private void CalculateCoverageTableVSDataForPoint(bool isTotal, string currentPointFeatureClassName,
                                                          string visibilityAreasFCName, int pointCount, int curPointId)
        {
            if (isTotal)
            {
                CalculateVSTotalValues(pointCount, visibilityAreasFCName);
            }

            if (!isTotal || pointCount == 1)
            {
                if(isTotal)
                {
                    curPointId = 1;
                }

                var observPointFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, currentPointFeatureClassName);
                var observPoint = GetObservationPoints(currentPointFeatureClassName).First();

                var totalExpectedPolygonData = _coverageAreaData.FirstOrDefault(area => area.PointId == -1);

                if (totalExpectedPolygonData == null || totalExpectedPolygonData.Polygon == null)
                {
                    AddEmptyAreaRow(observPoint.Title, curPointId);
                    return;
                }

                var visibilityPolygonsForPointFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);

                //TODO: Here is the problem  with ID on the first type of calc!!!
                if (_coverageAreaData.Any(area => area.PointId == curPointId))
                {

                    var expectedPolygon = _coverageAreaData.FirstOrDefault(area => area.PointId == curPointId).Polygon;

                    var totalExpectedArea = GetProjectedPolygonArea(totalExpectedPolygonData.Polygon);

                    var expectedPolygonArea = GetProjectedPolygonArea(expectedPolygon);
                    var visibleArea = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsForPointFeatureClass, VisibilityManager.CurrentMap);

                    AddVSRowModel(observPoint.Title, curPointId, 1, expectedPolygonArea, visibleArea, totalExpectedArea);
                }
            }
        }

        private void CalculateCoverageTableVADataForPoint(bool isTotal, string currentPointFeatureClassName,
                                                          string visibilityAreasFCName, int pointCount, int curPointId)
        {
            _logger.InfoEx("> CalculateCoverageTableVADataForPoint START. isTotal:{0} currPointId:{1}", isTotal.ToString(), curPointId);

            if (isTotal)
            {
                CalculateVATotalValues(pointCount, visibilityAreasFCName);
            }

            if (!isTotal || pointCount == 1)
            {
                var observPoint = GetObservationPoints(currentPointFeatureClassName).First();
                if (observPoint == null)
                {
                    _logger.ErrorEx($"> CalculateCoverageTableVADataForPoint END. Observation point with id {curPointId} is not found");
                    return;
                }
                else
                {
                    _logger.DebugEx("CalculateCoverageTableVADataForPoint. observPoint.Title: {0}", observPoint.Title);
                }

                try
                {
                    var totalObjArea = (IArea)_objPolygons[-1];
                    var visibilityPolygonsForPointFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);

                    foreach (var polygon in _objPolygons)
                    {
                        if (polygon.Key == -1)
                        {
                            continue;
                        }

                        var obj = VisibilityManager.GetObservationObjectsFromFeatureClass(_observationStationFeatureClass)
                                                   .FirstOrDefault(observationObject => observationObject.ObjectId == polygon.Key);

                        if (obj == null)
                        {
                            continue;
                        }

                        if (totalObjArea.Area == 0)
                        {
                            AddEmptyAreaRow(observPoint.Title, curPointId, polygon.Key, obj.Title);
                            continue;
                        }

                        var visibilityArea = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, VisibilityManager.CurrentMap, polygon.Value);
                        var objArea = GetProjectedPolygonArea(polygon.Value);

                        AddVARowModel(observPoint.Title, curPointId, obj.Title, polygon.Key, 1, visibilityArea, objArea);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorEx($"> CalculateCoverageTableVADataForPoint Exception: {ex.Message}");
                    return;
                }
            }
            _logger.InfoEx("> CalculateCoverageTableVADataForPoint END");
        }

        private void CalculateVATotalValues(int pointsCount, string visibilityAreasFCName)
        {
            _logger.InfoEx("> CalculateVATotalValues START");

            var visibilityPolygonsForPointFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);
            var totalObjArea = GetProjectedPolygonArea(_objPolygons[-1]);

            try
            {
                foreach (var polygon in _objPolygons)
                {
                    if (polygon.Key == -1)
                    {
                        continue;
                    }

                    var obj = VisibilityManager.GetObservationObjectsFromFeatureClass(_observationStationFeatureClass)
                                                    .FirstOrDefault(observationObject => observationObject.ObjectId == polygon.Key);

                    if (obj == null)
                    {
                        _logger.ErrorEx($"CalculateVATotalValues. Observation object with id {polygon.Key} is not found");
                        return;
                    }

                    if (totalObjArea == 0)
                    {
                        AddEmptyAreaRow(_allTitle, -1, polygon.Key, obj.Title, -1);
                        continue;
                    }

                    var visibilityArea = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, VisibilityManager.CurrentMap, polygon.Value);
                    var polygonArea = GetProjectedPolygonArea(polygon.Value);

                    AddVARowModel(_allTitle, -1, obj.Title, polygon.Key, -1, visibilityArea, polygonArea);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorEx($"CalculateVATotalValues Exception(1): {ex.Message}");
            }

            try
            {
                for (int i = 1; i <= pointsCount; i++)
                {
                    var areaByPointsSee = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, VisibilityManager.CurrentMap, _objPolygons[-1], i);
                    var polygon = _objPolygons[-1];

                    var polygonArea = GetProjectedPolygonArea(polygon); 

                    AddVARowModel(_allTitle, -1, _allTitle, -1, i, areaByPointsSee, polygonArea);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorEx($"CalculateVATotalValues Exception(2): {ex.Message}");
            }

            _logger.InfoEx("> CalculateVATotalValues END");
        }

        private void CalculateVSTotalValues(int pointsCount, string visibilityAreasFCName)
        {
            var totalExpectedPolygonData = _coverageAreaData.FirstOrDefault(area => area.PointId == -1);

            if (totalExpectedPolygonData == null)
            {
                AddEmptyAreaRow(_allTitle, -1);
                return;
            }

            try
            {
                var visibilityPolygonsFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);
                _totalVisibleArea = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsFeatureClass, VisibilityManager.CurrentMap);

                var totalExpectedArea = GetProjectedPolygonArea(totalExpectedPolygonData.Polygon);

                AddVSRowModel(_allTitle, -1, -1, totalExpectedArea, _totalVisibleArea, totalExpectedArea);

                for (int i = 1; i <= pointsCount; i++)
                {
                    var areaByPointsSee = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsFeatureClass, VisibilityManager.CurrentMap, i);
                    AddVSRowModel(_allTitle, -1, i, totalExpectedArea, areaByPointsSee, totalExpectedArea);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorEx($"> CalculateVSTotalValues Exception:{ex.Message}");
            }
        }

        private void AddEmptyAreaRow(
            string observPointTitle,
            int observPointId,
            int observObjId = -1,
            string observObjName = null,
            int pointSee = 0)
        {
            _coverageTableModel.Add(new CoverageTableRowModel
            {
                ObservPointName = observPointTitle,
                ObservPointId = observPointId,
                ObservObjId = observObjId,
                ObservObjName = observObjName,
                ObservObjArea = 0,
                ExpectedArea = 0,
                VisibilityArea = 0,
                VisibilityPercent = 0,
                ToAllExpectedAreaPercent = 0,
                ToAllVisibilityAreaPercent = 0,
                ObservPointsSeeCount = pointSee
            });
        }

        private void AddVSRowModel(string pointName, int pointId, int pointsCount,
                                    double expectedArea, double visibleArea, double totalExpectedArea)
        {
            _logger.InfoEx("> AddVSRowModel START. pointName:{0} visibleArea:{1}", pointName, visibleArea);

            _coverageTableModel.Add(new CoverageTableRowModel
            {
                ObservPointName = pointName,
                ObservPointId = pointId,
                ExpectedArea = Convert.ToInt32(Math.Round(expectedArea, 0)),
                VisibilityArea = Convert.ToInt32(Math.Round(visibleArea, 0)),
                VisibilityPercent = GetPercent(expectedArea, visibleArea),
                ToAllExpectedAreaPercent = GetPercent(totalExpectedArea, expectedArea),
                ToAllVisibilityAreaPercent = GetPercent(totalExpectedArea, visibleArea),
                ObservPointsSeeCount = pointsCount
            });
            _logger.InfoEx("> AddVSRowModel END");
        }

        private void AddVARowModel(string pointName, int pointId, string objName, int objId,
                                    int pointsCount, double visibleArea, double objPolygonArea)
        {
            _logger.InfoEx("> AddVARowModel START. pointName:{0} objName:{1} visibleArea:{2}", pointName, objName, visibleArea);

            _coverageTableModel.Add(new CoverageTableRowModel
            {
                ObservPointName = pointName,
                ObservPointId = pointId,
                ObservObjId = objId,
                ObservObjName = objName,
                ObservObjArea = Convert.ToInt32(Math.Round(objPolygonArea, 0)),
                VisibilityArea = Convert.ToInt32(Math.Round(visibleArea, 0)),
                VisibilityPercent = GetPercent(objPolygonArea, visibleArea),
                ObservPointsSeeCount = pointsCount
            });
            _logger.InfoEx("> AddVARowModel END");
        }

        private double GetPercent(double totalArea, double pointArea)
        {
            return Math.Round(((pointArea * 100) / totalArea), 1);
        }

        private double GetProjectedPolygonArea(IPolygon polygon)
        {
            polygon.Project(VisibilityManager.CurrentMap.SpatialReference);

            var polygonArea = (IArea)polygon;
            return polygonArea.Area;
        }
        
        private IEnumerable<ObservationPoint> GetObservationPoints(string observerPointsFeatureClassName)
        {
            var observerPointsFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, observerPointsFeatureClassName);
            var pointsFromLayer = VisibilityManager
                                       .GetObservationPointsFromAppropriateLayer(string.Empty, null,
                                                                                  null, observerPointsFeatureClass);

            if (!pointsFromLayer.Any())
            {
                Debugger.Launch();
            }

            return pointsFromLayer.Select(point => { return point as ObservationPoint; }).ToArray();
        }

    }
}
