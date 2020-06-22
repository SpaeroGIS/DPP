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
using ESRI.ArcGIS.Editor;
using MilSpace.Core.DataAccess;
using System.Runtime.InteropServices;
using MilSpace.Tools.GraphicsLayer;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Core.ModalWindows;
using ESRI.ArcGIS.esriSystem;
using MilSpace.Configurations;

namespace MilSpace.Visibility.ViewController
{
    public class ObservationPointsController
    {
        IObservationPointsView view;
        private static readonly string _observPointFeature = "MilSp_Visible_ObservPoints";
        private static readonly string _observStationFeature = "MilSp_Visible_ObjectsObservation_R";
        private KeyValuePair<string, string> _observPointFromFeatureLayerData;
        private List<IObserverPoint> _observationPoints = new List<IObserverPoint>();
        private List<ObservationObject> _observationObjects = new List<ObservationObject>();
        private List<ObservationStationToObservPointRelationModel> _relationLines = new List<ObservationStationToObservPointRelationModel>();
        private static bool localized = false;
        private IPolygon _coverageArea;
        private string _observaitonObjectsLayerName;
        private string _observaitonPointsLayerName;
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
        private IMxApplication application;
        private static Logger log = Logger.GetLoggerEx("MilSpace.Visibility.ViewController.ObservationPointsController");
        private static GraphicsLayerManager _graphicsLayerManager;

        public ObservationPointsController(IMxDocument mapDocument, IMxApplication application)
        {
            this.mapDocument = mapDocument;
            this.application = application;
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

        internal string ObserverPointsLayerName => _observaitonPointsLayerName;
        internal string ObservationObjectsLayerName => _observaitonObjectsLayerName;

        internal void SetView(IObservationPointsView view)
        {
            this.view = view;
        }

        internal void SetGrahicsLayerManager()
        {
            _graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView);
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
            _observationPoints = VisibilityZonesFacade.GetAllObservationPoints()
                                                      .Select(point => point as IObserverPoint)
                                                      .ToList();

            view.FillObservationPointList(_observationPoints, view.GetFilter);
        }

        internal ObservationPoint GetObservPointByIdAsObservationPoint(int id)
        {
            return GetObservationPointFromInterface(
                     _observationPoints.FirstOrDefault(point => point.Objectid == id));
        }

        internal IObserverPoint GetObservPointById(int id)
        {
            return _observationPoints.FirstOrDefault(point => point.Objectid == id);
        }

        internal ObservationObject GetObservObjectById(string id)
        {
            return _observationObjects.FirstOrDefault(obj => obj.Id == id);
        }
        internal ObservationObject GetObservObjectByOId(string oid)
        {
            return _observationObjects.FirstOrDefault(obj => obj.ObjectId.ToString() == oid);
        }

