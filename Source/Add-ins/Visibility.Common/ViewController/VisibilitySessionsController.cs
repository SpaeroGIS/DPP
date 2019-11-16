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
        private static Dictionary<VisibilitySessionStateEnum, string> states = Enum.GetValues(typeof(VisibilitySessionStateEnum)).Cast<VisibilitySessionStateEnum>().ToDictionary(t => t, ts => ts.ToString());

        public VisibilitySessionsController()
        {
            VisibilityManager.OnGenerationStarted += UpdateVisibilitySessionsList;
        }

        internal void SetView(IObservationPointsView view)
        {
            _view = view;
        }

        public IEnumerable<string> GetVisibilitySessionStateTypes()
        {
            return states.Select(t => t.Value);
        }
        

        public string GetStringForStateType(VisibilitySessionStateEnum type)
        {
            return states[type];
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
        public IEnumerable<VisibilityTask> GetAllSessions() => _visibilitySessions;
        
        internal bool RemoveSession(string id)
        {
            var removedSession = _visibilitySessions.First(session => session.Id == id);

           var result = VisibilityZonesFacade.DeleteVisibilitySession(id);

            if(result)
            {
                _visibilitySessions.Remove(removedSession);
            }

            return result;
        }

        internal bool RemoveResult(string id)
        {
            var selectedResults = _visibilityResults.First(res => res.Id == id);
            var results = selectedResults.Results();

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

            var removingResult = VisibilityZonesFacade.DeleteVisibilityResults(id);

            if(removingResult)
            {
                _visibilityResults.Remove(selectedResults);
            }

            return removingResult;
        }
    }
}
 