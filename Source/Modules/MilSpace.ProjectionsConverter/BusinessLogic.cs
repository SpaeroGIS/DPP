using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using MilSpace.ProjectionsConverter.Interfaces;
using System;
using System.Threading.Tasks;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using MilSpace.ProjectionsConverter.Models;
using MilSpace.ProjectionsConverter.ReferenceData;

namespace MilSpace.ProjectionsConverter
{
    public class BusinessLogic : IBusinessLogic
    {
        private const string NoMapExceptionMessage = "Can't get current Map Document.";
        private readonly IApplication _arcMapApp;
        private readonly IDataExport _dataExport;
        public BusinessLogic(IApplication arcMapApp, IDataExport dataExport)
        {
            _arcMapApp = arcMapApp ?? throw new ArgumentNullException(nameof(arcMapApp));
            _dataExport = dataExport;
        }

        public async Task<IPoint> ConvertFromMgrs(string mgrsInputValue, int falseOriginX = 0, int falseOriginY = 0, int scaleUnits = 1000)
        {
            return await Task.Run(() =>
            {
                var resultPoint = new Point();
                //Create Spatial Reference Factory
                var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
                //Create Spatial Reference
                ISpatialReference spatialReference = spatialReferenceFactory.CreateProjectedCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
                spatialReference.SetFalseOriginAndUnits(falseOriginX, falseOriginY, scaleUnits);
                resultPoint.SpatialReference = spatialReference;
                (resultPoint as IConversionMGRS).PutCoordsFromMGRS(mgrsInputValue, esriMGRSModeEnum.esriMGRSMode_Automatic);
                return resultPoint;
            });
        }

        public async Task<string> ConvertToMgrs(IPoint wgsInputPoint)
        {
            return await Task.Run(() =>
            {
                var conversionNotation = wgsInputPoint as IConversionNotation;
                //5 for 1m resolution
                return conversionNotation?.CreateMGRS(5, true, esriMGRSModeEnum.esriMGRSMode_Automatic);
            });
        }

