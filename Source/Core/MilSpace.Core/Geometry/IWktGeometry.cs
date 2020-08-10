using Microsoft.SqlServer.Types;
using System.Collections.Generic;

namespace MilSpace.Core.Geometry
{
    public interface IWktGeometry
    {
        string Wkt { get; }
        void SetWkt(string wkt);
        bool Intersects(IWktGeometry wktGeometry);
        SqlGeometry Geometry { get; }

        IEnumerable<WktPoint> ToPoints { get; }

        WktGeometryTypesEnum GeometryType { get; }

    }
}
