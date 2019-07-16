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
        static Test3D instance = null;
        MilSpaceProfileCalsController calsController;
        ProfileSession profileSession = new ProfileSession();

        internal delegate MilSpaceProfileCalsController GetControllerDelegate();
        internal event GetControllerDelegate GetController;

        private Test3D()
        { 

        }

        public static Test3D Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Test3D();
                }
                return instance;
            }
        }

        public void SetCurrentProfile()
        {
            calsController = GetController.Invoke();
            profileSession = calsController.GetSelectedSession();
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
            catch(Exception ex) { }
        }
    }
}
