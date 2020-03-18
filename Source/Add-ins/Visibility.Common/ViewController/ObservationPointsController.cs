﻿using ESRI.ArcGIS.ArcMapUI;
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

namespace MilSpace.Visibility.ViewController
{
    public class ObservationPointsController
    {
        IObservationPointsView view;
        private static readonly string _observPointFeature = "MilSp_Visible_ObservPoints";
        private static readonly string _observStationFeature = "MilSp_Visible_ObjectsObservation_R";
        private List<ObservationPoint> _observationPoints = new List<ObservationPoint>();
        private List<ObservationObject> _observationObjects = new List<ObservationObject>();
        private List<ObservationStationToObservPointRelationModel> _relationLines = new List<ObservationStationToObservPointRelationModel>();
        private static bool localized = false;
        private IPolygon _coverageArea;
        private string _previousPickedRasterLayer { get; set; }

        /// <summary>
        /// The dictionary to localise the types
        /// </summary>
        private static Dictionary<ObservationPointMobilityTypesEnum, string> _mobilityTypes = null;// = Enum.GetValues(typeof(ObservationPointMobilityTypesEnum)).Cast<ObservationPointMobilityTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<ObservationPointTypesEnum, string> _affiliationTypes = null;//Enum.GetValues(typeof(ObservationPointTypesEnum)).Cast<ObservationPointTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<string, ObservationObjectTypesEnum> _observObjectsTypesToConvert = Enum.GetValues(typeof(ObservationObjectTypesEnum)).Cast<ObservationObjectTypesEnum>().ToDictionary(ts => ts.ToString(), t => t);
        private static Dictionary<ObservationObjectTypesEnum, string> _observObjectsTypes = null; //Enum.GetValues(typeof(ObservationObjectTypesEnum)).Cast<ObservationObjectTypesEnum>().ToDictionary(ts => ts.ToString(), t => t);
        private static Dictionary<LayerPositionsEnum, string> _layerPositions = Enum.GetValues(typeof(LayerPositionsEnum)).Cast<LayerPositionsEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<ObservationSetsEnum, string> _observPointSets = new Dictionary<ObservationSetsEnum, string>
        {
            {ObservationSetsEnum.Gdb, LocalizationContext.Instance.ObservPointsSet },
            {ObservationSetsEnum.GeoCalculator, LocalizationContext.Instance.GeoCalcSet }
        };

        private static Dictionary<ObservationSetsEnum, string> _observObjectsSets = new Dictionary<ObservationSetsEnum, string>
        {
            {ObservationSetsEnum.Gdb, LocalizationContext.Instance.ObservObjectsSet },
            {ObservationSetsEnum.GeoCalculator, LocalizationContext.Instance.GeoCalcSet },
            {ObservationSetsEnum.FeatureLayers, LocalizationContext.Instance.FeatureLayerSet }
        };

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

        internal void SetView(IObservationPointsView view)
        {
            this.view = view;
        }

