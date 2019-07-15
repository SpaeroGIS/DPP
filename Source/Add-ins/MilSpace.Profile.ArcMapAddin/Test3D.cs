using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile
{
    public class Test3D
    {
        MilSpaceProfileCalsController milSpaceProfileCalsController = new MilSpaceProfileCalsController();
        ProfileSession profileSession = new ProfileSession();

        public Test3D()
        {
            profileSession = milSpaceProfileCalsController.GetSelectedSession();
        }

        public void SetSelectedProfileTo3D()
        {
            try
            {
                var polylines = new List<IPolyline>();

                foreach(var line in profileSession.ProfileLines)
                {
                    polylines.Add(line.Line);
                }

                GdbAccess.Instance.AddProfileLinesTo3D(polylines);
            }
            catch { }
        }
    }
}
