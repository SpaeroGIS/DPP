using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MilSpace.Core;
using MilSpace.DataAccess.Facade;

namespace MilSpace.Tools.Sentinel
{
    public static class ImportManager
    {
        public static Dictionary<IndexesEnum, string> IndexesDictionary = typeof(IndexesEnum).GetEnumToDictionary<IndexesEnum>();//(  Enum.GetValues(typeof(IndexesEnum)).Cast<IndexesEnum>().ToDictionary(k => k, v => v.ToString());
        public static Dictionary<ValuebaleProductEnum, string> productItemsDictionary = Enum.GetValues(typeof(ValuebaleProductEnum)).Cast<ValuebaleProductEnum>().ToDictionary(k => k, v => v.ToString().Replace("_", " ").Replace("9", "(").Replace("0", ")"));

        public static void ReadJsonFromFile(string fileName = @"E:\Data\S1\Tiles_S1B_UA-EXT_2044434443542054_1.json") //e:\Data\S1\Tiles_S1A_UA-EXT_2044434443542054.json
        {

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Santilel Json file was not found.", fileName);
            }
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<Rootobject>(json);
                var imports = new List<ImportSentinelData>();

                items.products.ToList().ForEach(p =>
                {
                    var import = new ImportSentinelData();

                    var child = p.indexes.FirstOrDefault(i => i.name == IndexesDictionary[IndexesEnum.product])?.children;

                    import.Uuid = p.uuid;
                    import.Id = p.id;
                    import.Identifier = p.identifier.Substring(0, 16);

                    if (child != null)
                    {
                        import.Footprint = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Footprint])?.value;
                        import.JTSfootprint = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.JTS_footprint])?.value;
                        import.RelativeOrbit = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Relative_orbit_9start0])?.value;
                        import.PassDirection = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Pass_direction])?.value;

                        import.ReadPolugon();

                        if (string.IsNullOrWhiteSpace(import.RelativeOrbit))
                        {
                            import.RelativeOrbit = "0";
                        }


                        SentinelFacade.AddFootprint(new DataAccess.DataTransfer.Sentinel.SentinelFootprint
                        {
                            Footprint = import.FootprintPoly,
                            Uuid = p.uuid,
                            Id = p.id,
                            Identifier = import.Identifier,
                            PassDirection = import.PassDirection,
                            RelativeOrbit = int.Parse(import.RelativeOrbit)
                        });
                    }

                    //import.Footprint
                    imports.Add(import);
                });
            }
        }
        public static IEnumerable<ImportSentinelData> ReadJson(string json) //e:\Data\S1\Tiles_S1A_UA-EXT_2044434443542054.json
        {

            var items = JsonConvert.DeserializeObject<Rootobject>(json);
            var imports = new List<ImportSentinelData>();

            items.products.ToList().ForEach(p =>
            {
                var import = new ImportSentinelData();

                var child = p.indexes.FirstOrDefault(i => i.name == IndexesDictionary[IndexesEnum.product])?.children;

                import.Uuid = p.uuid;
                import.Id = p.id;
                import.Identifier = p.identifier.Substring(0, 16);

                if (child != null)
                {
                    import.Footprint = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Footprint])?.value;
                    import.JTSfootprint = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.JTS_footprint])?.value;
                    import.RelativeOrbit = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Relative_orbit_9start0])?.value;
                    import.PassDirection = child.FirstOrDefault(c => c.name == productItemsDictionary[ValuebaleProductEnum.Pass_direction])?.value;

                    import.ReadPolugon();

                    if (string.IsNullOrWhiteSpace(import.RelativeOrbit))
                    {
                        import.RelativeOrbit = "0";
                    }


                    //SentinelFacade.AddFootprint(new DataAccess.DataTransfer.Sentinel.SentinelFootprint
                    //{
                    //    Footprint = import.FootprintPoly,
                    //    Uuid = p.uuid,
                    //    Id = p.id,
                    //    Identifier = import.Identifier,
                    //    PassDirection = import.PassDirection,
                    //    RelativeOrbit = int.Parse(import.RelativeOrbit)
                    //});
                }

                    //import.Footprint
                    imports.Add(import);
            });

            return imports;
        }

    }
}
