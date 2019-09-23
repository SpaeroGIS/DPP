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
        private static readonly string observPointFeature = "MilSp_Visible_ObservPoints";

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
            return observPointFeature;
        }

        internal void UpdateObservationPointsList()
        {
            view.FillObservationPointList(VisibilityZonesFacade.GetAllObservationPoints(), view.GetFilter);
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
                if(layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    obserPointsLayersNames.Add(layer.Name);
                }

                layer = layers.Next();
            }
            return obserPointsLayersNames;
        }

        public bool IsObservPointsExists(IActiveView view)
        {
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            while(layer != null)
            {
                if(layer is IFeatureLayer fl && fl.FeatureClass != null && fl.FeatureClass.AliasName.Equals(observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                layer = layers.Next();
            }

            return false;
        }

        public void AddPoint(ObservationPoint point, string featureName, IActiveView activeView)
        {
            var pointGeometry = new PointClass { X = (double)point.X, Y = (double)point.Y, SpatialReference = EsriTools.Wgs84Spatialreference };
            
            pointGeometry.Z = (double)point.RelativeHeight;
            pointGeometry.ZAware = true;

            var featureClass = GetFeatureClass(featureName, activeView);

            GdbAccess.Instance.AddObservPoint(pointGeometry, point, featureClass);
            UpdateObservationPointsList();
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
