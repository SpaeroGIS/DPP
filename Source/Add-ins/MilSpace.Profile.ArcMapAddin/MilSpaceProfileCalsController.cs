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

        /// <summary>
        /// Set values to the text boxes of the secont point on the Points tab
        /// </summary>
        /// <param name="pointToView">The point in WGS 84 to be shown on the form</param>
        /// <param name="pointToShow">The point in Map Spatial Reference to be shown on the map</param>
        internal void SetFirsPointForLineProfile(IPoint pointToView, IPoint pointToShow)
        {
            linePropertiesFirstPoint = pointToShow;
            View.LinePropertiesFirstPoint = pointToView;
        }


        /// <summary>
        /// Set values to the text boxes of the secont point on the Points tab
        /// </summary>
        /// <param name="pointToView">The point in WGS 84 to be shown on the form</param>
        /// <param name="pointToShow">The point in Map Spatial Reference to be shown on the map</param>
        internal void SetSecondfPointForLineProfile(IPoint pointToView, IPoint pointToShow)
        {
            View.LinePropertiesSecondPoint = pointToView;
            linePropertiesSecondPoint = pointToShow;
        }

        /// <summary>
        /// Set values to the text boxes of the center of fun 
        /// </summary>
        /// <param name="pointToView">The point in WGS 84 to be shown on the form</param>
        /// <param name="pointToShow">The point in Map Spatial Reference to be shown on the map</param>
        internal void SetCenterPointForFunProfile(IPoint pointToView, IPoint pointToShow)
        {
            funPropertiesCenterPoint = pointToShow;
            View.FunPropertiesCenterPoint = pointToView;
        }

        internal IMilSpaceProfileView View { get; private set; }
    }
}
