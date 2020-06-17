using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.Tools
{
    public class BestOPParametersManager
    {
        private Dictionary<int, short> _visibilityPercents = new Dictionary<int, short>();
        private const string _temporaryObserverPointParamsFeatureClassSuffix = "_VO_ObservPointParams";
        private const string _temporaryObservationStationFeatureClassSuffix = "_VO_ObservStation";

        public static IFeatureClass CreateOPFeatureClass(WizardResult calcResult, IFeatureClass observatioPointsFeatureClass,
                                                            IActiveView activeView, IRaster raster)
        {
            var observPointTemporaryFeatureClass = 
                    GdbAccess.Instance.GenerateTemporaryFeatureClassWithRequitedFields(observatioPointsFeatureClass.Fields,
                                                                                        $"{calcResult.TaskName}{_temporaryObserverPointParamsFeatureClassSuffix}");

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

            var minDistance = FindMinDistance(observStationEnvelopePoints, observerPointGeometry);


            // ---- Get min and max azimuths ----
            
            var points = EsriTools.GetPointsFromGeometries(new IGeometry[] { calcResult.ObservationStation },
                                                            observerPointGeometry.SpatialReference,
                                                            out isCircle).ToArray();

            bool isPointInside = EsriTools.IsPointOnExtent(observStationEnvelope,  observerPointGeometry);

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

                    if(i == 0)
                    {
                        maxDistance = line.Length;
                    }
                    else if(maxDistance < line.Length)
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
            EsriTools.RemoveDataSet(gdb, $"{taskId}{_temporaryObserverPointParamsFeatureClassSuffix}");
        }
        
        public void FindVisibilityPercent(string visibilityArePolyFCName,
                                               IFeatureClass observStationFeatureClass, int[] observStationsIds,
                                               int pointId)
        {
            var observationStationPolygon = observStationFeatureClass.GetFeature(observStationsIds.First()).ShapeCopy as IPolygon;
            var visibilityPolygonsForPointFeatureClass = 
                    GdbAccess.Instance.GetFeatureClass(MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection,
                                                        visibilityArePolyFCName);

            if(visibilityPolygonsForPointFeatureClass == null)
            {
                _visibilityPercents.Add(pointId, 0);
                return;
            }
            
            // Get visibility area of observation station
            var visibilityArea = EsriTools.GetObjVisibilityArea(visibilityPolygonsForPointFeatureClass, observationStationPolygon);
            var observationStationPolygonArea = observationStationPolygon as IArea;

            var visibilityPercent = Math.Round(((visibilityArea * 100) / observationStationPolygonArea.Area), 0);
            
            _visibilityPercents.Add(pointId, Convert.ToInt16(visibilityPercent));
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
                                    string bestParamsTableName)
        {
            var appropriateParams = new Dictionary<int, short>();

            bool isParametersFound = false;
            KeyValuePair<int, short> bestParams = new KeyValuePair<int, short>(-1, 0);

            // Clone obsever points feature class fields
            var fieldsClone = observPointFeatureClass.Fields as IClone;
            var fields = fieldsClone.Clone() as IFields;

            // Remove shape field
            var shapeFieldIndex = fields.FindField(observPointFeatureClass.ShapeFieldName);
            var fieldsEdit = (IFieldsEdit)fields;
            fieldsEdit.DeleteField(fields.Field[shapeFieldIndex]);

            // Generate best parameters table with obsever points feature class fields and visibility percent field
            var bestParamsTable = GdbAccess.Instance.GenerateVOTable(fields, bestParamsTableName);

            foreach (var paramsVisibilityPercent in _visibilityPercents)
            {
                if (paramsVisibilityPercent.Value >= expectedVisibilityPercent)
                {
                    appropriateParams.Add(paramsVisibilityPercent.Key, paramsVisibilityPercent.Value);
                    isParametersFound = true;
                }
                else
                {
                    if (bestParams.Value < paramsVisibilityPercent.Value)
                    {
                        bestParams = paramsVisibilityPercent;
                    }
                }
            }

            var bestParamsFeatures = new Dictionary<IFeature, short>();

            if (isParametersFound)
            {
                foreach (var parameters in appropriateParams)
                {
                    bestParamsFeatures.Add(observPointFeatureClass.GetFeature(parameters.Key), parameters.Value);
                }
            }
            else
            {
                if (bestParams.Key > -1)
                {
                    bestParamsFeatures.Add(observPointFeatureClass.GetFeature(bestParams.Key), bestParams.Value);
                }
            }

            GdbAccess.Instance.FillBestParametersTable(bestParamsFeatures, bestParamsTable, bestParamsTableName);

            return isParametersFound;
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
