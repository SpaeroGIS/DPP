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
        private List<CoverageAreaData> _coverageAreaByObjData = new List<CoverageAreaData>();
        private List<CoverageTableRowModel> _coverageTableModel = new List<CoverageTableRowModel>();
        private double _totalExpectedArea;
        private double _totalVisibleArea;

        private string _gdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;

        public  void CalculateAreas(int[] observPointsIds, int[] observObjectsIds, IFeatureClass observPointFC, IFeatureClass observObjFC = null)
        {
            SetCoverageAreas(observPointsIds, observObjectsIds, observPointFC, observObjFC);
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

        private  void SetCoverageAreas(int[] observPointsIds, int[] observObjectsIds, IFeatureClass observPointFC, IFeatureClass observObjFC)
        {
            var observPoints = VisibilityZonesFacade.GetObservationPointByObjectIds(observPointsIds);

            foreach(var pointId in observPointsIds)
            {
                var point = observPointFC.GetFeature(pointId);
                IPoint pointGeom = point.Shape as IPoint;
                pointGeom.Project(VisibilityManager.CurrentMap.SpatialReference);

                var pointModel = observPoints.First(p => p.Objectid == pointId);

                if(observObjectsIds != null && observObjectsIds.Count() > 0 && observObjFC != null)
                {
                    foreach(var objId in observObjectsIds)
                    {
                        var obj = observObjFC.GetFeature(objId);
                        IPolygon objGeom = obj.Shape as IPolygon;

                        var visibilityObjPolygon = EsriTools.GetCoverageArea(pointGeom, pointModel.AzimuthStart.Value, pointModel.AzimuthEnd.Value,
                                                                                pointModel.InnerRadius.Value, pointModel.OuterRadius.Value, objGeom);

                        _coverageAreaByObjData.Add(new CoverageAreaData
                        {
                            ObjId = objId,
                            PointId = pointId,
                            Polygon = visibilityObjPolygon
                        });
                    }
                }

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

        public void CalculateCoverageTableDataForPoint(int currPointId, string visibilityAreasFCName, int pointCount)
        {
            if(currPointId == -1)
            {
                CalculateTotalValues(pointCount, visibilityAreasFCName);
            }
            else
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

                AddRowModel(observPoint.Title, currPointId, 1, expectedPolygonArea.Area, visibleArea);
            }
        }

        public void SaveDataToCoverageTable(string tableName)
        {
            GdbAccess.Instance.FillVSCoverageTable(_coverageTableModel, tableName, _gdb);
        }

        private void CalculateCoverageTableVADataForPoint(int currPointId, string visibilityAreasFCName, int pointCount)
        {
            if(currPointId == -1)
            {
                //todo totalVAValues
                CalculateTotalValues(pointCount, visibilityAreasFCName);
            }
            else
            {
                var observPoint = VisibilityZonesFacade.GetObservationPointByObjectIds(new int[] { currPointId }).First();
                var pointPolygons = _coverageAreaByObjData.Where(area => area.PointId == currPointId).ToList();

                foreach(var polygon in pointPolygons)
                {
                    var obj = VisibilityZonesFacade.GetObservationObjectByObjectIds(new int[] { polygon.ObjId }).First();

                    if(_totalExpectedArea == 0)
                    {
                        AddEmptyAreaRow(observPoint.Title, currPointId, polygon.ObjId, obj.Title);
                        return;
                    }
                }
            }
        }

        private void CalculateTotalValues(int pointsCount, string visibilityAreasFCName)
        {
            string allTitle = "All";

            if(_totalExpectedArea == 0)
            {
                AddEmptyAreaRow(allTitle, -1);
                return;
            }

            var visibilityPolygonsFeatureClass = GdbAccess.Instance.GetFeatureClass(_gdb, visibilityAreasFCName);

            _totalVisibleArea = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsFeatureClass);

            AddRowModel(allTitle, -1, -1, _totalExpectedArea, _totalVisibleArea);
            
            for(int i = 1; i <= pointsCount; i++)
            {
                var areaByPointsSee = EsriTools.GetTotalAreaFromFeatureClass(visibilityPolygonsFeatureClass, i);

                AddRowModel(allTitle, -1, i, _totalExpectedArea, areaByPointsSee);
            }
        }

        private void AddEmptyAreaRow(string observPointTitle, int observPointId, int observObjId = -1, string observObjName = null)
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
                ObservPointsSeeCount = 0
            });
        }

        private void AddRowModel(string pointName, int pointId, int pointsCount, double expectedArea, double visibleArea)
        {
            _coverageTableModel.Add(new CoverageTableRowModel
            {
                ObservPointName = pointName,
                ObservPointId = pointId,
                ExpectedArea = expectedArea,
                VisibilityArea = visibleArea,
                VisibilityPercent = GetPercent(expectedArea, visibleArea),
                ToAllExpectedAreaPercent = GetPercent(_totalExpectedArea, expectedArea),
                ToAllVisibilityAreaPercent = GetPercent(_totalExpectedArea, visibleArea),
                ObservPointsSeeCount = pointsCount
            });
        }

        private int GetPercent(double totalArea, double pointArea)
        {
            return Convert.ToInt32(Math.Round((pointArea * 100) / totalArea));
        }

    }
}
