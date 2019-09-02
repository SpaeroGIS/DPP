using ESRI.ArcGIS.Geometry;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Interfaces
{
    public interface IBusinessLogic
    {
        IPoint CreatePoint(double X, double Y, CoordinateSystemModel geoModel, bool createGeoCoordinateSystem = false);
        IPoint ProjectPoint(IPoint inputPoint, CoordinateSystemModel singleProjectionModel);
        IPoint ProjectWgsToPulkovoWithGeoTransformation(IPoint inputPoint, CoordinateSystemModel coordinateSystemModel, esriTransformDirection transformationDirection);
        IPoint ProjectWgsToUrkaine2000WithGeoTransformation(IPoint inputPoint, CoordinateSystemModel coordinateSystemModel, esriTransformDirection transformationDirection);
        Task<IPoint> ProjectSelectedPointAsync(int targetCoordinateSystemType, int mousePositionX, int mousePositionY, double falseOriginX = 0, double falseOriginY = 0);
        void CopyCoordinatesToClipboard(List<PointModel> pointModels);
        void CopyCoordinatesToClipboard(ExtendedPointModel pointModel);
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
        Task SaveLastProjectionToCsvFileAsync(PointModel pointModel, string path);
        Task SaveProjectionsToCsvFileAsync(List<PointModel> pointModels, string path);
        Task<List<PointModel>> ImportProjectionsFromXmlAsync(string path);
        Task<List<PointModel>> ImportProjectionsFromCsvAsync(string path);
    }
}
