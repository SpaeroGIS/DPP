using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.Core;
using MilSpace.Tools.Exceptions;
using System.Text.RegularExpressions;
using MilSpace.Visibility.Localization;
using MilSpace.Visibility.DTO;

namespace MilSpace.Visibility.ViewController
{
    public class ObservationPointsController
    {
        IObservationPointsView view;
        private static readonly string _observPointFeature = "MilSp_Visible_ObservPoints";
        private static readonly string _observStationFeature = "MilSp_Visible_ObjectsObservation_R";
        private List<ObservationPoint> _observationPoints = new List<ObservationPoint>();
        private List<ObservationObject> _observationObjects = new List<ObservationObject>();
        private static bool localized = false;
        private string _previousPickedRasterLayer { get; set; }

        /// <summary>
        /// The dictionary to localise the types
        /// </summary>
        private static Dictionary<ObservationPointMobilityTypesEnum, string> _mobilityTypes = null;// = Enum.GetValues(typeof(ObservationPointMobilityTypesEnum)).Cast<ObservationPointMobilityTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<ObservationPointTypesEnum, string> _affiliationTypes = null;//Enum.GetValues(typeof(ObservationPointTypesEnum)).Cast<ObservationPointTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<string, ObservationObjectTypesEnum> _observObjectsTypesToConvert = Enum.GetValues(typeof(ObservationObjectTypesEnum)).Cast<ObservationObjectTypesEnum>().ToDictionary(ts => ts.ToString(), t => t);
        private static Dictionary<ObservationObjectTypesEnum, string> _observObjectsTypes = null; //Enum.GetValues(typeof(ObservationObjectTypesEnum)).Cast<ObservationObjectTypesEnum>().ToDictionary(ts => ts.ToString(), t => t);
        private static Dictionary<LayerPositionsEnum, string> _layerPositions = Enum.GetValues(typeof(LayerPositionsEnum)).Cast<LayerPositionsEnum>().ToDictionary(t => t, ts => ts.ToString());

        private IMxDocument mapDocument;
        private static Logger log = Logger.GetLoggerEx("ObservationPointsController");

        public ObservationPointsController(IMxDocument mapDocument)
        {
            this.mapDocument = mapDocument;
            LocalizeDictionaries();
        }

        private static void LocalizeDictionaries()
        {
            if (!localized)
            {
                _affiliationTypes = LocalizationContext.Instance.AffiliationTypes;
                _observObjectsTypes = LocalizationContext.Instance.ObservObjectsTypes;
                _mobilityTypes = LocalizationContext.Instance.MobilityTypes;
                _layerPositions[LayerPositionsEnum.Above] = LocalizationContext.Instance.PlaceLayerAbove;
                _layerPositions[LayerPositionsEnum.Below] = LocalizationContext.Instance.PlaceLayerBelow;
                localized = true;
            }
        }

        internal void SetView(IObservationPointsView view)
        {
            this.view = view;
        }

        internal void OnCreateFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            throw new NotImplementedException();
        }

        internal string GetObservPointFeatureName()
        {
            return VisibilityManager.ObservPointFeature;
        }

        internal string GetObservObjectFeatureName()
        {
            return VisibilityManager.ObservStationFeature;
        }

        internal void UpdateObservationPointsList()
        {
            _observationPoints = VisibilityZonesFacade.GetAllObservationPoints().ToList();
            view.FillObservationPointList(_observationPoints, view.GetFilter);
        }

        internal ObservationPoint GetObservPointById(int id)
        {
            return _observationPoints.FirstOrDefault(point => point.Objectid == id);
        }


        internal ObservationObject GetObservObjectById(string id)
        {
            return _observationObjects.FirstOrDefault(obj => obj.Id == id);
        }

