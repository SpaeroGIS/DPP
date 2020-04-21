using ESRI.ArcGIS.Geodatabase;
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
        private IFeatureClass obserpPointsfeatureClass;
        private IFeatureClass obserpStationsfeatureClass;
        private string rasterSource;
        private string outputSourceName;
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

            obserpPointsfeatureClass = parameters.GetParameterWithValidition<IFeatureClass>(ActionParameters.FeatureClass, null).Value;
            obserpStationsfeatureClass = parameters.GetParameter<IFeatureClass>(ActionParameters.FeatureClassX, null).Value;
            rasterSource = parameters.GetParameterWithValidition<string>(ActionParameters.ProfileSource, null).Value;
            pointsFilteringIds = parameters.GetParameterWithValidition<int[]>(ActionParameters.FilteringPointsIds, null).Value;
            stationsFilteringIds = parameters.GetParameterWithValidition<int[]>(ActionParameters.FilteringStationsIds, null).Value;
            calcResults = parameters.GetParameterWithValidition<VisibilityCalculationResultsEnum>(ActionParameters.Calculationresults, VisibilityCalculationResultsEnum.None).Value;
            outputSourceName = parameters.GetParameterWithValidition<string>(ActionParameters.OutputSourceName, string.Empty).Value;
            session = parameters.GetParameterWithValidition<VisibilityTask>(ActionParameters.Session, null).Value;
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
                   new ActionParam<string>() { ParamName = ActionParameters.Session, Value = null}
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

            //Handle Observation Objects
            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
            {
                oservStationsFeatureClassName = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.ObservationObjects, outputSourceName);
                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(
                    obserpStationsfeatureClass as IDataset,
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

            //Handle Observation Points
            List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>> pointsIDs =
                new List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>>();

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster))
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

            int index = -1;
            bool removeFullImageFromresult = false;
            int[] objIds = null;
            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
            {
                objIds = stationsFilteringIds;
            }

            CoverageTableManager coverageTableManager = new CoverageTableManager();
            coverageTableManager.SetCalculateAreas(pointsFilteringIds, objIds, obserpPointsfeatureClass, obserpStationsfeatureClass);

            foreach (var curPoints in pointsIDs)
            {
                //curPoints.Key is VisibilityCalculationresultsEnum.ObservationPoints or VisibilityCalculationresultsEnum.ObservationPointSingle

                var pointId = curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ? -1 : ++index;
                var observPointFeatureClassName = VisibilityTask.GetResultName(curPoints.Key, outputSourceName, pointId);

                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(
                    obserpPointsfeatureClass as IDataset,
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

                    var visibilityPotentialAreaFCName =
                        VisibilityTask.GetResultName(pointId > -1 ?
                        VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle :
                        VisibilityCalculationResultsEnum.VisibilityAreasPotential, outputSourceName, pointId);

                    coverageTableManager.AddPotentialArea(
                        visibilityPotentialAreaFCName,
                        (curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints),
                        curPoints.Value[0]);

                    results.Add(iStepNum.ToString() + ". " + "Розраховано потенційне покриття: " + visibilityPotentialAreaFCName + " ПС: " + pointId.ToString());
                    iStepNum++;

                    var pointsCount = pointsFilteringIds.Where(id => id > -1).Count();
                    coverageTableManager.CalculateCoverageTableDataForPoint(
                        (pointId == -1),
                        visibilityArePolyFCName,
                        pointsCount,
                        curPoints.Value[0]);

                    results.Add(iStepNum.ToString() + ". " + "Сформовані записи таблиці покриття. для ПС: " + pointId.ToString());
                    iStepNum++;
                }
            }

            var coverageTable = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.CoverageTable, outputSourceName);
            coverageTableManager.SaveDataToCoverageTable(coverageTable);

            results.Add(iStepNum.ToString() + ". " + "Збережена загальна таблиця покриття: " + coverageTable);
            iStepNum++;

            //Add Coverage table to Available results
            if (!calcResults.HasFlag(VisibilityCalculationResultsEnum.CoverageTable))
            {
                calcResults |= VisibilityCalculationResultsEnum.CoverageTable;
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
    }
}
