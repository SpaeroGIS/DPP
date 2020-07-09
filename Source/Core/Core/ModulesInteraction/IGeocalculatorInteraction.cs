using ESRI.ArcGIS.Geometry;
using MilSpace.Core.DataAccess;
using System;
using System.Collections.Generic;

namespace MilSpace.Core.ModulesInteraction
{
    public interface IGeocalculatorInteraction
    {
        event Action<int> OnPointDeleted;
        event Action OnPointUpdated;

        Dictionary<int, IPoint> GetPoints();
        IObserverPoint[] GetGeoCalcPoints();
        void UpdateGeoCalcPoint(IObserverPoint geoCalcPoint);
        void UpdateGraphics();
    }
}
