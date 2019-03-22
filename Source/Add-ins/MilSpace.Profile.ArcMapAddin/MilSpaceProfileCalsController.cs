using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile
{

    public class MilSpaceProfileCalsController
    {
        private IPoint linePropertiesFirstPoint;
        private IPoint linePropertiesSecondPoint;
        private IPoint funPropertiesCenterPoint;

        internal MilSpaceProfileCalsController() { }

        internal void SetView(IMilSpaceProfileView view)
        {
            View = view;
        }

        internal void SetFirsPointForLineProfile(IPoint point)
        {
            linePropertiesFirstPoint = 
            View.LinePropertiesFirstPoint = point;
        }

        internal void SetSecondfPointForLineProfile(IPoint point)
        {
            linePropertiesSecondPoint = View.LinePropertiesSecondPoint = point;
        }

        internal void SetCenterPointForFunProfile(IPoint point)
        {
            funPropertiesCenterPoint = View.FunPropertiesCenterPoint = point;
        }

        internal IMilSpaceProfileView View { get; private set; }
    }
}
