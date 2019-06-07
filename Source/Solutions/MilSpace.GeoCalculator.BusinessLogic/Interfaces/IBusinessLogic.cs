using ESRI.ArcGIS.Geometry;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Interfaces
{
    public interface IBusinessLogic
    {
        IPoint ProjectPoint(IPoint inputPoint, SingleProjectionModel singleProjectionModel);
        Task<IPoint> ProjectSelectedPointAsync(int targetCoordinateSystemType, int mousePositionX, int mousePositionY, double falseOriginX = 0, double falseOriginY = 0);
        void CopyCoordinatesToClipboard(List<PointModel> pointModels);
        IPoint GetDisplayCenter();
        IPoint GetSelectedPoint(int mousePositionX, int mousePositionY);
        Task MoveToNewCoordinate(double x, double y);
        string ConvertToMgrs(IPoint wgsInputPoint);
        Task<IPoint> ConvertFromMgrs(string mgrsInputValue, int falseOriginX = 0, int falseOriginY = 0, int scaleUnits = 1000);
        string ConvertToUtm(IPoint wgsInputPoint);
        Task<IPoint> ConvertFromUtm(string utmInputValue, int falseOriginX = 0, int falseOriginY = 0, int scaleUnits = 1000);
        Task<IPoint> ConvertToWgsMeters(IPoint wgsInputPoint);
        Task<IPoint> ConvertFromWgsMeters(string gkInputValue, int falseOriginX = 0, int falseOriginY = 0, int scaleUnits = 1000);
        Task SaveProjectionsToXmlFileAsync(List<PointModel> pointModels, string path);      
    }
}
