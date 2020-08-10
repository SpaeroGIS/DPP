using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Sentinel;
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

        public void AddTilesToList(IEnumerable<Tile>  newTiles)
        {
            tiles.Clear();
            newTiles.ToList().
                ForEach(tile =>
               {
                   if (!tiles.Any(t => t.Equals(tile)))
                   {
                       tiles.Add(tile);
                   }
               });
        }

        public void RemoveTileFromList(string tileName)
        {
            var tileToRemove = new Tile(tileName);
            if (!tileToRemove.IsEmpty)
            {
                tiles.Remove(tileToRemove);
            }
        }

        public Tile AddTileToList()
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
                if (!tiles.Any(t => t.Equals(tile)))
                {
                    tiles.Add(tile);
                }
                return tile;
            }

            return null;
        }


        public IEnumerable<SentinelTilesCoverage> GetQaziTilesByTileName(string tileName)
        {

            var tile = tiles.FirstOrDefault(t => t.Name == tileName);
            if (tile != null)
            {

                var facede = new DemPreparationFacade();
                quaziTiles =
                 facede.GeTileCoveragesHaveGeometry().
                    Where(c => c.Geometry.Intersects(tile.Geometry)).ToList();
                return quaziTiles;

            }


            return null;
        }

        public Tile GetTilesByPoint()
        {
            var latString = view.TileDemLatitude;
            var lonString = view.TileDemLongitude;
            double latDouble;
            double lonDouble;
            Tile testTile = null;

            if (latString.TryParceToDouble(out latDouble) && lonString.TryParceToDouble(out lonDouble))
            {
                int lat = Convert.ToInt32(latDouble);
                int lon = Convert.ToInt32(lonDouble);

                if (!Tiles.Any(t => t.Lat == lat && t.Lon == lon))
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

        public bool IsTIleCoveragedByQuaziTiles()
        {
            var tile = tiles.First(t => t.Name == view.SelectedTileDem);
            return SantinelExportDemToTileManager.CheckTileCompleteness(tile, quaziTiles);
        }
    }
}
