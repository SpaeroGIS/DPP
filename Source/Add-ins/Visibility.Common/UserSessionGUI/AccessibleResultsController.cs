using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visibility
{
    internal class AccessibleResultsController
    {
        private VisibilityCalcResults[] _accessibleProfilesSets;
        private Dictionary<ProfileSettingsTypeEnum, string> _typesDisplayValues = new Dictionary<ProfileSettingsTypeEnum, string>();
        private Dictionary<string, ProfileSettingsTypeEnum> _typesValues = new Dictionary<string, ProfileSettingsTypeEnum>();

        internal AccessibleResultsController(List<VisibilityCalcResults> userSession, ISpatialReference spatialReference)
        {
            SetAccessibleResultsSets(userSession, spatialReference);
        }

        internal VisibilityCalcResults[] GetAllAccessibleProfilesSets()
        {
            return (VisibilityCalcResults[])_accessibleProfilesSets.Clone();
        }

        internal VisibilityCalcResults[] FilterByName(string name, VisibilityCalcResults[] sessions)
        {
            return sessions.Where(session => session.Name.Contains(name)).ToArray();
        }

        internal VisibilityCalcResults[] FilterByCreator(string creator ,VisibilityCalcResults[] sessions)
        {
            return sessions.Where(session => session.UserName.Contains(creator)).ToArray();
        }

        internal VisibilityCalcResults[] FilterByDate(DateTime fromDate, DateTime toDate, VisibilityCalcResults[] sessions)
        {
            return sessions.Where(session => session.Created >= fromDate && session.Created <= toDate).ToArray();
        }

        internal VisibilityCalcResults[] FilterByType(VisibilityCalcTypeEnum type, VisibilityCalcResults[] sessions)
        {
            return sessions.Where(session => session.CalculationType == type).ToArray();
        }

        internal DateTime GetMinDateTime(VisibilityCalcResults[] sessions = null)
        {
            if (sessions == null)
            {
                sessions = _accessibleProfilesSets;
            }

            return sessions.Min(session => session.Created.Value);
        }

        internal DateTime GetMaxDateTime(VisibilityCalcResults[] sessions = null)
        {
            if(sessions == null)
            {
                sessions = _accessibleProfilesSets;
            }

            return sessions.Max(session => session.Created.Value);
        }

        private void SetAccessibleResultsSets(List<VisibilityCalcResults> userSession, ISpatialReference spatialReference)
        {
            var currentSessionProfiles = new List<VisibilityCalcResults>(userSession);

            var allAccessibleProfilesSets = VisibilityZonesFacade.GetAllVisibilityResults().ToList();
            var notInCurrentSessionProfilesSet = new List<VisibilityCalcResults>(allAccessibleProfilesSets.Count);

            foreach(var set in allAccessibleProfilesSets)
            {
                var currentSessionSet = currentSessionProfiles.FirstOrDefault(session => session.Id == set.Id);

                if (currentSessionSet != null)
                {
                    currentSessionProfiles.Remove(currentSessionSet);
                }
                else
                {
                    notInCurrentSessionProfilesSet.Add(set);
                }
            }


            //notInCurrentSessionProfilesSet.ForEach(session =>
            //{
            //    session.ConvertLinesToEsriPolypile(spatialReference);
            //});

            _accessibleProfilesSets = notInCurrentSessionProfilesSet.ToArray();
        }


    }
}
