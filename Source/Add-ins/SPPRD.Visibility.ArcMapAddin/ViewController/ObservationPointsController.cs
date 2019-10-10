using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.ViewController
{
    public class ObservationPointsController
    {
        IObservationPointsView view;
        private static readonly string _observPointFeature = "MilSp_Visible_ObservPoints";
        private List<ObservationPoint> _observationPoints = new List<ObservationPoint>();
        /// <summary>
        /// The dictionary to localise the types
        /// </summary>
        private static Dictionary<ObservationPointMobilityTypesEnum, string> mobilityTypes = Enum.GetValues(typeof(ObservationPointMobilityTypesEnum)).Cast<ObservationPointMobilityTypesEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<ObservationPointTypesEnum, string> affiliationTypes = Enum.GetValues(typeof(ObservationPointTypesEnum)).Cast<ObservationPointTypesEnum>().ToDictionary(t => t, ts => ts.ToString());

        public ObservationPointsController()
        { }

        internal void SetView(IObservationPointsView view)
        {
            this.view = view;
        }

        internal void OnCreateFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            throw new NotImplementedException();
        }

        internal string GetObservFeatureName()
        {
            return _observPointFeature;
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

        internal void UpdateObservPoint(ObservationPoint newPoint, string featureName, IActiveView activeView, int objId)
        {
            var isCoordChanges = false;
            var oldPoint = GetObservPointById(objId);
            var featureClass = GetFeatureClass(featureName, activeView);
            PointClass pointGeometry;

            if(oldPoint.X != newPoint.X && oldPoint.Y != newPoint.Y)
            {
                pointGeometry = new PointClass{ X = (double)newPoint.X, Y = (double)newPoint.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

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
            var pointGeometry = new PointClass { X = (double)point.X, Y = (double)point.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

            pointGeometry.Z = (double)point.RelativeHeight;
            pointGeometry.ZAware = true;

            var featureClass = GetFeatureClass(featureName, activeView);

            GdbAccess.Instance.AddObservPoint(pointGeometry, point, featureClass);

            var updPoints = VisibilityZonesFacade.GetAllObservationPoints().ToList();
            _observationPoints.Add(updPoints.First(observPoint => !_observationPoints.Exists(oldPoints => oldPoints.Objectid == observPoint.Objectid)));
            view.AddRecord(_observationPoints.Last());
        }

        internal IPoint GetEnvelopeCenterPoint(IEnvelope envelope)
        {
            var x = (envelope.XMin + envelope.XMax) / 2;
            var y = (envelope.YMin + envelope.YMax) / 2;

            var point = new PointClass { X = x, Y = y, SpatialReference = envelope.SpatialReference };
            point.Project(EsriTools.Wgs84Spatialreference);
            return point;
        }

        public IEnumerable<string> GetObservationPointTypes()
        {
            return affiliationTypes.Where(t => t.Key != ObservationPointTypesEnum.All).Select(t => t.Value);
        }

        public IEnumerable<string> GetObservationPointMobilityTypes()
        {
            return mobilityTypes.Where(t => t.Key != ObservationPointMobilityTypesEnum.All).Select(t => t.Value);
        }

        public IEnumerable<string> GetObservationPointsLayers(IActiveView view)
        {
            var obserPointsLayersNames = new List<string>();
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            while (layer != null)
            {
                if (layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(_observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    obserPointsLayersNames.Add(layer.Name);
                }

                layer = layers.Next();
            }
            return obserPointsLayersNames;
        }

        public IFeatureClass GetObservatioStationFeatureClass(IActiveView esriView)
        {
            return GetFeatureClass(view.ObservationPointsFeatureClass, esriView);
        }

        public bool IsObservPointsExists(IActiveView view)
        {
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            while (layer != null)
            {
                if (layer is IFeatureLayer fl && fl.FeatureClass != null && fl.FeatureClass.AliasName.Equals(_observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                layer = layers.Next();
            }

            return false;
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
                if (layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(featureClassName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fl.FeatureClass;
                }

                layer = layers.Next();
            }

            return null;
        }

        private IFeatureLayer GetFeatureLayer(string featureClassName, IActiveView activeView)
        {
            if(string.IsNullOrWhiteSpace(featureClassName))
            {
                return null;
            }

            var layers = activeView.FocusMap.Layers;
            var layer = layers.Next();

            while(layer != null)
            {
                if(layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(featureClassName, StringComparison.InvariantCultureIgnoreCase))
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
    }
}
