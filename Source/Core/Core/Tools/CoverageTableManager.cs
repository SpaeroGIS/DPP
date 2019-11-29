using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.SurfaceProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools
{
    public class CoverageTableManager
    {
        private List<CoverageAreaData> _coverageAreaData = new List<CoverageAreaData>();
        private List<CoverageTableRowModel> _coverageTableModel = new List<CoverageTableRowModel>();
        private Dictionary<int, IPolygon> _objPolygons = new Dictionary<int, IPolygon>();
        private double _totalExpectedArea;
        private double _totalVisibleArea;
        private string _allTitle = "All";
        private VisibilityCalcTypeEnum _calcType;

        private string _gdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;

        public  void CalculateAreas(int[] observPointsIds, int[] observObjectsIds, IFeatureClass observPointFC, IFeatureClass observObjFC = null)
        {
            if(observObjectsIds != null && observObjectsIds.Count() > 0 && observObjFC != null)
            {
                _calcType = VisibilityCalcTypeEnum.ObservationObjects;
                SetObjPolygons(observObjectsIds, observObjFC);
            }
            else
            {
                _calcType = VisibilityCalcTypeEnum.OpservationPoints;
            }

            SetCoverageAreas(observPointsIds, observPointFC);

            var totalExpectedPolygon = _coverageAreaData.First(area => area.PointId == -1).Polygon;
            var totalExpectedPolygonArea = (IArea)totalExpectedPolygon;
            _totalExpectedArea = totalExpectedPolygonArea.Area;
        }

        public  void AddPotentialArea(string featureClassName, bool isTotal, int currPointId = 0)
        {
            if(isTotal)
            {
                currPointId = -1;
            }

            var pointAreas = _coverageAreaData.Where(area => area.PointId == currPointId);

            if(pointAreas != null)
            {
                foreach(var area in pointAreas)
                {
                    GdbAccess.Instance.AddCoverageAreaFeature(area.Polygon, area.PointId, featureClassName, _gdb);
                }
            }

        }

        private void SetObjPolygons(int[] observObjectsIds, IFeatureClass observObjFC)
        {
            foreach(var objId in observObjectsIds)
            {
                var obj = observObjFC.GetFeature(objId);
                var objGeom = obj.Shape as IPolygon;
                objGeom.Project(VisibilityManager.CurrentMap.SpatialReference);

                _objPolygons.Add(objId, objGeom);

                var objArea = (IArea)objGeom;
            }

            var totalObjArea = EsriTools.GetTotalPolygon(_objPolygons.Select(area => { return area.Value; }).ToList());

            _objPolygons.Add(-1, totalObjArea);
        }

        private void SetCoverageAreas(int[] observPointsIds, IFeatureClass observPointFC)
        {
            var observPoints = VisibilityZonesFacade.GetObservationPointByObjectIds(observPointsIds);
            var observObjPolygons = new Dictionary<int, IPolygon>();

            foreach(var pointId in observPointsIds)
            {
                var point = observPointFC.GetFeature(pointId);
                IPoint pointGeom = point.Shape as IPoint;
                pointGeom.Project(VisibilityManager.CurrentMap.SpatialReference);

                var pointModel = observPoints.First(p => p.Objectid == pointId);

                var visibilityPolygon = EsriTools.GetCoverageArea(pointGeom, pointModel.AzimuthStart.Value, pointModel.AzimuthEnd.Value,
                                                                            pointModel.InnerRadius.Value, pointModel.OuterRadius.Value);

                _coverageAreaData.Add(new CoverageAreaData
                {
                    ObjId = -1,
                    PointId = pointId,
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
        }

        public void CalculateCoverageTableDataForPoint(bool isTotal, string visibilityAreasFCName, int pointCount, int currPointId)
        {

            if(_calcType == VisibilityCalcTypeEnum.ObservationObjects)
            {
                CalculateCoverageTableVADataForPoint(isTotal, currPointId, visibilityAreasFCName, pointCount);
            }
            else
            {
                CalculateCoverageTableVSDataForPoint(isTotal, currPointId, visibilityAreasFCName, pointCount);
            }
        }

        public void SaveDataToCoverageTable(string tableName)
        {
            if(_calcType == VisibilityCalcTypeEnum.ObservationObjects)
            {
                GdbAccess.Instance.FillVACoverageTable(_coverageTableModel, tableName, _gdb);
            }
            else
            {
                GdbAccess.Instance.FillVSCoverageTable(_coverageTableModel, tableName, _gdb);
            }
        }

        private void CalculateCoverageTableVSDataForPoint(bool isTotal, int currPointId, string visibilityAreasFCName, int pointCount)
        {
            if(isTotal)
            {
                CalculateVSTotalValues(pointCount, visibilityAreasFCName);
            }

            if(!isTotal || pointCount == 1)
            {
                var observPoint = VisibilityZonesFacade.GetObservationPointByObjectIds(new int[] { currPointId }).First();

                if(_totalExpectedArea == 0)
                {
                    AddEmptyAreaRow(observPoint.Title, currPointId);
                    return;
                }

                var visibilityPolygonsForPointFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);
                var expectedPolygonArea = (IArea)_coverageAreaData.FirstOrDefault(area => area.PointId == currPointId).Polygon;
                var visibleArea = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsForPointFeatureClass);

                AddVSRowModel(observPoint.Title, currPointId, 1, expectedPolygonArea.Area, visibleArea);
            }
        }

        private void CalculateCoverageTableVADataForPoint(bool isTotal, int currPointId, string visibilityAreasFCName, int pointCount)
        {
            if(isTotal)
            {
                CalculateVATotalValues(pointCount, visibilityAreasFCName);
            }
            
            if(!isTotal || pointCount == 1)
            {
                var observPoint = VisibilityZonesFacade.GetObservationPointByObjectIds(new int[] { currPointId }).First();

                var totalObjArea = (IArea)_objPolygons[-1];
                var visibilityPolygonsForPointFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);

                foreach(var polygon in _objPolygons)
                {
                    if(polygon.Key == -1)
                    {
                        continue;
                    }

                    var obj = VisibilityZonesFacade.GetObservationObjectByObjectIds(new int[] { polygon.Key }).First();

                    if(totalObjArea.Area == 0)
                    {
                        AddEmptyAreaRow(observPoint.Title, currPointId, polygon.Key, obj.Title);
                        continue;
                    }

                    var visibilityArea = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, polygon.Value);
                    AddVARowModel(observPoint.Title, currPointId, obj.Title, polygon.Key, 1, visibilityArea);
                }
            }
        }

        private void CalculateVATotalValues(int pointsCount, string visibilityAreasFCName)
        {
            var totalObjArea = (IArea)_objPolygons[-1];
            var visibilityPolygonsForPointFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);

            foreach(var polygon in _objPolygons)
            {
                if(polygon.Key == -1)
                {
                    continue;
                }

                var obj = VisibilityZonesFacade.GetObservationObjectByObjectIds(new int[] { polygon.Key }).First();

                if(totalObjArea.Area == 0)
                {
                    AddEmptyAreaRow(_allTitle, -1, polygon.Key, obj.Title, -1);
                    continue;
                }

                var visibilityArea = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, polygon.Value);
                AddVARowModel(_allTitle, -1, obj.Title, polygon.Key, -1, visibilityArea);
            }

            for(int i = 1; i <= pointsCount; i++)
            {
                var areaByPointsSee = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, _objPolygons[-1], i);
                AddVARowModel(_allTitle, -1, _allTitle, -1, i, areaByPointsSee);
            }
        }

        private void CalculateVSTotalValues(int pointsCount, string visibilityAreasFCName)
        {
            if(_totalExpectedArea == 0)
            {
                AddEmptyAreaRow(_allTitle, -1);
                return;
            }

            var visibilityPolygonsFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);

            _totalVisibleArea = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsFeatureClass);

            AddVSRowModel(_allTitle, -1, -1, _totalExpectedArea, _totalVisibleArea);
            
            for(int i = 1; i <= pointsCount; i++)
            {
                var areaByPointsSee = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsFeatureClass, i);
                AddVSRowModel(_allTitle, -1, i, _totalExpectedArea, areaByPointsSee);
            }
        }

        private void AddEmptyAreaRow(string observPointTitle, int observPointId, int observObjId = -1, string observObjName = null, int pointSee = 0)
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

        private void AddVSRowModel(string pointName, int pointId, int pointsCount, double expectedArea, double visibleArea)
        {
            _coverageTableModel.Add(new CoverageTableRowModel
            {
                ObservPointName = pointName,
                ObservPointId = pointId,
                ExpectedArea = Convert.ToInt32(Math.Round(expectedArea, 0)),
                VisibilityArea = Convert.ToInt32(Math.Round(visibleArea, 0)),
                VisibilityPercent = GetPercent(expectedArea, visibleArea),
                ToAllExpectedAreaPercent = GetPercent(_totalExpectedArea, expectedArea),
                ToAllVisibilityAreaPercent = GetPercent(_totalExpectedArea, visibleArea),
                ObservPointsSeeCount = pointsCount
            });
        }

        private void AddVARowModel(string pointName, int pointId, string objName, int objId, int pointsCount, double visibleArea)
        {
            var objArea = (IArea)_objPolygons[objId];
            _coverageTableModel.Add(new CoverageTableRowModel
            {
                ObservPointName = pointName,
                ObservPointId = pointId,
                ObservObjId = objId,
                ObservObjName = objName,
                ObservObjArea = Convert.ToInt32(Math.Round(objArea.Area, 0)),
                VisibilityArea = Convert.ToInt32(Math.Round(visibleArea, 0)),
                VisibilityPercent = GetPercent(objArea.Area, visibleArea),
                ObservPointsSeeCount = pointsCount
            });
        }

        private double GetPercent(double totalArea, double pointArea)
        {
            return Math.Round(((pointArea * 100) / totalArea), 1);
        }
    }
}
