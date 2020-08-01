using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing
{
    public class PrepareDemContrellerGenerateTile
    {
        Logger log = Logger.GetLoggerEx("PrepareDemGenerateTileContreller");
        IPrepareDemViewGenerateTile view;

        List<SentinelTilesCoverage> quaziTiles = new List<SentinelTilesCoverage>();


        public PrepareDemContrellerGenerateTile()
        { }

        public void SetView(IPrepareDemViewGenerateTile view)
        {
            this.view = view;
        }

        private List<Tile> tiles = new List<Tile>();

        public IEnumerable<Tile> Tiles => tiles; 

        public void AddTileToList()
        {
            var latString = view.TileDemLatitude;
            var lonString = view.TileDemLongitude;
            double latDouble;
            double lonDouble;
            Tile tile = null;

            if (latString.TryParceToDouble(out latDouble) && lonString.TryParceToDouble(out lonDouble))
            {
                int lat = Convert.ToInt32(latDouble);
                int lon = Convert.ToInt32(lonDouble);
                tile = new Tile { Lat = lat, Lon = lon };
                if (tiles.Any(t => t.Equals(tile)))
                    { return; }
                tiles.Add(tile);
            }
        }
   

        public Tile GetTileByPoint()
        {
            var latString = view.TileDemLatitude;
            var lonString = view.TileDemLongitude;
            double latDouble;
            double lonDouble;
            Tile tile = null;

            if (latString.TryParceToDouble(out latDouble) && lonString.TryParceToDouble(out lonDouble))
            {
                int lat = Convert.ToInt32(latDouble);
                int lon = Convert.ToInt32(lonDouble);
                tile = new Tile { Lat = lat, Lon = lon };

                var facede = new DemPreparationFacade();
                var quzaitiles = facede.GeTileCoveragesHaveGeometry().
                    Where(c => c.Geometry.Intersects(tile.Geometry));

            }


            return tile;
        }
    }
}
