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

namespace MilSpace.Visibility.ViewController.WizardController
{
       internal class WizardViewController
    {
        IWizardView view;
        private static readonly string _observPointFeature = "MilSp_Visible_ObservPoints";
        private static readonly string _observStationFeature = "MilSp_Visible_ObjectsObservation_R";
        private IEnumerable<ObservationPoint> _observationPoints;
        private IEnumerable<ObservationObject> _observationObjects;
        private static bool localized = false;
        private string _previousPickedRasterLayer { get; set; }

        /// <summary>
        /// The dictionary to localise the types
        /// </summary>
        private static Dictionary<ObservationPointMobilityTypesEnum, string> _mobilityTypes = Enum.GetValues(typeof(ObservationPointMobilityTypesEnum)).Cast<ObservationPointMobilityTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<ObservationPointTypesEnum, string> _affiliationTypes = Enum.GetValues(typeof(ObservationPointTypesEnum)).Cast<ObservationPointTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<ObservationObjectTypesEnum, string> _observObjectsTypes = Enum.GetValues(typeof(ObservationObjectTypesEnum)).Cast<ObservationObjectTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<LayerPositionsEnum, string> _layerPositions = Enum.GetValues(typeof(LayerPositionsEnum)).Cast<LayerPositionsEnum>().ToDictionary(t => t, ts => ts.ToString());

        private IMxDocument mapDocument;
        private static Logger log = Logger.GetLoggerEx("WizardViewController");

        public WizardViewController(IMxDocument mapDocument)
        {
            this.mapDocument = mapDocument;
            LocalizeDictionaries();
        }

        private static void LocalizeDictionaries()
        {
            if (!localized)
            {
                _layerPositions[LayerPositionsEnum.Above] = LocalizationContext.Instance.PlaceLayerAbove;
                _layerPositions[LayerPositionsEnum.Below] = LocalizationContext.Instance.PlaceLayerBelow;
                localized = true;
            }
        }

        internal void SetView(IWizardView view)
        {
            this.view = view;
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
            view.FillObservationPointList(_observationPoints);
        }
        internal void UpdateObservObjectsList(bool isCurrent)
        {
            _observationObjects = GetAllObservObjects();
            view.FillObsObj( _observationObjects, isCurrent);
        }
        internal void UpdateObservationPointsList_OnCurrentExtend(IActiveView ActiveView)
        {
            _observationPoints = GetObservPointsOnCurrentMapExtent(ActiveView);
            view.FillObservPointsOnCurrentView(_observationPoints);
        }

        internal ObservationPoint GetObservPointById(int id)
        {
            return _observationPoints.FirstOrDefault(point => point.Objectid == id);
        }
        
        internal IEnumerable<ObservationPoint> GetAllObservationPoints()
        {
            return VisibilityZonesFacade.GetAllObservationPoints();
        }
        
        
        // TODO: Define the field in the View Interface to take sessionName, rasterLayerName and  visibilityCalculationResults
       

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
        
      
        public IEnumerable<string> GetObservationPointTypes()
        {
            return _affiliationTypes.Where(t => t.Key != ObservationPointTypesEnum.All).Select(t => t.Value);
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

        public IFeatureClass GetObservatioPointFeatureClass(IActiveView esriView)
        {
            return GetFeatureClass(view.ObservationPointsFeatureClass, esriView);
        }
        public IFeatureClass GetObservatioStationFeatureClass(IActiveView esriView)
        {
            return GetFeatureClass(_observStationFeature, esriView);
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

        
        private bool IsPointOnExtent(IEnvelope envelope, IPoint point)
        {
            if (point.X >= envelope.XMin && point.X <= envelope.XMax && point.Y >= envelope.YMin && point.Y <= envelope.YMax)
            {
                return true;
            }

            return false;
        }
    
        
    }
}

