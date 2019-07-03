using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile.ModalWindows
{
    internal class AccessibleProfilesController
    {
        private ProfileSession[] _accessibleProfilesSets;
        private Dictionary<ProfileSettingsTypeEnum, string> _typesDisplayValues = new Dictionary<ProfileSettingsTypeEnum, string>();
        private Dictionary<string, ProfileSettingsTypeEnum> _typesValues = new Dictionary<string, ProfileSettingsTypeEnum>();

        internal AccessibleProfilesController(List<ProfileSession> userSession, ISpatialReference spatialReference)
        {
            SetAccessibleProfilesSets(userSession, spatialReference);
        }

        internal ProfileSession[] GetAllAccessibleProfilesSets()
        {
            return (ProfileSession[])_accessibleProfilesSets.Clone();
        }

        internal ProfileSession[] FilterByName(string name, ProfileSession[] sessions)
        {
            return sessions.Where(session => session.SessionName.Contains(name)).ToArray();
        }

        internal ProfileSession[] FilterByCreator(string creator ,ProfileSession[] sessions)
        {
            foreach(var session in sessions)
            {
                if (session.CreatedBy.Contains(creator))
                {

                }
            }
            return sessions.Where(session => session.CreatedBy.Contains(creator)).ToArray();
        }

        internal ProfileSession[] FilterByDate(DateTime fromDate, DateTime toDate, ProfileSession[] sessions)
        {
            return sessions.Where(session => session.CreatedOn >= fromDate && session.CreatedOn <= toDate).ToArray();
        }

        internal ProfileSession[] FilterByType(ProfileSettingsTypeEnum type, ProfileSession[] sessions)
        {
            return sessions.Where(session => session.DefinitionType == type).ToArray();
        }

        internal DateTime GetMinDateTime(ProfileSession[] sessions = null)
        {
            if (sessions == null)
            {
                sessions = _accessibleProfilesSets;
            }

            return sessions.Min(session => session.CreatedOn);
        }

        internal DateTime GetMaxDateTime(ProfileSession[] sessions = null)
        {
            if(sessions == null)
            {
                sessions = _accessibleProfilesSets;
            }

            return sessions.Max(session => session.CreatedOn);
        }

        internal Dictionary<ProfileSettingsTypeEnum, string> GetGraphDisplayTypes()
        {
            if(_typesDisplayValues == null || _typesDisplayValues.Count == 0)
            {
                _typesDisplayValues.Add(ProfileSettingsTypeEnum.Points, /*LocalizationConstants.PointsTypeText*/"Lines");
                _typesDisplayValues.Add(ProfileSettingsTypeEnum.Fun, /*LocalizationConstants.FunTypeText*/"Fun");
                _typesDisplayValues.Add(ProfileSettingsTypeEnum.Primitives, /*LocalizationConstants.PrimitiveTypeText*/"Primitive");
            }

            return _typesDisplayValues;
        }

        internal Dictionary<string, ProfileSettingsTypeEnum> GetGraphTypes()
        {
            if(_typesValues == null || _typesValues.Count == 0)
            {
                _typesValues.Add(/*LocalizationConstants.PointsTypeText*/"Lines", ProfileSettingsTypeEnum.Points);
                _typesValues.Add(/*LocalizationConstants.FunTypeText*/"Fun", ProfileSettingsTypeEnum.Fun);
                _typesValues.Add(/*LocalizationConstants.PrimitiveTypeText*/"Primitive", ProfileSettingsTypeEnum.Primitives);
            }

            return _typesValues;
        }

        private void SetAccessibleProfilesSets(List<ProfileSession> userSession, ISpatialReference spatialReference)
        {
            var currentSessionProfiles = new List<ProfileSession>(userSession);

            var allAccessibleProfilesSets = MilSpaceProfileFacade.GetAvaibleProfileSessions().ToList();
            var notInCurrentSessionProfilesSet = new List<ProfileSession>(allAccessibleProfilesSets.Count);

            foreach(var set in allAccessibleProfilesSets)
            {
                var currentSessionSet = currentSessionProfiles.FirstOrDefault(session => session.SessionId == set.SessionId);

                if (currentSessionSet != null)
                {
                    currentSessionProfiles.Remove(currentSessionSet);
                }
                else
                {
                    notInCurrentSessionProfilesSet.Add(set);
                }
            }

            notInCurrentSessionProfilesSet.ForEach(session =>
            {
                session.ConvertLinesToEsriPolypile(spatialReference);
            });

            _accessibleProfilesSets = notInCurrentSessionProfilesSet.ToArray();
        }


    }
}
