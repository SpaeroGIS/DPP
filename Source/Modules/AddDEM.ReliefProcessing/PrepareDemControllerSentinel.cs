using MilSpace.AddDem.ReliefProcessing.GuiData;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                tilesToImport.Add(new SentinelTile() { ParentTile = tile });
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

            request.Tile = prepareSentinelView.SelectedTile.ParentTile;
            request.Position = prepareSentinelView.SentinelRequestDate;

            prepareSentinelView.SelectedTile.AddProducts(SentinelImportManager.GetProductsMetadata(request));
            prepareSentinelView.SentinelProductsToDownload = prepareSentinelView.SelectedTile.TileScenes;

            OnProductLoaded?.Invoke(prepareSentinelView.SentinelProductsToDownload);
        }


        public IEnumerable<SentinelProduct> GetScenePairProduct(SentinelProduct baseScene)
        {
            var rr = prepareSentinelView.SentinelProductsToDownload.Where(p => !p.Equals(baseScene) &&
                p.OrbitNumber == baseScene.OrbitNumber);

            var slaveScene = prepareSentinelView.SentinelProductsToDownload.Where(p => !p.Equals(baseScene) &&
                p.OrbitNumber == baseScene.OrbitNumber &&
                12 - Math.Abs((p.DateTime - baseScene.DateTime).TotalDays) < 0.05 &&
                p.ExtendEqual(baseScene)).FirstOrDefault();


            var result = new List<SentinelProduct> { baseScene };

            if (slaveScene == null)
            {
                return result;
            }

            result.Add(slaveScene);

            return result;
        }

        public List<string[]> GetSentinelProductProperties(SentinelProduct product)
        {
            return SentinelProductHelper.GetProductProperies(product);
        }




        public bool CheckProductExistanceToDownload(SentinelProduct product)
        {
            return product == null || prepareSentinelView.SelectedTile.DownloadingScenes.Any(pg => pg.Id == product.Id);
        }

        public IEnumerable<SentinelProductGui> AddProductsToDownload(IEnumerable<SentinelProduct> products)
        {
            var pgs = products.Select(p => CheckProductExistanceToDownload(p) ? null : SentinelProductGui.Get(p)).Where(p => p != null);
            prepareSentinelView.SelectedTile.AddProductsToDownload(pgs);
            return prepareSentinelView.SelectedTile.DownloadingScenes;
        }


        public void DownloadProducts()
        {
            downloading = true;
            var demPrepare = new DataAccess.Facade.DemPreparationFacade();
            foreach (var p in prepareSentinelView.SelectedTile.DownloadingScenes)
            {
                string fileName = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelDownloadStorage, p.Identifier + ".zip");
                p.Downloaded = File.Exists(fileName);
                p.Downloading = !p.Downloaded;
                var productRecords = demPrepare.AddOrUpdateSentinelProduct(p);
            }
            SentinelImportManager.DownloadProducs(prepareSentinelView.SelectedTile.DownloadingScenes.Where(p => p.Downloading), prepareSentinelView.SelectedTile.ParentTile);

        }

        public bool DownloadStarted => downloading;

        public void ProcessPreliminary()
        {
            SentinelImportManager.DoPreProcessing();
        }


        private void OnProductDownloaded(string productId)
        {
            var probuct = prepareSentinelView.SelectedTile.DownloadingScenes.FirstOrDefault(p => p.Identifier == productId);
            if (probuct != null)
            {
                probuct.Downloaded = true;
                probuct.RelatedTile = prepareSentinelView.SelectedTile.ParentTile;
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


        public IEnumerable<SentinelProduct> GetAllSentinelProduct()
        {
            var demFacade = new DemPreparationFacade();
            return demFacade.GetAllSentinelProduct().ToArray();
        }

        public SentinelPairCoherence AddSentinelPairCoherence(SentinelProduct product1, SentinelProduct product2)
        {
            var demFacade = new DemPreparationFacade();
            demFacade.AddOrUpdateSentinelProduct(product1);
            demFacade.AddOrUpdateSentinelProduct(product2);
            return demFacade.AddSentinelPairCoherence(product1, product2);
        }
    }
}
