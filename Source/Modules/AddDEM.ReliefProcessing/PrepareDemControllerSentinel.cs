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

        List<SentinelTile> tilesToImport = new List<SentinelTile>();
        //List<SentinelProductGui> sentinelProductsToDownload = new List<SentinelProductGui>();

        IPrepareDemViewSentinel prepareSentinelView;
        internal PrepareDemControllerSentinel()
        {
            SentinelImportManager.OnProductDownloaded += OnProductDownloaded;
        }

        public IEnumerable<SentinelTile> TilesToImport => tilesToImport;

        public void AddTileForImport()
        {
            var tile = GetTilesByPoint();
            if (tile != null)
            {
                tilesToImport.Add( new SentinelTile() { ParentTile = tile });
            }
        }

        public SentinelTile GetTileByName(string tileName)
        {
            tileName = tileName == null ? string.Empty : tileName;
            return tilesToImport.FirstOrDefault(st => st.ParentTile.Name.Equals(tileName, StringComparison.OrdinalIgnoreCase));
        }

        public void SetView(IPrepareDemViewSentinel view)
        {
            prepareSentinelView = view;
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

                if (!tilesToImport.Select(tl => tl.ParentTile).Any(t => t.Lat == lat && t.Lon == lon))
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

            request.AddTile(prepareSentinelView.SelectedTile.ParentTile);
            request.Position = prepareSentinelView.SentinelRequestDate;

            prepareSentinelView.SelectedTile.AddProducts(SentinelImportManager.GetProductsMetadata(request));
            prepareSentinelView.SentinelProducts = prepareSentinelView.SelectedTile.TileScenes;

            OnProductLoaded?.Invoke(prepareSentinelView.SentinelProducts);
        }

        public IEnumerable<SentinelProduct> GetScenePairProduct(SentinelProduct baseScene)
        {
            var pairs = prepareSentinelView.SentinelProducts.Where(p => p.OrbitNumber == baseScene.OrbitNumber);
            //TO

            return pairs;
        }

        public List<string[]> GetSentinelProductProperties(SentinelProduct product)
        {
            return SentinelProductHelper.GetProductProperies(product);
        }


        public bool CheckProductExistanceToDownload(SentinelProduct product)
        {
            return product == null || prepareSentinelView.SelectedTile.DownloadingScenes.Any(pg => pg.Id == product.Id);
        }

        public IEnumerable<SentinelProductGui> AddProductsToDownload(IEnumerable<SentinelProduct> products, SentinelProduct asBase)
        {
            var pgs = products.Select(p => CheckProductExistanceToDownload(p) ? null : SentinelProductGui.Get(p)).Where(p => p != null);
            prepareSentinelView.SelectedTile.AddProductsToDownload(pgs, asBase);
            return prepareSentinelView.SelectedTile.DownloadingScenes;
        }
      

        public void DownloadProducts()
        {
            downloading = true;
            foreach( var p in prepareSentinelView.SelectedTile.DownloadingScenes)
            {
                p.Downloading = true;
            }
            SentinelImportManager.DownloadProducs(prepareSentinelView.SelectedTile.DownloadingScenes, prepareSentinelView.SelectedTile.ParentTile.Name);

        }

        public bool DownloadStarted => downloading;

        private void OnProductDownloaded(string productId)
        {
            var probuct = prepareSentinelView.SelectedTile.DownloadingScenes.FirstOrDefault(p => p.Identifier == productId);
            if (probuct != null)
            {
                probuct.Downloaded = true;
            }

            if (prepareSentinelView.SelectedTile.DownloadingScenes.Any(p => p.Downloading && !p.Downloaded))
            {
                return;
            }
            downloading = false;
            foreach (var p in prepareSentinelView.SelectedTile.DownloadingScenes)
            {
                p.Downloading = false;
            }
            OnProductsDownloaded?.Invoke(prepareSentinelView.SelectedTile.DownloadingScenes);
        }
    }
}
