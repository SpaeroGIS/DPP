using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Exceptions;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.DTO;
using MilSpace.Tools;
using MilSpace.Tools.GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Profile
{

    /// <summary>
    /// MilSpace Profile Calcs Controller
    /// </summary>
    public class MilSpaceProfileCalsController
    {

        private int profileId;
        private string profileName;
        GraphicsLayerManager graphicsLayerManager;

        public delegate void ProfileSettingsChangedDelegate(ProfileSettingsEventArgs e);

        public event ProfileSettingsChangedDelegate OnProfileSettingsChanged;

        private static ProfileSettingsTypeEnum[] profileSettingsType = Enum.GetValues(typeof(ProfileSettingsTypeEnum)).Cast<ProfileSettingsTypeEnum>().ToArray();


        List<ProfileSession> _workingProfiles = new List<ProfileSession>();
        private const string NewProfilePrefix = "Новый профиль";


        private Dictionary<ProfileSettingsPointButtonEnum, IPoint> pointsToShow = new Dictionary<ProfileSettingsPointButtonEnum, IPoint>()

        {
            {ProfileSettingsPointButtonEnum.CenterFun, null},
            {ProfileSettingsPointButtonEnum.PointsFist, null},
            {ProfileSettingsPointButtonEnum.PointsSecond, null}
        };


        private Dictionary<ProfileSettingsTypeEnum, ProfileSettings> profileSettings = new Dictionary<ProfileSettingsTypeEnum, ProfileSettings>()
        {
            {ProfileSettingsTypeEnum.Points, null},
            {ProfileSettingsTypeEnum.Fun, null},
            {ProfileSettingsTypeEnum.SelectedFeatures, null},
            {ProfileSettingsTypeEnum.Load, null}
        };

        internal Dictionary<ProfileSettingsTypeEnum, ProfileSettings> ProfileSettings => profileSettings;

        private ProfileSettingsTypeEnum ProfileType => View.SelectedProfileSettingsType;


        internal MilSpaceProfileCalsController() { }

        internal void SetView(IMilSpaceProfileView view)
        {           
            View = view;
            SetPeofileId();
            SetProfileName();
        }

        /// <summary>
        /// Set values to the text boxes of the secont point on the Points tab
        /// </summary>
        /// <param name="pointToView">The point in WGS 84 to be shown on the form</param>
        /// <param name="pointToShow">The point in Map Spatial Reference to be shown on the map</param>
        internal void SetFirsPointForLineProfile(IPoint pointToView, IPoint pointToShow)
        {
            pointsToShow[ProfileSettingsPointButtonEnum.PointsFist] = pointToShow;
            View.LinePropertiesFirstPoint = pointToView;

            SetProfileSettings(ProfileSettingsTypeEnum.Points);
        }


        /// <summary>
        /// Set values to the text boxes of the secont point on the Points tab
        /// </summary>
        /// <param name="pointToView">The point in WGS 84 to be shown on the form</param>
        /// <param name="pointToShow">The point in Map Spatial Reference to be shown on the map</param>
        internal void SetSecondfPointForLineProfile(IPoint pointToView, IPoint pointToShow)
        {
            View.LinePropertiesSecondPoint = pointToView;
            pointsToShow[ProfileSettingsPointButtonEnum.PointsSecond] = pointToShow;

            SetProfileSettings(ProfileSettingsTypeEnum.Points);

        }

        /// <summary>
        /// Set values to the text boxes of the center of fun 
        /// </summary>
        /// <param name="pointToView">The point in WGS 84 to be shown on the form</param>
        /// <param name="pointToShow">The point in Map Spatial Reference to be shown on the map</param>
        internal void SetCenterPointForFunProfile(IPoint pointToView, IPoint pointToShow)
        {
            pointsToShow[ProfileSettingsPointButtonEnum.CenterFun] = pointToShow;
            View.FunPropertiesCenterPoint = pointToView;

            SetProfileSettings(ProfileSettingsTypeEnum.Fun);

        }

        internal void FlashPoint(ProfileSettingsPointButtonEnum pointType)
        {
            IEnvelope env = new EnvelopeClass();
            env = View.ActiveView.Extent;
            env.CenterAt(pointsToShow[pointType]);
            View.ActiveView.Extent = env;
            View.ActiveView.Refresh();
            EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, pointsToShow[pointType]);
            View.ActiveView.Refresh();
        }

        internal IEnumerable<IPolyline> GetProfileLines()
        {

            if (View.SelectedProfileSettingsType == ProfileSettingsTypeEnum.Points)
            {
                return new IPolyline[] { EsriTools.CreatePolylineFromPoints(pointsToShow[ProfileSettingsPointButtonEnum.PointsFist], pointsToShow[ProfileSettingsPointButtonEnum.PointsSecond]) };
            }
            if (View.SelectedProfileSettingsType == ProfileSettingsTypeEnum.Fun)
            {
                return EsriTools.CreatePolylinesFromPointAndAzimuths(pointsToShow[ProfileSettingsPointButtonEnum.CenterFun], View.FunLength, View.FunLinesCount, View.FunAzimuth1, View.FunAzimuth2);
            }

            return null;
        }
        internal IMilSpaceProfileView View { get; private set; }

        internal ProfileSettingsTypeEnum[] ProfileSettingsType => profileSettingsType;

        internal void SetProfileSettings(ProfileSettingsTypeEnum profileType)
        {
            SetSettings(profileType, profileId);
        }

        internal void SetProfileSettings(ProfileSettingsTypeEnum profileType, int profileIdValue)
        {
            SetSettings(profileType, profileIdValue);
        }

        private void SetSettings(ProfileSettingsTypeEnum profileType, int profileIdValue)
        {
            List<IPolyline> profileLines = new List<IPolyline>();

            var profileSetting = profileSettings[profileType];
            if (profileSetting == null)
            {
                profileSetting = new ProfileSettings();
            }


            //Check if the View.DemLayerName if Layer name
            profileSetting.DemLayerName = View.DemLayerName;


            if (profileType == ProfileSettingsTypeEnum.Points)
            {

                var line = EsriTools.CreatePolylineFromPoints(pointsToShow[ProfileSettingsPointButtonEnum.PointsFist], pointsToShow[ProfileSettingsPointButtonEnum.PointsSecond]);
                if (line != null)
                {
                    profileLines.Add(line);
                }

            }

            if (View.SelectedProfileSettingsType == ProfileSettingsTypeEnum.Fun)
            {
                try
                {
                    var lines = EsriTools.CreatePolylinesFromPointAndAzimuths(pointsToShow[ProfileSettingsPointButtonEnum.CenterFun], View.FunLength, View.FunLinesCount, View.FunAzimuth1, View.FunAzimuth2);
                    if (lines != null)
                    {
                        profileLines.AddRange(lines);
                    }
                }
                catch (MilSpaceProfileLackOfParameterException ex)
                {
                    //TODO: Wtite log
                    MessageBox.Show(ex.Message, "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (Exception ex)
                {
                    //TODO: Wtite log
                }

            }

            profileSetting.ProfileLines = profileLines.ToArray();

            profileSettings[profileType] = profileSetting;

            InvokeOnProfileSettingsChanged();

            GraphicsLayerManager.UpdateCalculatingGraphic(profileSetting.ProfileLines, profileIdValue, (int)profileType);
        }


        /// <summary>
        /// Do Actions to generate profile(s), save them and set properties to default values
        /// </summary>
        /// <returns>Profile Session data</returns>
        internal ProfileSession GenerateProfile()
        {
            try
            {

                ProfileManager manager = new ProfileManager();
                var profileSetting = profileSettings[View.SelectedProfileSettingsType];
                var newProfileId = GenerateProfileId();
                var newProfileName = GenerateProfileName(newProfileId);

                
                var session = manager.GenerateProfile(View.DemLayerName, profileSetting.ProfileLines, View.SelectedProfileSettingsType, newProfileId, newProfileName);

                SetPeofileId();
                SetProfileName();
                return session;

            }
            catch (Exception ex)
            {
                //TODO log error
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        internal void ShowWorkingProfile(int profileId, int lineId = -1)
        {
            var profile = _workingProfiles.FirstOrDefault(p => p.SessionId == profileId);
            if (profile != null)
            {
                if (lineId < 0)
                {
                    GraphicsLayerManager.AddLinesToWorkingGraphics(profile.ConvertLinesToEsriPolypile(ArcMap.Document.FocusMap.SpatialReference), profile.SessionId);
                }
                else if (profile.ProfileLines.Any(l => l.Id == lineId))
                {
                    GraphicsLayerManager.ShowLineOnWorkingGraphics(profileId, lineId);
                }
            }
        }

        internal void HideWorkingProfile(int profileId, int lineId = -1)
        {
            var profile = _workingProfiles.FirstOrDefault(p => p.SessionId == profileId);
            if (profile != null)
            {
                if (lineId < 0)
                {
                    GraphicsLayerManager.RemoveLinesFromWorkingGraphics(profile.ConvertLinesToEsriPolypile(ArcMap.Document.FocusMap.SpatialReference), profile.SessionId);
                }else if (profile.ProfileLines.Any(l => l.Id == lineId))
                {
                    GraphicsLayerManager.HideLineFromWorkingGraphics(profileId, lineId);
                }
            }
        }

        internal void AddProfileToList(ProfileSession profile)
        {
            bool isAddToGraphics = false;
            
            switch (profile.DefinitionType)
            {
                case ProfileSettingsTypeEnum.Points:
                    {
                        isAddToGraphics = View.AddSectionProfileNodes(profile);
                        View.AddSectionProfileToList(profile);
                        break;
                    }

                case ProfileSettingsTypeEnum.Fun:
                    {
                        isAddToGraphics = View.AddFanProfileNode(profile);
                        View.AddFanProfileToList(profile);
                        break;
                    }
            }

            //Add Profile to the working list
            _workingProfiles.Add(profile);

            //Add graphics 
            if (isAddToGraphics)
            {
               // GraphicsLayerManager.AddLinesToWorkingGraphics(profile.ConvertLinesToEsriPolypile(ArcMap.Document.FocusMap.SpatialReference), profile.SessionId);
            }

            GraphicsLayerManager.EmptyProfileGraphics(MilSpaceGraphicsTypeEnum.Calculating);
        }

        private ProfileSession GetProfileSessionFromSelectedNode()
        {
            var profileType = View.GetProfileTypeFromNode();
            var profileName = View.GetProfileNameFromNode();
            switch (profileType)
            {
                
                case ProfileSettingsTypeEnum.Points:
                    return View.GetSectionProfile(profileName);
                case ProfileSettingsTypeEnum.Fun:
                    return View.GetFanProfile(profileName);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void ShowProfileOnMap(int profileId, int lineId)
        {          
            GraphicsLayerManager.FlashLineOnWorkingGraphics(profileId, lineId);
        }

        private IPoint GetEnvelopeCenterPoint(IEnvelope envelope)
        {
            var x = (envelope.XMin + envelope.XMax) / 2;
            var y = (envelope.YMin + envelope.YMax) / 2;
            return new PointClass{X = x, Y = y};
        }





        internal void CallGraphsHandle(ProfileSession profileSession, ProfileSettingsTypeEnum profileType, double observerHeight)
        {
            var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileGraph.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileGraph);

            winImpl.MilSpaceProfileCalsController.ShowWindow();
            winImpl.MilSpaceProfileCalsController.AddSession(profileSession, observerHeight, this);
        }

        private GraphicsLayerManager GraphicsLayerManager
        {
            get
            {
                if (graphicsLayerManager == null)
                {
                    graphicsLayerManager = new GraphicsLayerManager(View.ActiveView);
                }

                return graphicsLayerManager;
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
            profileId = GenerateProfileId();
        }

        private void SetProfileName()
        {
            View.ProfileName = $"{NewProfilePrefix} {profileId}"; 
        }

        private string GenerateProfileName(int id)
        {
            if (!string.IsNullOrWhiteSpace(View.ProfileName)) return View.ProfileName;
            var result = $"{NewProfilePrefix} {id}";
            return result;

        }

        private int GenerateProfileId()
        {
            return (int)(DateTime.Now.ToOADate() * 10000);
        }
    }
}