        internal IPoint GetIPointObservPoint(int objId)
        {
            try
            {
                IFeatureClass observPointFC = GetObservatioPointFeatureClass(mapDocument.ActiveView);
                if (observPointFC != null)
                {
                    var point = observPointFC.GetFeature(objId);
                    if (point != null)
                    {
                        IPoint pointGeom = point.Shape as IPoint;
                        return pointGeom;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        internal void UpdateObservPoint(IObserverPoint newPoint, string featureName,
                                        IActiveView activeView, int objId, bool isCoordsChanged)
        {
            var oldPoint = GetObservPointByIdAsObservationPoint(objId);

            var featureClass = GetFeatureClass(featureName, activeView);
            PointClass pointGeometry;

            if (isCoordsChanged)
            {
                pointGeometry = new PointClass
                {
                    X = (double)newPoint.X,
                    Y = (double)newPoint.Y,
                    SpatialReference = EsriTools.Wgs84Spatialreference
                };

                pointGeometry.Z = (double)newPoint.RelativeHeight;
                pointGeometry.ZAware = true;
            }
            else
            {
                pointGeometry = null;
            }
            GdbAccess.Instance.UpdateObservPoint(pointGeometry, featureClass, GetObservationPointFromInterface(newPoint), objId);

            if (isCoordsChanged)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, GetFeatureLayer(featureName, activeView), null);
            }
        }

        internal void RemoveObservPoint(string featureName, IActiveView activeView, int id)
        {
            var featureClass = GetFeatureClass(featureName, activeView);
            GdbAccess.Instance.RemoveObservPoint(featureClass, id);
            _observationPoints.Remove(GetObservPointByIdAsObservationPoint(id));
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, GetFeatureLayer(featureName, activeView), null);
        }

        internal void RemoveGeoCalcPointAsObserverPoint(int id)
        {
            var geoCalcPoint = _observationPoints.FirstOrDefault(point => point.Objectid == id);

            if (geoCalcPoint == null)
            {
                return;
            }

            _observationPoints.Remove(geoCalcPoint);
            view.RemoveObserverPoint(id);
        }

        internal void ShowObservPoint(IActiveView activeView, int id)
        {
            var point = GetObservPointByIdAsObservationPoint(id);
            var pointGeometry = new PointClass();

            if (point.X == null || point.Y == null)
            {
                IPoint p = GetIPointObservPoint(id);
                if (p != null)
                {
                    pointGeometry.X = (double)p.X;
                    pointGeometry.Y = (double)p.Y;
                    pointGeometry.SpatialReference = p.SpatialReference;
                }
                else
                {
                    var pc = GetEnvelopeCenterPoint(activeView.Extent.Envelope);
                    pointGeometry.X = (double)pc.X;
                    pointGeometry.Y = (double)pc.Y;
                    pointGeometry.SpatialReference = activeView.FocusMap.SpatialReference;
                }
            }
            else
            {
                pointGeometry.X = (double)point.X;
                pointGeometry.Y = (double)point.Y;
                pointGeometry.SpatialReference = EsriTools.Wgs84Spatialreference;
            }
            pointGeometry.Project(activeView.FocusMap.SpatialReference);

            if (!IsPointOnExtent(activeView.Extent, pointGeometry))
            {
                EsriTools.PanToGeometry(activeView, pointGeometry, true);

            }
            EsriTools.FlashGeometry(pointGeometry, 500, ArcMap.Application);
        }
        // DS: Exception
        internal IEnumerable<VisibilityTasknGui> SortTasks(IEnumerable<VisibilityTasknGui> source, VeluableTaskSortFieldsEnum sortBy, bool sortDireaction)
        {
            if (sortBy == VeluableTaskSortFieldsEnum.Created)
            {
                return sortDireaction ? source.OrderBy(p => p.Created) : source.OrderByDescending(p => p.Created);
            }
            if (sortBy == VeluableTaskSortFieldsEnum.State)
            {
                return sortDireaction ? source.OrderBy(p => p.State) : source.OrderByDescending(p => p.State);
            }
            if (sortBy == VeluableTaskSortFieldsEnum.Title)
            {
                return sortDireaction ? source.OrderBy(p => p.Name) : source.OrderByDescending(p => p.Name);
            }

            return source;
        }

        internal IEnumerable<ObservObjectGui> SortObservationObjects(IEnumerable<ObservObjectGui> source, VeluableObservObjectSortFieldsEnum sortBy, bool sortDireaction)
        {
            if (sortBy == VeluableObservObjectSortFieldsEnum.Affiliation)
            {
                return sortDireaction ? source.OrderBy(p => p.Affiliation) : source.OrderByDescending(p => p.Affiliation);
            }
            if (sortBy == VeluableObservObjectSortFieldsEnum.Date)
            {
                return sortDireaction ? source.OrderBy(p => p.Created) : source.OrderByDescending(p => p.Created);
            }
            if (sortBy == VeluableObservObjectSortFieldsEnum.Group)
            {
                return sortDireaction ? source.OrderBy(p => p.Group) : source.OrderByDescending(p => p.Group);
            }
            if (sortBy == VeluableObservObjectSortFieldsEnum.Title)
            {
                return sortDireaction ? source.OrderBy(p => p.Title) : source.OrderByDescending(p => p.Title);
            }
            if (sortBy == VeluableObservObjectSortFieldsEnum.Id)
            {
                return sortDireaction ? source.OrderBy(p => p.Id) : source.OrderByDescending(p => p.Id);
            }

            return source;
        }

        internal IEnumerable<ObservPointGui> SortObservationPoints(IEnumerable<ObservPointGui> source, ValuableObservPointSortFieldsEnum sortBy, bool sortDireaction)
        {
            if (sortBy == ValuableObservPointSortFieldsEnum.Name)
            {
                return sortDireaction ? source.OrderBy(p => p.Title) : source.OrderByDescending(p => p.Title);
            }
            if (sortBy == ValuableObservPointSortFieldsEnum.Date)
            {
                return sortDireaction ? source.OrderBy(p => p.Date) : source.OrderByDescending(p => p.Date);
            }
            if (sortBy == ValuableObservPointSortFieldsEnum.Affiliation)
            {
                return sortDireaction ? source.OrderBy(p => p.Affiliation) : source.OrderByDescending(p => p.Affiliation);
            }
            if (sortBy == ValuableObservPointSortFieldsEnum.Type)
            {
                return sortDireaction ? source.OrderBy(p => p.Type) : source.OrderByDescending(p => p.Type);
            }

            return source.ToArray();
        }


        internal IEnumerable<ObservationPoint> GetAllObservationPoints()
        {
            return VisibilityZonesFacade.GetAllObservationPoints();
        }


        internal ObservationPoint CreatePointWithDefaultValues(IEnvelope envelope)
        {
            log.DebugEx("> CreatePointWithDefaultValues START");
            var centerPoint = GetEnvelopeCenterPoint(envelope);
            log.DebugEx("CreatePointWithDefaultValues. centerPoint.X:{0} centerPoint.Y:{1}", centerPoint.X, centerPoint.Y);

            ObservationPoint op = new ObservationPoint();
            log.DebugEx("CreatePointWithDefaultValues. 0");

            op.X = centerPoint.X;
            op.Y = centerPoint.Y;
            log.DebugEx("CreatePointWithDefaultValues. 1");

            op.Title = ObservPointDefaultValues.ObservPointNameText;

            op.Type = ObservationPointMobilityTypesEnum.Stationary.ToString();
            op.Affiliation = ObservationPointTypesEnum.Enemy.ToString();

            op.AzimuthStart = 0; // Convert.ToDouble(ObservPointDefaultValues.AzimuthBText);
            op.AzimuthEnd = 360; // Convert.ToDouble(ObservPointDefaultValues.AzimuthEText);

            op.RelativeHeight = 2; // Convert.ToDouble(ObservPointDefaultValues.RelativeHeightText);
            op.InnerRadius = 0;
            op.OuterRadius = 2500;

            log.DebugEx("CreatePointWithDefaultValues. 2");

            op.AvailableHeightLover = 0; // Convert.ToDouble(ObservPointDefaultValues.HeightMinText);
            op.AvailableHeightUpper = 0; // Convert.ToDouble(ObservPointDefaultValues.HeightMaxText);
            log.DebugEx("CreatePointWithDefaultValues. 3");

            op.AngelMinH = -90; //Convert.ToDouble(ObservPointDefaultValues.AngleOFViewMinText);
            op.AngelMaxH = 90; //Convert.ToDouble(ObservPointDefaultValues.AngleOFViewMaxText);
            log.DebugEx("CreatePointWithDefaultValues. 4");

            op.AngelFrameH = 0; // Convert.ToDouble(ObservPointDefaultValues.AngleFrameHText);
            op.AngelFrameV = 0; // Convert.ToDouble(ObservPointDefaultValues.AngleFrameVText);
            op.AngelCameraRotationH = 0; // Convert.ToDouble(ObservPointDefaultValues.CameraRotationHText);
            op.AngelCameraRotationV = 0; // Convert.ToDouble(ObservPointDefaultValues.CameraRotationVText);
            log.DebugEx("CreatePointWithDefaultValues. 5");

            op.AzimuthMainAxis = 0; // Convert.ToDouble(ObservPointDefaultValues.AzimuthMainAxisText);

            log.DebugEx("CreatePointWithDefaultValues. 6");

            op.Dto = DateTime.Now.Date;
            op.Operator = Environment.UserName;

            log.DebugEx("> CreatePointWithDefaultValues END");
            return op;
        }

        internal void AddPoint(string featureName, IActiveView activeView)
        {
            log.DebugEx("> AddPoint START. featureName:{0}", featureName);
            try
            {
                var point = CreatePointWithDefaultValues(activeView.Extent.Envelope);

                log.DebugEx("AddPoint. point.X:{0} point.Y:{1}", point.X, point.Y);

                var pointGeometry = new PointClass
                {
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
                view.AddRecord(GetObservationPointFromInterface(_observationPoints.Last()));

                log.DebugEx("> AddPoint END");

            }
            catch (Exception ex)
            {
                log.DebugEx("> AddPoint Exception:{0}", ex.Message);

            }
        }

        // TODO: Define the field in the View Interface to take sessionName, rasterLayerName and  visibilityCalculationResults
        internal bool ExsecuteVisibilityCalculations(WizardResult calcParams)
        {
            log.InfoEx("> CalculateVisibility START");

            var statusBar = ArcMap.Application.StatusBar;
            var animationProgressor = statusBar.ProgressAnimation;
            int exx = 1;

            try
            {
                if (calcParams.CalculationType == VisibilityCalcTypeEnum.OpservationPoints || calcParams.CalculationType == VisibilityCalcTypeEnum.ObservationObjects)
                {
                    exx = CalculateVisibility(calcParams, animationProgressor);
                }
                else if (calcParams.CalculationType == VisibilityCalcTypeEnum.BestObservationParameters)
                {
                    exx = CalculateBestOPParams(calcParams, animationProgressor);
                }
            }
            catch (Exception ex)
            {
                log.ErrorEx("> CalculateVisibility Exception. exx:{0} Exception:{1}", exx, ex.Message);
                return false;
            }
            finally
            {
                animationProgressor.Stop();
                animationProgressor.Hide();
            }
            log.InfoEx("> CalculateVisibility END");
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
                    result = VisibilityZonesFacade.GetObservationPointsByObjectIds(visiblePoints);
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
            var obj = _observationObjects.FirstOrDefault(o => o.Id == bbservObjectGu.Id || o.ObjectId == bbservObjectGu.ObjectID);
            if (obj == null)
            {
                return false;
            }

            obj.ObjectType = _observObjectsTypes.First(v => v.Value == bbservObjectGu.Affiliation).Key;
            obj.Title = bbservObjectGu.Title;
            obj.Group = bbservObjectGu.Group;
            obj.Id = bbservObjectGu.Id;

            return VisibilityZonesFacade.SaveObservationObject(obj);
        }


        internal IPoint GetEnvelopeCenterPoint(IEnvelope envelope)
        {
            var point = EsriTools.GetCenterPoint(envelope);
            point.Project(EsriTools.Wgs84Spatialreference);
            return point;
        }

        internal void UpdateObservObjectsList()
        {
            _observationObjects = VisibilityZonesFacade.GetAllObservationObjects().ToList();
            view.FillObservationObjectsList(_observationObjects);
        }

        internal ObservationPoint GetObservationPointFromInterface(IObserverPoint observerPoint)
        {
            if (observerPoint == null)
            {
                return null;
            }

            if (observerPoint is ObservationPoint)
            {
                return observerPoint as ObservationPoint;
            }

            if (observerPoint is GeoCalcPoint)
            {
                var geoCalcPoint = observerPoint as GeoCalcPoint;

                return new ObservationPoint
                {
                    Objectid = geoCalcPoint.Objectid,
                    Affiliation = ObservationPointTypesEnum.All.ToString(),
                    Type = ObservationPointMobilityTypesEnum.All.ToString(),
                    Title = geoCalcPoint.Title,
                    X = geoCalcPoint.X,
                    Y = geoCalcPoint.Y,
                    AngelMaxH = geoCalcPoint.AngelMaxH ?? 90,
                    AngelMinH = geoCalcPoint.AngelMinH ?? -90,
                    AzimuthStart = geoCalcPoint.AzimuthStart ?? 0,
                    AzimuthEnd = geoCalcPoint.AzimuthEnd ?? 360,
                    RelativeHeight = geoCalcPoint.RelativeHeight ?? 0,
                    InnerRadius = geoCalcPoint.InnerRadius ?? 0,
                    OuterRadius = (geoCalcPoint.OuterRadius == 0 || geoCalcPoint.OuterRadius == null) ? 1000 : geoCalcPoint.OuterRadius,
                    Dto = DateTime.Now,
                    Operator = geoCalcPoint.UserName
                };
            }

            return new ObservationPoint
            {
                Objectid = observerPoint.Objectid,
                Affiliation = ObservationPointTypesEnum.All.ToString(),
                Type = ObservationPointMobilityTypesEnum.All.ToString(),
                Title = observerPoint.Title,
                X = observerPoint.X,
                Y = observerPoint.Y,
                AngelMaxH = observerPoint.AngelMaxH ?? 90,
                AngelMinH = observerPoint.AngelMinH ?? -90,
                AzimuthStart = observerPoint.AzimuthStart ?? 0,
                AzimuthEnd = observerPoint.AzimuthEnd ?? 360,
                RelativeHeight = observerPoint.RelativeHeight ?? 0,
                InnerRadius = 0,
                OuterRadius = 1000,
                Dto = DateTime.Now,
                Operator = Environment.UserName
            };
        }

        public bool AddObservObjectsLayer()
        {
            return VisibilityManager.AddObservationObjectLayer(mapDocument.ActiveView);
        }

        public bool AddObservPointsLayer()
        {
            return VisibilityManager.AddVisibilityPointLayer(mapDocument.ActiveView);
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

        public IEnumerable<string> GetObservationObjectTypes(bool useUseAll = true)
        {
            return (useUseAll ? _observObjectsTypes : _observObjectsTypes.Where(t => t.Key != ObservationObjectTypesEnum.All)).Select(t => t.Value);
        }
        public string GetAllAffiliationType_for_objects()
        {
            return _observObjectsTypes[ObservationObjectTypesEnum.All];
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
                if (layer is IFeatureLayer fl
                    && fl.FeatureClass.AliasName.Equals(GetObservPointFeatureName(), StringComparison.InvariantCultureIgnoreCase))
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
                if (layer is IFeatureLayer fl
                   && fl.FeatureClass.AliasName.EndsWith(GetObservObjectFeatureName(), StringComparison.InvariantCultureIgnoreCase))
                {
                    observstsLayersNames.Add(layer.Name);
                }
            });

            return observstsLayersNames;
        }

        public IFeatureClass GetObservatioPointFeatureClass(IActiveView esriView = null)
        {
            return GetFeatureClass(_observPointFeature, esriView);
        }
        public IFeatureClass GetObservatioStationFeatureClass(IActiveView esriView)
        {
            return GetFeatureClass(_observStationFeature, esriView);
        }

        public bool IsObservPointsExists()
        {
            try
            {
                return IsFeatureLayerExists(mapDocument.ActiveView, _observPointFeature);
            }
            catch
            {
                return false;
            }
        }

        public bool IsObservObjectsExists()
        {
            try
            {
                return IsFeatureLayerExists(mapDocument.ActiveView, _observStationFeature);
            }
            catch
            {
                return false;
            }
        }

        public string GetObservationPointsLayerName => view.ObservationPointsFeatureClass;

        public string GetObservationStationLayerName => _observStationFeature;

        private int CalculateVisibility(WizardResult calcParams, IAnimationProgressor animationProgressor)
        {
            int exx = 1;

            MapLayersManager layersManager = new MapLayersManager(mapDocument.ActiveView);
            exx++;
            var demLayer = layersManager.RasterLayers.FirstOrDefault(l => l.Name.Equals(calcParams.RasterLayerName));
            exx++;
            if (demLayer == null)
            {
                throw new MilSpaceVisibilityCalcFailedException($"Cannot find DEM layer {calcParams.RasterLayerName }.");
            }
            calcParams.RasterLayerName = demLayer.FilePath;
            exx++;

            var featureLayer =
                GetObserverPointsFeatureClass(calcParams.ObserverPointsSourceType, demLayer.Raster, calcParams.ObserverPointsLayerName);
            var observPoints = featureLayer;
            exx++;
            var observObjects = GetObserverObjectsFeatureClass(calcParams.ObserverObjectsSourceType,
                                                               calcParams.ObservationObjectLayerName, calcParams.Buffer);
            exx++;

            if (calcParams.ObservPointIDs == null) // Get points from the current extent
            {
                calcParams.ObservPointIDs = EsriTools.GetSelectionByExtent(observPoints, mapDocument.ActiveView);
            }
            if (calcParams.ObservObjectIDs == null) // Get points from the current extent
            {
                calcParams.ObservObjectIDs = EsriTools.GetSelectionByExtent(observObjects, mapDocument.ActiveView);
            }
            exx++;

            animationProgressor.Show();
            animationProgressor.Play(0, 200);

            exx++;

            try
            {
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
                    mapDocument.ActiveView.FocusMap,
                    100);

                exx++;

                if (calcTask.Finished != null)
                {
                    var isLayerAbove = (calcParams.ResultLayerPosition == LayerPositionsEnum.Above);

                    var datasets = GdbAccess.Instance.GetDatasetsFromCalcWorkspace(calcTask.ResultsInfo);
                    var tbls = mapDocument.TableProperties;

                    ArcMapHelper.AddResultsToMapAsGroupLayer(
                        calcTask,
                        mapDocument.ActiveView,
                        calcParams.RelativeLayerName,
                        isLayerAbove,
                        calcParams.ResultLayerTransparency
                        , null);

                    exx++;

                    EsriTools.AddTableToMap(
                        tbls,
                        VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.CoverageTable, calcTask.Name),
                        calcTask.ReferencedGDB,
                        mapDocument,
                        application);
                    exx++;

                }
            }
            finally
            {
                //test
                //var layer = EsriTools.GetFeatureLayer(featureLayer);
                //mapDocument.FocusMap.AddLayer(layer);

                //var observPointFeatureClassName = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.ObservationPoints, calcParams.TaskName, -1);
                //var feature = GdbAccess.Instance.GetFeatureClass(MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection, observPointFeatureClassName);
                //var layer1 = EsriTools.GetFeatureLayer(feature);
                //mapDocument.FocusMap.AddLayer(layer1);


                EsriTools.RemoveDataSet(MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection, $"GeoCalculatorPoint_VisiilityMasterResult", esriDatasetType.esriDTFeatureClass);

            }

