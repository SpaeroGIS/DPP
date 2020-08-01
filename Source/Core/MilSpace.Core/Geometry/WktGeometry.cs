using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.Geometry
{
    public abstract class WktGeometry : IWktGeometry
    {
        protected string wkt = string.Empty;
        public string Wkt => ToString();
        internal abstract string WktGeometryDescription { get; }

        public bool Intersects(IWktGeometry wktGeometry)
        {
            return Geometry.STIntersects(wktGeometry.Geometry).IsTrue;
        }

        public SqlGeography Geometry => SqlGeography.STMPolyFromText(new SqlChars(new SqlString(Wkt)), 4326);

        public abstract WktGeometryTypesEnum GeometryType { get; }

        public void SetWkt(string wkt)
        {
            this.wkt = wkt;
        }

        public static IWktGeometry Get(string wkt)
        {
            if (string.IsNullOrWhiteSpace(wkt))
            {
                return null;
            }
            if (wkt.StartsWith(WktGeometryTypesEnum.MULTIPOLYGON.ToString()))
            {
                return CreateGeometry[WktGeometryTypesEnum.MULTIPOLYGON].Invoke(wkt);
            }
            if (wkt.StartsWith(WktGeometryTypesEnum.POLYGON.ToString()))
            {
                return CreateGeometry[WktGeometryTypesEnum.POLYGON].Invoke(wkt);
            }

            throw new NotImplementedException();
        }

        private static Dictionary<WktGeometryTypesEnum, Func<string, IWktGeometry>> CreateGeometry =
            new Dictionary<WktGeometryTypesEnum, Func<string, IWktGeometry>>()
            { {WktGeometryTypesEnum.MULTIPOLYGON, (wkt) => new WktMulyiPolygon(wkt) },
               {WktGeometryTypesEnum.POLYGON, (wkt) => new WktPolygon(wkt) }};

    }
}
