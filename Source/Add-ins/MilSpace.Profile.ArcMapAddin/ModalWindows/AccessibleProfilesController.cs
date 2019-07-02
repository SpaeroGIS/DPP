using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
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

        internal AccessibleProfilesController(List<ProfileSession> userSession, ISpatialReference spatialReference)
        {
            SetAccessibleProfilesSets(userSession, spatialReference);
        }

        internal ProfileSession[] GetAllAccessibleProfilesSets()
        {
            return _accessibleProfilesSets;
        }

        private void SetAccessibleProfilesSets(List<ProfileSession> userSession, ISpatialReference spatialReference)
        {
            var currentSessionProfiles = new List<ProfileSession>(userSession);

            var allAccessibleProfilesSets = MilSpaceProfileFacade.GetUserProfileSessions().ToList();
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
