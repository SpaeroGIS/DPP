using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.Sentinel
{
    public enum IndexesEnum
    {
        product,
        summary
    }


    public enum ValuebaleProductEnum
    {
        Footprint,
        JTS_footprint,
        Relative_orbit_9start0,
        Pass_direction,
        Slice_number,
        Orbit_number_9start0,
        Sensing_start
    }

    public enum ValuebaleProductSummaryEnum
    {
        Date,
        Identifier,
        Instrument
    }

    public enum ValuebaSummaryEnum
    {
        Footprint,
        JTS_footprint

    }
    public class Rootobject
    {
        public Product[] products { get; set; }
        public int totalresults { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string identifier { get; set; }
        public object footprint { get; set; }
        public string[] summary { get; set; }
        public Index[] indexes { get; set; }
        public bool thumbnail { get; set; }
        public bool quicklook { get; set; }
        public string instrument { get; set; }
        public string productType { get; set; }
        public string itemClass { get; set; }
        public string wkt { get; set; }
        public bool offline { get; set; }
    }

    public class Index
    {
        public string name { get; set; }
        public string value { get; set; }
        public Child[] children { get; set; }
    }

    public class Child
    {
        public string name { get; set; }
        public string value { get; set; }
        public object children { get; set; }
    }
}
