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
            _accessibleProfilesSets = MilSpaceProfileFacade.GetUserProfileSessions().ToArray();
            _accessibleProfilesSets.ToList().ForEach(session =>
            {
                session.ConvertLinesToEsriPolypile(spatialReference);
            });
        }


    }
}
