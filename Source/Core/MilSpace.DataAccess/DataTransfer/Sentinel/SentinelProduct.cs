using System;
using MilSpace.Core;
using System.Linq;
using System.Collections.Generic;

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
        public string JTSfootprint;
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
                    var latLon = n.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (latLon.Length != 2)
                    {
                        return null;
                    }
                    double lat = latLon[0].ParceToDouble();
                    if (double.IsNaN(lat))
                    {
                        return null;
                    }
                    double lon = latLon[1].ParceToDouble();
                    if (double.IsNaN(lon))
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

            for (int i = 0; i < productExtend.Length; i++)
            {
                if (!comparedProductExtend[i].Equals(productExtend[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

