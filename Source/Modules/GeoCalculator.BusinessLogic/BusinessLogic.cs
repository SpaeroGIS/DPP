using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using MilSpace.GeoCalculator.BusinessLogic.ReferenceData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.GeoCalculator.BusinessLogic
{
    public class BusinessLogic : IBusinessLogic
    {
        private const string NoMapExceptionMessage = "Can't get current Map Document.";
        private readonly IApplication _arcMapApp;
        private readonly IDataImportExport _dataImportExport;
        public BusinessLogic(IApplication arcMapApp, IDataImportExport dataExport)
        {
            _arcMapApp = arcMapApp ?? throw new ArgumentNullException(nameof(arcMapApp));
            _dataImportExport = dataExport;
        }

        public IPoint ConvertFromMgrs(string mgrsInputValue, CoordinateSystemModel coordinateSystemModel)
        {
            var resultPoint = new Point();
            //Create Spatial Reference Factory
            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            //Create Spatial Reference
            ISpatialReference spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(coordinateSystemModel.ESRIWellKnownID);
            spatialReference.SetFalseOriginAndUnits(coordinateSystemModel.FalseOriginX, coordinateSystemModel.FalseOriginY, coordinateSystemModel.Units);
            resultPoint.SpatialReference = spatialReference;
            (resultPoint as IConversionMGRS).PutCoordsFromMGRS(mgrsInputValue, esriMGRSModeEnum.esriMGRSMode_Automatic);
            return resultPoint;
        }

        public IPoint ConvertFromUtm(string utmInputValue, CoordinateSystemModel coordinateSystemModel)
        {
            var resultPoint = new Point();
            //Create Spatial Reference Factory
            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            //Create Spatial Reference
            ISpatialReference spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(coordinateSystemModel.ESRIWellKnownID);
            spatialReference.SetFalseOriginAndUnits(coordinateSystemModel.FalseOriginX, coordinateSystemModel.FalseOriginY, coordinateSystemModel.Units);
            resultPoint.SpatialReference = spatialReference;
            (resultPoint as IConversionNotation).PutCoordsFromUTM(esriUTMConversionOptionsEnum.esriUTMAddSpaces, utmInputValue);
            return resultPoint;
        }

        public IPoint ConvertToWgsMeters(IPoint wgsInputPoint)
        {
            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference wgsMetersSpatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            wgsInputPoint.Project(wgsMetersSpatialReference);
            return wgsInputPoint;
        }

        public string ConvertToMgrs(IPoint wgsInputPoint)
        {
            var conversionNotation = wgsInputPoint as IConversionNotation;
            //5 for 1m resolution
            return conversionNotation?.CreateMGRS(5, true, esriMGRSModeEnum.esriMGRSMode_Automatic);
        }

        public string ConvertToUtm(IPoint wgsInputPoint)
        {
            var conversionNotation = wgsInputPoint as IConversionNotation;
            return conversionNotation?.GetUTMFromCoords(esriUTMConversionOptionsEnum.esriUTMAddSpaces);
        }

        public IPoint ConvertToDecimalDegrees(IPoint point, CoordinateSystemModel coordinateSystemModel)
        {
            var bufferPoint = new PointClass { X = point.X, Y = point.Y, SpatialReference = point.SpatialReference };

            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            //Create Spatial Reference
            ISpatialReference spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(coordinateSystemModel.ESRIWellKnownID);
            spatialReference.SetFalseOriginAndUnits(coordinateSystemModel.FalseOriginX, coordinateSystemModel.FalseOriginY, coordinateSystemModel.Units);
            bufferPoint.Project(spatialReference);

            return bufferPoint;
        }

        public void CopyCoordinatesToClipboard(List<PointModel> pointModels)
        {
            Clipboard.Clear();
            Clipboard.SetText(_dataImportExport.GetStringRepresentationOfProjections(pointModels));
        }

        public void CopyCoordinatesToClipboard(ExtendedPointModel pointModel)
        {
            if (pointModel == null) return;

            Clipboard.Clear();
            Clipboard.SetText(pointModel.ToString());
        }

        public IPoint GetDisplayCenter()
        {
            if (!(_arcMapApp.Document is IMxDocument currentDocument)) throw new Exception(NoMapExceptionMessage);
            var centerPoint = new Point();

            var activeView = currentDocument.ActiveView;
            var envelope = activeView.Extent as IEnvelope;
            centerPoint.X = ((envelope.XMax - envelope.XMin) / 2) + envelope.XMin;
            centerPoint.Y = ((envelope.YMax - envelope.YMin) / 2) + envelope.YMin;
            centerPoint.SpatialReference = envelope.SpatialReference;

            return centerPoint;
        }

        public IPoint GetSelectedPoint(int mousePositionX, int mousePositionY)
        {
            if (!(_arcMapApp.Document is IMxDocument currentDocument)) throw new Exception(NoMapExceptionMessage);

            IPoint resultPoint = new Point();

            resultPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(mousePositionX, mousePositionY);
            return resultPoint;
        }

        public async Task MoveToNewCoordinate(double x, double y)
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

        public IPoint CreatePoint(double X, double Y, CoordinateSystemModel geoModel, bool createGeoCoordinateSystem = false)
        {
            var resultPoint = new PointClass();
            resultPoint.PutCoords(X, Y);
            //Create Spatial Reference Factory
            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();

            ISpatialReference spatialReference;

            if (createGeoCoordinateSystem)
            {
                //Geographical Coordinate System
                spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(geoModel.ESRIWellKnownID);
            }
            else
            {
                //Projected Coordinate System to project into
                spatialReference = spatialReferenceFactory.CreateProjectedCoordinateSystem(geoModel.ESRIWellKnownID);
            }
            spatialReference.SetFalseOriginAndUnits(geoModel.FalseOriginX, geoModel.FalseOriginY, geoModel.Units);

            resultPoint.SpatialReference = spatialReference;

            return resultPoint;
        }

        public IPoint ProjectPoint(IPoint inputPoint, CoordinateSystemModel singleProjectionModel)
        {
            if (inputPoint == null) return null;

            var bufferPoint = new PointClass { X = inputPoint.X, Y = inputPoint.Y, SpatialReference = inputPoint.SpatialReference };

            //Create Spatial Reference Factory
            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            //Projected Coordinate System to project into
            var projectedCoordinateSystem = spatialReferenceFactory.CreateProjectedCoordinateSystem(singleProjectionModel.ESRIWellKnownID);
            projectedCoordinateSystem.SetFalseOriginAndUnits(singleProjectionModel.FalseOriginX, singleProjectionModel.FalseOriginY, singleProjectionModel.Units);

            bufferPoint.Project(projectedCoordinateSystem);

            return bufferPoint;
        }

        public IPoint ProjectWgsToPulkovoWithGeoTransformation(IPoint inputPoint, CoordinateSystemModel coordinateSystemModel, esriTransformDirection transformationDirection)
        {
            if (inputPoint == null) return null;

            var bufferPoint = new PointClass { X = inputPoint.X, Y = inputPoint.Y, SpatialReference = inputPoint.SpatialReference };

            //Create Spatial Reference Factory
            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            var targetSpatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(coordinateSystemModel.ESRIWellKnownID);

            var coordinateFrameGeoTransformation = new CoordinateFrameTransformationClass();
            coordinateFrameGeoTransformation.PutSpatialReferences(bufferPoint.SpatialReference, targetSpatialReference);
            coordinateFrameGeoTransformation.PutParameters(Constants.PulkovoToWGS.XAxisTranslation, 
                                                           Constants.PulkovoToWGS.YAxisTranslation, 
                                                           Constants.PulkovoToWGS.ZAxisTranslation, 
                                                           Constants.PulkovoToWGS.XAxisRotation,
                                                           Constants.PulkovoToWGS.YAxisRotation,
                                                           Constants.PulkovoToWGS.ZAxisRotation,
                                                           Constants.PulkovoToWGS.ScaleDifference);

            var geometry = bufferPoint as IGeometry5;
            geometry.ProjectEx(targetSpatialReference, transformationDirection, coordinateFrameGeoTransformation, false, 0.0, 0.0);

            return geometry as IPoint;
        }

        public IPoint ProjectWgsToUrkaine2000WithGeoTransformation(IPoint inputPoint, CoordinateSystemModel coordinateSystemModel, esriTransformDirection transformationDirection)
        {
            if (inputPoint == null) return null;

            var bufferPoint = new PointClass { X = inputPoint.X, Y = inputPoint.Y, SpatialReference = inputPoint.SpatialReference };

            //Create Spatial Reference Factory
            var spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            var targetSpatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(coordinateSystemModel.ESRIWellKnownID);

            var compositeGeoTransformation = new CompositeGeoTransformationClass();
            var predefinedGeoTransformation = spatialReferenceFactory.CreateGeoTransformation(Constants.ItrfToWgsGeoTransformationID) as IGeoTransformation;
            compositeGeoTransformation.Add(esriTransformDirection.esriTransformReverse, predefinedGeoTransformation);

            var coordinateFrameGeoTransformation = new CoordinateFrameTransformationClass();
            var itrfSpatialReference = spatialReferenceFactory.CreateSpatialReference((int)esriSRGeoCS3Type.esriSRGeoCS_IERSTerrestrialReferenceFrame2000);
            coordinateFrameGeoTransformation.PutSpatialReferences(itrfSpatialReference, targetSpatialReference);
            coordinateFrameGeoTransformation.PutParameters(Constants.UkraineToItrf.XAxisTranslation,
                                                           Constants.UkraineToItrf.YAxisTranslation,
                                                           Constants.UkraineToItrf.ZAxisTranslation,
                                                           Constants.UkraineToItrf.XAxisRotation,
                                                           Constants.UkraineToItrf.YAxisRotation,
                                                           Constants.UkraineToItrf.ZAxisRotation,
                                                           Constants.UkraineToItrf.ScaleDifference);            
            compositeGeoTransformation.Add(esriTransformDirection.esriTransformForward, coordinateFrameGeoTransformation);

            var geometry = bufferPoint as IGeometry5;
            geometry.ProjectEx(targetSpatialReference, transformationDirection, compositeGeoTransformation, false, 0.0, 0.0);

            return geometry as IPoint;
        }

        public async Task<IPoint> ProjectSelectedPointAsync(int targetCoordinateSystemType, int mousePositionX, int mousePositionY, double falseOriginX = 0, double falseOriginY = 0)
        {
            if (!(_arcMapApp.Document is IMxDocument currentDocument)) throw new Exception(NoMapExceptionMessage);

            var resultPoint = await Task.Run(() =>
            {
                var mouseMapPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(mousePositionX, mousePositionY);
                if (mouseMapPoint == null) return null;
                currentDocument.FocusMap.ClearSelection();
                // Select using the shape (point) to
                // select the feature(s) - false to select any intersecting, true to select just the first
                try
                {
                    currentDocument.FocusMap.SelectByShape(mouseMapPoint, (_arcMapApp as IMxApplication).SelectionEnvironment, false);
                }
                catch (NullReferenceException) { return null; }
                var selectedFeatures = (IEnumFeature)currentDocument.FocusMap.FeatureSelection;
                var selectedFeature = selectedFeatures.Next();
                if (selectedFeature == null) return null;
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
                var bufferPoint = new Point();
                bufferPoint.PutCoords(bufferXCoord / featuresCount, bufferYCoord / featuresCount);
                return bufferPoint;
            });

            return ProjectPoint(resultPoint, new CoordinateSystemModel(targetCoordinateSystemType, falseOriginX, falseOriginY, "", currentDocument.FocusMap.MapScale));
        }

        public async Task SaveProjectionsToXmlFileAsync(List<PointModel> pointModels, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            await _dataImportExport.ExportProjectionsToXmlAsync(pointModels, path);
        }

        public async Task SaveLastProjectionToXmlFileAsync(PointModel pointModel, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            await _dataImportExport.ExportProjectionsToXmlAsync(pointModel, path);
        }

        public async Task SaveLastProjectionToCsvFileAsync(PointModel pointModel, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            await _dataImportExport.ExportProjectionsToCsvAsync(pointModel, path);
        }

        public async Task SaveProjectionsToCsvFileAsync(List<PointModel> pointModels, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            await _dataImportExport.ExportProjectionsToCsvAsync(pointModels, path);
        }

        public Task<List<PointModel>> ImportProjectionsFromXmlAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return Task.FromResult<List<PointModel>>(null);

            return _dataImportExport.ImportProjectionsFromXmlAsync(path);
        }

        public Task<List<PointModel>> ImportProjectionsFromCsvAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return Task.FromResult<List<PointModel>>(null);

            return _dataImportExport.ImportProjectionsFromCsvAsync(path);
        }

        private Tuple<IGeoTransformation, esriTransformDirection> GetPredefinedTransformation(SpatialReferenceEnvironmentClass spatialReferenceFactory, ISpatialReference fromSpatialReference, ISpatialReference toSpatialReference)
        {
            int? fromFactoryCode = GetGCSFactoryCode(fromSpatialReference);
            int? toFactoryCode = GetGCSFactoryCode(toSpatialReference);

            if (!fromFactoryCode.HasValue || !toFactoryCode.HasValue) return null;

            var geographicTransformationsSet = spatialReferenceFactory.CreatePredefinedGeographicTransformations();

            geographicTransformationsSet.Reset();

            var geoTransformation = geographicTransformationsSet.Next() as IGeoTransformation;

            try
            {
                while (geoTransformation != null)
                {
                    geoTransformation.GetSpatialReferences(out ISpatialReference bufferForFromGeoCoordSystemSR, out ISpatialReference bufferForToGeoCoordSystemSR);

                    if (bufferForFromGeoCoordSystemSR.FactoryCode == fromFactoryCode && bufferForToGeoCoordSystemSR.FactoryCode == toFactoryCode)
                    {
                        return new Tuple<IGeoTransformation, esriTransformDirection>(geoTransformation, esriTransformDirection.esriTransformForward);
                    }

                    if (bufferForFromGeoCoordSystemSR.FactoryCode == toFactoryCode && bufferForToGeoCoordSystemSR.FactoryCode == fromFactoryCode)
                    {
                        return new Tuple<IGeoTransformation, esriTransformDirection>(geoTransformation, esriTransformDirection.esriTransformReverse);
                    }
                    geoTransformation = geographicTransformationsSet.Next() as IGeoTransformation;
                }
            }
            finally
            {
                Marshal.ReleaseComObject(geographicTransformationsSet);
            }
            return null;
        }

        private int? GetGCSFactoryCode(ISpatialReference spatialReference)
        {
            if (spatialReference is IProjectedCoordinateSystem projectedCoordinateSystem)
                return projectedCoordinateSystem.GeographicCoordinateSystem.FactoryCode;
            else if (spatialReference is IGeographicCoordinateSystem geographicCoordinateSystem)
                return geographicCoordinateSystem.FactoryCode;
            else
                return null;
        }
    }
}
