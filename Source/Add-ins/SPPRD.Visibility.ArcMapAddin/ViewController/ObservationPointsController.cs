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
            var featureClass = GetFeatureClass(featureName, activeView);

            GdbAccess.Instance.UpdateObservPoint(featureClass, newPoint, objId);

            _observationPoints = VisibilityZonesFacade.GetAllObservationPoints().ToList();
            view.ChangeRecord(objId, newPoint);
        }

        internal IEnumerable<ObservationPoint> GetAllObservationPoints()
        {
            return VisibilityZonesFacade.GetAllObservationPoints();
        }

        public IEnumerable<string> GetObservationPointTypes()
        {
            return Enum.GetNames(typeof(ObservationPointTypesEnum));
        }

        public IEnumerable<string> GetObservationPointMobilityTypes()
        {
            return Enum.GetNames(typeof(ObservationPointMobilityTypesEnum));
        }

        public IEnumerable<string> GetObservationPointsLayers(IActiveView view)
        {
            var obserPointsLayersNames = new List<string>();
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            while(layer != null)
            {
                if(layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(_observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    obserPointsLayersNames.Add(layer.Name);
                }

                layer = layers.Next();
            }
            return obserPointsLayersNames;
        }

        public IFeatureClass GetObservatioStationFeatureClass(IActiveView view)
        {
            return GetFeatureClass(observPointFeature, view);
        }

        public bool IsObservPointsExists(IActiveView view)
        {
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            while(layer != null)
            {
                if(layer is IFeatureLayer fl && fl.FeatureClass != null && fl.FeatureClass.AliasName.Equals(_observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                layer = layers.Next();
            }

            return false;
        }

        public ObservationPoint CreatePointWithDefaultValues(IEnvelope envelope)
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

        public void AddPoint(string featureName, IActiveView activeView)
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

        public IPoint GetEnvelopeCenterPoint(IEnvelope envelope)
        {
            var x = (envelope.XMin + envelope.XMax) / 2;
            var y = (envelope.YMin + envelope.YMax) / 2;

            var point = new PointClass { X = x, Y = y, SpatialReference = envelope.SpatialReference };
            point.Project(EsriTools.Wgs84Spatialreference);
            return point;
        }

        private IFeatureClass GetFeatureClass(string featureClassName, IActiveView activeView)
        {
            var layers = activeView.FocusMap.Layers;
            var layer = layers.Next();

            while(layer != null)
            {
                if(layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(featureClassName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fl.FeatureClass;
                }

                layer = layers.Next();
            }

            return null;
        }
    }
}
