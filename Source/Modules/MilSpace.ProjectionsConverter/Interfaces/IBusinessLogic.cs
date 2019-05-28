using ESRI.ArcGIS.Geometry;
using MilSpace.ProjectionsConverter.Models;
using System.Threading.Tasks;

namespace MilSpace.ProjectionsConverter.Interfaces
{
    public interface IBusinessLogic
    {
        Task<IPoint> ProjectPointAsync(IPoint inputPoint, SingleProjectionModel singleProjectionModel);
        Task<IPoint> ProjectSelectedPointAsync(int targetCoordinateSystemType, int mousePositionX, int mousePositionY, double falseOriginX = 0, double falseOriginY = 0);
        void CopyCoordinatesToClipboardAsync(PointModel pointModel);
        Task<IPoint> GetDisplayCenterAsync();
        Task<IPoint> GetSelectedPointAsync(int mousePositionX, int mousePositionY);
        Task MoveToNewCoordinateAsync(double x, double y);
        Task<string> ConvertToMgrs(IPoint wgsInputPoint);
        Task<IPoint> ConvertFromMgrs(string mgrsInputValue, int falseOriginX = 0, int falseOriginY = 0, int scaleUnits = 1000);
        Task SaveProjectionsToXmlFileAsync(PointModel pointModel, string path);      
    }
}
