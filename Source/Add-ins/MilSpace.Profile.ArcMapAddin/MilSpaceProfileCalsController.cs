using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.Profile.DTO;
using MilSpace.Tools.GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using MilSpace.DataAccess.DataTransfer;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Desktop.AddIns;

namespace MilSpace.Profile
{

    /// <summary>
    /// MilSpace Profile Calcs Controller
    /// </summary>
    public class MilSpaceProfileCalsController
    {

        private int profileId;
        private MilSpaceProfileGraphsController graphsController = new MilSpaceProfileGraphsController();

        GraphicsLayerManager graphicsLayerManager;

        public delegate void ProfileSettingsChangedDelegate(ProfileSettingsEventArgs e);

        public event ProfileSettingsChangedDelegate OnProfileSettingsChanged;

        private static ProfileSettingsTypeEnum[] profileSettingsType = Enum.GetValues(typeof(ProfileSettingsTypeEnum)).Cast<ProfileSettingsTypeEnum>().ToArray();

        private Dictionary<ProfileSettingsPointButton, IPoint> pointsToShow = new Dictionary<ProfileSettingsPointButton, IPoint>()
        {
            {ProfileSettingsPointButton.CenterFun, null},
            {ProfileSettingsPointButton.PointsFist, null},
            {ProfileSettingsPointButton.PointsSecond, null}
        };


        private Dictionary<ProfileSettingsTypeEnum, ProfileSettings> profileSettings = new Dictionary<ProfileSettingsTypeEnum, ProfileSettings>()
        {
            {ProfileSettingsTypeEnum.Points, null},
            {ProfileSettingsTypeEnum.Fun, null},
            {ProfileSettingsTypeEnum.SelectedFeatures, null},
            {ProfileSettingsTypeEnum.Load, null}
        };



        internal MilSpaceProfileCalsController() { }

        internal void SetView(IMilSpaceProfileView view)
        {
            View = view;
            SetPeofileId();
            graphicsLayerManager = new GraphicsLayerManager(view.ActiveView);
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

            SetPeofileSettigs(ProfileSettingsTypeEnum.Points);
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

            SetPeofileSettigs(ProfileSettingsTypeEnum.Points);

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

            SetPeofileSettigs(ProfileSettingsTypeEnum.Fun);

        }

        internal void FlashPoint(ProfileSettingsPointButton pointType)
        {
            IEnvelope env = new EnvelopeClass();
            env = View.ActiveView.Extent;
            env.CenterAt(pointsToShow[pointType]);
            View.ActiveView.Extent = env;
            View.ActiveView.Refresh();
            EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, pointsToShow[pointType]);
        }

        internal ILine GetProfileLine()
        {
            ILine segment = new LineClass();
            segment.FromPoint = pointsToShow[ProfileSettingsPointButton.PointsFist];
            segment.ToPoint = pointsToShow[ProfileSettingsPointButton.PointsSecond];


            return segment;
        }
        internal IMilSpaceProfileView View { get; private set; }

        internal ProfileSettingsTypeEnum[] ProfileSettingsType => profileSettingsType;

        internal void SetPeofileSettigs(ProfileSettingsTypeEnum profileType)
        {
            List<IPolyline> profileLines = new List<IPolyline>();

            if (profileType == ProfileSettingsTypeEnum.Points)
            {
                var line = EsriTools.CreatePolylineFromPoints(pointsToShow[ProfileSettingsPointButton.PointsFist], pointsToShow[ProfileSettingsPointButton.PointsSecond]);
                if (line != null)
                {
                    profileLines.Add(line);
                }
            }

            var profileSetting = profileSettings[profileType];
            if (profileSetting == null)
            {
                profileSetting = new ProfileSettings();

            }

            profileSetting.DemLayerName = View.DemLayerName;
            profileSetting.ProfileLines = profileLines.ToArray();

            profileSettings[profileType] = profileSetting;

            InvokeOnProfileSettingsChanged();

            graphicsLayerManager.UpdateGraphic(profileSetting.ProfileLines, profileId, (int)profileType);

        }

        internal void CallGraphsHandle(ProfileSession profileSession, ProfileSettingsTypeEnum profileType)
        {
            var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileGraph.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileGraph);

             winImpl.MilSpaceProfileCalsController.ShowWindow();

            if (profileType == ProfileSettingsTypeEnum.Points)
            {
                winImpl.MilSpaceProfileCalsController.AddSession(profileSession);
            }
            else
            {
                winImpl.MilSpaceProfileCalsController.AddGraph(profileSession);
            }
        }

        private void InvokeOnProfileSettingsChanged()
        {
            if (OnProfileSettingsChanged != null)
            {
                var currentTab = View.SelectedProfileSettingsType;
                ProfileSettingsEventArgs args = new ProfileSettingsEventArgs(profileSettings[currentTab], currentTab);
                OnProfileSettingsChanged(args);

            }
        }

        private void SetPeofileId()
        {
            profileId = View.ProfileId = (int)(DateTime.Now.ToOADate() * 10000);
        }
    }
}
