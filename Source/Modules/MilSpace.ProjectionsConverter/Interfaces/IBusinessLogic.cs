using ESRI.ArcGIS.Geometry;
using System.Threading.Tasks;

namespace MilSpace.ProjectionsConverter.Interfaces
{
    public interface IBusinessLogic
    {
        Task<IPoint> ProjectPointAsync(IPoint inputPoint, int targetCoordinateSystemType, double falseOriginX = 0, double falseOriginY = 0, double scaleUnits = 1000);
        Task<IPoint> ProjectSelectedPointAsync(int targetCoordinateSystemType, int mousePositionX, int mousePositionY, double falseOriginX = 0, double falseOriginY = 0);
        Task CopyCoordinatesToClipboardAsync(IPoint inputPoint);
        Task<IPoint> GetDisplayCenterAsync();
        Task<IPoint> GetSelectedPointAsync(int mousePositionX, int mousePositionY);
        Task MoveToNewCoordinateAsync(double x, double y);
        Task<string> ConvertToMgrs(IPoint wgsInputPoint);
        Task<IPoint> ConvertFromMgrs(string mgrsInputValue, int falseOriginX = 0, int falseOriginY = 0, int scaleUnits = 1000);
        Task SaveProjectionsToXmlFileAsync(IPoint inputPoint, string path);
    }
}
