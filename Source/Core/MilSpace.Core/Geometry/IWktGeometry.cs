using Microsoft.SqlServer.Types;

namespace MilSpace.Core.Geometry
{
    public interface IWktGeometry
    {
        string Wkt { get; }
        void SetWkt(string wkt);
        bool Intersects(IWktGeometry wktGeometry);
        SqlGeography Geometry { get; }

        WktGeometryTypesEnum GeometryType { get; }

    }
}
