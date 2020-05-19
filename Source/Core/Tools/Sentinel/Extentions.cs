using ESRI.ArcGIS.Geometry;
using Microsoft.SqlServer.Types;
using MilSpace.Core;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MilSpace.Tools.Sentinel
{
    public static class Extentions
    {

        public static void ReadPolugon(this ImportSentinelData importProduct)
        {
            if (string.IsNullOrWhiteSpace(importProduct.Footprint))
            {
                return;
            }

            string text = importProduct.Footprint;

            //"<gml:Polygon srsName=\"http://www.opengis.net/gml/srs/epsg.xml#4326\" xmlns:gml=\"http://www.opengis.net/gml\"><gml:outerBoundaryIs><gml:LinearRing><gml:coordinates>45.308281,24.306789 45.711151,21.006590 47.207355,21.359417 46.803715,24.750584 45.308281,24.306789</gml:coordinates></gml:LinearRing></gml:outerBoundaryIs></gml:Polygon>";


            var doc = XDocument.Parse(text);
            var elem = doc.Descendants(XName.Get("coordinates", "http://www.opengis.net/gml")).Select(node =>
            {
                var pointCollection = new MultipointClass();
                node.Value.Split(' ').Select(p =>
                {
                    var coords = p.Split(','); return new Point { X = coords[1].ParceToDouble(), Y = coords[0].ParceToDouble(), SpatialReference = EsriTools.Wgs84Spatialreference };
                }).ToList().ForEach(p => pointCollection.AddPoint(p));

                return EsriTools.GetPolygonByPointCollection(pointCollection);

            });

            importProduct.FootprintPoly = elem.FirstOrDefault();
        }

        public static void UpdateWkt(this ImportSentinelData importProduct, string wktData)
        {
            SqlString sqlString = new SqlString(wktData);

            SqlChars f = new SqlChars(sqlString);
            var geometry = SqlGeography.Parse(sqlString).ReorientObject();

            var res = SqlGeography.STMPolyFromText(f, 4326);
        }

    }
}