            return exx;
        }

        private int CalculateBestOPParams(WizardResult calcParams, IAnimationProgressor animationProgressor)
        {
            int exx = 1;

            MapLayersManager layersManager = new MapLayersManager(mapDocument.ActiveView);
            exx++;

            var demLayer = layersManager.RasterLayers.FirstOrDefault(l => l.Name.Equals(calcParams.RasterLayerName));
            exx++;

            if (demLayer == null)
            {
                throw new MilSpaceVisibilityCalcFailedException($"Cannot find DEM layer {calcParams.RasterLayerName }.");
            }

            calcParams.RasterLayerName = demLayer.FilePath;
            exx++;

            var observPointsFeatureClass = GetObservatioPointFeatureClass(mapDocument.ActiveView);
            exx++;

            var observerPointTemporaryFeatureClass = BestOPParametersManager.CreateOPFeatureClass(
                    calcParams,
                    observPointsFeatureClass,
                    mapDocument.ActiveView,
                    demLayer.Raster);

            //test
            //var layer = EsriTools.GetFeatureLayer(observerPointTemporaryFeatureClass);
            //mapDocument.AddLayer(layer);
            exx++;

            var observationStationTemporaryFeatureClass = BestOPParametersManager.CreateOOFeatureClass(
                    calcParams.ObservationStation,
                    mapDocument.ActiveView,
                    calcParams.TaskName);

            //test
            //var layer1 = EsriTools.GetFeatureLayer(observationStationTemporaryFeatureClass);
            //mapDocument.AddLayer(layer1);
            exx++;

            var observPointsIds = BestOPParametersManager.GetAllIdsFromFeatureClass(observerPointTemporaryFeatureClass);
            var observStationsIds = BestOPParametersManager.GetAllIdsFromFeatureClass(observationStationTemporaryFeatureClass);

            exx++;

            if (observPointsIds == null || observStationsIds == null)
            {
                var featureClassName = observPointsIds == null ?
                         LocalizationContext.Instance.FindLocalizedElement("ObserverPointParametersText", "параметрів пункту спостереження") :
                         LocalizationContext.Instance.FindLocalizedElement("ObservationStationText", "об'єкту спостреження");

                MessageBox.Show(
                     String.Format(LocalizationContext.Instance.FindLocalizedElement("ErrorInTemporaryStorageGenerating",
                         "Під час генерації векторного класу {0} сталася помилка"), featureClassName),
                     LocalizationContext.Instance.MessageBoxCaption);

                BestOPParametersManager.ClearTemporaryData(calcParams.TaskName);
                return exx;
            }

            animationProgressor.Show();
            animationProgressor.Play(0, 200);

            var calcTask = VisibilityManager.Generate(
                observerPointTemporaryFeatureClass,
                observPointsIds,
                observationStationTemporaryFeatureClass,
                observStationsIds,
                calcParams.RasterLayerName,
                calcParams.VisibilityCalculationResults,
                calcParams.TaskName,
                calcParams.TaskName,
                calcParams.CalculationType,
                mapDocument.ActiveView.FocusMap,
                calcParams.VisibilityPercent);

            exx++;

           // BestOPParametersManager.ClearTemporaryData(calcParams.TaskName, calcTask.ReferencedGDB);
            exx++;

            if (calcTask.Finished != null)
            {
                var tbls = mapDocument.TableProperties;

                EsriTools.AddTableToMap(
                    tbls,
                    VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.BestParametersTable, calcTask.Name),
                    calcTask.ReferencedGDB,
                    mapDocument,
                    application);
                exx++;

            }
            return exx;
        }


        private bool IsFeatureLayerExists(IActiveView view, string featureClass)
        {
            log.DebugEx("> IsFeatureLayerExists START. featureClass:{0}", featureClass);

            try
            {
                var pattern = @"^[A-Za-z0-9]+\.[A-Za-z0-9]+\." + featureClass + "$";
                //log.DebugEx("IsFeatureLayerExists featureClass:{0} pattern:{1}", featureClass, pattern);

                var layers = view.FocusMap.Layers;
                var layer = layers.Next();

                log.DebugEx("IsFeatureLayerExists. layers OK");

                while (layer != null)
                {
                    if (layer is IFeatureLayer fl
                        && fl.FeatureClass != null
                        && (fl.FeatureClass.AliasName.Equals(featureClass, StringComparison.InvariantCultureIgnoreCase)
                            || Regex.IsMatch(fl.FeatureClass.AliasName, pattern)
                            ))
                    {
                        log.DebugEx("> IsFeatureLayerExists END. FeatureLayerExists: {0}", fl.FeatureClass.AliasName);
                        return true;
                    }

                    layer = layers.Next();
                }
                log.DebugEx("> IsFeatureLayerExists END. NOT FeatureLayerExists");
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
            if (activeView == null)
            {
                activeView = mapDocument.ActivatedView;
            }

            if (string.IsNullOrWhiteSpace(featureClassName))
            {
                return null;
            }

            MapLayersManager mlm = new MapLayersManager(activeView);
            var fl = mlm.FindFeatureLayer(featureClassName);

            return fl?.FeatureClass;
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

        internal void DeleteObservationObject(string id)
        {
            var obj = _observationObjects.FirstOrDefault(o => o.Id == id);
            if (obj != null)
            {
                VisibilityZonesFacade.DeleteObservationObject(obj);
                UpdateObservObjectsList();
            }

        }
        internal void FlashObservationObject(string id)
        {
            var observObjects = GetObservatioStationFeatureClass(mapDocument.ActiveView);
            // observObjects.Search()
            var query = new QueryFilter();
            query.WhereClause = $"[idOO] = '{id}'";

            var objects = observObjects.Search(query, false);
            var observObject = objects.NextFeature();
            if (observObject != null)
            {
                EsriTools.PanToGeometry(mapDocument.ActiveView, observObject.Shape, true);
                EsriTools.FlashGeometry(mapDocument.ActiveView.ScreenDisplay, new IGeometry[] { observObject.Shape });
            }
        }

        internal void ShowGeometry(IGeometry geometry, int buffer = -1)
        {
            var geometryWithBuffer = CalculateGeometryWithBuffer(geometry, buffer);

            if (geometryWithBuffer.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                var point = geometryWithBuffer as IPoint;

                if (!IsPointOnExtent(mapDocument.ActiveView.Extent, point))
                {
                    EsriTools.PanToGeometry(mapDocument.ActiveView, point, true);

                }
                EsriTools.FlashGeometry(point, 500, ArcMap.Application);

                return;
            }

            EsriTools.ZoomToGeometry(mapDocument.ActiveView, geometryWithBuffer);
            EsriTools.FlashGeometry(mapDocument.ActiveView.ScreenDisplay, new IGeometry[] { geometryWithBuffer });
        }

        internal void ShowPoint(ObservationPoint observPoint)
        {
            if (!observPoint.X.HasValue || !observPoint.Y.HasValue)
            {
                return;
            }

            var point = new PointClass { X = observPoint.X.Value, Y = observPoint.Y.Value, SpatialReference = EsriTools.Wgs84Spatialreference };
            point.Project(mapDocument.FocusMap.SpatialReference);

            if (!IsPointOnExtent(mapDocument.ActiveView.Extent, point))
            {
                EsriTools.PanToGeometry(mapDocument.ActiveView, point, true);

            }
            EsriTools.FlashGeometry(point, 500, ArcMap.Application);
        }

        internal IGeometry CalculateGeometryWithBuffer(IGeometry geometry, int buffer = -1)
        {
            if (geometry.GeometryType != esriGeometryType.esriGeometryPoint && geometry.GeometryType != esriGeometryType.esriGeometryPolygon
                && geometry.GeometryType != esriGeometryType.esriGeometryPolyline)
            {
                log.ErrorEx($"> CalculateGeometryWithBuffer Exception. Input geometry is not high level");
                return null;
            }

            if (buffer < 1)
            {
                return geometry;
            }
            else
            {
                var topologicalOperator = (ITopologicalOperator)geometry;
                var geometryWithBuffer = topologicalOperator.Buffer(buffer);

                return geometryWithBuffer;
            }
        }

        internal List<FromLayerPointModel> GetObservationPointsFromModule()
        {
            if (!IsObservPointsExists())
            {
                return null;
            }

            var points = new List<FromLayerPointModel>();

            var featureClass = GetObservatioPointFeatureClass();
            var idFieldIndex = featureClass.FindField(featureClass.OIDFieldName);
            var titleFieldIndex = featureClass.FindField("TitleOp");

            if (idFieldIndex == -1)
            {
                log.WarnEx($"> GetObservationPointsFromModule. Warning: Cannot find fild {featureClass.OIDFieldName} in featureClass {featureClass.AliasName}");
                throw new MissingFieldException();
            }

            if (titleFieldIndex == -1)
            {
                log.WarnEx($"> GetObservationPointsFromModule. Warning: Cannot find fildTitleOp in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter
            {
                WhereClause = $"{featureClass.OIDFieldName} >= 0"
            };

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();

            try
            {
                while (feature != null)
                {
                    var shape = feature.ShapeCopy;

                    var point = shape as IPoint;
                    var pointCopy = point.Clone();
                    //pointCopy.Project(EsriTools.Wgs84Spatialreference);

                    int id = -1;
                    string titleField = string.Empty;

                    if (idFieldIndex >= 0)
                    {
                        id = (int)feature.Value[idFieldIndex];
                    }

                    if (titleFieldIndex >= 0)
                    {
                        titleField = feature.Value[titleFieldIndex].ToString();
                    }

                    points.Add(new FromLayerPointModel { Point = pointCopy, ObjId = id, DisplayedField = titleField });

                    feature = featureCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                log.ErrorEx($"> GetObservationPointsFromModule Exception. ex.Message:{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            return points;
        }

        internal List<FromLayerGeometry> GetObservObjectsFromModule()
        {
            if (!IsObservObjectsExists())
            {
                return null;
            }

            var objects = new List<FromLayerGeometry>();

            var featureClass = GetObservatioStationFeatureClass(mapDocument.ActiveView);
            var idFieldIndex = featureClass.FindField(featureClass.OIDFieldName);
            var titleFieldIndex = featureClass.FindField("sTitleOO");

            if (idFieldIndex == -1)
            {
                log.WarnEx($"> GetObservObjectsFromModule. Warning: Cannot find fild {featureClass.OIDFieldName} in featureClass {featureClass.AliasName}");
                throw new MissingFieldException();
            }

            if (titleFieldIndex == -1)
            {
                log.WarnEx($"> GetObservObjectsFromModule. Warning: Cannot find fild sTitleOO in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = $"{featureClass.OIDFieldName} >= 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while (feature != null)
                {
                    var shape = feature.ShapeCopy;

                    int id = -1;
                    string titleField = string.Empty;

                    if (idFieldIndex >= 0)
                    {
                        id = (int)feature.Value[idFieldIndex];
                    }

                    if (titleFieldIndex >= 0)
                    {
                        titleField = feature.Value[titleFieldIndex].ToString();
                    }

                    objects.Add(new FromLayerGeometry { Geometry = shape, ObjId = id, Title = titleField });

                    feature = featureCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                log.ErrorEx($"> GetObservObjectsFromModule Exception. ex.Message:{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            return objects;
        }

        internal void DrawObservPointsGraphics(bool drawAllGraphics, int id = -1)
        {
            if (drawAllGraphics)
            {
                foreach (var point in _observationPoints)
                {
                    DrawObservPointsGraphics(point.Objectid);
                }
            }
            else
            {
                DrawObservPointsGraphics(id);
            }
        }

        internal void DrawObservPointsGraphics(int id)
        {
            var observPoint = _observationPoints.FirstOrDefault(point => point.Objectid == id);

            if (observPoint == null || observPoint.X == null || observPoint.Y == null)
            {
                return;
            }

            var pointGeom = new Point { X = observPoint.X.Value, Y = observPoint.Y.Value, SpatialReference = EsriTools.Wgs84Spatialreference };
            pointGeom.Project(mapDocument.FocusMap.SpatialReference);

            try
            {
                var maxDistance = CalcCoverageArea(pointGeom, GetObservationPointFromInterface(observPoint));

                GraphicsLayerManager.AddObservPointsGraphicsToMap(_coverageArea, $"coverageArea_{id}");
                GraphicsLayerManager.AddCrossPointerToPoint(pointGeom, Convert.ToInt32(maxDistance), $"crossPointer_coverageArea_{id}_");
            }
            catch (ArgumentException exception)
            {
                log.WarnEx($"> DrawObservPointsGraphics. Exception {exception}");
                MessageBox.Show(LocalizationContext.Instance.CoverageAreaIsEmptyMessage, LocalizationContext.Instance.MessageBoxCaption);
                RemoveObservPointsGraphics(true, true);
            }
        }

        internal double CalcCoverageArea(IPoint pointGeom, ObservationPoint observPoint)
        {
            if (!observPoint.OuterRadius.HasValue)
            {
                observPoint.OuterRadius = 1000;
            }

            if (!observPoint.InnerRadius.HasValue)
            {
                observPoint.InnerRadius = 0;
            }

            // Get min and max distances taking into account min and max distance from parameters and vertical angles
            var realMaxDistance = EsriTools.GetMaxDistance(observPoint.OuterRadius.Value, observPoint.AngelMaxH.Value, observPoint.RelativeHeight.Value);
            var realMinDistance = EsriTools.GetMinDistance(observPoint.InnerRadius.Value, observPoint.AngelMinH.Value, observPoint.RelativeHeight.Value);

            // If min distance more than max distance or min vertical angle isn`n negative we can not calculate coverage
            // area on the flat surface
            if (realMaxDistance < realMinDistance || observPoint.AngelMinH.Value >= 0)
            {
                _coverageArea = null;
                throw new ArgumentException("Observation point doesn`t has a coverage area");
            }

            _coverageArea = EsriTools.GetCoverageArea(
                pointGeom,
                observPoint.AzimuthStart.Value,
                observPoint.AzimuthEnd.Value,
                realMinDistance,
                realMaxDistance);

            _coverageArea.SpatialReference = mapDocument.FocusMap.SpatialReference;

            return realMaxDistance;
        }

        internal void CalcRelationLines(int id, ObservationSetsEnum set, bool fromNewLayer = false, bool fromNewCoverageArea = false)
        {
            var observPoint = _observationPoints.FirstOrDefault(point => point.Objectid == id);

            if (observPoint == null || observPoint.X == null || observPoint.Y == null)
            {
                return;
            }

            var observerPointAsNativeType = GetObservationPointFromInterface(observPoint);

            var pointGeom = new Point { X = observerPointAsNativeType.X.Value, Y = observerPointAsNativeType.Y.Value, SpatialReference = EsriTools.Wgs84Spatialreference };
            pointGeom.Project(mapDocument.FocusMap.SpatialReference);

            var geometries = GetObservationObjects(set, fromNewLayer);

            if (_coverageArea == null || fromNewCoverageArea)
            {
                try
                {
                    var maxDistance = CalcCoverageArea(pointGeom, observerPointAsNativeType);
                }
                catch (Exception ex)
                {
                    log.ErrorEx($"> CalcRelationLines Unexpected exception: {ex.Message}");
                }
            }

            _relationLines.Clear();

            foreach (var geometry in geometries)
            {
                var relationLine = EsriTools.GetToGeometryCenterPolyline(pointGeom, geometry.Value);
                var intersectionArea = (_coverageArea != null) ? EsriTools.GetIntersection(_coverageArea, geometry.Value) : null;

                var simpleLine = new Line { FromPoint = relationLine.FromPoint, ToPoint = relationLine.ToPoint, SpatialReference = relationLine.SpatialReference };

                if (!_observationObjects.Any())
                {
                    UpdateObservObjectsList();
                }

                string title;

                if (set == ObservationSetsEnum.Gdb)
                {
                    var currentObj = _observationObjects.FirstOrDefault(obj => obj.ObjectId == geometry.Key);
                    title = (currentObj == null) ? string.Empty : currentObj.Title;
                }
                else
                {
                    title = geometry.Key.ToString();
                }

                var line = new ObservationStationToObservPointRelationModel
                {
                    Id = geometry.Key,
                    Polyline = relationLine,
                    Azimuth = simpleLine.PosAzimuth(),
                    Title = title
                };

                if (intersectionArea == null || intersectionArea.IsEmpty)
                {
                    line.CoverageType = CoverageTypesEnum.None;
                }
                else
                {
                    var intersectionAreaValue = intersectionArea.Envelope as IArea;
                    var geometryAreaValue = geometry.Value.Envelope as IArea;
                    var diff = geometryAreaValue.Area - intersectionAreaValue.Area;

                    if (diff < 0.01)
                    {
                        line.CoverageType = CoverageTypesEnum.Full;
                    }
                    else
                    {
                        line.CoverageType = CoverageTypesEnum.Partly;
                    }
                }

                _relationLines.Add(line);
            }
        }

        internal Dictionary<int, IGeometry> GetObservationObjects(ObservationSetsEnum set, bool newLayer = true)
        {
            Dictionary<int, IGeometry> geometries = new Dictionary<int, IGeometry>();

            switch (set)
            {
                case ObservationSetsEnum.Gdb:

                    geometries = EsriTools.GetGeometriesFromLayer(VisibilityManager.ObservationStationsFeatureLayer.FeatureClass, mapDocument.ActiveView.FocusMap.SpatialReference);

                    break;

                case ObservationSetsEnum.GeoCalculator:

                    var points = GetPointsFromGeoCalculator();

                    if (points == null)
                    {
                        return null;
                    }

                    foreach (var point in points)
                    {
                        point.Value.Project(mapDocument.FocusMap.SpatialReference);
                        geometries.Add(point.Key, point.Value);
                    }

                    break;

                case ObservationSetsEnum.FeatureLayers:

                    if (string.IsNullOrEmpty(_observaitonObjectsLayerName) || newLayer)
                    {
                        var getLayerWindow = new ChooseVectorLayerFromMapModalWindow(mapDocument.ActiveView);
                        var result = getLayerWindow.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            var layer = EsriTools.GetLayer(getLayerWindow.SelectedLayer, mapDocument.FocusMap);

                            if (layer != null && layer is IFeatureLayer)
                            {
                                geometries = EsriTools.GetGeometriesFromLayer((layer as IFeatureLayer).FeatureClass, mapDocument.ActiveView.FocusMap.SpatialReference);
                                _observaitonObjectsLayerName = getLayerWindow.SelectedLayer;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        var layer = EsriTools.GetLayer(_observaitonObjectsLayerName, mapDocument.FocusMap);

                        if (layer != null && layer is IFeatureLayer fetaureLayer)
                        {
                            geometries = EsriTools.GetGeometriesFromLayer(fetaureLayer.FeatureClass, mapDocument.ActiveView.FocusMap.SpatialReference);
                        }
                    }

                    break;
            }

            return geometries;
        }

        internal void DrawObservPointToObservObjectsRelationsGraphics(int id, ObservationSetsEnum set)
        {
            if (_relationLines == null || !_relationLines.Any())
            {
                CalcRelationLines(id, set);
            }

            foreach (var line in _relationLines)
            {
                IRgbColor color;

                if (line.CoverageType == CoverageTypesEnum.None)
                {
                    color = new RgbColor { Red = 255, Blue = 0, Green = 0 };
                }
                else
                {
                    if (line.CoverageType == CoverageTypesEnum.Full)
                    {
                        color = new RgbColor { Red = 69, Blue = 0, Green = 230 };
                    }
                    else
                    {
                        color = new RgbColor { Red = 229, Blue = 1, Green = 167 };
                    }
                }

                GraphicsLayerManager.AddObservPointsRelationLineToMap(line.Polyline, color, $"relationLine_{id}", line.Title);
            }
        }

        internal void RemoveObservPointsGraphics(bool removeCoverageArea = true, bool removeObservObjectsRelations = true)
        {
            if (removeCoverageArea)
            {
                GraphicsLayerManager.RemoveAllGeometryFromMap($"coverageArea_", MilSpaceGraphicsTypeEnum.Visibility, true);
            }
            if (removeObservObjectsRelations)
            {
                GraphicsLayerManager.RemoveAllGeometryFromMap($"relationLine_", MilSpaceGraphicsTypeEnum.Visibility, true);
            }
        }

        internal string[] GetObservStationSetsStrings()
        {
            return LocalizationContext.Instance.ObservObjectsSets.Select(set => set.Value).ToArray();
        }

        internal ObservationSetsEnum GetObservStationSet(string setString)
        {
            return LocalizationContext.Instance.ObservObjectsSets.FirstOrDefault(set => set.Value == setString).Key;
        }

        internal ObservationSetsEnum GetObservPointsSet(string setString)
        {
            return LocalizationContext.Instance.ObservPointSets.FirstOrDefault(set => set.Value == setString).Key;
        }

        internal ObservationStationToObservPointRelationModel[] GetObservationStationToObservPointRelations(int id, ObservationSetsEnum set)
        {
            if (_relationLines == null || !_relationLines.Any())
            {
                CalcRelationLines(id, set);
            }

            return _relationLines.ToArray();
        }

        internal string GetObservPointsFromGdbFeatureClassName()
        {
            return VisibilityManager.ObservationPointsFeatureLayer.Name;
        }

        internal string GetObservObjectsFromGdbFeatureClassName()
        {
            return VisibilityManager.ObservationStationsFeatureLayer.Name;
        }

        internal void UpdateLocalObservPoint(IObserverPoint newPoint, int pointId, bool updateTable)
        {
            var updatedPointIndex = _observationPoints.FindIndex(point => point.Objectid == pointId);
            newPoint.Objectid = pointId;

            if (updatedPointIndex == -1)
            {
                return;
            }
            else
            {
                _observationPoints[updatedPointIndex] = newPoint;

                if (updateTable)
                {
                    view.FillObservationPointList(_observationPoints, view.GetFilter, false, false);
                }
            }
        }

        internal void UpdateObservPoint(int objId, ObservationSetsEnum set, bool isCoordsChanged)
        {
            var newPoint = _observationPoints.FirstOrDefault(point => point.Objectid == objId);

            if (newPoint == null)
            {
                return;
            }

            UpdateObservPoint(newPoint, objId, set, isCoordsChanged, false);
        }

        internal void UpdateObservPoint(IObserverPoint newPoint, int objId,
                                        ObservationSetsEnum set, bool isCoordsChanged,
                                        bool updateAllPoints = true)
        {
            if (newPoint == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("CannotSavePointError", "Виникла помилка. Неможливо зберегти дані точки."),
                                 LocalizationContext.Instance.MessageBoxCaption);

                return;
            }

            newPoint.Objectid = objId;

            switch (set)
            {
                case ObservationSetsEnum.Gdb:

                    var observPointAsNativeType = GetObservationPointFromInterface(newPoint);
                    UpdateObservPoint(observPointAsNativeType, VisibilityManager.ObservPointFeature, mapDocument.ActiveView,
                                        objId, isCoordsChanged);

                    break;

                case ObservationSetsEnum.GeoCalculator:

                    UpdateObserverPointsFromGeoCalculator(newPoint);

                    break;

                case ObservationSetsEnum.FeatureLayers:

                    var featureLayer = GetFeatureLayer(_observPointFromFeatureLayerData.Key);

                    if (featureLayer == null)
                    {
                        return;
                    }

                    GdbAccess.Instance.UpdateObservPoint(featureLayer.FeatureClass, newPoint);

                    break;
            }

            if (updateAllPoints)
            {
                GetObserverPointsFromSelectedSource(set, false);
            }
        }

        internal void SelectObservationStationFromSet(ObservationSetsEnum set)
        {
            IGeometry geometry = null;
            var title = string.Empty;

            switch (set)
            {
                case ObservationSetsEnum.Gdb:

                    var geometryWithId = GetObservationStationFromGdb();

                    if (geometryWithId.Key == -1)
                    {
                        break;
                    }

                    geometry = geometryWithId.Value;

                    if (!_observationObjects.Any())
                    {
                        _observationObjects = VisibilityZonesFacade.GetAllObservationObjects().ToList();
                    }

                    title = _observationObjects.First(obj => obj.ObjectId == geometryWithId.Key).Title;

                    break;

                case ObservationSetsEnum.GeoCalculator:

                    geometry = GetObservationStationFromGeoCalc();

                    break;

                case ObservationSetsEnum.FeatureLayers:

                    var geometryFromLayer = GetObservationStationFromFeatureLayer();
                    geometry = geometryFromLayer.Key;
                    title = geometryFromLayer.Value;

                    break;
            }

            if (geometry == null)
            {
                return;
            }

            geometry.Project(ArcMap.Document.FocusMap.SpatialReference);
            view.AddSelectedOO(geometry, title);
            //TEST GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).TestObjects(geometry);
        }

        internal void GetObserverPointsFromSelectedSource(ObservationSetsEnum set, bool newSource = true,
                                                            bool mainWindow = true)
        {
            if (newSource)
            {
                UnsubscribeFromDeletePointEvent();
            }

            switch (set)
            {
                case ObservationSetsEnum.Gdb:

                    _observationPoints = VisibilityZonesFacade.GetAllObservationPoints()
                                                       .Select(point => point as IObserverPoint)
                                                       .ToList();

                    break;

                case ObservationSetsEnum.GeoCalculator:

                    _observationPoints = GetObserverPointsFromGeoCalculator((newSource && mainWindow));

                    break;

                case ObservationSetsEnum.FeatureLayers:

                    _observationPoints = GetObservationPointsFromPointLayer(newSource);

                    break;
            }

            if (_observationPoints != null)
            {
                if (mainWindow)
                {
                    view.SetFieldsEditingAbility(!(set == ObservationSetsEnum.Gdb));
                }

                view.FillObservationPointList(_observationPoints, view.GetFilter, true);
            }
            else
            {
                view.ClearObserverPointsList(set == ObservationSetsEnum.Gdb);
            }
        }

        internal void FillObservationObjectsInMasterFromSelectedSource(ObservationSetsEnum set, VisibilityCalcTypeEnum visibilityCalcType)
        {
            if (visibilityCalcType == VisibilityCalcTypeEnum.BestObservationParameters)
            {
                SelectObservationStationFromSet(set);
            }
            else
            {
                var geometries = GetObservationObjects(set);
                view.FillObservationObjectsList(geometries);
            }
        }

        internal void FillObserverPointsInMasterFromSelectedSource(ObservationSetsEnum set, VisibilityCalcTypeEnum visibilityCalcType)
        {
            if (visibilityCalcType == VisibilityCalcTypeEnum.BestObservationParameters)
            {
                SelectObservationPointFromSet(set);
            }
            else
            {
                GetObserverPointsFromSelectedSource(set, true, false);
            }
        }

        internal void SelectObservationPointFromSet(ObservationSetsEnum set)
        {
            ObservationPoint point = null;

            switch (set)
            {
                case ObservationSetsEnum.Gdb:

                    point = GetObservationPointFromGdb();

                    break;

                case ObservationSetsEnum.GeoCalculator:

                    point = GetObservationPointFromGeoCalc();

                    break;

                case ObservationSetsEnum.FeatureLayers:

                    point = GetObservationPointFromPointLayer();

                    break;
            }

            if (point == null)
            {
                return;
            }

            view.FillSelectedOPFields(point);
        }

        internal bool SetObserverPointsToRouteMode(ObservationSetsEnum set, bool showGraphics)
        {
            if (_observationPoints.Count < 2)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("NotEnoughPointsToBuildRoute", "Недостатньо точок для побудови маршруту."),
                                LocalizationContext.Instance.MessageBoxCaption);

                return false;
            }

            if (!CheckAllPointsHaveSameFields())
            {
                var dialogResult = MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("RewriteAllPointsToTheSameValues",
                                                                                   "Неможливо побудувати маршрут з точок, які мають різні параметри. \nВи бажаєте привести всі точки до параметрів першої?"),
                                 LocalizationContext.Instance.MessageBoxCaption, MessageBoxButtons.OKCancel);

                if (dialogResult != DialogResult.OK)
                {
                    return false;
                }

                var standardPoint = _observationPoints.First();

                var standardPointGeometry = GetObserverPointGeometry(standardPoint);
                var nextPointGeometry = GetObserverPointGeometry(_observationPoints[0]);

                var standardBaseStartAzimuth = standardPoint.AzimuthStart.Value;
                var standardBaseEndAzimuth = standardPoint.AzimuthEnd.Value;

                for (int i = 0; i < _observationPoints.Count; i++)
                {
                    IPoint currentPointGeometry;

                    if (i != _observationPoints.Count - 1)
                    {
                        currentPointGeometry = nextPointGeometry.Clone();
                        nextPointGeometry = GetObserverPointGeometry(_observationPoints[i + 1]);
                    }
                    else
                    {
                        currentPointGeometry = GetObserverPointGeometry(_observationPoints[i - 1]);
                    }

                    var currentPointAzimuths = FindAzimuthRelativeToDirection(currentPointGeometry,
                                                                              nextPointGeometry,
                                                                              standardBaseStartAzimuth,
                                                                              standardBaseEndAzimuth);

                    var currentPointBaseStartAzimuth = currentPointAzimuths.StartAzimuth;
                    var currentPointBaseEndAzimuth = currentPointAzimuths.EndAzimuth;

                    _observationPoints[i].AngelMaxH = standardPoint.AngelMaxH;
                    _observationPoints[i].AngelMinH = standardPoint.AngelMinH;
                    _observationPoints[i].InnerRadius = standardPoint.InnerRadius;
                    _observationPoints[i].OuterRadius = standardPoint.OuterRadius;
                    _observationPoints[i].AzimuthEnd = currentPointBaseEndAzimuth;
                    _observationPoints[i].AzimuthStart = currentPointBaseStartAzimuth;

                    UpdateObservPoint(_observationPoints[i], _observationPoints[i].Objectid, set, false);
                }

                view.SetFieldsEditingAbility(true);
                view.FillObservationPointList(_observationPoints, view.GetFilter, true);
            }

            return true;
        }

        internal double GetDirection(IPoint currentPoint, IPoint nextPoint)
        {
            var lineBetweenPoints = new Line
            {
                FromPoint = currentPoint,
                ToPoint = nextPoint,
                SpatialReference = currentPoint.SpatialReference
            };

            return lineBetweenPoints.PosAzimuth();
        }

        internal Azimuths FindBaseAzimuthFromRelativeToDirection(IPoint currentPoint, IPoint nextPoint,
                                                               double relativeStartAzimuth,
                                                               double relativeEndAzimuth)
        {
            var direction = GetDirection(currentPoint, nextPoint);
            return FindBaseAzimuthFromRelativeToDirection(direction, relativeStartAzimuth, relativeEndAzimuth);
        }

        internal Azimuths FindBaseAzimuthFromRelativeToDirection(double direction,
                                                                 double relativeStartAzimuth,
                                                                 double relativeEndAzimuth)
        {
            var azimuths = new Azimuths();

            if (relativeStartAzimuth == 0 && relativeEndAzimuth == 360)
            {
                azimuths.StartAzimuth = 0;
                azimuths.EndAzimuth = 360;

                return azimuths;
            }

            azimuths.StartAzimuth = Math.Round(relativeStartAzimuth - direction, 0);
            azimuths.EndAzimuth = Math.Round(relativeEndAzimuth - direction, 0);

            azimuths.FormatAzimuth();

            return azimuths;
        }

        internal IObserverPoint GetNextPoint(int currentPointId)
        {
            var currentPoint = _observationPoints.FirstOrDefault(point => point.Objectid == currentPointId);
            var nextPointIndex = _observationPoints.IndexOf(currentPoint) + 1;

            if (nextPointIndex < _observationPoints.Count)
            {
                return _observationPoints[nextPointIndex];
            }

            return null;
        }

        internal IObserverPoint GetPrevPoint(int currentPointId)
        {
            var currentPoint = _observationPoints.FirstOrDefault(point => point.Objectid == currentPointId);
            var nextPointIndex = _observationPoints.IndexOf(currentPoint) - 1;

            if (nextPointIndex > 0)
            {
                return _observationPoints[nextPointIndex];
            }

            return null;
        }

        internal IPoint GetObserverPointGeometry(IObserverPoint observerPoint)
        {
            if (observerPoint == null)
            {
                return null;
            }

            var point = new Point
            {
                X = observerPoint.X.Value,
                Y = observerPoint.Y.Value,
                SpatialReference = EsriTools.Wgs84Spatialreference
            };

            point.Project(ArcMap.Document.FocusMap.SpatialReference);

            return point;
        }

        internal Azimuths FindAzimuthRelativeToDirection(IPoint currentPoint, IPoint nextPoint,
                                                         double baseStartAzimuth,
                                                         double baseEndAzimuth)
        {
            var direction = GetDirection(currentPoint, nextPoint);

            return FindAzimuthRelativeToDirection(direction, baseStartAzimuth, baseEndAzimuth);
        }

        internal Azimuths FindAzimuthRelativeToDirection(double direction,
                                                         double baseStartAzimuth,
                                                         double baseEndAzimuth)
        {
            var azimuths = new Azimuths();

            if (baseStartAzimuth == 0 && baseEndAzimuth == 360)
            {
                azimuths.StartAzimuth = 0;
                azimuths.EndAzimuth = 360;

                return azimuths;
            }

            azimuths.StartAzimuth = Math.Round(baseStartAzimuth + direction, 0);
            azimuths.EndAzimuth = Math.Round(baseEndAzimuth + direction, 0);

            azimuths.FormatAzimuth();

            return azimuths;
        }

        internal void UpdateAllPointsWithNewValues(IObserverPoint newPoint,
                                                   double baseStartAzimuth,
                                                   double baseEndAzimuth,
                                                   ObservationSetsEnum set,
                                                   bool changeHeight,
                                                   bool savePoints)
        {
            IPoint nextPointGeometry = GetObserverPointGeometry(_observationPoints[0]);
            IPoint currentPointGeometry;

            for (int i = 0; i < _observationPoints.Count; i++)
            {
                if (i != _observationPoints.Count - 1)
                {
                    currentPointGeometry = nextPointGeometry.Clone();
                    nextPointGeometry = GetObserverPointGeometry(_observationPoints[i + 1]);
                }
                else
                {
                    currentPointGeometry = GetObserverPointGeometry(_observationPoints[i - 1]);
                }

                var relativeAzimuth = FindAzimuthRelativeToDirection(currentPointGeometry, nextPointGeometry,
                                                                      baseStartAzimuth, baseEndAzimuth);

                _observationPoints[i].AzimuthStart = relativeAzimuth.StartAzimuth;
                _observationPoints[i].AzimuthEnd = relativeAzimuth.EndAzimuth;
                _observationPoints[i].AngelMaxH = newPoint.AngelMaxH;
                _observationPoints[i].AngelMinH = newPoint.AngelMinH;
                _observationPoints[i].InnerRadius = newPoint.InnerRadius;
                _observationPoints[i].OuterRadius = newPoint.OuterRadius;

                if (_observationPoints[i].Objectid == newPoint.Objectid || changeHeight)
                {
                    _observationPoints[i].RelativeHeight = newPoint.RelativeHeight;
                }

                if (savePoints)
                {
                    UpdateObservPoint(_observationPoints[i], _observationPoints[i].Objectid, set, false);
                }
            }
        }

        const string geoCalcTemFeatureClassName = "GCPoints_";
        const string featureClassTemFeatureClassName = "FCPoints_";

        internal IFeatureClass GetObserverPointsFeatureClass(ObservationSetsEnum source, IRaster raster, string layerName)
        {
            switch (source)
            {
                case ObservationSetsEnum.Gdb:

                    return GetObservatioPointFeatureClass(mapDocument.ActiveView);

                case ObservationSetsEnum.GeoCalculator:

                    var observationPoints = GetObserverPointsFromGeoCalculator(false);
                    return CreateTempPointsFeatureClass(raster, observationPoints, geoCalcTemFeatureClassName);

                case ObservationSetsEnum.FeatureLayers:

                    var pointsFromLayer = VisibilityManager
                                            .GetObservationPointsFromAppropriateLayer(layerName, ArcMap.Document.ActiveView);

                    return CreateTempPointsFeatureClass(raster, pointsFromLayer, featureClassTemFeatureClassName);
            }

            return null;
        }

        const string geoCalcObjTemFeatureClassName = "GCObjects_";
        const string featureClassObjTemFeatureClassName = "FCObjects_";

        internal IFeatureClass GetObserverObjectsFeatureClass(ObservationSetsEnum source,
                                                              string layerName, int buffer)
        {
            UnsubscribeFromDeletePointEvent();
            switch (source)
            {
                case ObservationSetsEnum.Gdb:

                    return GetObservatioStationFeatureClass(mapDocument.ActiveView);

                case ObservationSetsEnum.GeoCalculator:

                    var observationObjects = GetPointsFromGeoCalculator();
                    return CreateOOFeatureClass(observationObjects.Values.Select(obj => obj as IGeometry).ToList(),
                                                 mapDocument.ActiveView,
                                                 geoCalcObjTemFeatureClassName, buffer);

                case ObservationSetsEnum.FeatureLayers:

                    var layer = EsriTools.GetLayer(layerName, mapDocument.FocusMap);
                    var geometries = new Dictionary<int, IGeometry>();

                    if (layer != null && layer is IFeatureLayer featureLayer)
                    {
                        geometries = EsriTools.GetGeometriesFromLayer(featureLayer.FeatureClass, mapDocument.ActiveView.FocusMap.SpatialReference);

                        return CreateOOFeatureClass(geometries.Values.Select(obj => obj as IGeometry).ToList(),
                                              mapDocument.ActiveView,
                                              featureClassObjTemFeatureClassName, buffer);
                    }
                    else
                    {
                        return null;
                    }

            }

            return null;
        }

        internal Dictionary<int, bool> GetAllIds()
        {
            var pointsIds = new Dictionary<int, bool>();
            foreach (var point in _observationPoints)
            {
                pointsIds.Add(point.Objectid, false);
            }

            return pointsIds;
        }

        #region Private methods

        private ObservationPoint GetObservationPointFromGdb()
        {
            if (!IsObservPointsExists())
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("ObservPointscModuleDoesnotExistMessage", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.MessageBoxCaption);
                log.WarnEx($"> GetObservationPointFromGdb Exception: {LocalizationContext.Instance.FindLocalizedElement("ObservPointscModuleDoesnotExistMessage", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            if (_observationPoints.Count == 0)
            {
                UpdateObservationPointsList();
            }

            var observPointsModel = _observationPoints.Where(point => point.X.HasValue && point.Y.HasValue).Select(point =>
            {
                return new FromLayerPointModel
                {
                    Point = new Point { X = point.X.Value, Y = point.Y.Value, SpatialReference = EsriTools.Wgs84Spatialreference },
                    ObjId = point.Objectid,
                    DisplayedField = point.Title
                };

            }).ToList();

            var observPointsListModal = new ObservationPointsListModalWindow(observPointsModel);
            var result = observPointsListModal.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (observPointsListModal.SelectedPoint != null)
                {
                    return GetObservationPointFromInterface(
                            _observationPoints.FirstOrDefault(
                                    point => point.Objectid == observPointsListModal.SelectedPoint.ObjId));
                }
            }

            return null;
        }

        private ObservationPoint GetObservationPointFromGeoCalc()
        {
            var geoCalcPoints = GetPointsFromGeoCalculator();

            if (geoCalcPoints == null)
            {
                return null;
            }

            var geoCalcPointsListModal = new PointsListModalWindow(geoCalcPoints);
            var result = geoCalcPointsListModal.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (geoCalcPointsListModal.SelectedPoint != null)
                {
                    return new ObservationPoint
                    {
                        X = geoCalcPointsListModal.SelectedPoint.Point.X,
                        Y = geoCalcPointsListModal.SelectedPoint.Point.Y,
                        Title = geoCalcPointsListModal.SelectedPoint.DisplayedField,
                        AzimuthStart = 0,
                        AzimuthEnd = 360,
                        AngelMinH = -90,
                        AngelMaxH = 90,
                        RelativeHeight = 0
                    };
                }
            }

            return null;
        }

        private ObservationPoint GetObservationPointFromPointLayer()
        {
            var manager = new MapLayersManager(mapDocument.ActiveView);

            var fromLayerPointsListModal =
                    new PointsFromLayerModalWindow(
                                                   ArcMap.Document.ActiveView,
                                                   manager.GetObservPointsAppropriateLayers()
                                                          .Where(layer => !layer.EndsWith(GetObservPointsFromGdbFeatureClassName()))
                                                          .ToArray(),
                                                   true);

            var result = fromLayerPointsListModal.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (fromLayerPointsListModal.SelectedPoint != null)
                {
                    var pointsFromLayer = VisibilityManager.GetObservationPointsFromAppropriateLayer(fromLayerPointsListModal.LayerName, ArcMap.Document.ActiveView, fromLayerPointsListModal.TitleField);
                    var observPoint = pointsFromLayer.FirstOrDefault(point => point.Objectid == fromLayerPointsListModal.SelectedPoint.ObjId);

                    return GetObservationPointFromInterface(observPoint);
                }
            }

            return null;
        }

        private KeyValuePair<int, IGeometry> GetObservationStationFromGdb()
        {
            var fromGdbObservStationListModal = new ObservObjForFunModalWindow(GetObservObjectsFromModule(), false);
            var result = fromGdbObservStationListModal.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (fromGdbObservStationListModal.SelectedGeometries != null)
                {
                    return fromGdbObservStationListModal.SelectedGeometries.First();
                }
            }

            return new KeyValuePair<int, IGeometry>(-1, null);
        }

        private IGeometry GetObservationStationFromGeoCalc()
        {
            var points = GetPointsFromGeoCalculator();
            List<IGeometry> selectedPointsGeoms = null;

            if (points == null || points.Count == 0)
            {
                return null;
            }

            var calcPointsModal = new CalcPointsForFunToPointsModalWindow(points);
            var result = calcPointsModal.ShowDialog();

            if (result == DialogResult.OK)
            {
                selectedPointsGeoms = calcPointsModal.SelectedPoints;
            }

            if (selectedPointsGeoms == null || !selectedPointsGeoms.Any())
            {
                return null;
            }

            if (selectedPointsGeoms.Count > 1)
            {
                var selectedPoints = selectedPointsGeoms.Select(point => point as IPoint).ToArray();
                var polyline = EsriTools.CreatePolylineFromPointsArray(selectedPoints, ArcMap.Document.FocusMap.SpatialReference);

                return polyline.First();
            }
            else
            {
                return selectedPointsGeoms.First();
            }
        }

        private KeyValuePair<IGeometry, string> GetObservationStationFromFeatureLayer()
        {
            var geometryFromFeatureLayerModal = new GeometryFromFeatureLayerModalWindow(ArcMap.Document.ActiveView, false, true);
            var result = geometryFromFeatureLayerModal.ShowDialog();

            if (result == DialogResult.OK)
            {
                var geometry = geometryFromFeatureLayerModal.SelectedGeometry;
                var selectedObjTitle = geometryFromFeatureLayerModal.SelectedGeometryTitle;

                return new KeyValuePair<IGeometry, string>(geometry, selectedObjTitle);
            }

            return new KeyValuePair<IGeometry, string>();
        }

        private GraphicsLayerManager GraphicsLayerManager
        {
            get
            {
                if (_graphicsLayerManager == null)
                {
                    _graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView);
                }

                return _graphicsLayerManager;
            }
        }


        private Dictionary<int, IPoint> GetPointsFromGeoCalculator()
        {
            Dictionary<int, IPoint> points;

            var geoModule = ModuleInteraction.Instance.GetModuleInteraction<IGeocalculatorInteraction>(out bool changes);

            if (!changes && geoModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("GeoCalcModuleDoesnotExistMessage", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.ErrorMessage);
                log.ErrorEx($"> GetPointFromGeoCalculator Exception: {LocalizationContext.Instance.FindLocalizedElement("GeoCalcModuleDoesnotExistMessage", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                points = geoModule.GetPoints();
            }
            catch (Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorMessage, LocalizationContext.Instance.MsgBoxErrorHeader);
                log.ErrorEx($"> GetPointFromGeoCalculator Exception: {ex.Message}");
                return null;
            }

            return points;
        }


        private List<IObserverPoint> GetObserverPointsFromGeoCalculator(bool subscribeOnEvents)
        {
            var geoModule = ModuleInteraction.Instance.GetModuleInteraction<IGeocalculatorInteraction>(out bool changes);

            if (!changes && geoModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("GeoCalcModuleDoesnotExistMessage", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.ErrorMessage);
                log.ErrorEx($"> GetPointFromGeoCalculator Exception: {LocalizationContext.Instance.FindLocalizedElement("GeoCalcModuleDoesnotExistMessage", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                if (subscribeOnEvents)
                {
                    geoModule.OnPointDeleted += RemoveGeoCalcPointAsObserverPoint;
                    geoModule.OnPointUpdated += UpdateGeoCalcPoints;
                }

                return geoModule.GetGeoCalcPoints()
                                .Select(point => SetDefaultValuesToGeoCalcPoint(point as GeoCalcPoint))
                                .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorMessage, LocalizationContext.Instance.MsgBoxErrorHeader);
                log.ErrorEx($"> GetPointFromGeoCalculator Exception: {ex.Message}");
                return null;
            }
        }

        private void UnsubscribeFromDeletePointEvent()
        {
            var geoModule = ModuleInteraction.Instance.GetModuleInteraction<IGeocalculatorInteraction>(out bool changes);

            if (!changes && geoModule == null)
            {
                log.ErrorEx($"> UnsubscribeFromDeletePointEvent Exception: {LocalizationContext.Instance.FindLocalizedElement("GeoCalcModuleDoesnotExistMessage", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return;
            }

            try
            {
                geoModule.OnPointDeleted -= RemoveGeoCalcPointAsObserverPoint;
                geoModule.OnPointUpdated -= UpdateGeoCalcPoints;
            }
            catch (Exception ex)
            {
                log.ErrorEx($"> UnsubscribeFromDeletePointEvent Exception: {ex.Message}");
                return;
            }
        }

        private void UpdateGeoCalcPoints()
        {
            GetObserverPointsFromSelectedSource(ObservationSetsEnum.GeoCalculator, false);
        }

        private List<IObserverPoint> GetObservationPointsFromPointLayer(bool changeFeatureLayer)
        {
            IFeatureLayer featureLayer;
            string selectedField;
            var manager = new MapLayersManager(mapDocument.ActiveView);

            if (changeFeatureLayer)
            {
                var observPointsLayerName = VisibilityManager.ObservationPointsFeatureLayer;
                var layers = manager.PointLayers
                                    .Where(pointLayer => !pointLayer.Name.Equals(observPointsLayerName))
                                    .Select(pointLayer => pointLayer.Name);

                var chooseLayerFromMapModal = new ChooseVectorLayerFromMapModalWindow(mapDocument.ActiveView, true, layers.ToArray());
                var result = chooseLayerFromMapModal.ShowDialog();

                if (result != DialogResult.OK)
                {
                    return null;
                }

                var layer = manager.GetLayer(chooseLayerFromMapModal.SelectedLayer);
                featureLayer = layer as IFeatureLayer;
                selectedField = chooseLayerFromMapModal.SelectedFiled;

                _observPointFromFeatureLayerData =
                        new KeyValuePair<string, string>(featureLayer.Name, selectedField);

                _observaitonPointsLayerName = chooseLayerFromMapModal.SelectedLayer;

            }
            else
            {
                featureLayer = GetFeatureLayer(_observPointFromFeatureLayerData.Key);

                if (featureLayer == null)
                {
                    return null;
                }

                selectedField = _observPointFromFeatureLayerData.Value;
            }

            if (!manager.HasFeatureClassObserverPointFields(featureLayer.FeatureClass))
            {
                var dialogResult = MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("AddNecessaryFieldsMessage",
                                                        "Неможливо використовувати даний шар, відсутні необхідні поля. \nВи бажаєте додати поля?"),
                                                     LocalizationContext.Instance.MessageBoxCaption,
                                                     MessageBoxButtons.OKCancel);

                if (dialogResult == DialogResult.OK)
                {
                    GdbAccess.Instance.AddObserverPointFields(featureLayer.FeatureClass);
                }
                else
                {
                    return null;
                }
            }

            return VisibilityManager.GetObservationPointsFromAppropriateLayer(
                                                    featureLayer.Name, mapDocument.ActiveView,
                                                    selectedField).Select(point => { return SetDefaultValuesToObserverPoint(point as ObservationPoint); }).ToList();
        }

        private void UpdateObserverPointsFromGeoCalculator(IObserverPoint observationPoint)
        {
            var geoModule = ModuleInteraction.Instance.GetModuleInteraction<IGeocalculatorInteraction>(out bool changes);

            if (!changes && geoModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement(
                                        "GeoCalcModuleDoesnotExistMessage",
                                        "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"),
                                LocalizationContext.Instance.ErrorMessage);

                log.ErrorEx($"> UpdateObserverPointsFromGeoCalculator Exception: " +
                            $"{LocalizationContext.Instance.FindLocalizedElement("GeoCalcModuleDoesnotExistMessage", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return;
            }

            try
            {
                geoModule.UpdateGeoCalcPoint(observationPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorMessage, LocalizationContext.Instance.MsgBoxErrorHeader);
                log.ErrorEx($"> UpdateObserverPointsFromGeoCalculator Exception: {ex.Message}");
                return;
            }
        }

        private IFeatureLayer GetFeatureLayer(string layerName)
        {
            var layer = EsriTools.GetLayer(layerName, mapDocument.FocusMap);

            if (layer is IFeatureLayer)
            {
                return layer as IFeatureLayer;
            }
            else
            {
                if (layerName == null)
                {
                    layerName = string.Empty;
                }

                MessageBox.Show(String.Format(
                                    LocalizationContext.Instance.FindLocalizedElement(
                                        "LayerIsNotFeatureLayerError",
                                        "Помилка під час зчитування даних шару. Шар {0} не є векторним"),
                                    layerName),
                                LocalizationContext.Instance.MessageBoxCaption);
                return null;
            }
        }

        private bool CheckAllPointsHaveSameFields()
        {
            var standardPoint = _observationPoints.First();

            var standardPointGeometry = GetObserverPointGeometry(standardPoint);
            var nextPointGeometry = GetObserverPointGeometry(_observationPoints[1]);

            var standardPointBaseAzimuths = FindBaseAzimuthFromRelativeToDirection(standardPointGeometry,
                                                                                   nextPointGeometry,
                                                                                   standardPoint.AzimuthStart.Value,
                                                                                   standardPoint.AzimuthEnd.Value);

            var standardBaseStartAzimuth = standardPointBaseAzimuths.StartAzimuth;
            var standardBaseEndAzimuth = standardPointBaseAzimuths.EndAzimuth;

            for (int i = 1; i < _observationPoints.Count; i++)
            {
                IPoint currentPointGeometry;

                if (i != _observationPoints.Count - 1)
                {
                    currentPointGeometry = nextPointGeometry.Clone();
                    nextPointGeometry = GetObserverPointGeometry(_observationPoints[i + 1]);
                }
                else
                {
                    currentPointGeometry = GetObserverPointGeometry(_observationPoints[i - 1]);
                }

                var currentPointBaseAzimuths = FindBaseAzimuthFromRelativeToDirection(currentPointGeometry,
                                                                                      nextPointGeometry,
                                                                                      _observationPoints[i].AzimuthStart.Value,
                                                                                      _observationPoints[i].AzimuthEnd.Value);

                var currentPointBaseStartAzimuth = currentPointBaseAzimuths.StartAzimuth;
                var currentPointBaseEndAzimuth = currentPointBaseAzimuths.EndAzimuth;

                if (_observationPoints[i].AngelMaxH != standardPoint.AngelMaxH
                    || _observationPoints[i].AngelMinH != standardPoint.AngelMinH
                    || Math.Abs(currentPointBaseEndAzimuth - standardBaseEndAzimuth) > 0.00001
                    || Math.Abs(currentPointBaseStartAzimuth - standardBaseStartAzimuth) > 0.00001
                    || _observationPoints[i].InnerRadius != standardPoint.InnerRadius
                    || _observationPoints[i].OuterRadius != standardPoint.OuterRadius)
                {
                    return false;
                }
            }

            return true;
        }

        private IObserverPoint SetDefaultValuesToGeoCalcPoint(GeoCalcPoint geoCalcPoint)
        {
            if (geoCalcPoint == null)
            {
                return null;
            }

            Func<double?, bool> isNullOrNaN = (value) => !value.HasValue || double.IsNaN(value.Value);

            geoCalcPoint.AngelMaxH = isNullOrNaN(geoCalcPoint.AngelMaxH) ? 90 : geoCalcPoint.AngelMaxH;
            geoCalcPoint.AngelMinH = isNullOrNaN(geoCalcPoint.AngelMinH) ? -90 : geoCalcPoint.AngelMinH;
            geoCalcPoint.AzimuthStart = isNullOrNaN(geoCalcPoint.AzimuthStart) ? 0 : geoCalcPoint.AzimuthStart;
            geoCalcPoint.AzimuthEnd = isNullOrNaN(geoCalcPoint.AzimuthEnd) ? 360 : geoCalcPoint.AzimuthEnd;
            geoCalcPoint.InnerRadius = isNullOrNaN(geoCalcPoint.InnerRadius) ? 0 : geoCalcPoint.InnerRadius;
            geoCalcPoint.OuterRadius = isNullOrNaN(geoCalcPoint.OuterRadius) ? 1000 : geoCalcPoint.OuterRadius;
            geoCalcPoint.RelativeHeight = isNullOrNaN(geoCalcPoint.RelativeHeight) ? 0 : geoCalcPoint.RelativeHeight;

            return geoCalcPoint;
        }

        private IObserverPoint SetDefaultValuesToObserverPoint(ObservationPoint observerPoint)
        {
            return new ObservationPoint
            {
                Objectid = observerPoint.Objectid,
                Affiliation = ObservationPointTypesEnum.All.ToString(),
                Type = ObservationPointMobilityTypesEnum.All.ToString(),
                Title = observerPoint.Title,
                X = observerPoint.X,
                Y = observerPoint.Y,
                AngelMaxH = observerPoint.AngelMaxH ?? 90,
                AngelMinH = observerPoint.AngelMinH ?? -90,
                AzimuthStart = observerPoint.AzimuthStart ?? 0,
                AzimuthEnd = observerPoint.AzimuthEnd ?? 360,
                RelativeHeight = observerPoint.RelativeHeight ?? 0,
                InnerRadius = observerPoint.InnerRadius ?? 0,
                OuterRadius = observerPoint.OuterRadius ?? 1000,
                Dto = observerPoint.Dto ?? DateTime.Now,
                Operator = string.IsNullOrEmpty(observerPoint.Operator) ? Environment.UserName : observerPoint.Operator
            };
        }


        private IFeatureClass CreateTempPointsFeatureClass(IRaster raster, List<IObserverPoint> observerPoints,
                                                            string featureClassName)
        {
            var observPointsFeatureClass = GetObservatioPointFeatureClass(mapDocument.ActiveView);

            var observPointsFromGeoCalcTemporaryFeatureClass =
                        GdbAccess.Instance.GenerateTemporaryFeatureClassWithRequitedFields(observPointsFeatureClass.Fields,
                                                                                          featureClassName, true);

            try
            {
                foreach (var point in observerPoints)
                {
                    var observerPointGeometry = new PointClass
                    {
                        X = point.X.Value,
                        Y = point.Y.Value,
                        SpatialReference = EsriTools.Wgs84Spatialreference
                    };

                    MapLayersManager layersManager = new MapLayersManager(mapDocument.ActiveView);

                    if (raster != null)
                    {
                        observerPointGeometry.ZAware = true;
                        observerPointGeometry.AddZCoordinate(raster);
                    }

                    // observerPointGeometry.Project(mapDocument.FocusMap.SpatialReference);

                    GdbAccess.Instance.AddObservPoint(observerPointGeometry, GetObservationPointFromInterface(point),
                                                      observPointsFromGeoCalcTemporaryFeatureClass);
                }
            }
            catch (Exception ex)
            {
                log.ErrorEx($"> CreateTempPointsFeatureClass Unexpected exception: {ex.Message}");
            }

            return observPointsFromGeoCalcTemporaryFeatureClass;
        }

        public IFeatureClass CreateOOFeatureClass(List<IGeometry> geometries,
                                                         IActiveView activeView, string storageName,
                                                         int buffer)
        {
            var featureClass = GdbAccess.Instance.GenerateTempStorage(storageName, null, esriGeometryType.esriGeometryPolygon,
                                                                        activeView, false, true);

            foreach (var geometry in geometries)
            {
                geometry.Project(ArcMap.Document.FocusMap.SpatialReference);
                var geometryWithBuffer = CalculateGeometryWithBuffer(geometry, buffer);
                GdbAccess.Instance.AddGeometryToFeatureClass(geometryWithBuffer, featureClass);
            }

            return featureClass;
        }

        #endregion

        #region ArcMap Eventts

        internal bool IsArcMapEditingStarted()
        {
            return ArcMap.Editor.EditState == esriEditState.esriStateEditing;
        }

        EventListener eventListener;

        public void UnsubscribeForEvents()
        {
            if (eventListener != null)
            {
                eventListener.Changed -= EventListenerChanged;

                eventListener.ListenToEvents(EventListener.EditorEventEnum.OnStartEditing, false);
                eventListener.ListenToEvents(EventListener.EditorEventEnum.OnStopEditing, false);
                eventListener.ListenToEvents(EventListener.EditorEventEnum.OnCreateFeature, false);
                eventListener.ListenToEvents(EventListener.EditorEventEnum.OnChangeFeature, false);
                eventListener = null;
            }
        }

        public void SubscribeForEvents()
        {
            eventListener = new EventListener(ArcMap.Editor);

            eventListener.ListenToEvents(EventListener.EditorEventEnum.OnStartEditing, true);
            eventListener.ListenToEvents(EventListener.EditorEventEnum.OnStopEditing, true);
            eventListener.ListenToEvents(EventListener.EditorEventEnum.OnCreateFeature, true);
            eventListener.ListenToEvents(EventListener.EditorEventEnum.OnChangeFeature, true);

            eventListener.Changed += EventListenerChanged;
        }

        private void EventListenerChanged(object sender, EditorEventArgs e)
        {
            if (e.EventType == EventListener.EditorEventEnum.OnCreateFeature)
            {
                //_observPointsController.OnCreateFeature
                if (e.Object != null)
                {
                    OnCreateOrChangeFeature(e.Object);
                }
            }
            if (e.EventType == EventListener.EditorEventEnum.OnChangeFeature)
            {
                //_observPointsController.OnCreateFeature
                if (e.Object != null)
                {
                    OnCreateOrChangeFeature(e.Object, false);
                }
            }
            if (e.EventType == EventListener.EditorEventEnum.OnStopEditing)
            {
                bool saveEdits = false;
                if (e.SaveEdits.HasValue)
                {
                    saveEdits = e.SaveEdits.Value;
                }
                OnStopEditing(saveEdits);
            }
        }

        public void OnStartEditing()
        {
            log.InfoEx("> OnStartEditing Event");
        }

        internal void OnStopEditing(bool save)
        {
            log.InfoEx("> OnStopEditing Event START");
            if (save)
            {
                UpdateObservationPointsList();
            }
            log.InfoEx("> OnStopEditing Event END");
        }




        //internal void OnDeleteFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        //{
        //    log.InfoEx("> OnDeleteFeature Event");
        //}

        private void OnCreateOrChangeFeature(ESRI.ArcGIS.Geodatabase.IObject obj, bool addOrChange = true)
        {
            log.InfoEx("> OnCreateFeature Event START");

            if (obj.Class is IFeatureClass fcl)
            {
                if (fcl == GetObservatioPointFeatureClass())
                {
                    IGeometry g = fcl.GetFeature(obj.OID).ShapeCopy;
                    IPoint p = g as IPoint;
                    p.Project(EsriTools.Wgs84Spatialreference);

                    var fldXWgs = obj.Fields.FindField("XWGS");
                    var fldYWgs = obj.Fields.FindField("YWGS");

                    obj.Value[fldXWgs] = p.X;
                    obj.Value[fldYWgs] = p.Y;
                    if (addOrChange)
                    {
                        var fldTitleIndex = obj.Fields.FindField("TitleOP");
                        var fldDtoIndex = obj.Fields.FindField("DTO");
                        var fldIdOP = obj.Fields.FindField("idOP");
                        var fldHRel = obj.Fields.FindField("HRel");

                        var sidOP = $"OP{DataAccess.Helper.GetTemporaryNameSuffix()}";
                        obj.Value[fldIdOP] = sidOP;
                        obj.Value[fldTitleIndex] = LocalizationContext.Instance.DeafultObservationpointTitle.InvariantFormat(obj.OID);
                        obj.Value[fldHRel] = 2500;

                        log.InfoEx("OnCreateFeature. New observation point was added wiht objectid:{0} idOP:{1} Title {2}",
                            obj.OID, sidOP, LocalizationContext.Instance.DeafultObservationpointTitle.InvariantFormat(obj.OID));
                    }
                    else
                    {
                        log.InfoEx($"OnChaneFeature. New observation point was added wiht objectid:{obj.OID} ");
                    }
                }
            }
            log.InfoEx("> OnCreateFeature Event END");
        }
        #endregion

    }
}
