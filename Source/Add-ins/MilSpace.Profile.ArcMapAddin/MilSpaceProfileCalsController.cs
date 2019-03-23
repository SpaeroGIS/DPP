using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile
{

    /// <summary>
    /// MilSpace Profile Calcs Controller
    /// </summary>
    public class MilSpaceProfileCalsController
    {

        private Dictionary<ProfileSettingsPointButton, IPoint> pointsToShow = new Dictionary<ProfileSettingsPointButton, IPoint>()
        {
            {ProfileSettingsPointButton.CenterFun, null},
            {ProfileSettingsPointButton.PointsFist, null},
            {ProfileSettingsPointButton.PointsSecond, null}
        };

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
            pointsToShow[ProfileSettingsPointButton.PointsFist] = pointToShow;
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
            pointsToShow[ProfileSettingsPointButton.PointsSecond] = pointToShow;
        }

        /// <summary>
        /// Set values to the text boxes of the center of fun 
        /// </summary>
        /// <param name="pointToView">The point in WGS 84 to be shown on the form</param>
        /// <param name="pointToShow">The point in Map Spatial Reference to be shown on the map</param>
        internal void SetCenterPointForFunProfile(IPoint pointToView, IPoint pointToShow)
        {
            pointsToShow[ProfileSettingsPointButton.CenterFun] = pointToShow;
            View.FunPropertiesCenterPoint = pointToView;
        }

        internal void FlashPoint(ProfileSettingsPointButton pointType, IActiveView view)
        {
            IEnvelope env = new EnvelopeClass();
            env = view.Extent;
            env.CenterAt(pointsToShow[pointType]);
            view.Extent = env;
            view.Refresh();
            EsriTools.FLashGeometry(view.ScreenDisplay, pointsToShow[pointType]);
        }

        internal ILine GetProfileLine()
        {
            ILine segment = new LineClass();
            segment.FromPoint = pointsToShow[ProfileSettingsPointButton.PointsFist];
            segment.ToPoint = pointsToShow[ProfileSettingsPointButton.PointsSecond];


            return segment;
        }
        internal IMilSpaceProfileView View { get; private set; }
    }
}
