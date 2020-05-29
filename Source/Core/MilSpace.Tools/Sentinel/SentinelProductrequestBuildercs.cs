using MilSpace.Configurations;
using MilSpace.Core;
using System;

namespace MilSpace.Tools.Sentinel
{
    internal class SentinelProductrequestBuildercs
    {
        private string uuid;
        private const string productTempleate = @"('{0}')/$value";

        public SentinelProductrequestBuildercs(string uuid)
        {
            this.uuid = uuid;
        }

        public Uri Url => new Uri($"{MilSpaceConfiguration.DemStorages.ScihubProductsApi}{productTempleate.InvariantFormat(uuid)}");
    }
}
