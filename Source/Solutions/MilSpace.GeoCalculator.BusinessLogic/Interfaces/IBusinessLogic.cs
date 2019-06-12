using ESRI.ArcGIS.Geometry;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Interfaces
{
    public interface IBusinessLogic
    {
        IPoint CreatePoint(double X, double Y, CoordinateSystemModel geoModel);
        IPoint ProjectPoint(IPoint inputPoint, CoordinateSystemModel singleProjectionModel);
        Task<IPoint> ProjectSelectedPointAsync(int targetCoordinateSystemType, int mousePositionX, int mousePositionY, double falseOriginX = 0, double falseOriginY = 0);
        void CopyCoordinatesToClipboard(List<PointModel> pointModels);
        IPoint GetDisplayCenter();
        IPoint GetSelectedPoint(int mousePositionX, int mousePositionY);
        Task MoveToNewCoordinate(double x, double y);
        string ConvertToMgrs(IPoint wgsInputPoint);
        IPoint ConvertFromMgrs(string mgrsInputValue, CoordinateSystemModel coordinateSystemModel);
        string ConvertToUtm(IPoint wgsInputPoint);
        IPoint ConvertFromUtm(string utmInputValue, CoordinateSystemModel coordinateSystemModel);
        IPoint ConvertToWgsMeters(IPoint wgsInputPoint);
        IPoint ConvertToDecimalDegrees(IPoint point, CoordinateSystemModel coordinateSystemModel);
        Task SaveProjectionsToXmlFileAsync(List<PointModel> pointModels, string path);
        Task SaveLastProjectionToXmlFileAsync(PointModel pointModel, string path);
    }
}
