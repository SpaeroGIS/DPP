using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using A = MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.SurfaceProfile.Actions
{
    class BuildStackVisibilityAction : A.Action<VisibilityCalculationResult>
    {
        private IFeatureClass observPointsfeatureClass;
        private IFeatureClass observStationsfeatureClass;
        private string rasterSource;
        private string outputSourceName;
        private short visibilityPercent;
        private VisibilityTask session;

        private VisibilityCalculationResultsEnum calcResults;
        private List<string> results;
        /// <summary>
        /// Ids are selected to expotr for visibility calculation
        /// </summary>
        private int[] pointsFilteringIds;
        private int[] stationsFilteringIds;

        private VisibilityCalculationResult result;

        public BuildStackVisibilityAction() : base()
        {
        }

        public BuildStackVisibilityAction(IActionProcessor parameters)
                : base(parameters)
        {

            observPointsfeatureClass = parameters.GetParameterWithValidition<IFeatureClass>(ActionParameters.FeatureClass, null).Value;
            observStationsfeatureClass = parameters.GetParameter<IFeatureClass>(ActionParameters.FeatureClassX, null).Value;
            rasterSource = parameters.GetParameterWithValidition<string>(ActionParameters.ProfileSource, null).Value;
            pointsFilteringIds = parameters.GetParameterWithValidition<int[]>(ActionParameters.FilteringPointsIds, null).Value;
            stationsFilteringIds = parameters.GetParameterWithValidition<int[]>(ActionParameters.FilteringStationsIds, null).Value;
            calcResults = parameters.GetParameterWithValidition<VisibilityCalculationResultsEnum>(ActionParameters.Calculationresults, VisibilityCalculationResultsEnum.None).Value;
            outputSourceName = parameters.GetParameterWithValidition<string>(ActionParameters.OutputSourceName, string.Empty).Value;
            session = parameters.GetParameterWithValidition<VisibilityTask>(ActionParameters.Session, null).Value;
            visibilityPercent = parameters.GetParameterWithValidition<short>(ActionParameters.VisibilityPercent, -1).Value;
        }

        public override string ActionId => ActionsEnum.vblt.ToString();

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
               {
                   new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClass, Value = null},
                   new ActionParam<IFeatureClass>() { ParamName = ActionParameters.FeatureClassX, Value = null},
                   new ActionParam<int[]>() { ParamName = ActionParameters.FilteringStationsIds, Value = null},
                   new ActionParam<int[]>() { ParamName = ActionParameters.FilteringPointsIds, Value = null},
                   new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = string.Empty},
                   new ActionParam<VisibilityCalculationResultsEnum>() { ParamName = ActionParameters.Calculationresults, Value = VisibilityCalculationResultsEnum.None},
                   new ActionParam<string>() { ParamName = ActionParameters.OutputSourceName, Value = null},
                   new ActionParam<string>() { ParamName = ActionParameters.Session, Value = null},
                   new ActionParam<short>() { ParamName = ActionParameters.VisibilityPercent, Value = -1}
               };
            }
        }


        public override VisibilityCalculationResult GetResult()
        {
            return result;
        }

        public override void Process()
        {
            result = new VisibilityCalculationResult();
            result.Result.Session = session;

            results = new List<string>();

            if (calcResults == VisibilityCalculationResultsEnum.None)
            {
                string errorMessage = $"The value {VisibilityCalculationResultsEnum.None.ToString()} is not acceptable for calculatuion results";
                logger.WarnEx(errorMessage);
                result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                return;
            }

            if (!(calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster)
                || calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle)))
            {
                string errorMessage = $"The values {VisibilityCalculationResultsEnum.ObservationPoints.ToString()} and {VisibilityCalculationResultsEnum.VisibilityAreaRaster.ToString()} must be in calculatuion results";
                logger.WarnEx(errorMessage);
                result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                return;
            }

            try
            {
                var messages = ProcessObservationPoint(results);
                //Here is the place to extend handle the rest of results 
                result.Result.CalculationMessages = results;

                if (results != null && results.Count > 0)
                {
                    results.ForEach(r => { logger.InfoEx(r); });
                }

                if (messages != null && messages.Any())
                {
                    //messages.ToList().ForEach(m => {if (result.Exception != null) logger.ErrorEx(m); else logger.InfoEx(m);});
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            finally
            {
                result.Result.Session.TaskLog = string.Join(System.Environment.NewLine, results);
            }

        }

        private IEnumerable<string> ProcessObservationPoint(List<string> results)
        {
            logger.InfoEx("> ProcessObservationPoint START");

            int iStepNum = 1;
            IEnumerable<string> messages = null;

            string oservStationsFeatureClassName = null;
            //Used to clip the base immge to make it smaller and reduce the culculation time
            string potentialAreaFeatureClassName = null;

            //Handle Observation Objects
            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
            {
                oservStationsFeatureClassName = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.ObservationObjects, outputSourceName);
                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(
                    observStationsfeatureClass as IDataset,
                    oservStationsFeatureClassName,
                    stationsFilteringIds);
                if (!string.IsNullOrWhiteSpace(exportedFeatureClass))
                {
                    results.Add(iStepNum.ToString() + ". " + "Створено копію ОН для розрахунку: " + exportedFeatureClass);
                    iStepNum++;
                }
                else
                {
                    string errorMessage = $"The feature calss {oservStationsFeatureClassName} was not exported";
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx("> ProcessObservationPoint ERROR ExportObservationFeatureClass (1). errorMessage:{0}", errorMessage);
                    messages.Append(errorMessage);
                    results.Add("Помилка: " + errorMessage);
                    return messages;
                }
            }
            else if (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
            {
                oservStationsFeatureClassName = observStationsfeatureClass.AliasName;
            }


            //Handle Observation Points
            List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>> pointsIDs =
                new List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>>();

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster) && !calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
            {
                pointsIDs.Add(
                    new KeyValuePair<VisibilityCalculationResultsEnum, int[]>(
                        VisibilityCalculationResultsEnum.ObservationPoints,
                        pointsFilteringIds));
            }

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationPointSingle))
            {

                if (pointsFilteringIds.Length > 1)
                {
                    pointsIDs.AddRange(
                        pointsFilteringIds.Select(
                            id => new KeyValuePair<VisibilityCalculationResultsEnum, int[]>(
                                VisibilityCalculationResultsEnum.ObservationPointSingle, new int[] { id }))
                                .ToArray());
                }
                else
                {
                    calcResults &= ~VisibilityCalculationResultsEnum.ObservationPointSingle;
                    calcResults &= ~VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle;
                }
            }

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
            {
                pointsIDs.AddRange(
                        pointsFilteringIds.Select(
                            id => new KeyValuePair<VisibilityCalculationResultsEnum, int[]>(
                                VisibilityCalculationResultsEnum.ObservationPointSingle, new int[] { id }))
                                .ToArray());
            }

            int index = -1;
            bool removeFullImageFromresult = false;
            int[] objIds = null;
            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
            {
                objIds = stationsFilteringIds;
            }

            var coverageTableManager = new CoverageTableManager();
            var bestOPParametersManager = new BestOPParametersManager();


            foreach (var curPoints in pointsIDs)
            {
                //curPoints.Key is VisibilityCalculationresultsEnum.ObservationPoints or VisibilityCalculationresultsEnum.ObservationPointSingle

                var pointId = curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ? -1 : ++index;
                var observPointFeatureClassName = VisibilityTask.GetResultName(curPoints.Key, outputSourceName, pointId);

                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(
                    observPointsfeatureClass as IDataset,
                    observPointFeatureClassName,
                    curPoints.Value);


                if (string.IsNullOrWhiteSpace(exportedFeatureClass))
                {
                    string errorMessage = $"The feature calss {observPointFeatureClassName} was not exported";
                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx("> ProcessObservationPoint ERROR ExportObservationFeatureClass. errorMessage:{0}", errorMessage);
                    results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                    return messages;
                }

                if (curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints
                        && calcResults.HasFlag(VisibilityCalculationResultsEnum.CoverageTable))
                {
                    coverageTableManager.SetCalculateAreas(exportedFeatureClass, oservStationsFeatureClassName);

                    var visibilityPotentialAreaFCName =
                    VisibilityCalcResults.GetResultName(pointId > -1 ?
                    VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle :
                    VisibilityCalculationResultsEnum.VisibilityAreasPotential, outputSourceName, pointId);

                    coverageTableManager.AddPotentialArea(
                        visibilityPotentialAreaFCName,
                        (curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints), pointId + 1);

                    results.Add(iStepNum.ToString() + ". " + "Розраховано потенційне покриття: " + visibilityPotentialAreaFCName + " ПС: " + pointId.ToString());

                    //Calc protentilly visiblw area as Image
                    var fc = GenerateWorknArea(visibilityPotentialAreaFCName, observPointFeatureClassName, outputSourceName);

                    results.Add(iStepNum.ToString() + ". " + "Розраховано потенційне покриття для розрахунку: " + fc.AliasName + " ПС: " + pointId.ToString());


                    //Try to clip the raster source by visibilityPotentialAreaFCName
                    var visibilityPotentialAreaImgName =
                    VisibilityCalcResults.GetResultName(VisibilityCalculationResultsEnum.VisibilityRastertPotentialArea, outputSourceName, pointId);
                    if (!CalculationLibrary.ClipVisibilityZonesByAreas(
                           rasterSource,
                           visibilityPotentialAreaImgName,
                           fc.AliasName,
                           messages, "NONE"))
                    {
                        string errorMessage = $"The result {visibilityPotentialAreaImgName} was not generated";
                        result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                        result.ErrorMessage = errorMessage;
                        logger.ErrorEx("> ProcessObservationPoint ERROR ClipVisibilityZonesByAreas. errorMessage:{0}", errorMessage);
                        results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                        return messages;
                    }
                    else
                    {
                        results.Add(iStepNum.ToString() + ". " + "Розраховано покриття для розрахунку на основі потенційноі видимості: " + visibilityPotentialAreaImgName + " ПС: " + pointId.ToString());
                        rasterSource = visibilityPotentialAreaImgName;
                        //Delete temporary Featureclass usewd for clipping the base image by potential area
                        GdbAccess.Instance.RemoveFeatureClass(fc.AliasName);
                    }

                }

                results.Add(iStepNum.ToString() + ". " + "Створено копію ПС для розрахунку: " + exportedFeatureClass);
                iStepNum++;

                //Generate Visibility Raster
                string featureClass = observPointFeatureClassName;
                string outImageName = VisibilityTask.GetResultName(
                    curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ?
                    VisibilityCalculationResultsEnum.VisibilityAreaRaster :
                    VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle,
                    outputSourceName,
                    pointId);

                if (!CalculationLibrary.GenerateVisibilityData(
                    rasterSource,
                    featureClass,
                    VisibilityAnalysisTypesEnum.Frequency,
                    outImageName,
                    messages))
                {
                    string errorMessage = $"The result {outImageName} was not generated";
                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx("> ProcessObservationPoint ERROR ConvertRasterToPolygon. errorMessage:{0}", errorMessage);
                    results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                    return messages;
                }
                else
                {
                    results.Add(iStepNum.ToString() + ". " + "Розраховано видимість: " + outImageName + " ПС: " + pointId.ToString());
                    iStepNum++;

                    string visibilityArePolyFCName = null;
                    //ConvertToPolygon full visibility area
                    if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaPolygons)
                        && !calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
                    {
                        visibilityArePolyFCName =
                            VisibilityTask.GetResultName(pointId > -1 ?
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle :
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygons, outputSourceName, pointId);

                        if (!CalculationLibrary.ConvertRasterToPolygon(outImageName, visibilityArePolyFCName, out messages))
                        {
                            if (!messages.Any(m => m.StartsWith("ERROR 010151"))) // Observatioj areas dont intersect Visibility aresa
                            {
                                string errorMessage = $"The result {visibilityArePolyFCName} was not generated";
                                result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                                result.ErrorMessage = errorMessage;
                                logger.ErrorEx("> ProcessObservationPoint ERROR ConvertRasterToPolygon. errorMessage:{0}", errorMessage);
                                results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                                return messages;
                            }

                            results.Add(iStepNum.ToString() + ". " + "Видимисть відсутня. Полігони не були конвертовані: " + visibilityArePolyFCName + " ПС: " + pointId.ToString());
                            visibilityArePolyFCName = string.Empty;
                        }
                        else
                        {
                            results.Add(iStepNum.ToString() + ". " + "Конвертовано у полігони: " + visibilityArePolyFCName + " ПС: " + pointId.ToString());
                        }
                        iStepNum++;
                    }

                    //Clip 
                    if (!string.IsNullOrEmpty(oservStationsFeatureClassName))
                    {
                        var inClipName = outImageName;

                        var resultLype =
                            pointId > -1 ?
                            VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle :
                            VisibilityCalculationResultsEnum.VisibilityObservStationClip;

                        var outClipName = VisibilityTask.GetResultName(resultLype,
                            outputSourceName, pointId);

                        if (!CalculationLibrary.ClipVisibilityZonesByAreas(
                            inClipName,
                            outClipName,
                            oservStationsFeatureClassName,
                            messages))
                        {
                            string errorMessage = $"The result {outClipName} was not generated";
                            result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                            result.ErrorMessage = errorMessage;
                            logger.ErrorEx("> ProcessObservationPoint ERROR ClipVisibilityZonesByAreas. errorMessage:{0}", errorMessage);
                            results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                            return messages;
                        }
                        else
                        {
                            results.Add(iStepNum.ToString() + ". " + "Зона видимості зведена до дійсного розміру: " + outClipName + " ПС: " + pointId.ToString());
                            iStepNum++;

                            if (!calcResults.HasFlag(resultLype))
                            {
                                calcResults |= resultLype;
                            }

                            //Change to VisibilityAreaPolygonForObjects
                            var curCulcRResult =
                                pointId > -1 ?
                                VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle :
                                VisibilityCalculationResultsEnum.VisibilityAreaPolygons;

                            visibilityArePolyFCName =
                                VisibilityTask.GetResultName(pointId > -1 ?
                                VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle :
                                VisibilityCalculationResultsEnum.VisibilityAreaPolygons, outputSourceName, pointId);

                            visibilityArePolyFCName = VisibilityTask.GetResultName(curCulcRResult, outputSourceName, pointId);

                            var rasterDataset = GdbAccess.Instance.GetDatasetFromCalcWorkspace(
                                outClipName, VisibilityCalculationResultsEnum.VisibilityAreaRaster);
                            bool isEmpty = EsriTools.IsRasterEmpty((IRasterDataset2)rasterDataset);

                            if (isEmpty)
                            {
                                if (calcResults.HasFlag(curCulcRResult))
                                {
                                    calcResults &= ~curCulcRResult;
                                }
                                results.Add(iStepNum.ToString() + ". " + "Видимість відсутня. Полігони не було сформовано: " + visibilityArePolyFCName + " ПС: " + pointId.ToString());
                            }
                            else
                            {
                                if (!CalculationLibrary.ConvertRasterToPolygon(outClipName, visibilityArePolyFCName, out messages))
                                {
                                    if (!messages.Any(m => m.StartsWith("ERROR 010151"))) // Observatioj areas dont intersect Visibility area
                                    {
                                        string errorMessage = $"The result {visibilityArePolyFCName} was not generated";
                                        result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                                        result.ErrorMessage = errorMessage;
                                        logger.ErrorEx("> ProcessObservationPoint ERROR ConvertRasterToPolygon. errorMessage:{0}", errorMessage);
                                        results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                                        return messages;
                                    }
                                    results.Add(iStepNum.ToString() + ". " + "Видимість відсутня. Полігони не було сформовано: " + visibilityArePolyFCName + " ПС: " + pointId.ToString());
                                }
                                else
                                {
                                    results.Add(iStepNum.ToString() + ". " + "Конвертовано у полігони: " + visibilityArePolyFCName + " ПС: " + pointId.ToString());
                                }
                            }
                            iStepNum++;
                        }
                    }
                    else if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly)
                        && !string.IsNullOrEmpty(visibilityArePolyFCName))
                    {
                        //Clip visibility images to valuable extent
                        var inClipName = outImageName;
                        var outClipName = VisibilityTask.GetResultName(pointId > -1 ?
                          VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle :
                          VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly, outputSourceName, pointId);

                        if (!CalculationLibrary.ClipVisibilityZonesByAreas(
                            inClipName,
                            outClipName,
                            visibilityArePolyFCName,
                            messages,
                            "NONE"))
                        {
                            string errorMessage = $"The result {outClipName} was not generated";
                            result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                            result.ErrorMessage = errorMessage;
                            logger.ErrorEx("> ProcessObservationPoint ERROR ClipVisibilityZonesByAreas. errorMessage:{0}", errorMessage);
                            results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                            return messages;
                        }
                        else
                        {
                            results.Add(iStepNum.ToString() + ". " + "Зона видимості зведена до дійсного розміру: " + outClipName + " ПС: " + pointId.ToString());
                            iStepNum++;
                            removeFullImageFromresult = true;
                        }
                    }

                    if (calcResults.HasFlag(VisibilityCalculationResultsEnum.CoverageTable))
                    {
                        //var visibilityPotentialAreaFCName =
                        //VisibilityTask.GetResultName(pointId > -1 ?
                        //VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle :
                        //VisibilityCalculationResultsEnum.VisibilityAreasPotential, outputSourceName, pointId);

                        //coverageTableManager.AddPotentialArea(
                        //    visibilityPotentialAreaFCName,
                        //    (curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints),  pointId + 1);

                        //results.Add(iStepNum.ToString() + ". " + "Розраховано потенційне покриття: " + visibilityPotentialAreaFCName + " ПС: " + pointId.ToString());
                        iStepNum++;

                        var pointsCount = pointsFilteringIds.Where(id => id > -1).Count();
                        coverageTableManager.CalculateCoverageTableDataForPoint(
                            (pointId == -1),
                            visibilityArePolyFCName,
                            pointsCount,
                            exportedFeatureClass,
                            pointId + 1);

                        results.Add(iStepNum.ToString() + ". " + "Сформовані записи таблиці покриття. для ПС: " + pointId.ToString());
                        iStepNum++;
                    }

                    if (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
                    {
                        if (pointId != -1)
                        {
                            bestOPParametersManager.FindVisibilityPercent(visibilityArePolyFCName, observStationsfeatureClass,
                                                                                   stationsFilteringIds, pointsFilteringIds[pointId]);

                            results.Add(iStepNum.ToString() + ". " + "Знайдено відсоток покриття для кроку " + pointId.ToString());
                            iStepNum++;
                        }
                    }
                }
            }

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.CoverageTable))
            {
                var coverageTable = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.CoverageTable, outputSourceName);
                coverageTableManager.SaveDataToCoverageTable(coverageTable);

                results.Add(iStepNum.ToString() + ". " + "Збережена загальна таблиця покриття: " + coverageTable);
                iStepNum++;
            }

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
            {
                var bestParamsTableName = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.BestParametersTable, outputSourceName);

                if (bestOPParametersManager.CreateVOTable(observPointsfeatureClass,
                                                            visibilityPercent, bestParamsTableName))
                {
                    results.Add(iStepNum.ToString() + ". " + "Збережена таблиця накращих параметрів ПН для мінімальної видимості " + visibilityPercent + "%: " + bestParamsTableName);
                    iStepNum++;
                }
                else
                {
                    results.Add(iStepNum.ToString() + ". " + "Відсутні результати для видимості " + visibilityPercent +
                                    "%. Збережена таблиця з накращими можливими параметрами ПН: " + bestParamsTableName);
                    iStepNum++;
                }
            }

            //Set real results to show
            if (removeFullImageFromresult)
            {
                if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster))
                { calcResults &= ~VisibilityCalculationResultsEnum.VisibilityAreaRaster; }
                if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle))
                { calcResults &= ~VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle; }
            }

            session.CalculatedResults = (int)calcResults;

            logger.InfoEx("> ProcessObservationPoint END");
            return messages;
        }

        private static IFeatureClass GenerateWorknArea(string poterniallyVisibleAreaFCName, string pointsFCName, string workinAreaFCName)
        {
            var points = EsriTools.GetFeatreClassExtent(GdbAccess.Instance.GetCalcWorkspaceFeatureClass(pointsFCName));
            var poterniallyVisibleArea = EsriTools.GetFeatreClassExtent(GdbAccess.Instance.GetCalcWorkspaceFeatureClass(poterniallyVisibleAreaFCName));

            poterniallyVisibleArea.Project(EsriTools.Wgs84Spatialreference);
            points.Project(EsriTools.Wgs84Spatialreference);

            var combined = EsriTools.GetEnvelopeOfGeometriesList(new IGeometry[] { points, poterniallyVisibleArea });

            var workinAreaFC = GdbAccess.Instance.GenerateTempStorage(workinAreaFCName, null, esriGeometryType.esriGeometryEnvelope, null, false, true);

            var area = workinAreaFC.CreateFeature();
            area.Shape = combined;
            area.Store();
            return workinAreaFC;
        }
    }
}
