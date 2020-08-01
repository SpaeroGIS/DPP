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
    public class GenerateTileContreller
    {
        Logger log = Logger.GetLoggerEx("PrepareDemControllerSrtm");
        IPrepareDemViewGenerateTile view;

        List<SentinelTilesCoverage> quaziTiles = new List<SentinelTilesCoverage>();


        public GenerateTileContreller()
        { }

        public void SetView(IPrepareDemViewGenerateTile view)
        {
            this.view = view;
        }

        public Tile GetTileByPoint()
        {
            var latString = view.TileLatitude;
            var lonString = view.TileLongtitude;
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
