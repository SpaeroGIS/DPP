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
        internal WktGeometry()
        { }
        internal WktGeometry(string wkt)
        {
            this.wkt = wkt;
            ParceWkt();
        }
        protected string wkt = string.Empty;
        public string Wkt => ToString();
        internal abstract string WktGeometryDescription { get; }

        public bool Intersects(IWktGeometry wktGeometry)
        {

            
            return Geometry.STIntersects(wktGeometry.Geometry).IsTrue;
        }

        public SqlGeometry Geometry => CreateSqlGeometry[GeometryType](Wkt);

        public abstract WktGeometryTypesEnum GeometryType { get; }

        public abstract IEnumerable<WktPoint> ToPoints { get; }

        public void SetWkt(string wkt)
        {
            this.wkt = wkt;
        }

        internal abstract void ParceWkt();


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

        private static Dictionary<WktGeometryTypesEnum, Func<string, SqlGeometry>> CreateSqlGeometry =
            new Dictionary<WktGeometryTypesEnum, Func<string, SqlGeometry>>()
            { {WktGeometryTypesEnum.MULTIPOLYGON, (wkt) => SqlGeometry.STMPolyFromText(new SqlChars(new SqlString(wkt)), 4326).MakeValid()},
               {WktGeometryTypesEnum.POLYGON, (wkt) => SqlGeometry.STPolyFromText(new SqlChars(new SqlString(wkt)), 4326).MakeValid()}};
    }
}
