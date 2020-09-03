using System;
using MilSpace.Core;
using System.Linq;
using System.Collections.Generic;
using MilSpace.Core.Geometry;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class SentinelProduct
    {
        public string Uuid; // "70dea7e6-01f9-476a-8707-5e81a45b89fb",
        public int Id;
        public string Identifier { get; set; }
        public DateTime DateTime;
        public string Instrument;
        public string Footprint;

        private string jtsfootprint;
        public string JTSfootprint
        {
            get { return jtsfootprint; }
            set
            {
                jtsfootprint = value;
                wktGeometry = Core.Geometry.WktGeometry.Get(jtsfootprint);
            }
        }
        public string PassDirection;
        public int RelativeOrbit;
        public int OrbitNumber;
        public int SliceNumber;
        public string Wkt;
        public string Satellite;
        public string Mode;
        public string ProductType;
        public DateTime Dto;
        public string Operator;
        public Tile RelatedTile;
        public bool Downloaded;
        public IWktGeometry GetWktGeometry()
        {
            return wktGeometry;
        }
        IWktGeometry wktGeometry;

        // MULTIPOLYGON (((50.906448 27.699667, 51.26548 29.325937, 48.677246 29.735947, 48.359959 28.112333, 50.906448 27.699667)))

        LatLon[] productExtend = null;
        public IEnumerable<LatLon> ProductExtend
        {
            get
            {
                if (productExtend != null)
                {
                    return productExtend;
                }

                if (string.IsNullOrWhiteSpace(JTSfootprint))
                {
                    return null;
                }

                var numbers = JTSfootprint.Replace("MULTIPOLYGON", "").Replace("(", "").Replace(")", "").Split(',');

                if (numbers.Length != 5)
                {
                    return null;
                }

                var result = numbers.Select(n =>
                {
                    var lonLat = n.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lonLat.Length != 2)
                    {
                        return null;
                    }
                    double lon = lonLat[0].ParceToDouble();
                    if (double.IsNaN(lon))
                    {
                        return null;
                    }
                    double lat = lonLat[1].ParceToDouble();
                    if (double.IsNaN(lat))
                    {
                        return null;
                    }
                    return new LatLon { Lat = lat, Lon = lon };
                });

                if (result.Any(ll => ll == null))
                {
                    return null;
                }

                productExtend = result.ToArray();
                return productExtend;
            }
            set
            {
                productExtend = value?.ToArray();
            }
        }

        public bool ExtendEqual(SentinelProduct product)
        {
            if (ProductExtend == null || product.ProductExtend == null)
            {
                return false;
            }

            var comparedProductExtend = product.ProductExtend.ToArray();

            for (int i = 0; i < productExtend.Length - 1; i++)
            {
                bool fok = false;
                for (int ii = 0; ii < comparedProductExtend.Length - 1; ii++)
                    if (comparedProductExtend[ii].Equals(productExtend[i]))
                    {
                        fok = true;
                    }
                if (!fok) return false;
            }
            return true;
        }
    }
}

