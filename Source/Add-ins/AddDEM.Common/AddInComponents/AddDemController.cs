using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.AddDem.ReliefProcessing;
using MilSpace.Configurations;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents
{
    public enum DemSourceTypeEnum
    {
        STRM,
        Sentinel1
    }
    public class AddDemController
    {

        private IAddDemView view;

        public AddDemController()
        {
        }

        public void RegisterView(IAddDemView view)
        {
            this.view = view;
        }

        public void OpenDemCalcForm()
        {
            var demCalcForm = new PrepareDem();
            demCalcForm.ShowDialog();
        }


        public void SearchSelectedTiles(IGeometry area)
        {
            if (view.CurrentSourceType == DemSourceTypeEnum.Sentinel1)
            {
                view.SelectedS1Grid = TileManager.GetS1GridByArea(area);
            }
            else if (view.CurrentSourceType == DemSourceTypeEnum.STRM)
            {
                view.SelectedSrtmGrid = TileManager.GetSrtmGridByArea(area);
            }
        }

        public bool GenerateUnionTile(IEnumerable<string> tileName)
        {
            var pathToStorage = string.Empty;

            var tile = tileName.Select(t => new Tile(t)).Where(t => !t.IsEmpty);

            IEnumerable<string> fileName = null;

            if (view.CurrentSourceType == DemSourceTypeEnum.Sentinel1)
            {
                pathToStorage = MilSpaceConfiguration.DemStorages.SentinelStorage;
                fileName = tile.Select(t => AddDemFacade.GetS1GridByTile(t)).Select(t =>
                    System.IO.Path.Combine(pathToStorage, t.FileName)).Where(f => System.IO.File.Exists(f));
            }
            else //(view.CurrentSourceType == DemSourceTypeEnum.STRM)
            {
                pathToStorage = MilSpaceConfiguration.DemStorages.SrtmStorage;

                fileName = tile.Select(t => AddDemFacade.GetSrtmGridByTile(t)).Select(t =>
                    System.IO.Path.Combine(pathToStorage, t.FileName)).Where(f => System.IO.File.Exists(f));
            }

            if (!System.IO.Directory.Exists(System.IO.Path.Combine(pathToStorage, "Gtiles")))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(pathToStorage, "Gtiles"));
            }

            var resultFileName = System.IO.Path.Combine(pathToStorage, "Gtiles", $"{view.CurrentSourceType}_{string.Join("", tile.Select(t => t.Name))}.tif");

            if (!TileManager.GenerateTiles(fileName, resultFileName))
            {
                return false;
            }

            var layer = TileManager.GetRasterLayer(resultFileName);
            if (layer == null)
            {
                return false;
            }

            view.ActiveMap.AddLayer(layer);
            return true;
        }

        public bool AddSelectedTileToMap(string tileName)
        {
            var layer = TileManager.GetRasterLayer(tileName, view.CurrentSourceType);
            if (layer == null)
            {
                return false;
            }

            view.ActiveMap.AddLayer(layer);
            return true;
        }

        public bool AddRasterToMap(string resultFileName)
        {
            var layer = TileManager.GetRasterLayer(resultFileName);
            if (layer == null)
            {
                return false;
            }

            view.ActiveMap.AddLayer(layer);
            return true;
        }
    }
}
