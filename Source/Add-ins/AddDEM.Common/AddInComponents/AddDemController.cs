using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.AddDem.ReliefProcessing;
using MilSpace.Core.Tools;
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
    public  class AddDemController
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
    }
}
