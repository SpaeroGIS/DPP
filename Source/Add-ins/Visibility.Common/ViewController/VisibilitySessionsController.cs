using ESRI.ArcGIS.Carto;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visibility.ViewController
{
    public class VisibilitySessionsController
    {
        private IObservationPointsView _view;
        private List<VisibilityTask> _visibilitySessions = new List<VisibilityTask>();
        private List<VisibilityCalcResults> _visibilityResults = new List<VisibilityCalcResults>();
        private static Dictionary<VisibilitySessionStateEnum, string> _states = Enum.GetValues(typeof(VisibilitySessionStateEnum)).Cast<VisibilitySessionStateEnum>().ToDictionary(t => t, ts => ts.ToString());
        private static Dictionary<VisibilityCalcTypeEnum, string> _calcTypes = Enum.GetValues(typeof(VisibilityCalcTypeEnum)).Cast<VisibilityCalcTypeEnum>().ToDictionary(t => t, ts => ts.ToString());

        public VisibilitySessionsController()
        {
            VisibilityManager.OnGenerationStarted += UpdateVisibilitySessionsList;
        }

        internal void SetView(IObservationPointsView view)
        {
            _view = view;
        }

        internal IEnumerable<string> GetVisibilitySessionStateTypes()
        {
            return _states.Select(t => t.Value);
        }

        internal IEnumerable<string> GetVisibilityCalcTypesStrings()
        {
            return _calcTypes.Select(t => t.Value);
        }

        internal string GetStringForStateType(VisibilitySessionStateEnum type)
        {
            return _states[type];
        }

        internal string GetStringForCalcType(VisibilityCalcTypeEnum type)
        {
            return _calcTypes[type];
        }

        internal string GetImgName(VisibilityCalculationresultsEnum resultType)
        {
            if(resultType == VisibilityCalculationresultsEnum.ObservationPoints || resultType == VisibilityCalculationresultsEnum.ObservationPointSingle)
            {
                return "Flag.png";
            }

            if(resultType == VisibilityCalculationresultsEnum.ObservationStations)
            {
                return "Target.png";
            }

            if(resultType == VisibilityCalculationresultsEnum.VisibilityAreaRaster || resultType == VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle
                || resultType == VisibilityCalculationresultsEnum.VisibilityAreaPolygons || resultType == VisibilityCalculationresultsEnum.VisibilityObservStationClip)
            {
                return "Dots Up.png";
            }

            return string.Empty;
        }

        internal void UpdateVisibilitySessionsList(bool isNewSessionAdded = false)
        {
            _visibilitySessions = VisibilityZonesFacade.GetAllVisibilityTasks(true).ToList();
            _view.FillVisibilitySessionsList(_visibilitySessions, isNewSessionAdded);
        }
        internal void UpdateVisibilityResultsTree(bool isNewSessionAdded = false)
        {
            _visibilityResults = VisibilityZonesFacade.GetAllVisibilityResults(true).ToList();

            _view.FillVisibilityResultsTree(_visibilityResults);
        }

        internal VisibilityTask GetSession(string id)
        {
            return _visibilitySessions.FirstOrDefault(session => session.Id == id);
        }

        internal Dictionary<VisibilityCalcTypeEnum, string> GetCalcTypes() => _calcTypes;
        internal IEnumerable<VisibilityCalcResults> GetAllResults() => _visibilityResults;

        public IEnumerable<VisibilityTask> GetAllSessions() => _visibilitySessions;

        internal bool RemoveSession(string id)
        {
            var removedSession = _visibilitySessions.First(session => session.Id == id);

            var result = VisibilityZonesFacade.DeleteVisibilitySession(id);

            if(result)
            {
                _visibilitySessions.Remove(removedSession);
                _view.RemoveSessionFromList(id);
            }

            return result;
        }

        internal bool RemoveResult(string id, IActiveView activeView = null, bool fromBase = false)
        {
            var selectedResults = _visibilityResults.First(res => res.Id == id);
            var results = selectedResults.Results();
            var removingResult = true;

            if(fromBase)
            {
                foreach(var result in results)
                {
                    if(result != selectedResults.Id)
                    {
                        if(!EsriTools.RemoveDataSet(selectedResults.ReferencedGDB, result))
                        {
                            return false;
                        }
                    }
                }

                removingResult = VisibilityZonesFacade.DeleteVisibilityResults(id);

                if(removingResult)
                {
                    RemoveSession(id);
                    if(activeView != null)
                    {
                        EsriTools.RemoveLayer(selectedResults.Name, activeView.FocusMap);
                    }
                }
            }

            if(removingResult)
            {
                _visibilityResults.Remove(selectedResults);
            }

            return removingResult;
        }

        internal bool ShareResults(string id)
        {
            var selectedResults = _visibilityResults.First(res => res.Id == id);
            if(!selectedResults.Shared)
            {
                selectedResults.Shared = true;
                VisibilityZonesFacade.UpdateVisibilityResults(selectedResults);
                return true;
            }

            return false;
        }

        internal bool AddSharedResults(IEnumerable<VisibilityCalcResults> results)
        {
            var res = true;

            foreach(var result in results)
            {
                if(!VisibilityZonesFacade.AddSharedVisibilityResultsToUserSession(result))
                {
                    res = false;
                }
                else
                {
                    _visibilityResults.Add(result);
                }
            }

            return res;
        }

        internal bool IsResultsLayerExist(string resultsId, IActiveView activeView)
        {
            var selectedResults = _visibilityResults.First(res => res.Id == resultsId).Name;
            var layer = EsriTools.GetLayer(selectedResults, activeView.FocusMap);

            return (layer != null);
        }

        internal void AddResultsGroupLayer(string id, IActiveView activeView)
        {
            var selectedResults = _visibilityResults.First(res => res.Id == id);

            var datasets = GdbAccess.Instance.GetDatasetsFromCalcWorkspace(selectedResults.ResultsInfo);
            EsriTools.AddVisibilityGroupLayer(datasets, selectedResults.Name, selectedResults.Id, selectedResults.ReferencedGDB, GetLastLayer(activeView),
                                                true, 33, activeView);
        }

        private string GetLastLayer(IActiveView activeView)
        {
            var map = activeView.FocusMap;
            return map.Layer[map.LayerCount - 1].Name;
        }
    }
}
 