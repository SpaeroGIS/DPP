﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MilSpace.Core;
using MilSpace.DataAccess.Facade;

namespace MilSpace.Tools.Sentinel
{
    public static class SentinelImportManager
    {
        public static Dictionary<IndexesEnum, string> IndexesDictionary = typeof(IndexesEnum).GetEnumToDictionary<IndexesEnum>();//(  Enum.GetValues(typeof(IndexesEnum)).Cast<IndexesEnum>().ToDictionary(k => k, v => v.ToString());
        public static Dictionary<ValuebaleProductEnum, string> productItemsDictionary = Enum.GetValues(typeof(ValuebaleProductEnum)).Cast<ValuebaleProductEnum>().ToDictionary(k => k, v => v.ToString().Replace("_", " ").Replace("9", "(").Replace("0", ")"));
        public static Dictionary<ValuebaleProductSummaryEnum, string> productSummaryItemsDictionary = Enum.GetValues(typeof(ValuebaleProductSummaryEnum) ).Cast<ValuebaleProductSummaryEnum>().ToDictionary(k => k, v => v.ToString().Replace("_", " ").Replace("9", "(").Replace("0", ")"));
        public static IEnumerable<SentinelProduct> ReadJsonFromFile(string fileName = @"E:\Data\S1\Tiles_S1B_UA-EXT_2044434443542054_1.json") //e:\Data\S1\Tiles_S1A_UA-EXT_2044434443542054.json
        {

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Santilel Json file was not found.", fileName);
            }
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                return ReadJson(json);
            }
        }
        public static IEnumerable<SentinelProduct> ReadJson(string json) //e:\Data\S1\Tiles_S1A_UA-EXT_2044434443542054.json
        {

            var items = JsonConvert.DeserializeObject<Rootobject>(json);
            var imports = new List<SentinelProduct>();

            items.products.ToList().ForEach(p =>
            {
                var import = new SentinelProduct();

                var child = p.indexes.FirstOrDefault(i => i.name == IndexesDictionary[IndexesEnum.product])?.children;

                import.Uuid = p.uuid;
                import.Id = p.id;
                import.Identifier = p.identifier;
                import.Instrument = p.instrument;
                

                if (child != null)
                {
                    import.DateTime = Helper.Convert<DateTime>(child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Sensing_start])?.value);
                    import.Footprint = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Footprint])?.value;
                    import.JTSfootprint = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.JTS_footprint])?.value;
                    import.RelativeOrbit = Helper.Convert<int>(child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Relative_orbit_9start0])?.value);
                    import.PassDirection = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Pass_direction])?.value;
                    import.SliceNumber= Helper.Convert<int>(child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Slice_number])?.value);
                    import.OrbitNumber = Helper.Convert<int>(child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Relative_orbit_9start0])?.value);
                }

                imports.Add(import);

            });

            return imports;
        }

    }
}