        internal void SetGrahicsLayerManager()
        {
            _graphicsLayerManager = new GraphicsLayerManager(mapDocument.ActiveView);
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

            internal void UpdateObservPoint(ObservationPoint newPoint, string featureName, IActiveView activeView, int objId)
        {
            var isCoordChanges = false;
            var oldPoint = GetObservPointById(objId);

            var featureClass = GetFeatureClass(featureName, activeView);
            PointClass pointGeometry;

            if (oldPoint.X != newPoint.X && oldPoint.Y != newPoint.Y)
            {
                pointGeometry = new PointClass
                {
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

            if(!IsPointOnExtent(activeView.Extent, pointGeometry))
            {
                EsriTools.PanToGeometry(activeView, pointGeometry, true);

            }
            EsriTools.FlashGeometry(pointGeometry, 500, ArcMap.Application);
        }

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
                view.AddRecord(_observationPoints.Last());

                log.DebugEx("> AddPoint END");

            }
            catch (Exception ex)
            {
                log.DebugEx("> AddPoint Exception:{0}", ex.Message);

            }
        }

        // TODO: Define the field in the View Interface to take sessionName, rasterLayerName and  visibilityCalculationResults
        internal bool CalculateVisibility(WizardResult calcParams)
        {
            log.InfoEx("> CalculateVisibility START");

            var statusBar = ArcMap.Application.StatusBar;
            var animationProgressor = statusBar.ProgressAnimation;
            int exx = 1;

            try
            {
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
                var observPoints = GetObservatioPointFeatureClass(mapDocument.ActiveView);
                exx++;
                var observObjects = GetObservatioStationFeatureClass(mapDocument.ActiveView);
                exx++;
                if (calcParams.ObservPointIDs == null) // Get points forn the current extent
                {
                    calcParams.ObservPointIDs = EsriTools.GetSelectionByExtent(observPoints, mapDocument.ActiveView);
                }
                if (calcParams.ObservObjectIDs == null) // Get points forn the current extent
                {
                    calcParams.ObservObjectIDs = EsriTools.GetSelectionByExtent(observObjects, mapDocument.ActiveView);
                }
                exx++;
                animationProgressor.Show();
                animationProgressor.Play(0, 200);

                exx++;

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
                    mapDocument.ActiveView.FocusMap);

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

        internal List<FromLayerPointModel> GetObservationPointsFromModule()
        {
            if(!IsObservPointsExists())
            {
                return null;
            }

            var points = new List<FromLayerPointModel>();

            var featureClass = GetObservatioPointFeatureClass();
            var idFieldIndex = featureClass.FindField("OBJECTID");
            var titleFieldIndex = featureClass.FindField("TitleOp");

            if(idFieldIndex == -1)
            {
                log.WarnEx($"> GetObservationPointsFromModule. Warning: Cannot find fild \"OBJECTID\" in featureClass {featureClass.AliasName}");
                throw new MissingFieldException();
            }

            if(titleFieldIndex == -1)
            {
                log.WarnEx($"> GetObservationPointsFromModule. Warning: Cannot find fildTitleOp in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = "OBJECTID > 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while(feature != null)
                {
                    var shape = feature.ShapeCopy;

                    var point = shape as IPoint;
                    var pointCopy = point.Clone();
                    //pointCopy.Project(EsriTools.Wgs84Spatialreference);

                    int id = -1;
                    string titleField = string.Empty;

                    if(idFieldIndex >= 0)
                    {
                        id = (int)feature.Value[idFieldIndex];
                    }

                    if(titleFieldIndex >= 0)
                    {
                        titleField = feature.Value[titleFieldIndex].ToString();
                    }

                    points.Add(new FromLayerPointModel { Point = pointCopy, ObjId = id, DisplayedField = titleField });

                    feature = featureCursor.NextFeature();
                }
            }
            catch(Exception ex)
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
            if(!IsObservObjectsExists())
            {
                return null;
            }

            var objects = new List<FromLayerGeometry>();

            var featureClass = GetObservatioStationFeatureClass(mapDocument.ActiveView);
            var idFieldIndex = featureClass.FindField("OBJECTID");
            var titleFieldIndex = featureClass.FindField("sTitleOO");

            if(idFieldIndex == -1)
            {
                log.WarnEx($"> GetObservObjectsFromModule. Warning: Cannot find fild \"OBJECTID\" in featureClass {featureClass.AliasName}");
                throw new MissingFieldException();
            }

            if(titleFieldIndex == -1)
            {
                log.WarnEx($"> GetObservObjectsFromModule. Warning: Cannot find fild sTitleOO in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = "OBJECTID > 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while(feature != null)
                {
                    var shape = feature.ShapeCopy;

                    int id = -1;
                    string titleField = string.Empty;

                    if(idFieldIndex >= 0)
                    {
                        id = (int)feature.Value[idFieldIndex];
                    }

                    if(titleFieldIndex >= 0)
                    {
                        titleField = feature.Value[titleFieldIndex].ToString();
                    }

                    objects.Add(new FromLayerGeometry { Geometry = shape, ObjId = id, Title = titleField });

                    feature = featureCursor.NextFeature();
                }
            }
            catch(Exception ex)
            {
                log.ErrorEx($"> GetObservObjectsFromModule Exception. ex.Message:{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }

            return objects;
        }

        internal void DrawObservPointsGraphics(int id)
        {
            var observPoint = _observationPoints.FirstOrDefault(point => point.Objectid == id);

            if(observPoint == null || observPoint.X == null || observPoint.Y == null)
            {
                return;
            }

            var pointGeom = new Point { X = observPoint.X.Value, Y = observPoint.Y.Value, SpatialReference = EsriTools.Wgs84Spatialreference };
            pointGeom.Project(mapDocument.FocusMap.SpatialReference);

            var maxDistance = CalcCoverageArea(pointGeom, observPoint);
            _graphicsLayerManager.AddObservPointsGraphicsToMap(_coverageArea, $"coverageArea_{id}");
            _graphicsLayerManager.AddCrossPointerToPoint(pointGeom, Convert.ToInt32(maxDistance), $"crossPointer_coverageArea_{id}_");
        }

        internal double CalcCoverageArea(IPoint pointGeom, ObservationPoint observPoint)
        {
            var realMaxDistance = EsriTools.GetMaxDistance(observPoint.OuterRadius.Value, observPoint.AngelMaxH.Value, observPoint.RelativeHeight.Value);
            var realMinDistance = EsriTools.GetMinDistance(observPoint.InnerRadius.Value, observPoint.AngelMinH.Value, observPoint.RelativeHeight.Value);

            if(realMaxDistance < realMinDistance)
            {
                log.WarnEx("> DrawObservPointsGraphics. Observation point doesn`t has a coverage area");
                MessageBox.Show(LocalizationContext.Instance.CoverageAreaIsEmptyMessage, LocalizationContext.Instance.MessageBoxCaption);
                return observPoint.OuterRadius.Value;
            }

            _coverageArea = EsriTools.GetCoverageArea(pointGeom, observPoint.AzimuthStart.Value, observPoint.AzimuthEnd.Value,
                                                           realMinDistance, realMaxDistance);
            _coverageArea.SpatialReference = mapDocument.FocusMap.SpatialReference;
            return realMaxDistance;
        }

        internal void CalcRelationLines(int id, bool fromNewCoverageArea = false)
        {
            var observPoint = _observationPoints.FirstOrDefault(point => point.Objectid == id);

            if (observPoint == null || observPoint.X == null || observPoint.Y == null)
            {
                return;
            }

            var pointGeom = new Point { X = observPoint.X.Value, Y = observPoint.Y.Value, SpatialReference = EsriTools.Wgs84Spatialreference };
            pointGeom.Project(mapDocument.FocusMap.SpatialReference);

            var geometries = EsriTools.GetGeometriesFromLayer(VisibilityManager.ObservationStationsFeatureLayer, mapDocument.ActiveView);

            if (_coverageArea == null || fromNewCoverageArea)
            {
                CalcCoverageArea(pointGeom, observPoint);
            }

            _relationLines.Clear();

            foreach (var geometry in geometries)
            {
                var relationLine = EsriTools.GetToGeometryCenterPolyline(pointGeom, geometry.Value);
                var intersectionArea = EsriTools.GetIntersection(_coverageArea, geometry.Value);

                var simpleLine = new Line { FromPoint = relationLine.FromPoint, ToPoint = relationLine.ToPoint, SpatialReference = relationLine.SpatialReference };

                if (!_observationObjects.Any())
                {
                    UpdateObservObjectsList();
                }

                var currentObj = _observationObjects.FirstOrDefault(obj => obj.ObjectId == geometry.Key);
                var title = (currentObj == null) ? string.Empty : currentObj.Title;

                var line = new ObservationStationToObservPointRelationModel
                {
                    Id = geometry.Key,
                    Polyline = relationLine,
                    Azimuth = simpleLine.Azimuth(),
                    Title = title
                };

                if (intersectionArea.IsEmpty)
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


        internal void DrawObservPointToObservObjectsRelationsGraphics(int id)
        {
            if (_relationLines == null || !_relationLines.Any())
            {
                CalcRelationLines(id);
            }

            foreach(var line in _relationLines)
            {
                IRgbColor color;

                if(line.CoverageType == CoverageTypesEnum.None)
                {
                    color = new RgbColor { Red = 255, Blue = 0, Green = 0 };
                }
                else
                {
                    if(line.CoverageType == CoverageTypesEnum.Full)
                    {
                        color = new RgbColor { Red = 69, Blue = 0, Green = 230 };
                    }
                    else
                    {
                        color = new RgbColor { Red = 229, Blue = 1, Green = 167 };
                    }
                }
                
                _graphicsLayerManager.AddObservPointsRelationLineToMap(line.Polyline, color, $"relationLine_{id}", line.Title);
            }
        }

        internal void RemoveObservPointsGraphics(bool removeCoverageArea = true, bool removeObservObjectsRelations = true)
        {
            if(removeCoverageArea)
            {
                _graphicsLayerManager.RemoveAllGeometryFromMap($"coverageArea_", MilSpaceGraphicsTypeEnum.Visibility, true);
            }
            if(removeObservObjectsRelations)
            {
                _graphicsLayerManager.RemoveAllGeometryFromMap($"relationLine_", MilSpaceGraphicsTypeEnum.Visibility, true);
            }
        }

        internal string[] GetObservStationSetsStrings()
        {
            return _observObjectsSets.Select(set => set.Value).ToArray();
        }

        internal ObservationSetsEnum GetObservStationSet(string setString)
        {
            return _observObjectsSets.FirstOrDefault(set => set.Value == setString).Key;
        }

        internal ObservationStationToObservPointRelationModel[] GetObservationStationToObservPointRelations(int id, ObservationSetsEnum set)
        {
            switch (set)
            {
                case ObservationSetsEnum.Gdb:

                    if (_relationLines == null || !_relationLines.Any())
                    {
                        CalcRelationLines(id);
                    }

                    return _relationLines.ToArray();
            }

            return null;

        }

        #region ArcMap Eventts

        internal bool IsArcMapEditingStarted()
        {
            return ArcMap.Editor.EditState == esriEditState.esriStateEditing;
        }

        internal void OnStartEditing()
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

        internal void OnDeleteFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            log.InfoEx("> OnDeleteFeature Event");
            //     UpdateObservationPointsList();
        }

        internal void OnCreateFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            log.InfoEx("> OnCreateFeature Event START");

            if (obj.Class is IFeatureClass fcl)
            {
                if (fcl == GetObservatioPointFeatureClass())
                {
                    var fldTitleIndex = obj.Fields.FindField("TitleOP");
                    var fldDtoIndex = obj.Fields.FindField("DTO");
                    var fldXWgs = obj.Fields.FindField("XWGS");
                    var fldYWgs = obj.Fields.FindField("YWGS");
                    var fldIdOP = obj.Fields.FindField("idOP");
                    var fldHRel = obj.Fields.FindField("HRel");

                    IGeometry g = fcl.GetFeature(obj.OID).ShapeCopy;
                    IPoint p = g as IPoint;
                    p.Project(EsriTools.Wgs84Spatialreference);

                    var sidOP = $"OP{DataAccess.Helper.GetTemporaryNameSuffix()}";
                    obj.Value[fldIdOP] = sidOP;
                    obj.Value[fldTitleIndex] = LocalizationContext.Instance.DeafultObservationpointTitle.InvariantFormat(obj.OID);
                    obj.Value[fldXWgs] = p.X;
                    obj.Value[fldYWgs] = p.Y;
                    obj.Value[fldHRel] = 2500;

                    log.InfoEx("OnCreateFeature. New observation point was added wiht objectid:{0} idOP:{1} Title {2}",
                        obj.OID, sidOP, LocalizationContext.Instance.DeafultObservationpointTitle.InvariantFormat(obj.OID));
                }
            }
            log.InfoEx("> OnCreateFeature Event END");
        }
        #endregion

    }
}
