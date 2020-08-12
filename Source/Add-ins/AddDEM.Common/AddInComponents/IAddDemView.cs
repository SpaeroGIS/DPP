using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents
{
    public interface IAddDemView
    {
        DemSourceTypeEnum CurrentSourceType { get; }
        IEnumerable<S1Grid> SelectedS1Grid { get; set; }
        IEnumerable<SrtmGrid> SelectedSrtmGrid { get; set; }

        IEnvelope CurrentExtend { get; set;}

        IActiveView ActiveView { get; }

        IMap ActiveMap { get; }


    }
}
