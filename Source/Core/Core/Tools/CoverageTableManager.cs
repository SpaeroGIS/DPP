using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
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
        private  List<CoverageAreaData> _coverageAreaData = new List<CoverageAreaData>(); 

        public  void CalculateAreas(int[] observPointsIds, int[] observObjectsIds, IFeatureClass observPointFC, IFeatureClass observObjFC = null)
        {
            SetCoverageAreas(observPointsIds, observObjectsIds, observPointFC, observObjFC);
        }

        public  void AddPotentialArea(string featureClassName, bool isCommon, int currPointId = 0)
        {
            if(isCommon)
            {
                currPointId = -1;
            }

            var pointAreas = _coverageAreaData.Where(area => area.PointId == currPointId);

            if(pointAreas != null)
            {
                foreach(var area in pointAreas)
                {
                    GdbAccess.Instance.AddCoverageAreaFeature(area.Polygon, area.PointId, area.ObjId, featureClassName);
                }
            }

        }

        private  void SetCoverageAreas(int[] observPointsIds, int[] observObjectsIds, IFeatureClass observPointFC, IFeatureClass observObjFC)
        {
            var observPoints = VisibilityZonesFacade.GetAllObservationPoints();

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

                        var visibilityPolygon = EsriTools.GetCoverageArea(pointGeom, pointModel.AzimuthStart.Value, pointModel.AzimuthEnd.Value,
                                                                                pointModel.InnerRadius.Value, pointModel.OuterRadius.Value, objGeom);

                        if(!visibilityPolygon.IsEmpty)
                        {
                            _coverageAreaData.Add(new CoverageAreaData
                            {
                                ObjId = objId,
                                PointId = pointId,
                                Polygon = visibilityPolygon
                            });
                        }
                    }
                }
                else
                {
                    var visibilityPolygon = EsriTools.GetCoverageArea(pointGeom, pointModel.AzimuthStart.Value, pointModel.AzimuthEnd.Value,
                                                                                pointModel.InnerRadius.Value, pointModel.OuterRadius.Value);

                    if(!visibilityPolygon.IsEmpty)
                    {
                        _coverageAreaData.Add(new CoverageAreaData
                        {
                            ObjId = -1,
                            PointId = pointId,
                            Polygon = visibilityPolygon
                        });
                    }
                }
            }
            
            var commonArea = EsriTools.GetCommonPolygon(_coverageAreaData.Select(area => { return area.Polygon; }).ToList());
            _coverageAreaData.Add(new CoverageAreaData
            {
                ObjId = -1,
                PointId = -1,
                Polygon = commonArea
            });
        }
    }
}
