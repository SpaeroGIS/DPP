using MilSpace.AddDem.ReliefProcessing.GuiData;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing
{
    public class PrepareDemControllerSentinel
    {

        Logger log = Logger.GetLoggerEx("PrepareDemControllerSentinel");
        internal delegate void ProductsLoaded(IEnumerable<SentinelProduct> products);
        internal delegate void ProductsDownloaded(IEnumerable<SentinelProduct> products);

        private bool downloading = false;

        internal event ProductsLoaded OnProductLoaded;
        internal event ProductsDownloaded OnProductsDownloaded;

        List<Tile> tilesToImport = new List<Tile>();
        List<SentinelProductGui> sentinelProductsToDownload = new List<SentinelProductGui>();

        IPrepareDemViewSentinel prepareSentinelView;
        internal PrepareDemControllerSentinel()
        {
            SentinelImportManager.OnProductDownloaded += OnProductDownloaded;
        }

        public IEnumerable<Tile> TilesToImport => tilesToImport;

        public void AddTileForImport()
        {
            var tile = GetTilesByPoint();
            if (tile != null)
            {
                tilesToImport.Add(tile);
            }
        }

        public void SetView(IPrepareDemViewSentinel view)
        {
            prepareSentinelView = view;
            prepareSentinelView.SentinelProductsToDownload = sentinelProductsToDownload;
        }

        public void ReadConfiguration()
        {
            if (prepareSentinelView == null)
            {
                throw new MethodAccessException("prepareDemview cannot be null");
            }
            prepareSentinelView.SentinelSrtorage = MilSpaceConfiguration.DemStorages.SentinelStorage;
        }

        public Tile GetTilesByPoint()
        {
            var latString = prepareSentinelView.TileLatitude;
            var lonString = prepareSentinelView.TileLongtitude;
            double latDouble;
            double lonDouble;
            Tile testTile = null;

            if (latString.TryParceToDouble(out latDouble) && lonString.TryParceToDouble(out lonDouble))
            {
                int lat = Convert.ToInt32(latDouble);
                int lon = Convert.ToInt32(lonDouble);

                if (!tilesToImport.Any(t => t.Lat == lat && t.Lon == lon))
                {
                    testTile = new Tile
                    {
                        Lat = lat,
                        Lon = lon
                    };

                }
            }

            return testTile;
        }

        public void GetScenes()
        {
            SentineWeblRequestBuilder request = new SentineWeblRequestBuilder();

            prepareSentinelView.TilesToImport.ToList().ForEach(t => request.AddTile(t));
            request.Position = prepareSentinelView.SentinelRequestDate;
            prepareSentinelView.SentinelProducts = SentinelImportManager.GetProductsMetadata(request);
            OnProductLoaded?.Invoke(prepareSentinelView.SentinelProducts);
        }

        public List<string[]> GetSentinelProductProperties(SentinelProduct product)
        {
            return SentinelProductHelper.GetProductProperies(product);
        }


        public bool CheckProductExistanceToDownload(SentinelProduct product)
        {
            return sentinelProductsToDownload.Any(pg => pg.Id == product.Id);
        }

        public SentinelProductGui AddProductToDownload(SentinelProduct product)
        {
            if (!CheckProductExistanceToDownload(product))
            {
                var pg = SentinelProductGui.Get(product);
                pg.BaseScene = sentinelProductsToDownload.Count == 0;
                sentinelProductsToDownload.Add(pg);
                return pg;
            }

            return null;
        }

        public void DownloadProducts()
        {
            downloading = true;
            foreach( var p in sentinelProductsToDownload)
            {
                p.Downloading = true;
            }

            SentinelImportManager.DownloadProducs(sentinelProductsToDownload);

        }

        public bool DownloadStarted => downloading;

        private void OnProductDownloaded(string productId)
        {
            var probuct = sentinelProductsToDownload.FirstOrDefault(p => p.Identifier == productId);
            if (probuct != null)
            {
                probuct.Downloaded = true;
            }

            if (sentinelProductsToDownload.Any(p => p.Downloading && !p.Downloaded))
            {
                return;
            }
            downloading = false;
            foreach (var p in sentinelProductsToDownload)
            {
                p.Downloading = false;
            }
            OnProductsDownloaded?.Invoke(sentinelProductsToDownload);
        }
    }
}
