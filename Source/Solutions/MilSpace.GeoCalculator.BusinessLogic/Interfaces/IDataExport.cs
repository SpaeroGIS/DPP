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
        Task ExportProjectionsToXmlAsync(List<PointModel> pointModels, string path);
        Task ExportProjectionsToXmlAsync(PointModel pointModel, string path);
        Task ExportProjectionsToCsvAsync(List<PointModel> pointModels, string path);
        Task ExportProjectionsToCsvAsync(PointModel pointModel, string path);
        string GetStringRepresentationOfProjections(List<PointModel> pointModels);        
    }
}
