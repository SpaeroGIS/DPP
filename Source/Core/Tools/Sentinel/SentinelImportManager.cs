using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MilSpace.Core;
using MilSpace.DataAccess.Facade;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using System.Net;
using MilSpace.Configurations;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.ComponentModel;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;

namespace MilSpace.Tools.Sentinel
{
    public static class SentinelImportManager
    {
        public delegate void SentinelProductsDownloaded(string product);
        public static event SentinelProductsDownloaded OnProductDownloaded;
        public static event SentinelProductsDownloaded OnProductDownloadingError;

        private static Logger logger = Logger.GetLoggerEx("SentinelImportManager");
        public static Dictionary<IndexesEnum, string> IndexesDictionary;
        public static Dictionary<ValuebaleProductEnum, string> productItemsDictionary = Enum.GetValues(typeof(ValuebaleProductEnum)).Cast<ValuebaleProductEnum>().ToDictionary(k => k, v => v.ToString().Replace("_", " ").Replace("9", "(").Replace("0", ")"));
        public static Dictionary<ValuebaleProductSummaryEnum, string> productSummaryItemsDictionary = Enum.GetValues(typeof(ValuebaleProductSummaryEnum) ).Cast<ValuebaleProductSummaryEnum>().ToDictionary(k => k, v => v.ToString().Replace("_", " ").Replace("9", "(").Replace("0", ")"));

        static SentinelImportManager()
        {
            logger.InfoEx("Initializing..");
            IndexesDictionary = Enum.GetValues(typeof(IndexesEnum)).Cast<IndexesEnum>().ToDictionary(k => k, v => v.ToString()); //typeof(IndexesEnum).GetEnumToDictionary<IndexesEnum>();//
            logger.InfoEx("Initialized");
        }

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
                import.ProductType = p.productType;

                import.Dto = DateTime.Now;
                import.Operator = Environment.UserName;

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

        public static IEnumerable<SentinelProduct> GetProductsMetadata(SentineWeblRequestBuilder metadataRequest)
        {
            var url = metadataRequest.GetMetadataUrl;
            WebRequest myWebRequest = WebRequest.Create(url);
            logger.InfoEx($"Getting metadata: {HttpUtility.UrlDecode(url)}");

            myWebRequest.Credentials = new NetworkCredential(MilSpaceConfiguration.DemStorages.ScihubUserName, MilSpaceConfiguration.DemStorages.ScihubPassword);
            myWebRequest.Timeout = -1;
            string requestContent;
            // Send the 'WebRequest' and wait for response.
            try
            {
                using (WebResponse myWebResponse = myWebRequest.GetResponse())
                {
                    using (var reader = new StreamReader(myWebResponse.GetResponseStream()))
                    {
                        requestContent = reader.ReadToEnd();
                    }

                    myWebResponse.Close();
                }
                var resuslt = ReadJson(requestContent).OrderBy(s => s.DateTime).ToList();
                resuslt.ForEach(p => p.RelatedTile = metadataRequest.Tile);
                return resuslt;
            }
            catch (Exception ex)
            {
                logger.ErrorEx(ex.Message);
            }
            
            return null;
        }

        public static void DownloadProducs(IEnumerable<SentinelProduct> products, Tile tile)
        {
            foreach(var product in  products)
            {
                DownloadProbuct(product, tile.Name);
            }
            
        }

        public static void DoPreProcessing()
        {
            string commandFile = @"E:\SourceCode\40copoka\DPP\Source\UnitTests\CommandLineAction.UnitTest\Output\CommandLineAction.UnitTest.exe";

            var action = new ActionParam<string>()
            {
                ParamName = ActionParamNamesCore.Action,
                Value = ActionsCore.RunCommandLine
            };

            var prm = new IActionParam[]
             {
                  action,
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.PathToFile, Value = commandFile},
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.DataValue, Value = string.Empty},
                    new ActionParam<ActionProcessCommandLineDelegate>() { ParamName = ActionParamNamesCore.OutputDataReceivedDelegate, Value = OnOutputCommandLine},
                    new ActionParam<ActionProcessCommandLineDelegate>() { ParamName = ActionParamNamesCore.ErrorDataReceivedDelegate, Value = OnErrorCommandLine}

             };

            var procc = new ActionProcessor(prm);
            var res = procc.Process<StringActionResult>();
        }

        public static void OnErrorCommandLine(string consoleMessage, ActironCommandLineStatesEnum state)
        {
            logger.ErrorEx(consoleMessage);
        }

        public static void OnOutputCommandLine(string consoleMessage, ActironCommandLineStatesEnum state)
        {
            logger.InfoEx(consoleMessage);
        }


        private static void DownloadProbuct(SentinelProduct product, string tileFolderName)
        {

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(MilSpaceConfiguration.DemStorages.ScihubUserName, MilSpaceConfiguration.DemStorages.ScihubPassword);
                client.QueryString.Add("Id", product.Identifier);
                SentinelProductrequestBuildercs builder = new SentinelProductrequestBuildercs(product.Uuid);

                string tileFolder = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelDownloadStorageExternal, tileFolderName);

                string fileName = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelDownloadStorageExternal, product.Identifier + ".zip");
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(builder.Url, fileName);
            }
            
        }


        private static void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (sender is WebClient client)
            {
                //TODO: write message
                if (e.Error != null)
                {

                    OnProductDownloadingError?.Invoke(client.QueryString["Id"]);
                    logger.ErrorEx($"Error on download. Product {client.QueryString["Id"]} ");
                    logger.ErrorEx(e.Error.Message);
                }
                else
                {
                    OnProductDownloaded?.Invoke(client.QueryString["Id"]);
                    logger.InfoEx($"Download completed. Product {client.QueryString["Id"]}");
                }
            }
        }
    }
}
