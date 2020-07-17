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
        private bool showAllResults;
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
            showAllResults = parameters.GetParameterWithValidition<bool>(ActionParameters.ShowAllResults, false).Value;
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
                   new ActionParam<short>() { ParamName = ActionParameters.VisibilityPercent, Value = -1},
                   new ActionParam<bool>() { ParamName = ActionParameters.ShowAllResults, Value = false}
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
            logger.InfoEx($"> ProcessObservationPoint START with parameters {calcResults}");


            Dictionary<int, List<string>> generatedResults = null;
            if (!showAllResults)
            {
                logger.InfoEx("Show all results was not set");
                generatedResults = new Dictionary<int, List<string>>();
            }


            int iStepNum = 1;
            IEnumerable<string> messages = null;

            string oservStationsFeatureClassName = null;
            //Used to clip the base immge to make it smaller and reduce the culculation time
            int currentPointIdForBestParamsCalculations = pointsFilteringIds.Last();

            //Handle Observation Objects
            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
            {
                oservStationsFeatureClassName = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.ObservationObjects, outputSourceName);
                logger.InfoEx($"Export observation objects to {oservStationsFeatureClassName}");
                //Add result To Delete if not to show all results. Add OP result
                if (!showAllResults)
                {
                    //pointResults = 
                    generatedResults.Add(-1, new List<string>() { oservStationsFeatureClassName });
                }
                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(
                    observStationsfeatureClass as IDataset,
                    oservStationsFeatureClassName,
                    stationsFilteringIds);
                if (!string.IsNullOrWhiteSpace(exportedFeatureClass))
                {
                    logger.InfoEx($"Observation points feature class  was created:{exportedFeatureClass}");
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
                logger.InfoEx($"Export observation objects to {oservStationsFeatureClassName}");
            }


            //Handle Observation Points
            List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>> pointsIDs =
                new List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>>();

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster) &&
                !calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
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
            BestOPParametersManager bestOPParametersManager = null;
            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
            {
                bestOPParametersManager = new BestOPParametersManager(visibilityPercent, currentPointIdForBestParamsCalculations, showAllResults);
            }

            // There is no reason to clip image for the BestParameters mode, since the point just shance its Z value
            var clipSourceImageForEveryPoint = true;

            //Used to define a result layer name;
            int pointIndex = -1;


            var orifinalResterSource = rasterSource;

            foreach (var curPoints in pointsIDs)
            {
                int pointId;

                if (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable))
                {
                    pointId = currentPointIdForBestParamsCalculations;
                    pointIndex = pointId - 1;
                }
                else
                {
                    pointId = curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ? -1 : ++index;
                    pointIndex = pointId;
                }

                logger.InfoEx($"Start processing point id {pointId} with index {pointIndex}");

                if (!showAllResults)// Cretae the list to add datasets to delete
                {
                    generatedResults.Add(pointId, new List<string>());
                }

                int[] curPointsValue = (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable)) ?
                                            new int[] { pointId } : curPoints.Value;

                var observPointFeatureClassName = VisibilityTask.GetResultName(curPoints.Key, outputSourceName, pointIndex);
                logger.InfoEx($"Processing point(s) into {observPointFeatureClassName}");
                //Add result To Delete if not to show all results. Add OP result
                if (!showAllResults && generatedResults.ContainsKey(pointId))
                {
                    generatedResults[pointId].Add(observPointFeatureClassName);
                }

                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(
                    observPointsfeatureClass as IDataset,
                    observPointFeatureClassName,
                    curPointsValue);

                results.Add(iStepNum.ToString() + ". " + "Створено копію ПС для розрахунку: " + exportedFeatureClass);
                logger.InfoEx($"Point(s) was exported into {observPointFeatureClassName} to process visibility");
                iStepNum++;

                if (string.IsNullOrWhiteSpace(exportedFeatureClass))
                {
                    string errorMessage = $"The feature calss {observPointFeatureClassName} was not exported";
                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx("> ProcessObservationPoint ERROR ExportObservationFeatureClass. errorMessage:{0}", errorMessage);
                    results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                    return messages;
                }

                bool clipSoutceImageByPotentialArea = clipSourceImageForEveryPoint && (curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints || curPoints.Key == VisibilityCalculationResultsEnum.ObservationPointSingle);

                if (clipSoutceImageByPotentialArea)
                {
                    logger.InfoEx($"Clipping calc image to potential area");
                    string featureClassName = exportedFeatureClass;
                    if (calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable) && curPoints.Value.First() == 1)
                    {
                        featureClassName = observPointsfeatureClass.AliasName;
                    }
                    logger.InfoEx($"Calculate potential area for observation point in {featureClassName} and objects from {oservStationsFeatureClassName}");
                    coverageTableManager.SetCalculateAreas(featureClassName, oservStationsFeatureClassName, !calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable));

                    clipSourceImageForEveryPoint = false;// !calcResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable);

                    var visibilityPotentialAreaFCName = VisibilityCalcResults.GetResultName(VisibilityCalculationResultsEnum.VisibilityAreasPotential, outputSourceName);


                    coverageTableManager.AddPotentialArea(
                    visibilityPotentialAreaFCName,
                    (curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ||
                    curPoints.Key == VisibilityCalculationResultsEnum.ObservationPointSingle), pointId + 1);

                    results.Add(iStepNum.ToString() + ". " + "Розраховано потенційне покриття: " + visibilityPotentialAreaFCName + " ПС: " + pointId.ToString());

                    //Calc protentilly visiblw area as Image
                    var fc = GenerateWorknArea(visibilityPotentialAreaFCName, observPointFeatureClassName, outputSourceName);

                    results.Add(iStepNum.ToString() + ". " + "Розраховано потенційне покриття для розрахунку: " + fc.AliasName + " ПС: " + pointId.ToString());


                    //Try to clip the raster source by visibilityPotentialAreaFCName
                    var visibilityPotentialAreaImgName =
                    VisibilityCalcResults.GetResultName(pointIndex > -1 ?
                                        VisibilityCalculationResultsEnum.VisibilityRastertPotentialAreaSingle :
                                        VisibilityCalculationResultsEnum.VisibilityRastertPotentialArea,
                                        outputSourceName, pointIndex);
                    //Add result To Delete if not to show all results. Add OP result
                    if (!showAllResults && generatedResults.ContainsKey(pointId))
                    {
                        generatedResults[pointId].Add(visibilityPotentialAreaImgName);
                    }

                    if (!CalculationLibrary.ClipVisibilityZonesByAreas(
                           rasterSource,
                           visibilityPotentialAreaImgName,
                           fc.AliasName,
                           out messages, "NONE"))
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

                //Generate Visibility Raster
                string featureClass = observPointFeatureClassName;
                string outImageName = VisibilityTask.GetResultName(
                    curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ?
                    VisibilityCalculationResultsEnum.VisibilityAreaRaster :
                    VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle,
                    outputSourceName,
                    pointIndex);

                //Add result To Delete if not to show all results. Add Visibility raster result
                if (!showAllResults && generatedResults.ContainsKey(pointId))
                {
                    generatedResults[pointId].Add(outImageName);
                }

                if (!CalculationLibrary.GenerateVisibilityData(
                    rasterSource,
                    featureClass,
                    VisibilityAnalysisTypesEnum.Frequency,
                    outImageName,
                    out messages))
                {
                    string errorMessage = $"The result {outImageName} was not generated";
                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx("> ProcessObservationPoint ERROR ConvertRasterToPolygon. errorMessage:{0}", errorMessage);
                    logger.ErrorEx("> ProcessObservationPoint ERROR ConvertRasterToPolygon. errorMessage:{0}", errorMessage);
                    results.Add("Помилка: " + errorMessage + " ПС: " + pointId.ToString());
                    return messages;
                }
                else
                {
                    results.Add(iStepNum.ToString() + ". " + "Розраховано видимість: " + outImageName + " ПС: " + pointId.ToString());
                    iStepNum++;
                    logger.InfoEx($"Visibility image {outImageName} for point {pointId} was generated");

                    string visibilityArePolyFCName = null;
                    //ConvertToPolygon full visibility area
                    if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaPolygons)
                        && !calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
                    {
                        logger.InfoEx($"Calculation of {VisibilityCalculationResultsEnum.VisibilityAreaPolygons} (Convert visible areas to polygon) was swithced on");
                        visibilityArePolyFCName =
                            VisibilityTask.GetResultName(pointIndex > -1 ?
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle :
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygons, outputSourceName, pointIndex);

                        //Add result To Delete if not to show all results. Add Visibility polygon result
                        if (!showAllResults && generatedResults.ContainsKey(pointId))
                        {
                            generatedResults[pointId].Add(visibilityArePolyFCName);
                        }

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
                            logger.InfoEx($"There is no visibile areas for OP {pointId}");

                        }
                        else
                        {
                            results.Add(iStepNum.ToString() + ". " + "Конвертовано у полігони: " + visibilityArePolyFCName + " ПС: " + pointId.ToString());
                            logger.InfoEx($"Visibile areas for OP {pointId} was converted into {visibilityArePolyFCName} ");

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
                            outputSourceName, pointIndex);
                        //Add result To Delete if not to show all results. Add Visibility polygon result
                        if (!showAllResults && generatedResults.ContainsKey(pointId))
                        {
                            generatedResults[pointId].Add(outClipName);
                        }

                        logger.InfoEx($"Try to clip the visible areas {inClipName} by orservation  object {oservStationsFeatureClassName}");

                        if (!CalculationLibrary.ClipVisibilityZonesByAreas(
                            inClipName,
                            outClipName,
                            oservStationsFeatureClassName,
                            out messages))
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
                            logger.InfoEx($"Visible area {inClipName} was clipped by orservation objects {oservStationsFeatureClassName}");

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


                            visibilityArePolyFCName = VisibilityTask.GetResultName(curCulcRResult, outputSourceName, pointIndex);

                            var rasterDataset = GdbAccess.Instance.GetDatasetFromCalcWorkspace(
                                outClipName, VisibilityCalculationResultsEnum.VisibilityAreaRaster);
                            bool isEmpty = EsriTools.IsRasterEmpty((IRasterDataset2)rasterDataset);

                            if (isEmpty)
                            {
                                if (calcResults.HasFlag(curCulcRResult))
                                {
                                    calcResults &= ~curCulcRResult;
                                }
                                results.Add($"{iStepNum.ToString()}. Видимість відсутня. Полігони {visibilityArePolyFCName} не було сформовано. ПС: {pointId.ToString()}");
                                logger.InfoEx($"There is no visible areas in {visibilityArePolyFCName}. Point {pointId}");
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
                                    logger.InfoEx($"There is no visible areas. {visibilityArePolyFCName} was not generated. Point {pointId}");
                                }
                                else
                                {
                                    //Add result To Delete if not to show all results. Add Visibility polygon result
                                    if (!showAllResults && generatedResults.ContainsKey(pointId))
                                    {
                                        generatedResults[pointId].Add(visibilityArePolyFCName);
                                    }
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
                        var outClipName = VisibilityTask.GetResultName(pointIndex > -1 ?
                          VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle :
                          VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly, outputSourceName, pointIndex);


                        if (!CalculationLibrary.ClipVisibilityZonesByAreas(
                            inClipName,
                            outClipName,
                            visibilityArePolyFCName,
                            out messages,
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
                            //Add result To Delete if not to show all results. Add Visibility polygon result
                            if (!showAllResults && generatedResults.ContainsKey(pointId))
                            {
                                generatedResults[pointId].Add(outClipName);
                            }
                            results.Add(iStepNum.ToString() + ". " + "Зона видимості зведена до дійсного розміру: " + outClipName + " ПС: " + pointId.ToString());
                            iStepNum++;
                            removeFullImageFromresult = true;
                        }
                    }

                    if (calcResults.HasFlag(VisibilityCalculationResultsEnum.CoverageTable))
                    {
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
                            currentPointIdForBestParamsCalculations = bestOPParametersManager.FindVisibilityPercent(visibilityArePolyFCName, observStationsfeatureClass,
                                                                                                                   stationsFilteringIds, pointId);

                            results.Add(iStepNum.ToString() + ". " + "Знайдено відсоток покриття для кроку " + pointId.ToString());
                            iStepNum++;
                            if (currentPointIdForBestParamsCalculations < 0)
                            {
                                break;
                            }
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
                    //Delete unnecessary datasets
                    if (!showAllResults && bestOPParametersManager.AppropriateedParams != null)
                    {
                        foreach (var key in generatedResults.Keys)
                        {
                            if (!bestOPParametersManager.AppropriateedParams.Any(k => k.Key == key))
                            {
                                foreach (var fc in generatedResults[key])
                                {
                                    //try to remove feature class
                                    if (!GdbAccess.Instance.RemoveFeatureClass(fc))
                                    {
                                        // it is not FC try to remove raster
                                        GdbAccess.Instance.RemoveRasterDataset(fc);
                                    }
                                }
                            }
                        }
                    }

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