        internal void UpdateObservPoint(ObservationPoint newPoint, string featureName, IActiveView activeView, int objId)
        {
            var isCoordChanges = false;
            var oldPoint = GetObservPointById(objId);
            var featureClass = GetFeatureClass(featureName, activeView);
            PointClass pointGeometry;

            if (oldPoint.X != newPoint.X && oldPoint.Y != newPoint.Y)
            {
                pointGeometry = new PointClass {
                    X = (double)newPoint.X,
                    Y = (double)newPoint.Y,
                    SpatialReference = EsriTools.Wgs84Spatialreference
                };

                pointGeometry.Z = (double)newPoint.RelativeHeight;
                pointGeometry.ZAware = true;

                isCoordChanges = true;
            }
            else
            {
                pointGeometry = null;
            }
            GdbAccess.Instance.UpdateObservPoint(pointGeometry, featureClass, newPoint, objId);

            _observationPoints = VisibilityZonesFacade.GetAllObservationPoints().ToList();
            view.ChangeRecord(objId, newPoint);

            if (isCoordChanges)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, GetFeatureLayer(featureName, activeView), null);
            }
        }

        internal void RemoveObservPoint(string featureName, IActiveView activeView, int id)
        {
            var featureClass = GetFeatureClass(featureName, activeView);
            GdbAccess.Instance.RemoveObservPoint(featureClass, id);
            _observationPoints.Remove(GetObservPointById(id));
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, GetFeatureLayer(featureName, activeView), null);
        }

        internal void ShowObservPoint(IActiveView activeView, int id)
        {
            var point = GetObservPointById(id);
            var pointGeometry = new PointClass { X = (double)point.X, Y = (double)point.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

            pointGeometry.Project(activeView.FocusMap.SpatialReference);

            if (!IsPointOnExtent(activeView.Extent, pointGeometry))
            {
                EsriTools.PanToGeometry(activeView, pointGeometry, true);
            }

            EsriTools.FlashGeometry(activeView.ScreenDisplay, new IGeometry[] { pointGeometry });
        }

        internal IEnumerable<ObservationPoint> GetAllObservationPoints()
        {
            return VisibilityZonesFacade.GetAllObservationPoints();
        }


        internal ObservationPoint CreatePointWithDefaultValues(IEnvelope envelope)
        {
            var centerPoint = GetEnvelopeCenterPoint(envelope);

            return new ObservationPoint
            {
                X = centerPoint.X,
                Y = centerPoint.Y,
                Type = ObservationPointMobilityTypesEnum.Stationary.ToString(),
                Affiliation = ObservationPointTypesEnum.Enemy.ToString(),
                AzimuthStart = Convert.ToDouble(ObservPointDefaultValues.AzimuthBText),
                AzimuthEnd = Convert.ToDouble(ObservPointDefaultValues.AzimuthEText),
                RelativeHeight = Convert.ToDouble(ObservPointDefaultValues.RelativeHeightText),
                AvailableHeightLover = Convert.ToDouble(ObservPointDefaultValues.HeightMinText),
                AvailableHeightUpper = Convert.ToDouble(ObservPointDefaultValues.HeightMaxText),
                Title = ObservPointDefaultValues.ObservPointNameText,
                AngelMinH = Convert.ToDouble(ObservPointDefaultValues.AngleOFViewMinText),
                AngelMaxH = Convert.ToDouble(ObservPointDefaultValues.AngleOFViewMaxText),
                AngelFrameH = Convert.ToDouble(ObservPointDefaultValues.AngleFrameHText),
                AngelFrameV = Convert.ToDouble(ObservPointDefaultValues.AngleFrameVText),
                AngelCameraRotationH = Convert.ToDouble(ObservPointDefaultValues.CameraRotationHText),
                AngelCameraRotationV = Convert.ToDouble(ObservPointDefaultValues.CameraRotationVText),
                AzimuthMainAxis = Convert.ToDouble(ObservPointDefaultValues.AzimuthMainAxisText),
                Dto = DateTime.Now.Date,
                Operator = Environment.UserName
            };
        }

        internal void AddPoint(string featureName, IActiveView activeView)
        {
            var point = CreatePointWithDefaultValues(activeView.Extent.Envelope);
            var pointGeometry = new PointClass {
                X = (double)point.X,
                Y = (double)point.Y,
                SpatialReference = EsriTools.Wgs84Spatialreference
            };

            pointGeometry.Z = (double)point.RelativeHeight;
            pointGeometry.ZAware = true;

            var featureClass = GetFeatureClass(featureName, activeView);

            GdbAccess.Instance.AddObservPoint(pointGeometry, point, featureClass);

            var updPoints = VisibilityZonesFacade.GetAllObservationPoints().ToList();
            _observationPoints.Add(updPoints.First(observPoint => 
            !_observationPoints.Exists(oldPoints => oldPoints.Objectid == observPoint.Objectid)));
            view.AddRecord(_observationPoints.Last());
        }

        // TODO: Define the field in the View Interface to take sessionName, rasterLayerName and  visibilityCalculationResults
        internal bool CalculateVisibility(WizardResult calcParams)
        {
            var statusBar = ArcMap.Application.StatusBar;
            var animationProgressor = statusBar.ProgressAnimation;

            try
            {
                MapLayersManager layersManager = new MapLayersManager(mapDocument.ActiveView);

                var demLayer = layersManager.RasterLayers.FirstOrDefault(l => l.Name.Equals(calcParams.RasterLayerName));

                if (demLayer == null)
                {
                    throw new MilSpaceVisibilityCalcFailedException($"Cannot find DEM layer {calcParams.RasterLayerName }.");
                }

                calcParams.RasterLayerName = demLayer.FilePath;

                var observPoints = GetObservatioPointFeatureClass(mapDocument.ActiveView);

                var observObjects = GetObservatioStationFeatureClass(mapDocument.ActiveView);

                if (calcParams.ObservPointIDs == null) // Get points forn the current extent
                {
                    calcParams.ObservPointIDs = EsriTools.GetSelectionByExtent(observPoints, mapDocument.ActiveView);
                }
                if (calcParams.ObservObjectIDs == null) // Get points forn the current extent
                {
                    calcParams.ObservObjectIDs = EsriTools.GetSelectionByExtent(observObjects, mapDocument.ActiveView);
                }

                animationProgressor.Show();
                animationProgressor.Play(0, 200);

                var calcTask = VisibilityManager.Generate(
                    observPoints,
                    calcParams.ObservPointIDs,
                    observObjects, 
                    calcParams.ObservObjectIDs, 
                    calcParams.RasterLayerName, 
                    calcParams.VisibilityCalculationResults, 
                    calcParams.TaskName,
                    calcParams.TaskName,
                    calcParams.CalculationType,
                    mapDocument.ActivatedView.FocusMap);

                if(calcTask.Finished != null)
                {
                    var isLayerAbove = (calcParams.ResultLayerPosition == LayerPositionsEnum.Above);

                    var datasets = GdbAccess.Instance.GetDatasetsFromCalcWorkspace(calcTask.ResultsInfo);

                    EsriTools.AddVisibilityGroupLayer(
                        datasets, calcTask.Name, calcTask.Id, calcTask.ReferencedGDB, 
                        calcParams.RelativeLayerName, isLayerAbove, calcParams.ResultLayerTransparency, 
                        mapDocument.ActiveView);
                }
            }
            catch (Exception ex)
            {
                log.ErrorEx(ex.Message);
                return false;
            }
            finally
            {
                animationProgressor.Stop();
                animationProgressor.Hide();
            }
            return true;
        }

        //Returns Observation Points which are visible on the Current View Extent
        internal IEnumerable<ObservationPoint> GetObservPointsOnCurrentMapExtent(IActiveView activeView)
        {
            var observPoints = GetObservatioPointFeatureClass(activeView);
            IEnumerable<ObservationPoint> result = null;
            if (observPoints != null)
            {
                IEnumerable<int> visiblePoints = EsriTools.GetSelectionByExtent(observPoints, activeView);
                if (visiblePoints != null)
                {
                    result = VisibilityZonesFacade.GetObservationPointByObjectIds(visiblePoints);
                }
            }

            return result;
        }

        //Returns Observation Stations\Objects which are visible on the Current View Extent
        internal IEnumerable<ObservationObject> GetObservObjectsOnCurrentMapExtent(IActiveView activeView)
        {
            var observStation = GetObservatioStationFeatureClass(mapDocument.ActiveView);
            IEnumerable<ObservationObject> result = null;
            if (observStation != null)
            {
                IEnumerable<int> visibleStations = EsriTools.GetSelectionByExtent(observStation, activeView);
                if (visibleStations != null)
                {
                    result = VisibilityZonesFacade.GetObservationObjectByObjectIds(visibleStations);
                }
            }

            return result;
        }

        internal IEnumerable<ObservationObject> GetAllObservObjects()
        {
            return VisibilityZonesFacade.GetAllObservationObjects();
        }

        internal bool SaveObservationObject(ObservObjectGui bbservObjectGu)
        {

            var obj = _observationObjects.FirstOrDefault(o => o.Id == bbservObjectGu.Id);
            if (obj == null)
            {
                return false;
            }

            obj.ObjectType = _observObjectsTypes.First(v => v.Value == bbservObjectGu.Affiliation).Key;
            obj.Title = bbservObjectGu.Title;
            obj.Group = bbservObjectGu.Group;

            return VisibilityZonesFacade.SaveObservationObject(obj);
        }


        internal IPoint GetEnvelopeCenterPoint(IEnvelope envelope)
        {
            //TODO: Move this method to Core.Tools.EsriTools
            var x = (envelope.XMin + envelope.XMax) / 2;
            var y = (envelope.YMin + envelope.YMax) / 2;

            var point = new PointClass { X = x, Y = y, SpatialReference = envelope.SpatialReference };
            point.Project(EsriTools.Wgs84Spatialreference);
            return point;
        }

        internal void UpdateObservObjectsList()
        {
            _observationObjects = VisibilityZonesFacade.GetAllObservationObjects().ToList();
            view.FillObservationObjectsList(_observationObjects);
        }

        public void AddObservObjectsLayer()
        {
            VisibilityManager.AddObservationObjectLayer(mapDocument.ActiveView);
        }

        public void AddObservPointsLayer()
        {
            VisibilityManager.AddVisibilityPointLayer(mapDocument.ActiveView);
        }

        public IEnumerable<string> GetObservationPointTypes()
        {
            return _affiliationTypes.Where(t => t.Key != ObservationPointTypesEnum.All).Select(t => t.Value);
        }

        public string GetObservationPointTypeLocalized(ObservationPointTypesEnum type)
        {
            return _affiliationTypes[type];
        }

        public string GetObservationPointMobilityTypeLocalized(ObservationPointMobilityTypesEnum type)
        {
            return _mobilityTypes[type];
        }

        public IEnumerable<string> GetObservationObjectTypes()
        {
            return _observObjectsTypes.Where(t => t.Key != ObservationObjectTypesEnum.Undefined).Select(t => t.Value);
        }
        public string GetAllAffiliationType_for_objects()
        {
            return _observObjectsTypes.First(t => t.Key == ObservationObjectTypesEnum.Undefined).Value;
        }
        public IEnumerable<string> GetObservationPointMobilityTypes()
        {
            return _mobilityTypes.Where(t => t.Key != ObservationPointMobilityTypesEnum.All).Select(t => t.Value);
        }
        public IEnumerable<string> GetLayerPositions()
        {
            return _layerPositions.Select(t => t.Value);
        }

        public string GetAllAffiliationType()
        {
            return _affiliationTypes.First(t => t.Key == ObservationPointTypesEnum.All).Value;
        }

        public string GetAllMobilityType()
        {
            return _mobilityTypes.First(t => t.Key == ObservationPointMobilityTypesEnum.All).Value;
        }

        public string GetDefaultLayerPosition()
        {
            return _layerPositions.First(t => t.Key == LayerPositionsEnum.Above).Value;
        }

        public string GetObservObjectsTypeString(ObservationObjectTypesEnum type)
        {
            return _observObjectsTypes[type];
        }

        public LayerPositionsEnum GetPositionByStringValue(string positionStringValue)
        {
            foreach (var position in _layerPositions)
            {
                if (position.Value == positionStringValue)
                {
                    return position.Key;
                }
            }

            return LayerPositionsEnum.Above;
        }

        public IEnumerable<string> GetObservationPointsLayers(IActiveView view)
        {
            MapLayersManager manager = new MapLayersManager(mapDocument.ActiveView);

            var obserPointsLayersNames = new List<string>();
            manager.PointLayers.ToList().ForEach(layer =>
            {
                if (layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(GetObservPointFeatureName(), StringComparison.InvariantCultureIgnoreCase))
                {
                    obserPointsLayersNames.Add(layer.Name);
                }
            });

            return obserPointsLayersNames;
        }

        public IEnumerable<string> GetAllLayers()
        {
            MapLayersManager manager = new MapLayersManager(mapDocument.ActiveView);

            var allLayersNames = new List<string>();

            allLayersNames.AddRange(manager.FirstLevelLayers.Select(layer =>
            {
                return layer.Name;
            }));

            return allLayersNames;
        }

        public string GetLastLayer()
        {
            var map = mapDocument.ActiveView.FocusMap;
            return map.Layer[map.LayerCount - 1].Name;
        }

        public IEnumerable<string> GetObservationStationsLayers()
        {
            MapLayersManager manager = new MapLayersManager(mapDocument.ActiveView);
            var observstsLayersNames = new List<string>();

            //TODO: Use getting layers from a Helper to obtain all feature classes which can be inside a CompositeLayer also filter by Point type
            manager.PolygonLayers.ToList().ForEach(layer =>
            {
                if (layer is IFeatureLayer fl && fl.FeatureClass.AliasName.EndsWith(GetObservObjectFeatureName(), StringComparison.InvariantCultureIgnoreCase))
                {
                    observstsLayersNames.Add(layer.Name);
                }
            });

            return observstsLayersNames;
        }

        public IFeatureClass GetObservatioPointFeatureClass(IActiveView esriView)
        {
            return GetFeatureClass(view.ObservationPointsFeatureClass, esriView);
        }
        public IFeatureClass GetObservatioStationFeatureClass(IActiveView esriView)
        {
            return GetFeatureClass(_observStationFeature, esriView);
        }

        public bool IsObservPointsExists()
        {
            return IsFeatureLayerExists(mapDocument.ActiveView, _observPointFeature);
        }

        public bool IsObservObjectsExists()
        {
            return IsFeatureLayerExists(mapDocument.ActiveView, _observStationFeature);
        }

        public string GetObservationPointsLayerName => view.ObservationPointsFeatureClass;

        public string GetObservationStationLayerName => _observStationFeature;

        private bool IsFeatureLayerExists(IActiveView view, string featureClass)
        {
            log.DebugEx("> IsFeatureLayerExists START");

            var pattern = @"^[A-Za-z0-9]+\.[A-Za-z0-9]+\." + featureClass + "$";
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            log.DebugEx("IsFeatureLayerExists featureClass:{0} pattern:{1}", featureClass, pattern);

            try
            {
                while (layer != null)
                {
                    if (layer is IFeatureLayer fl
                        && fl.FeatureClass != null
                        && (fl.FeatureClass.AliasName.Equals(featureClass, StringComparison.InvariantCultureIgnoreCase) || Regex.IsMatch(fl.FeatureClass.AliasName, pattern)))
                    {
                        log.DebugEx("> IsFeatureLayerExists END, FeatureLayerExists");
                        return true;
                    }

                    layer = layers.Next();
                }
                log.DebugEx("> IsFeatureLayerExists END, NOT FeatureLayerExists");
                return false;
            }
            catch (Exception ex)
            {
                log.DebugEx("> IsFeatureLayerExists Exception. EX:{0}", ex.Message);
                return false;
            }
        }

        private IEnumerable<VisibilityTask> GetAllVisibilitySessions()
        {
            return VisibilityZonesFacade.GetAllVisibilityTasks();
        }

        private IFeatureClass GetFeatureClass(string featureClassName, IActiveView activeView)
        {
            if (string.IsNullOrWhiteSpace(featureClassName))
            {
                return null;
            }

            var layers = activeView.FocusMap.Layers;
            var layer = layers.Next();

            while (layer != null)
            {
                if (layer is IFeatureLayer fl && fl.FeatureClass != null && fl.FeatureClass.AliasName.EndsWith(featureClassName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fl.FeatureClass;
                }

                layer = layers.Next();
            }

            return null;
        }

        private IFeatureLayer GetFeatureLayer(string featureClassName, IActiveView activeView)
        {
            if (string.IsNullOrWhiteSpace(featureClassName))
            {
                return null;
            }

            var layers = activeView.FocusMap.Layers;
            var layer = layers.Next();

            while (layer != null)
            {
                if (layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(featureClassName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fl;
                }

                layer = layers.Next();
            }

            return null;
        }

        private bool IsPointOnExtent(IEnvelope envelope, IPoint point)
        {
            if (point.X >= envelope.XMin && point.X <= envelope.XMax && point.Y >= envelope.YMin && point.Y <= envelope.YMax)
            {
                return true;
            }

            return false;
        }
        public string GetPreviousPickedRasterLayer() => _previousPickedRasterLayer;

        public void UpdataPreviousPickedRasterLayer(string raster)
        {
            _previousPickedRasterLayer = raster;
        }

        //private void TestPolygonFinding(ObservationPoint point, ObservationObject obj, IFeatureClass pointsFC, IFeatureClass objFC)
        //{
        //    try
        //    {
        //        var feature = pointsFC.GetFeature(point.Objectid);
        //        IPoint pointGeom = feature.Shape as IPoint;

        //        pointGeom.Project(mapDocument.ActivatedView.FocusMap.SpatialReference);

        //        var featureObj = objFC.GetFeature(obj.ObjectId);
        //        IPolygon objGeom = featureObj.Shape as IPolygon;

        //        var area = EsriTools.GetCoverageArea(pointGeom, point.AzimuthStart.Value, point.AzimuthEnd.Value, point.InnerRadius.Value, point.OuterRadius.Value);
        //        var area1 = EsriTools.GetCoverageArea(pointGeom, point.AzimuthStart.Value, point.AzimuthEnd.Value, point.InnerRadius.Value, point.OuterRadius.Value, objGeom);

        //        IFeatureLayer test1Layer = new FeatureLayerClass();
        //        test1Layer.Name = $"WithObj{DateTime.Now.ToShortTimeString()}";
        //        test1Layer.FeatureClass = GdbAccess.Instance.GetTestFeature(area, $"WithObj");
        //        mapDocument.AddLayer(test1Layer);

        //        IFeatureLayer test2Layer = new FeatureLayerClass();
        //        test2Layer.Name = $"WithoutObj{DateTime.Now.ToShortTimeString()}";
        //        test2Layer.FeatureClass = GdbAccess.Instance.GetTestFeature(area1, $"WithoutObj");
        //        mapDocument.AddLayer(test2Layer);
        //    }
        //    catch(Exception Ex)
        //    {

        //    }
        //}
    }
}
