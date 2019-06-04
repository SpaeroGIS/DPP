using MilSpace.GeoCalculator.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Interfaces
{
    public interface IDataExport
    {
        Task ExportProjectionsToXmlAsync(PointModel point, string path);
    }
}
