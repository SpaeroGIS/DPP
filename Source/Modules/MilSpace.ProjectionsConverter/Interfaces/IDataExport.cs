using MilSpace.ProjectionsConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.ProjectionsConverter.Interfaces
{
    public interface IDataExport
    {
        Task ExportProjectionsToXmlAsync(PointModel point, string path);
    }
}
