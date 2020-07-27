using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Configurations.DemStorages
{
    public class DemStorages
    {
        public string SrtmStorage { get; internal set; }
        public string SentinelStorage { get; internal set; }
        public string SentinelDownloadStorage { get; internal set; }
        public string ScihubMetadataApi { get; internal set; }
        public string ScihubProductsApi { get; internal set; }
        public string ScihubUserName { get; internal set; }
        public string ScihubPassword { get; internal set; }
        public string GptExecPath { get; internal set; }
        public string GptCommandsPath { get; internal set; }
        public string GdalInfoExecPath { get; internal set; }
        
    }
}