        public async Task CopyCoordinatesToClipboardAsync(IPoint inputPoint)
        {
            var pulkovoPoint = await ProjectPointAsync(inputPoint, (int)esriSRGeoCSType.esriSRGeoCS_Pulkovo1942);
            var wgsPoint = await ProjectPointAsync(inputPoint, (int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            var ukrainePoint = await ProjectPointAsync(inputPoint, Constants.Ukraine2000ID);
            var mgrs = await ConvertToMgrs(wgsPoint);

            Clipboard.Clear();
            Clipboard.SetData(nameof(PointModel),
                new PointModel
                {
                    XCoord = inputPoint.X,
                    YCoord = inputPoint.Y,
                    PulkovoXCoord = pulkovoPoint.X,
                    PulkovoYCoord = pulkovoPoint.Y,
                    WgsXCoord = wgsPoint.X,
                    WgsYCoord = wgsPoint.Y,
                    UkraineXCoord = ukrainePoint.X,
                    UkraineYCoord = ukrainePoint.Y,
                    MgrsRepresentation = mgrs
                });
        }

        public async Task<IPoint> GetDisplayCenterAsync()
        {
            if (!(_arcMapApp.Document is IMxDocument currentDocument)) throw new Exception(NoMapExceptionMessage);
            var centerPoint = new Point();
            await Task.Run(() =>
            {
                var activeView = currentDocument.ActiveView;
                var envelope = activeView.Extent as IEnvelope;
                centerPoint.X = ((envelope.XMax - envelope.XMin) / 2) + envelope.XMin;
                centerPoint.Y = ((envelope.YMax - envelope.YMin) / 2) + envelope.YMin;
            });
            return centerPoint;
        }

        public async Task<IPoint> GetSelectedPointAsync(int mousePositionX, int mousePositionY)
        {
            if (!(_arcMapApp.Document is IMxDocument currentDocument)) throw new Exception(NoMapExceptionMessage);

            IPoint resultPoint = new Point();
            await Task.Run(() => 
            {
                resultPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(mousePositionX, mousePositionY);
            });
            return resultPoint;
        }

        public async Task MoveToNewCoordinateAsync(double x, double y)
        {
            if (!(_arcMapApp.Document is IMxDocument currentDocument)) throw new Exception(NoMapExceptionMessage);
            
            await Task.Run(() =>
            {
                var activeView = currentDocument.ActiveView;
                var point = new Point();
                point.PutCoords(x, y);
                activeView.Extent.CenterAt(point);
                activeView.Refresh();                
            });            
        }

        public async Task<IPoint> ProjectPointAsync(IPoint inputPoint, int targetCoordinateSystemType, double falseOriginX = 0, double falseOriginY = 0, double scaleUnits = 1000)
        {
            await Task.Run(() =>
            {
                //Create Spatial Reference Factory
                var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
                //Projected Coordinate System to project into
                var projectedCoordinateSystem = spatialReferenceFactory.CreateProjectedCoordinateSystem(targetCoordinateSystemType);
                projectedCoordinateSystem.SetFalseOriginAndUnits(falseOriginX, falseOriginY, scaleUnits);

                inputPoint.Project(projectedCoordinateSystem);
            });
            return inputPoint;
        }

        public async Task<IPoint> ProjectSelectedPointAsync(int targetCoordinateSystemType, int mousePositionX, int mousePositionY, double falseOriginX = 0, double falseOriginY = 0)
        {
            if (!(_arcMapApp.Document is IMxDocument currentDocument)) throw new Exception(NoMapExceptionMessage);
            var resultPoint = new Point();
            await Task.Run(() =>
            {
                IPoint MouseMapPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(mousePositionX, mousePositionY);
                currentDocument.FocusMap.ClearSelection();
                // Select using the shape (point) to
                // select the feature(s) - false to select any intersecting, true to select just the first
                currentDocument.FocusMap.SelectByShape(MouseMapPoint, (_arcMapApp as IMxApplication).SelectionEnvironment, false);

                var selectedFeatures = (IEnumFeature)currentDocument.FocusMap.FeatureSelection;
                var selectedFeature = selectedFeatures.Next();
                var featuresCount = 1;
                double bufferXCoord = 0.0;
                double bufferYCoord = 0.0;
                do
                {
                    var geometry = selectedFeature.ShapeCopy;
                    switch (geometry.GeometryType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                            var point = geometry as IPoint;
                            bufferXCoord += point.X;
                            bufferYCoord += point.Y;
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                            var polygon = geometry as IArea;
                            bufferXCoord += polygon.Centroid.X;
                            bufferYCoord += polygon.Centroid.Y;
                            break;
                        case esriGeometryType.esriGeometryPolyline:
                            var polyline = geometry as IPolyline;
                            bufferXCoord += (polyline.FromPoint.X + polyline.ToPoint.X) / 2;
                            bufferYCoord += (polyline.FromPoint.Y + polyline.ToPoint.Y) / 2;                            
                            break;
                        default:
                            break;
                    }
                    selectedFeature = selectedFeatures.Next();
                    featuresCount++;
                } while (selectedFeature != null);                
                resultPoint.PutCoords(bufferXCoord / featuresCount, bufferYCoord / featuresCount);
            });

            return await ProjectPointAsync(resultPoint, targetCoordinateSystemType, falseOriginX, falseOriginY, currentDocument.FocusMap.MapScale);            
        }

        public async Task SaveProjectionsToXmlFileAsync(IPoint inputPoint, string path)
        {
            var pulkovoPoint = await ProjectPointAsync(inputPoint, (int)esriSRGeoCSType.esriSRGeoCS_Pulkovo1942);
            var wgsPoint = await ProjectPointAsync(inputPoint, (int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            var ukrainePoint = await ProjectPointAsync(inputPoint, Constants.Ukraine2000ID);
            var mgrs = await ConvertToMgrs(wgsPoint);

            var pointModel = new PointModel
            {
                XCoord = inputPoint.X,
                YCoord = inputPoint.Y,
                PulkovoXCoord = pulkovoPoint.X,
                PulkovoYCoord = pulkovoPoint.Y,
                WgsXCoord = wgsPoint.X,
                WgsYCoord = wgsPoint.Y,
                UkraineXCoord = ukrainePoint.X,
                UkraineYCoord = ukrainePoint.Y,
                MgrsRepresentation = mgrs
            };
            await _dataExport.ExportProjectionsToXmlAsync(pointModel, path);
        }
    }
}
