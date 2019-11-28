using ESRI.ArcGIS.Geodatabase;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
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

            var results = new List<string>();

            if (calcResults == VisibilityCalculationResultsEnum.None)
            {
                string errorMessage = $"The value {VisibilityCalculationResultsEnum.None.ToString()} is not acceptable for calculatuion results";
                logger.WarnEx(errorMessage);
                result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                return;
            }

            if (!(calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster) || calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle)))
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
                if (messages != null && messages.Any())
                {
                    messages.ToList().ForEach(m => { if (result.Exception != null) logger.ErrorEx(m); else logger.InfoEx(m); });
                }

            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
        }


        private IEnumerable<string> ProcessObservationPoint(List<string> results)
        {

            IEnumerable<string> messages = null;

            string oservStationsFeatureClassName = null;

            //Handle Observation Objects
            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
            {
                oservStationsFeatureClassName = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.ObservationObjects, outputSourceName);
                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(obserpStationsfeatureClass as IDataset, oservStationsFeatureClassName, stationsFilteringIds);
                if (!string.IsNullOrWhiteSpace(exportedFeatureClass))
                {
                    results.Add(exportedFeatureClass);
                }
                else
                {
                    string errorMessage = $"The feature calss {oservStationsFeatureClassName} was not exported";
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx(errorMessage);
                }
            }

            //Handle Observation Points
            List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>> pointsIDs = new List<KeyValuePair<VisibilityCalculationResultsEnum, int[]>>();

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster))
            {
                pointsIDs.Add(new KeyValuePair<VisibilityCalculationResultsEnum, int[]>(VisibilityCalculationResultsEnum.ObservationPoints, pointsFilteringIds));
            }

            if (calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationPointSingle))
            {

                if (pointsFilteringIds.Length > 1)
                {
                    pointsIDs.AddRange(
                        pointsFilteringIds.Select(id => new KeyValuePair<VisibilityCalculationResultsEnum, int[]>(VisibilityCalculationResultsEnum.ObservationPointSingle, new int[] { id })).ToArray());
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
            if(calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
            {
                objIds = stationsFilteringIds;
            }

            CoverageTableManager coverageTableManager = new CoverageTableManager();

            coverageTableManager.CalculateAreas(pointsFilteringIds, objIds, obserpPointsfeatureClass, obserpStationsfeatureClass);

            foreach (var curPoints in pointsIDs)
            {
                //curPoints.Key is VisibilityCalculationresultsEnum.ObservationPoints or VisibilityCalculationresultsEnum.ObservationPointSingle

                var pointId = curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ? -1 : ++index;
                var oservPointFeatureClassName = VisibilityTask.GetResultName(curPoints.Key, outputSourceName, pointId);

                var exportedFeatureClass = GdbAccess.Instance.ExportObservationFeatureClass(obserpPointsfeatureClass as IDataset, oservPointFeatureClassName, curPoints.Value);

                if (string.IsNullOrWhiteSpace(exportedFeatureClass))
                {
                    string errorMessage = $"The feature calss {oservPointFeatureClassName} was not exported";
                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx(errorMessage);
                    return messages;
                }

                results.Add(exportedFeatureClass);

                //Generate Visibility Raster
                string featureClass = oservPointFeatureClassName;
                string outImageName = VisibilityTask.GetResultName(curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints ?
                        VisibilityCalculationResultsEnum.VisibilityAreaRaster : VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle, outputSourceName, pointId);

                if (!ProfileLibrary.GenerateVisibilityData(rasterSource, featureClass, VisibilityAnalysisTypesEnum.Frequency, outImageName, messages))
                {
                    string errorMessage = $"The result {outImageName} was not generated";
                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                    result.ErrorMessage = errorMessage;
                    logger.ErrorEx(errorMessage);
                    return messages;
                }
                else
                {
                    results.Add(outImageName);

                    string visibilityArePolyFCName = null;
                    //ConvertToPolygon full visibility area
                    if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaPolygons) && !calcResults.HasFlag(VisibilityCalculationResultsEnum.ObservationObjects))
                    {
                        visibilityArePolyFCName = VisibilityTask.GetResultName(pointId > -1 ?
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle : VisibilityCalculationResultsEnum.VisibilityAreaPolygons,
                            outputSourceName, pointId);


                        if (!ProfileLibrary.ConvertTasterToPolygon(outImageName, visibilityArePolyFCName, out messages))
                        {
                            if (!messages.Any(m => m.StartsWith("ERROR 010151"))) // Observatioj areas dont intersect Visibility aresa
                            {
                                string errorMessage = $"The result {visibilityArePolyFCName} was not generated";
                                result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                                result.ErrorMessage = errorMessage;
                                logger.ErrorEx(errorMessage);
                                return messages;
                            }

                        }
                        results.Add(visibilityArePolyFCName);
                    }

                    //Clip 
                    if (!string.IsNullOrEmpty(oservStationsFeatureClassName))
                    {
                        var inClipName = outImageName;

                        var resultLype = pointId > -1 ? VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle : VisibilityCalculationResultsEnum.VisibilityObservStationClip;

                        var outClipName = VisibilityTask.GetResultName(resultLype,
                            outputSourceName, pointId);

                        if (!ProfileLibrary.ClipVisibilityZonesByAreas(inClipName, outClipName, oservStationsFeatureClassName, messages))
                        {
                            string errorMessage = $"The result {outClipName} was not generated";
                            result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                            result.ErrorMessage = errorMessage;
                            logger.ErrorEx(errorMessage);
                            return messages;
                        }
                        else
                        {
                            results.Add(outClipName);


                            if (!calcResults.HasFlag(resultLype))
                            {
                                calcResults |= resultLype;
                            }

                            //Change to VisibilityAreaPolygonForObjects
                            visibilityArePolyFCName = VisibilityTask.GetResultName(pointId > -1 ?
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle : VisibilityCalculationResultsEnum.VisibilityAreaPolygons,
                            outputSourceName, pointId);


                            if (!ProfileLibrary.ConvertTasterToPolygon(outClipName, visibilityArePolyFCName, out messages))
                            {
                                if (!messages.Any(m => m.StartsWith("ERROR 010151"))) // Observatioj areas dont intersect Visibility aresa
                                {
                                    string errorMessage = $"The result {visibilityArePolyFCName} was not generated";
                                    result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                                    result.ErrorMessage = errorMessage;
                                    logger.ErrorEx(errorMessage);
                                    return messages;
                                }
                            }
                            results.Add(visibilityArePolyFCName);
                        }
                    }
                    else if (calcResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly) && !string.IsNullOrEmpty(visibilityArePolyFCName))
                    {
                        //Clip visibility images to valuable extent
                        var inClipName = outImageName;
                        var outClipName = VisibilityTask.GetResultName(pointId > -1 ?
                          VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle : VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly,
                          outputSourceName, pointId);

                        if (!ProfileLibrary.ClipVisibilityZonesByAreas(inClipName, outClipName, visibilityArePolyFCName, messages, "NONE"))
                        {
                            string errorMessage = $"The result {outClipName} was not generated";
                            result.Exception = new MilSpaceVisibilityCalcFailedException(errorMessage);
                            result.ErrorMessage = errorMessage;
                            logger.ErrorEx(errorMessage);
                            return messages;
                        }
                        else
                        {
                            results.Add(outClipName);
                            removeFullImageFromresult = true;
                        }
                    }

                    var visibilityPotentialAreaFCName = VisibilityTask.GetResultName(pointId > -1 ?
                            VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle : VisibilityCalculationResultsEnum.VisibilityAreasPotential,
                            outputSourceName, pointId);

                    coverageTableManager.AddPotentialArea(visibilityPotentialAreaFCName, (curPoints.Key == VisibilityCalculationResultsEnum.ObservationPoints), curPoints.Value[0]);
                    var pointsCount = pointsFilteringIds.Where(id => id > -1).Count();
                    coverageTableManager.CalculateCoverageTableDataForPoint(pointId > -1 ? curPoints.Value[0] : -1, visibilityArePolyFCName, pointsCount);

                    results.Add(visibilityPotentialAreaFCName);
                }
            }

            var coverageTable = VisibilityTask.GetResultName(VisibilityCalculationResultsEnum.CoverageTable, outputSourceName);
            coverageTableManager.SaveDataToCoverageTable(coverageTable);
            results.Add(coverageTable);
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
            return messages;
        }
    }
}
