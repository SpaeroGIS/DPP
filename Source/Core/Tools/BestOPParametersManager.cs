using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.Tools
{
    public class BestOPParametersManager
    {
        private int _requiredVisibilityPercent;
        private int _lastInappropriatePointId = 0;
        private int _lastAppropriateId = 0;
        private readonly int _maxId;
        private Dictionary<int, short> _visibilityPercents = new Dictionary<int, short>();

        private const string _temporaryObservationStationFeatureClassSuffix = "_VO_ObservStation";

        public BestOPParametersManager(int requiredVisibilityPercent, int maxId)
        {
            _requiredVisibilityPercent = requiredVisibilityPercent;
            _maxId = maxId;
        }

        public static IFeatureClass CreateOPFeatureClass(WizardResult calcResult, IFeatureClass observatioPointsFeatureClass,
                                                            IActiveView activeView, IRaster raster)
        {
            var calcObservPointsFeatureClass = VisibilityCalcResults.GetResultName(VisibilityCalculationResultsEnum.ObservationPoints, calcResult.TaskName);

            var observPointTemporaryFeatureClass =
                    GdbAccess.Instance.GenerateTemporaryFeatureClassWithRequitedFields(observatioPointsFeatureClass.Fields, calcObservPointsFeatureClass);

            bool isCircle;
            double maxAzimuth = 0;
            double minAzimuth = 360;
            double maxDistance = 0;

            calcResult.ObservationStation.Project(activeView.FocusMap.SpatialReference);

            var observationStationPolygon = calcResult.ObservationStation as IPolygon;
            var observStationEnvelope = observationStationPolygon.Envelope;
            var observStationEnvelopePoints = new IPoint[] { observStationEnvelope.LowerLeft, observStationEnvelope.LowerRight,
                                                                observStationEnvelope.UpperRight, observStationEnvelope.UpperLeft};

            var observerPointGeometry = new PointClass
            {
                X = calcResult.ObservationPoint.X.Value,
                Y = calcResult.ObservationPoint.Y.Value,
                SpatialReference = EsriTools.Wgs84Spatialreference
            };

            observerPointGeometry.Project(activeView.FocusMap.SpatialReference);

            observerPointGeometry.AddZCoordinate(raster);
            observerPointGeometry.ZAware = true;

            if (double.IsNaN(observerPointGeometry.Z))
            {
                throw new MilSpacePointOutOfRatserException(calcResult.ObservationPoint.Objectid, calcResult.RasterLayerName);
            }

            var minDistance = FindMinDistance(observStationEnvelopePoints, observerPointGeometry);


            // ---- Get min and max azimuths ----

            var points = EsriTools.GetPointsFromGeometries(new IGeometry[] { calcResult.ObservationStation },
                                                            observerPointGeometry.SpatialReference,
                                                            out isCircle).ToArray();

            bool isPointInside = EsriTools.IsPointOnExtent(observStationEnvelope, observerPointGeometry);

            // Set azimuth for circle polygon
            if (isCircle && !isPointInside)
            {
                for (int i = 0; i < observStationEnvelopePoints.Length; i++)
                {
                    var line = new Line()
                    {
                        FromPoint = observerPointGeometry,
                        ToPoint = observStationEnvelopePoints[i],
                        SpatialReference = observerPointGeometry.SpatialReference
                    };

                    if (i == 0)
                    {
                        maxDistance = line.Length;
                    }
                    else if (maxDistance < line.Length)
                    {
                        maxDistance = line.Length;
                    }

                    if (minAzimuth > line.PosAzimuth())
                    {
                        minAzimuth = line.PosAzimuth();
                    }

                    if (maxAzimuth < line.PosAzimuth())
                    {
                        maxAzimuth = line.PosAzimuth();
                    }
                }
            }
            else
            {
                EsriTools.CreateDefaultPolylinesForFun(observerPointGeometry, points, new IGeometry[] { calcResult.ObservationStation },
                                                                                       isCircle, isPointInside, -1, out minAzimuth, out maxAzimuth, out maxDistance).ToList();
            }
            // ---- End. Get min and max azimuths ----


            for (var currentHeight = calcResult.FromHeight; currentHeight <= calcResult.ToHeight; currentHeight += calcResult.Step)
            {
                double maxTiltAngle = 0;
                double minTiltAngle = 0;

                var height = currentHeight;

                // Set parameters for case if point located into the polygon
                if (isCircle && isPointInside)
                {
                    minTiltAngle = -90;
                    minDistance = 0;
                }
                else
                {
                    // Find angle to the nearest point of observation station
                    minTiltAngle = EsriTools.FindAngleByDistanceAndHeight(height, minDistance);
                }

                // Find angle to the farthest point of observation station
                maxTiltAngle = EsriTools.FindAngleByDistanceAndHeight(height, maxDistance);

                // Create observation point copy with changing height, distance and angles values
                var currentObservationPoint = new ObservationPoint();
                currentObservationPoint = calcResult.ObservationPoint.ShallowCopy();
                currentObservationPoint.RelativeHeight = currentHeight;
                currentObservationPoint.AngelMinH = minTiltAngle;
                currentObservationPoint.AngelMaxH = maxTiltAngle;
                currentObservationPoint.AzimuthStart = minAzimuth;
                currentObservationPoint.AzimuthEnd = maxAzimuth;
                currentObservationPoint.InnerRadius = minDistance;
                currentObservationPoint.OuterRadius = maxDistance;
                currentObservationPoint.AzimuthMainAxis = calcResult.ObservationPoint.AzimuthMainAxis;

                GdbAccess.Instance.AddObservPoint(observerPointGeometry, currentObservationPoint, observPointTemporaryFeatureClass);
            }

            return observPointTemporaryFeatureClass;
        }

        public static IFeatureClass CreateOOFeatureClass(IGeometry geometry, IActiveView activeView, string taskId)
        {
            var featureClass = GdbAccess.Instance.GenerateTempStorage($"{taskId}{_temporaryObservationStationFeatureClassSuffix}", null,
                                                            esriGeometryType.esriGeometryPolygon, activeView, false);

            GdbAccess.Instance.AddGeometryToFeatureClass(geometry, featureClass);

            return featureClass;
        }

        public static void ClearTemporaryData(string taskId, string gdb = null)
        {
            if (String.IsNullOrEmpty(gdb))
            {
                gdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;
            }

            EsriTools.RemoveDataSet(gdb, $"{taskId}{_temporaryObservationStationFeatureClassSuffix}");
        }

        public int FindVisibilityPercent(string visibilityArePolyFCName,
                                               IFeatureClass observStationFeatureClass, int[] observStationsIds,
                                               int pointId)
        {
            IPolygon observationStationPolygon = null;

            if (observStationFeatureClass == null)
            {
                throw new MilSpaceVisibilityCalcFailedException($"Observation station feature class doesn`t exists");
            }

            try
            {
                observationStationPolygon = observStationFeatureClass.GetFeature(1).ShapeCopy as IPolygon;
            }
            catch (Exception ex)
            {
                throw new MilSpaceVisibilityCalcFailedException($"Cannot get observation station polygon from feature class. Exception: {ex.Message}");
            }

            var visibilityPolygonsForPointFeatureClass =
                    GdbAccess.Instance.GetFeatureClass(MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection,
                                                        visibilityArePolyFCName);

            double visibilityPercent;

            if (visibilityPolygonsForPointFeatureClass == null)
            {
                visibilityPercent = 0;
            }
            else
            {
                // Get visibility area of observation station
                var visibilityArea = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, VisibilityManager.CurrentMap, observationStationPolygon);
                var observationStationPolygonArea = observationStationPolygon as IArea;

                visibilityPercent = Math.Round(((visibilityArea * 100) / observationStationPolygonArea.Area), 0);
            }

            _visibilityPercents.Add(pointId, Convert.ToInt16(visibilityPercent));

            return FindNextId(visibilityPercent, pointId);
        }

        private int FindNextId(double visibilityPercent, int pointId)
        {
            if (visibilityPercent < _requiredVisibilityPercent)
            {
                _lastInappropriatePointId = pointId;

                if (pointId == _maxId)
                {
                    return -1;
                }
                else
                {
                    if (pointId == _lastAppropriateId - 1)
                    {
                        return -1;
                    }
                    else
                    {
                        decimal nextId = pointId + (_lastAppropriateId - pointId) / 2;

                        if (_visibilityPercents.Any(percent => percent.Key == nextId))
                        {
                            return FindNextPointIdIfItRepeated(Convert.ToInt32(Math.Round(nextId)));
                        }

                        return Convert.ToInt32(Math.Round(nextId));
                    }
                }
            }
            else
            {
                if (visibilityPercent == _requiredVisibilityPercent || pointId == _lastInappropriatePointId + 1)
                {
                    return -1;
                }

                _lastAppropriateId = pointId;

                if (pointId == _maxId)
                {
                    return 1;
                }
                else
                {
                    _lastAppropriateId = pointId;

                    decimal nextId = pointId - (pointId - _lastInappropriatePointId) / 2;

                    if (_visibilityPercents.Any(percent => percent.Key == nextId))
                    {
                        return FindNextPointIdIfItRepeated(Convert.ToInt32(Math.Round(nextId)));
                    }

                    return Convert.ToInt32(Math.Round(nextId));
                }
            }
        }

        private int FindNextPointIdIfItRepeated(int repeatedId)
        {
            if (repeatedId > _lastInappropriatePointId + 1)
            {
                int nextId = repeatedId -= 1;

                while (_visibilityPercents.Any(percent => percent.Key == nextId))
                {
                    nextId -= 1;

                    if (nextId <= _lastInappropriatePointId)
                    {
                        nextId = -1;
                    }
                }

                return nextId;
            }
            else
            {
                return -1;
            }
        }

        public static List<int> GetAllIdsFromFeatureClass(IFeatureClass featureClass)
        {
            var ids = new List<int>();

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = $"{featureClass.OIDFieldName} >= 0";

            var idFieldIndex = featureClass.FindField(featureClass.OIDFieldName);

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();

            try
            {
                while (feature != null)
                {
                    ids.Add((int)feature.Value[idFieldIndex]);
                    feature = featureCursor.NextFeature();
                }

                return ids;
            }
            catch
            {
                return null;
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }
        }

        public bool CreateVOTable(IFeatureClass observPointFeatureClass, int expectedVisibilityPercent,
                                    string bestParamsTableName, bool showAllResults)
        {

            if (_visibilityPercents.Count == 0 || !_visibilityPercents.Any(percent => percent.Value > 0))
            {
                CreateNullVisibilityVOTable(observPointFeatureClass, expectedVisibilityPercent, bestParamsTableName);
                return false;
            }

            var appropriateParams = new Dictionary<int, short>();

            bool isParametersFound = false;
            KeyValuePair<int, short> bestParams = new KeyValuePair<int, short>(-1, 0);

            var bestParamsTable = GenerateVOTable(observPointFeatureClass, bestParamsTableName);
            List<int> keysOrderedByPercents;

            if (showAllResults)
            {
                keysOrderedByPercents = _visibilityPercents.Select(percent => percent.Key).ToList();
            }
            else
            {
                keysOrderedByPercents = _visibilityPercents.OrderByDescending(percent => percent.Value)
                                                         .Select(percent => percent.Key).ToList();
            }


            var acceptableResult = _visibilityPercents.ToDictionary(k => k.Key, v => Math.Abs(expectedVisibilityPercent - v.Value)).Where(k => k.Value >= 0).Min(k => k.Key);

            var nearestParamsId = _maxId;
            var minDelta = 100;

            foreach (var paramsVisibilityId in keysOrderedByPercents)
            {
                var currentPercent = _visibilityPercents[paramsVisibilityId];

                if (showAllResults)
                {
                    appropriateParams.Add(paramsVisibilityId, currentPercent);

                    if (currentPercent == expectedVisibilityPercent)
                    {
                        isParametersFound = true;
                    }
                }
                else
                {
                    if (currentPercent == expectedVisibilityPercent)
                    {
                        appropriateParams.Add(paramsVisibilityId, currentPercent);
                        isParametersFound = true;
                        break;
                    }
                    else
                    {
                        var delta = Math.Abs(expectedVisibilityPercent - currentPercent);
                        if (delta < minDelta)
                        {
                            minDelta = delta;
                            nearestParamsId = paramsVisibilityId;
                        }
                    }
                }
            }

            var bestParamsFeatures = new Dictionary<IFeature, short>();

            if (isParametersFound || showAllResults)
            {
                foreach (var parameters in appropriateParams)
                {
                    bestParamsFeatures.Add(observPointFeatureClass.GetFeature(parameters.Key), parameters.Value);
                }
            }
            else
            {
                bestParamsFeatures.Add(observPointFeatureClass.GetFeature(nearestParamsId), _visibilityPercents[nearestParamsId]);
            }

            GdbAccess.Instance.FillBestParametersTable(bestParamsFeatures, bestParamsTable, bestParamsTableName);

            return isParametersFound;
        }

        public void CreateNullVisibilityVOTable(IFeatureClass observPointFeatureClass, int expectedVisibilityPercent,
                                    string bestParamsTableName)
        {
            var bestParamsTable = GenerateVOTable(observPointFeatureClass, bestParamsTableName);

            var bestParamsFeatures = new Dictionary<IFeature, short>
            {
                { observPointFeatureClass.GetFeature(_maxId), 0}
            };

            GdbAccess.Instance.FillBestParametersTable(bestParamsFeatures, bestParamsTable, bestParamsTableName);
        }

        private static ITable GenerateVOTable(IFeatureClass observPointFeatureClass, string bestParamsTableName)
        {
            // Clone obsever points feature class fields
            var fieldsClone = observPointFeatureClass.Fields as IClone;
            var fields = fieldsClone.Clone() as IFields;

            // Remove shape field
            var shapeFieldIndex = fields.FindField(observPointFeatureClass.ShapeFieldName);
            var fieldsEdit = (IFieldsEdit)fields;
            fieldsEdit.DeleteField(fields.Field[shapeFieldIndex]);

            // Generate best parameters table with obsever points feature class fields and visibility percent field
            return GdbAccess.Instance.GenerateVOTable(fields, bestParamsTableName);
        }

        private static double FindMinDistance(IPoint[] pointsCollection, IPoint centerPoint)
        {
            double minDistance = 0;

            for (int i = 0; i < pointsCollection.Length; i++)
            {
                var line = new Line()
                {
                    FromPoint = centerPoint,
                    ToPoint = pointsCollection[i],
                    SpatialReference = centerPoint.SpatialReference
                };

                if (i == 0)
                {
                    minDistance = line.Length;
                }
                else
                {
                    if (minDistance > line.Length)
                    {
                        minDistance = line.Length;
                    }
                }

                // ---- Find distance between this and next points ----

                double xMiddle;
                double yMiddle;

                if (i != pointsCollection.Length - 1)
                {
                    xMiddle = (pointsCollection[i].X
                       + pointsCollection[i + 1].X) / 2;
                    yMiddle = (pointsCollection[i].Y
                       + pointsCollection[i + 1].Y) / 2;
                }
                else
                {
                    xMiddle = (pointsCollection[i].X
                       + pointsCollection[0].X) / 2;
                    yMiddle = (pointsCollection[i].Y
                       + pointsCollection[0].Y) / 2;
                }

                var middlePoint = new PointClass
                {
                    X = xMiddle,
                    Y = yMiddle,
                    SpatialReference = centerPoint.SpatialReference
                };

                var toMiddleLine = new Line()
                {
                    FromPoint = centerPoint,
                    ToPoint = middlePoint,
                    SpatialReference = middlePoint.SpatialReference
                };

                // ---- End. Find distance between this and next points ----

                if (minDistance > toMiddleLine.Length)
                {
                    minDistance = toMiddleLine.Length;
                }
            }

            return minDistance;
        }
    }
}
