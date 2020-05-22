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

        internal event ProductsLoaded OnProductLoaded;

        List<Tile> tilesToImport = new List<Tile>();

        IPrepareDemViewSentinel prepareSentinelView;
        internal PrepareDemControllerSentinel()
        { }

        public IEnumerable<Tile> TilesToImport => tilesToImport;

        public void AddTileForImport()
        {
            var tile = GetTilesByPoint();
            if (tile != null )
            {
                tilesToImport.Add(tile);
            }
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
            prepareSentinelView.SentinelProducts =  SentinelImportManager.GetProductsMetadata(request);
            OnProductLoaded?.Invoke(prepareSentinelView.SentinelProducts);

        }
    }
}
