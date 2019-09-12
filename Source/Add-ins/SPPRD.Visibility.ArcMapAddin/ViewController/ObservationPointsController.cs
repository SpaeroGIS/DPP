﻿using ESRI.ArcGIS.Carto;
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

        public bool IsObservPointsExists(IActiveView view)
        {
            var layers = view.FocusMap.Layers;
            var layer = layers.Next();

            while(layer != null)
            {
                if(layer is IFeatureLayer fl && fl.FeatureClass.AliasName.Equals(observPointFeature, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                layer = layers.Next();
            }

            return false;
        }

        public void AddPoint(IPoint point, ObservationPoint pointArgs)
        {
            GdbAccess.Instance.AddObservPoint(point, observPointFeature, pointArgs);
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
    }
}
