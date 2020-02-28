using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Exceptions;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Core.Tools;
using MilSpace.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Exceptions;
using MilSpace.DataAccess.Facade;
using MilSpace.Profile.DTO;
using MilSpace.Profile.Helpers;
using MilSpace.Profile.Localization;
using MilSpace.Profile.ModalWindows;
using MilSpace.Tools;
using MilSpace.Tools.GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Profile
{

    /// <summary>
    /// MilSpace Profile Calcs Controller
    /// </summary>
    public class MilSpaceProfileCalcsController
    {
        //TODO: Localize
        private static readonly string graphiclayerTitle = LocalizationContext.Instance.FindLocalizedElement("TxtGraphicsLayerValue", "графічні об'єкти");
        private MapLayersManager _mapLayersManager;

        private int profileId;
        GraphicsLayerManager graphicsLayerManager;
        private Logger logger = Logger.GetLoggerEx("MilSpace.Profile.MilSpaceProfileCalsController");

        public delegate void ProfileSettingsChangedDelegate(ProfileSettingsEventArgs e);

        public delegate void OnMapSelectionChangedDelegate(SelectedGraphicsArgs selectedLines);

        public event ProfileSettingsChangedDelegate OnProfileSettingsChanged;

        public event OnMapSelectionChangedDelegate OnMapSelectionChanged;

        private static ProfileSettingsTypeEnum[] profileSettingsType = Enum.GetValues(typeof(ProfileSettingsTypeEnum)).Cast<ProfileSettingsTypeEnum>().ToArray();

        private readonly string NewProfilePrefix = LocalizationContext.Instance.FindLocalizedElement("TxtNewProfileNameValue", "Новий профіль");

        List<ProfileSession> _workingProfiles = new List<ProfileSession>();

        private MilSpaceProfileGraphsController graphsController;

        private Dictionary<ProfileSettingsPointButtonEnum, IPoint> pointsToShow = new Dictionary<ProfileSettingsPointButtonEnum, IPoint>()

        {
            {ProfileSettingsPointButtonEnum.CenterFun, null},
            {ProfileSettingsPointButtonEnum.PointsFist, null},
            {ProfileSettingsPointButtonEnum.PointsSecond, null}
        };

        private Dictionary<ProfileSettingsPointButtonEnum, IPoint> startPoints = new Dictionary<ProfileSettingsPointButtonEnum, IPoint>()
        {
            {ProfileSettingsPointButtonEnum.CenterFun, null},
            {ProfileSettingsPointButtonEnum.PointsFist, null},
            {ProfileSettingsPointButtonEnum.PointsSecond, null}
        };

        private IEnumerable<IPolyline> selectedOnMapLines;
        private IEnumerable<IGeometry> _funGeometries;


        private Dictionary<ProfileSettingsTypeEnum, ProfileSettings> profileSettings = new Dictionary<ProfileSettingsTypeEnum, ProfileSettings>()
        {
            {ProfileSettingsTypeEnum.Points, null},
            {ProfileSettingsTypeEnum.Fun, null},
            {ProfileSettingsTypeEnum.Primitives, null},
            {ProfileSettingsTypeEnum.Load, null}
        };

        private Dictionary<AssignmentMethodsEnum, string> _assignmentMethods = new Dictionary<AssignmentMethodsEnum, string>()
        {
            { AssignmentMethodsEnum.FromMap, LocalizationContext.Instance.FindLocalizedElement("CmbAssignmentMethodFromMapTypeText", "Мапа") },
            { AssignmentMethodsEnum.GeoCalculator, LocalizationContext.Instance.FindLocalizedElement("CmbAssignmentMethodGeoCalcTypeText", "ГеоКалькулятор") },
            { AssignmentMethodsEnum.ObservationPoints, LocalizationContext.Instance.FindLocalizedElement("CmbAssignmentMethodObservPointsTypeText", "Шар пунктів спостереження") },
            { AssignmentMethodsEnum.FeatureLayers, LocalizationContext.Instance.FindLocalizedElement("CmbAssignmentMethodPointsLayerTypeText", "Точковий шар") }
        };

        private Dictionary<AssignmentMethodsEnum, string> _targetAssignmentMethods = new Dictionary<AssignmentMethodsEnum, string>()
        {
            { AssignmentMethodsEnum.Sector, LocalizationContext.Instance.FindLocalizedElement("CmbTargetAssignmentMethodInSectorText","У вказаному секторі")},
            { AssignmentMethodsEnum.GeoCalculator, LocalizationContext.Instance.FindLocalizedElement("CmbTargetAssignmentMethodGeoCalcText", "ГеоКалькулятор") },
            { AssignmentMethodsEnum.FeatureLayers, LocalizationContext.Instance.FindLocalizedElement("CmbTargetAssignmentMethodFeatureLayerText", "Векторний шар") },
            { AssignmentMethodsEnum.ObservationPoints, LocalizationContext.Instance.FindLocalizedElement("CmbTargetAssignmentMethodObservPointsText", "Шар пунктів спостереження") },
            { AssignmentMethodsEnum.ObservationObjects, LocalizationContext.Instance.FindLocalizedElement("CmbTargetAssignmentMethodObservObjText", "Шар об'єктів спостереження") },
            { AssignmentMethodsEnum.SelectedGraphic, LocalizationContext.Instance.FindLocalizedElement("CmbTargetAssignmentMethodSelectedGraphicText", "Обрана графіка") }
        };

        private Dictionary<ToPointsCreationMethodsEnum, string> _toPointsCreationMethods = new Dictionary<ToPointsCreationMethodsEnum, string>()
        {
            { ToPointsCreationMethodsEnum.AzimuthsCenter, LocalizationContext.Instance.FindLocalizedElement("CmbToPointsCreationMethodCenterText", "Азмимути (мін і макс), центр") },
            { ToPointsCreationMethodsEnum.AzimuthsLines, LocalizationContext.Instance.FindLocalizedElement("CmbToPointsCreationMethodLineNumberText", "К-ть ліній від мін до макс азимуту") },
            { ToPointsCreationMethodsEnum.ToVertices, LocalizationContext.Instance.FindLocalizedElement("CmbToPointsCreationMethodToVerticesText", "До вершин") },
        };

        internal Dictionary<ProfileSettingsTypeEnum, ProfileSettings> ProfileSettings => profileSettings;

        private ProfileSettingsTypeEnum ProfileType => View.SelectedProfileSettingsType;


        internal MilSpaceProfileCalcsController() { }

        internal void SetView(IMilSpaceProfileView view)
        {
            logger.InfoEx("> SetView START");
            View = view;
            SetPeofileId();
            SetProfileName();
            logger.InfoEx("> SetView END");
        }

        internal void OnDocumentsLoad()
        {
            logger.InfoEx("> OnDocumentsLoad START");

            IActiveViewEvents_Event activeViewEvents = (IActiveViewEvents_Event)View.ActiveView.FocusMap;
            IActiveViewEvents_SelectionChangedEventHandler handler = new IActiveViewEvents_SelectionChangedEventHandler(OnMapSelectionChangedLocal);
            activeViewEvents.SelectionChanged += handler;
            _mapLayersManager = new MapLayersManager(ArcMap.Document.ActiveView);

            logger.InfoEx("> OnDocumentsLoad END");
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
            logger.WarnEx("SetFirsPointForLineProfile");
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
            logger.WarnEx("SetSecondfPointForLineProfile");
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
            
            View.RecalculateFun();
        }


        internal IMilSpaceProfileView View { get; private set; }

        internal ProfileSettingsTypeEnum[] ProfileSettingsType => profileSettingsType;

        internal MilSpaceProfileGraphsController MilSpaceProfileGraphsController
        {
            get
            {
                if (graphsController == null)
                {
                    var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileGraph.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileGraph);
                    graphsController = winImpl.MilSpaceProfileGraphsController;

                    graphsController.ProfileRedrawn += GraphRedrawn;
                    graphsController.ProfileRemoved += ProfileRemove;
                    graphsController.SelectedProfileChanged += SelectedProfileChanged;
                    graphsController.IntersectionLinesDrawing += CalcIntesectionsWithLayers;
                    graphsController.CreateEmptyGraph += GenerateEmptyGraph;
                    graphsController.GetProfileName += GetProfileName;
                    graphsController.GetIsProfileShared += GetIsProfileShared;
                    graphsController.AddProfile += AddProfileToExistedGraph;
                    graphsController.GetProfileSessionById += GetProfileById;
                    graphsController.PanToSelectedProfile += PanToSelectedProfile;
                    graphsController.PanToSelectedProfilesSet += PanToSelectedProfilesSet;
                    graphsController.OnSessionsHeightsChanged += ChangeProfilesHeights;
                }

                return graphsController;
            }
        }

        internal void FlashPoint(ProfileSettingsPointButtonEnum pointType)
        {
            if(pointsToShow[pointType] == null)
            {
                return;
            }

            if(!EsriTools.IsPointOnExtent(ArcMap.Document.ActivatedView.Extent, pointsToShow[pointType]))
            {
                EsriTools.PanToGeometry(View.ActiveView, pointsToShow[pointType]);
            }

            EsriTools.FlashGeometry(pointsToShow[pointType], 500, ArcMap.Application);
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
            if (View.SelectedProfileSettingsType == ProfileSettingsTypeEnum.Primitives)
            {
                return selectedOnMapLines;
            }
            return null;
        }

        internal void SetProfileSettings(ProfileSettingsTypeEnum profileType, List<IPolyline> polylines = null)
        {
            SetSettings(profileType, profileId);
        }

        internal void SetProfileSettings(ProfileSettingsTypeEnum profileType, int profileIdValue, List<IPolyline> polylines = null)
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
                profileSetting.Type = profileType;
            }

            //Check if the View.DemLayerName if Layer name
            profileSetting.DemLayerName = View.DemLayerName;


            if (profileType == ProfileSettingsTypeEnum.Points)
            {

                var line = EsriTools.CreatePolylineFromPoints(pointsToShow[ProfileSettingsPointButtonEnum.PointsFist], pointsToShow[ProfileSettingsPointButtonEnum.PointsSecond]);
                if (line != null)
                {
                    profileLines.Add(line);

                    ILine ln = new Line()
                    {
                        FromPoint = line.FromPoint,
                        ToPoint = line.ToPoint,
                        SpatialReference = line.SpatialReference
                    };

                    View.SetProifileLineInfo(line.Length, ln.Azimuth());
                }
            }

            if (View.SelectedProfileSettingsType == ProfileSettingsTypeEnum.Fun)
            {       
                if(pointsToShow[ProfileSettingsPointButtonEnum.CenterFun] != null)
                {
                    View.RecalculateFunWithParams();
                    return;
                }
                //try
                //{
                //    var lines = EsriTools.CreatePolylinesFromPointAndAzimuths(pointsToShow[ProfileSettingsPointButtonEnum.CenterFun], View.FunLength, View.FunLinesCount, View.FunAzimuth1, View.FunAzimuth2);
                //    if(lines != null)
                //    {
                //        profileLines.AddRange(lines);
                //    }

                //    profileSetting.Azimuth1 = View.FunAzimuth1;
                //    profileSetting.Azimuth2 = View.FunAzimuth2;
                //}
                //catch(MilSpaceProfileLackOfParameterException ex)
                //{
                //    logger.ErrorEx(ex.Message);
                //    MessageBox.Show(ex.Message, Properties.Resources.AddinMessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);

                //}
                //catch(Exception ex)
                //{
                //    logger.ErrorEx(ex.Message);
                //}
            }

            if (View.SelectedProfileSettingsType == ProfileSettingsTypeEnum.Primitives)
            {
                GetSelectedGraphics();
                if (selectedOnMapLines != null)
                {
                    profileLines = selectedOnMapLines.ToList();
                }
            }

            profileSetting.ProfileLines = profileLines.ToArray();

            profileSettings[profileType] = profileSetting;
            

            InvokeOnProfileSettingsChanged();
            logger.WarnEx("");
            GraphicsLayerManager.UpdateCalculatingGraphic(profileSetting.ProfileLines, profileIdValue, (int)profileType);
        }


        /// <summary>
        /// Do Actions to generate profile(s), save them and set properties to default values
        /// </summary>
        /// <returns>Profile Session data</returns>
        internal ProfileSession GenerateProfile()
        {
            logger.DebugEx($"> GenerateProfile START");
            string errorMessage;
            try
            {
                ProfileManager manager = new ProfileManager();

                logger.DebugEx($"GenerateProfile. new ProfileManager OK");
                var profileSetting = profileSettings[View.SelectedProfileSettingsType];
                var newProfileId = GenerateProfileId();
                logger.DebugEx($"GenerateProfile.Profile. ID:{newProfileId}");
                var newProfileName = GenerateProfileName();
                logger.DebugEx($"GenerateProfile.Profile. Name:{newProfileName}");

                if (manager == null)
                {
                    logger.DebugEx("GenerateProfile. Cannot find profile manager");
                    throw new NullReferenceException("Cannot find profile manager");
                }

                if (profileSetting == null)
                {
                    logger.DebugEx("GenerateProfile. Cannot find profile manager");
                    throw new NullReferenceException("Cannot find profile manager");
                }

                logger.DebugEx($"GenerateProfile. Profile {newProfileId}. GenerateProfile CALL");
                var session = manager.GenerateProfile(
                    profileSetting.DemLayerName, 
                    profileSetting.ProfileLines, 
                    View.SelectedProfileSettingsType, 
                    newProfileId, 
                    newProfileName, 
                    View.ObserveHeight, 
                    profileSetting.AzimuthToStore);
                logger.DebugEx($"GenerateProfile. Profile {newProfileId}. GenerateProfile RETURN");

                if (session.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    session.Segments =
                        ProfileLinesConverter.GetSegmentsFromProfileLine(session.ProfileSurfaces, ArcMap.Document.FocusMap.SpatialReference);
                }
                else
                {
                    session.SetSegments(ArcMap.Document.FocusMap.SpatialReference);
                }

                SetPeofileId();
                SetProfileName();

                logger.InfoEx($"> GenerateProfile END");
                return session;

            }
            catch (MilSpaceCanotDeletePrifileCalcTable ex)
            {
                //TODO Localize error message
                logger.DebugEx($"GenerateProfile MilSpaceCanotDeletePrifileCalcTable. ex.Message:{0}", ex.Message);
                errorMessage = ex.Message;
            }
            catch (MilSpaceDataException ex)
            {
                //TODO Localize error message
                logger.DebugEx($"GenerateProfile MilSpaceDataException. ex.Message:{0}", ex.Message);
                errorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                //TODO Localize error message
                logger.DebugEx($"GenerateProfile Exception. ex.Message:{0}", ex.Message);
                errorMessage = ex.Message;
            }

            logger.InfoEx($"> GenerateProfile END with session IS NULL");
            MessageBox.Show(
                errorMessage
                );
            return null;
        }

        internal bool RemoveProfilesFromUserSession(bool eraseFromDB = false)
        {
            var result = MilSpaceProfileFacade.DeleteUserSessions(View.SelectedProfileSessionIds.ProfileSessionId);
            if (result)
            {
                if (eraseFromDB)
                {
                    result = result && MilSpaceProfileFacade.EraseProfileSessions(View.SelectedProfileSessionIds.ProfileSessionId);
                }

                _workingProfiles.Remove(_workingProfiles.First(session => session.SessionId == View.SelectedProfileSessionIds.ProfileSessionId));
                MilSpaceProfileGraphsController.RemoveTab(View.SelectedProfileSessionIds.ProfileSessionId);
                GraphicsLayerManager.RemoveGraphic(View.SelectedProfileSessionIds.ProfileSessionId);

                return View.RemoveTreeViewItem();
            }
            return true;
        }

        internal void ShowUserSessionProfile(int profileId, int lineId = -1)
        {
            var profile = GetProfileById(profileId); 
            if (profile != null)
            {
                if (lineId < 0)
                {
                    //TODO: Remove the references to ESRI lines not to store them in memory. Change creating of the tree view items to use MilSpace points (WGS84);
                    //profile.ProfileLines.Select( p => p.Line)
                    var spatialReference = ArcMap.Document.FocusMap.SpatialReference;

                    if (profile.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                    {
                        GraphicsLayerManager.AddLinesToWorkingGraphics(profile.ConvertLinesToEsriPolypile(spatialReference), profile.SessionId,
                                                                           profile.Segments.First());
                    }
                    else
                    {
                        GraphicsLayerManager.AddLinesToWorkingGraphics(profile.ConvertLinesToEsriPolypile(spatialReference), profile.SessionId);
                    }
                }
                else if (profile.ProfileLines.Any(l => l.Id == lineId))
                {
                    var sgmnt = profile.Segments.First(segment => segment.LineId == lineId);

                    sgmnt.Lines.ForEach(l => { logger.InfoEx($"WGS From Point {l.PointFrom.X}:{l.PointFrom.Y}"); logger.InfoEx($"WGS To Point {l.PointTo.X}:{l.PointTo.Y}"); }
                    );

     
                    GraphicsLayerManager.ShowLineOnWorkingGraphics(profileId,
                                                                    profile.Segments
                                                                           .First(segment => segment.LineId == lineId));
                }
            }
        }


        internal ProfileSession GetProfileById(int profileSetId)
        {
            return _workingProfiles.FirstOrDefault(p => p.SessionId == profileSetId);
        }

        internal void HideUserSessionProfile(int profileId, int lineId = -1)
        {
            var profile = GetProfileById(profileId);
            if (profile != null)
            {
                if (lineId < 0)
                {
                    //TODO: Remove the references to ESRI lines not to store them in memory. Change creating of the tree view items to use MilSpace points (WGS84);
                    //profile.ProfileLines.Select( p => p.Line)
                    GraphicsLayerManager.RemoveLinesFromWorkingGraphics(profile.Segments, profile.SessionId);
                }
                else if (profile.ProfileLines.Any(l => l.Id == lineId))
                {
                    GraphicsLayerManager.HideLineFromWorkingGraphics(profileId, lineId);
                }
            }
        }

        internal void AddProfileToList(ProfileSession profile)
        {
            bool isAddToGraphics = false;

            isAddToGraphics = View.AddNodeToTreeView(profile);

            //Add Profile to the working list
            _workingProfiles.Add(profile);

            //Add graphics 
            if (isAddToGraphics)
            {
                var spatialReference = ArcMap.Document.FocusMap.SpatialReference;

                if (profile.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    profile.Segments = ProfileLinesConverter.GetSegmentsFromProfileLine(profile.ProfileSurfaces, spatialReference);
                    GraphicsLayerManager.AddLinesToWorkingGraphics(ProfileLinesConverter.ConvertLineToPrimitivePolylines(profile.ProfileSurfaces[0],
                                                                                                                           spatialReference),
                                                                   profile.SessionId,
                                                                   profile.Segments.First());
                }
                else
                {
                    profile.SetSegments(spatialReference);
                    GraphicsLayerManager.AddLinesToWorkingGraphics(ProfileLinesConverter.ConvertSolidGroupedLinesToEsriPolylines(profile.Segments, spatialReference),
                                                                   profile.SessionId);
                }
               
            }

            GraphicsLayerManager.EmptyProfileGraphics(MilSpaceGraphicsTypeEnum.Calculating);
        }

        private ProfileSession GetProfileSessionFromSelectedNode()
        {
            var profileType = View.GetProfileTypeFromNode();
            var profileId = View.GetProfileNameFromNode();

            return _workingProfiles.FirstOrDefault(p => p.DefinitionType == profileType && p.SessionId == profileId);
        }

        internal void ShowProfileOnMap(int profileId = -1, ProfileLine line = null)
        {
            var mapScale = View.ActiveView.FocusMap.MapScale;
            ProfileSession profile;

            if(profileId == -1)
            {
                profile = GetProfileSessionFromSelectedNode();
            }
            else
            {
               profile = GetProfileSessionById(profileId);
            }

            if (profile == null)
            {
                logger.ErrorEx("Cannot find Selected Profiles set");
                return;
            }

            IEnumerable<IGeometry> profileLines;

            if(line == null)
            {
               profileLines = profile.ProfileLines.Select(profileLine => profileLine.Line as IGeometry);
            }
            else
            {
                profileLines = new List<IGeometry> { line.Line };
            }

            IEnvelope env = new EnvelopeClass();

            foreach (var profileLine in profileLines)
            {
                env.Union(profileLine.Envelope);
            }

            EsriTools.PanToGeometry(View.ActiveView, env);

            if (profile.DefinitionType == ProfileSettingsTypeEnum.Primitives)
            {
               EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, profile.Segments.First().Polylines);
            }
            else
            {
                logger.InfoEx("Flashing geomerty");
                EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, profileLines);
                logger.InfoEx("Geomerty flashed");

            }
        }

        internal void PanToFun()
        {
            var lines = profileSettings[ProfileSettingsTypeEnum.Fun].ProfileLines;
            IEnvelope env = new EnvelopeClass();

            if(lines == null)
            {
                return;
            }

            foreach(var line in lines)
            {
                env.Union(line.Envelope);
            }

            EsriTools.PanToGeometry(View.ActiveView, env);
            EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, lines);
        }

        internal void HighlightProfileOnMap(int profileId, int lineId)
        {
            var profilesToFlas = _workingProfiles.FirstOrDefault(p => p.SessionId == profileId);
            if (profilesToFlas != null)
            {
                if (profilesToFlas.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    GraphicsLayerManager.FlashLineOnWorkingGraphics(profilesToFlas.Segments.First().Polylines);
                }
                else
                {
                    GraphicsLayerManager.FlashLineOnWorkingGraphics(profilesToFlas.ConvertLinesToEsriPolypile(View.ActiveView.FocusMap.SpatialReference, lineId));
                }
            }
        }

        internal void InitiateUserProfiles()
        {
            logger.InfoEx("> InitiateUserProfiles START");
            MilSpaceProfileFacade.GetUserProfileSessions()
                .ToList()
                .ForEach(p => {p.ConvertLinesToEsriPolypile(View.ActiveView.FocusMap.SpatialReference); AddProfileToList(p); });
            OnDocumentsLoad();
            logger.InfoEx("> InitiateUserProfiles END");
        }

        internal void AddProfileToTab(int profileSessionId, int lineId)
        {
            var profileSession = GetProfileSessionById(profileSessionId);

            if (profileSession != null)
            {
                ProfileLine profileLine = null;

                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Points)
                {
                    profileLine = profileSession.ProfileLines.FirstOrDefault();
                }
                else
                {
                    profileLine = profileSession.ProfileLines.FirstOrDefault(line => line.Id == lineId);
                }

                if (profileLine != null)
                {
                    var profileSurface = profileSession.ProfileSurfaces.First(surface => surface.LineId == profileLine.Id);
                    GraphicsLayerManager.RemoveLineFromGraphic(profileSessionId, profileLine.Id);
                    profileLine.SessionId = profileSessionId;
                    var isOneLineProfile = (profileSession.DefinitionType == ProfileSettingsTypeEnum.Points || profileSession.DefinitionType == ProfileSettingsTypeEnum.Primitives);
                    graphsController.AddProfileToTab(profileLine, profileSurface, isOneLineProfile);
                }
            }
        }

        internal void AddAvailableSets()
        {
            var accessibleProfilesSetsModalWindow = new AccessibleProfilesModalWindow(_workingProfiles, ArcMap.Document.FocusMap.SpatialReference);
            var dialogResult = accessibleProfilesSetsModalWindow.ShowDialog(); 

            if (dialogResult == DialogResult.OK)
            {
                var profilesSets = accessibleProfilesSetsModalWindow.SelectedProfilesSets;

                foreach(var set in profilesSets)
                {
                    AddProfileToList(set);
                    MilSpaceProfileFacade.SaveUserSession(set.SessionId);
                }
            }
        }

        internal void CallGraphsHandle(int profileSessionId)
        {
            var profileSession = GetProfileSessionById(profileSessionId);

            if (profileSession != null)
            {
                if (_workingProfiles.FirstOrDefault(profile => profile.SessionId == profileSession.SessionId) != null)
                {
                    _workingProfiles.Remove(_workingProfiles.FirstOrDefault(profile => profile.SessionId == profileSession.SessionId));
                }
                _workingProfiles.Add(profileSession);

                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    profileSession.Segments = ProfileLinesConverter.GetSegmentsFromProfileLine(profileSession.ProfileSurfaces, ArcMap.Document.FocusMap.SpatialReference);
                }
                else
                {
                    profileSession.SetSegments(ArcMap.Document.FocusMap.SpatialReference);
                }
                logger.DebugEx("CallGraphsHandle Root");
                CallGraphsHandle(profileSession);
            }
        }

        /// <summary>
        /// Insert or Update the profile set in DB
        /// </summary>
        /// <param name="profileSet"></param>
        internal bool SaveProfileSet(ProfileSession profileSet)
        {
            //Write to DB
            bool res = MilSpaceProfileFacade.SaveProfileSession(profileSet);
            if (!res)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorHappendText, Properties.Resources.AddinMessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return res;

        }

        internal void CallGraphsHandle(ProfileSession profileSession)
        {
            MilSpaceProfileGraphsController.ShowWindow();
            MilSpaceProfileGraphsController.AddSession(profileSession);
        }

        internal void ShowGraphsWindow()
        {
            MilSpaceProfileGraphsController.ShowWindow();
        }

        internal bool? ShareProfileSession(ProfileSession profile)
        {
            if (profile.CreatedBy == Environment.UserName)
            {
                profile.Shared = true;
                return SaveProfileSet(profile);
            }

            logger.ErrorEx("You are not allowed to share this Profile.");
            return null;
        }

        internal bool CanEraseProfileSession(int profileSession)
        {
            if (profileSession < 0)
            {
                return false;
            }

            return MilSpaceProfileFacade.CanEraseProfileSessions(profileSession); ;
        }

        internal string GetProfileName(int id)
        {
            var session = GetProfileSessionById(id);
            if (session != null)
            {
              return  session.SessionName;
            }

            return null;
        }

        internal bool GetIsProfileShared(int id)
        {
            try
            {
                var session = GetProfileSessionById(id);
                if(session != null)
                {
                    return session.Shared;
                }

                throw new MilSpaceProfileNotFoundException(id); 
            }
            catch(MilSpaceProfileNotFoundException ex)
            {
                logger.ErrorEx(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.AddinMessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        internal void AddProfileToExistedGraph()
        {
            var profilesTreeModalWindow = new ProfilesTreeModalWindow(View.GetTreeView());
            DialogResult result = profilesTreeModalWindow.ShowDialog();

            if (result == DialogResult.OK)
            {
                AddProfileToTab(profilesTreeModalWindow.SelectedSessionId, profilesTreeModalWindow.SelectedLineId);
            }

        }

        internal void ClearMapFromOldGraphs()
        {
            GraphicsLayerManager.ClearMapFromOldGraphics();
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
            View.ProfileName = $"{NewProfilePrefix} {GenerateProfileNameSuffix()}";
        }

        private ProfileSession GetProfileSessionById(int profileId)
        {
            var profile = MilSpaceProfileFacade.GetProfileSessionById(profileId);
            profile.ConvertLinesToEsriPolypile(View.ActiveView.FocusMap.SpatialReference);

            if (profile.DefinitionType == ProfileSettingsTypeEnum.Primitives)
            {
                profile.SetSegments(View.ActiveView.FocusMap.SpatialReference);
            }

            return profile;
        }


        private string GenerateProfileName()
        {
            if (!string.IsNullOrWhiteSpace(View.ProfileName)) return View.ProfileName;
            var result = $"{NewProfilePrefix} {GenerateProfileNameSuffix()}";
            return result;

        }

        private int GenerateProfileId()
        {
            return (int)(DateTime.Now.ToOADate() * 10000);
        }

        private string GenerateProfileNameSuffix()
        {
            return DataAccess.Helper.GetTemporaryNameSuffix();
        }

        private IPoint GetEnvelopeCenterPoint(IEnvelope envelope)
        {
            var x = (envelope.XMin + envelope.XMax) / 2;
            var y = (envelope.YMin + envelope.YMax) / 2;
            return new PointClass { X = x, Y = y };
        }

        private void GraphRedrawn(GroupedLines profileLines, int sessionId, bool update, List<int> linesIds = null)
        {
            if (update)
            {
                GraphicsLayerManager
                        .UpdateGraphicLine(profileLines, sessionId);
            }
            else
            {
                if (profileLines.LineId == 1)
                {
                    GraphicsLayerManager.RemoveGraphic(sessionId, linesIds);
                }

                GraphicsLayerManager
                    .AddLinesToWorkingGraphics(profileLines.Polylines, sessionId, profileLines);
            }
        }

        private void ProfileRemove(int sessionId, int lineId)
        {
            GraphicsLayerManager.RemoveLineFromGraphic(sessionId, lineId);
        }

        private void SelectedProfileChanged(GroupedLines  oldSelectedLines, GroupedLines newSelectedLines, int profileId)
        {

            GraphicsLayerManager.ChangeSelectProfileOnGraph(oldSelectedLines, newSelectedLines, profileId);

            if (oldSelectedLines != null)
            {
                oldSelectedLines.IsSelected = false;
            }
        }
        
        private void CalcIntesectionsWithLayers(ProfileLine selectedLine, ProfileSession profileSession)
        {
            var allIntersectionLines = new List<IntersectionsInLayer>();
            var layers = View.GetLayers();
            var spatialReference = ArcMap.Document.FocusMap.SpatialReference;

            List<IPolyline> polylines;
            List<ProfilePoint> pointsFrom;

            profileSession.Layers = new List<string>();

            if (selectedLine == null)
            {
                return;
            }

            if (selectedLine.Line.SpatialReference != spatialReference)
            {
                selectedLine.Line.Project(spatialReference);
            }

            var lineSurface = profileSession.ProfileSurfaces.First(surface => surface.LineId == selectedLine.Id);
            var profileSegment = profileSession.Segments.First(segment => segment.LineId == selectedLine.Id);
            var distance = 0.0;

            if (profileSegment.IsPrimitive)
            {
                polylines = ProfileLinesConverter.ConvertLineToPrimitivePolylines(lineSurface, selectedLine.Line.SpatialReference);
                pointsFrom = profileSegment.Vertices;
            }
            else
            {
                polylines = new List<IPolyline> { selectedLine.Line };
                pointsFrom = new List<ProfilePoint> { selectedLine.PointFrom };
            }

            int j = 0;

            for (int n = 0; n <  polylines.Count; n++)
            {
                var intersectionLines = new List<IntersectionsInLayer>();
               
                for (int i = 0; i < layers.Count; i++)
                {
                    if (!string.IsNullOrEmpty(layers[i]))
                    {
                        var layer = EsriTools.GetLayer(layers[i], ArcMap.Document.FocusMap);
                        var lines = EsriTools.GetIntersections(polylines[n], layer);

                        var layerFullName = $"Path/{layer.Name}";

                        if (!profileSession.Layers.Exists(sessionLayer => sessionLayer == layerFullName))
                        {
                            profileSession.Layers.Add(layerFullName);
                        }

                        if (lines != null && lines.Count() > 0)
                        {
                            var layerType = (LayersEnum)Enum.GetValues(typeof(LayersEnum)).GetValue(i);
                            var intersectionLine = new IntersectionsInLayer
                            {
                                Lines = ProfileLinesConverter.ConvertEsriPolylineToIntersectionLines(lines, pointsFrom[j], layerType, distance),
                                Type = layerType,
                            };

                            intersectionLine.SetDefaultColor();
                            intersectionLines.Add(intersectionLine);
                            SetLayersForPoints(intersectionLine, lineSurface);
                        }
                    }
                }

                allIntersectionLines.AddRange(intersectionLines);

                if (polylines.Count > 1)
                {
                    j++;

                    if (n < polylines.Count - 1)
                    {
                        distance += EsriTools.CreatePolylineFromPoints(polylines[n].FromPoint, polylines[n + 1].FromPoint).Length;
                    }
                }
            }

            graphsController.SetIntersections(allIntersectionLines, selectedLine.Id);
        }

        private void SetLayersForPoints(IntersectionsInLayer intersections, ProfileSurface surface)
        {
            var surfaceForSearch = new List<ProfileSurfacePoint>(surface.ProfileSurfacePoints);
            intersections.Lines = intersections.Lines.OrderBy(line => line.PointFromDistance).ToList();

            var accuracy = 0.0000001;

            foreach (var line in intersections.Lines)
            {
                var startPoint = surfaceForSearch.First(surfacePoint => surfacePoint.Distance >= line.PointFromDistance
                                                            || Math.Abs(surfacePoint.Distance - line.PointFromDistance) < accuracy);

                surfaceForSearch =
                                surfaceForSearch.SkipWhile(surfacePoint => surfacePoint.Distance < startPoint.Distance).ToList();

                foreach (var point in surfaceForSearch)
                {
                    if (point.Distance >= startPoint.Distance)
                    {
                        if (point.Distance > line.PointToDistance + accuracy)
                        {
                            break;
                        }

                        if (point.Layers == 0)
                        {
                            point.Layers = line.LayerType;
                        }
                        else
                        {
                            point.Layers = point.Layers | line.LayerType;
                        }

                        if (point.Distance == line.PointToDistance)
                        {
                            break;
                        }

                    }
                }

            }
        }

        private void GenerateEmptyGraph()
        {
            graphsController.AddSession(GetEmptyProfileSession());
        }

        private void PanToSelectedProfilesSet(int sessionId)
        {
            ShowProfileOnMap(sessionId);
        }

        private void PanToSelectedProfile(int sessionId, ProfileLine line)
        {
            ShowProfileOnMap(sessionId, line);
        }

        private void ChangeProfilesHeights(List<int> sessionsIds, double height, ProfileSurface[] surfaces)
        {
            foreach(var id in sessionsIds)
            {
                var session = GetProfileSessionById(id);

                if(session != null)
                {
                    session.ObserverHeight = height;
                    var profileSurfaces = surfaces.Where(surface => surface.SessionId == id).ToArray();
                    session.ProfileSurfaces = profileSurfaces.Select(ps => 
                    {
                        return new ProfileSurface()
                        {
                            LineId = ps.LineId,
                            ProfileSurfacePoints = ps.ProfileSurfacePoints
                        };
                    }).ToArray();


                    if(session.ProfileSurfaces.Count() == 1)
                    {
                        session.ProfileSurfaces[0].LineId = session.ProfileLines[0].Id;
                    }

                    SaveProfileSet(session);
                    _workingProfiles.First(profileSession => profileSession.SessionId == id).ObserverHeight = height;

                    View.ChangeSessionHeightInNode(id, height, session.DefinitionType);
                }

            }
        }

        private ProfileSession GetEmptyProfileSession()
        {
            var newProfileId = GenerateProfileId();

            var session = new ProfileSession()
            {
                DefinitionType = ProfileSettingsTypeEnum.Composed,
                SessionId = newProfileId,
                SessionName = GenerateProfileName(),
                ObserverHeight = 0,
                SurfaceLayerName = View.DemLayerName,
                CreatedBy = Environment.UserName,
                CreatedOn = DateTime.Now,
                Shared = false
            };

            return session;
        }

        internal GraphicsLayerManager GraphicsLayerManager
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

        internal IEnumerable<string> GetLayersForLineSelection()
        {
            var res = new List<string>();
            res.Add(graphiclayerTitle);

            // To add the line layers uncomment this
            //res.AddRange(ProfileLayers.LineLayers.Select(l => l.Name));

            return res;
        }

        internal double GetDefaultSecondYCoord(IPoint firstPoint)
        {
            var firstPointCopy = new PointClass { X = firstPoint.X, Y = firstPoint.Y, Z = firstPoint.Z, SpatialReference = firstPoint.SpatialReference };
            firstPointCopy.Project(ArcMap.Document.ActiveView.FocusMap.SpatialReference);

            var y = firstPointCopy.Y - EsriTools.GetExtentHeightInMapUnits(ArcMap.Document.ActiveView.Extent, 0.1);
            var secondPoint = new PointClass { X = firstPointCopy.X, Y = y, Z = firstPointCopy.Z, SpatialReference = ArcMap.Document.ActiveView.FocusMap.SpatialReference };
            secondPoint.Project(firstPoint.SpatialReference);

            return secondPoint.Y;
        }

        internal string[] GetAssignmentMethodsStrings()
        {
            return _assignmentMethods.Values.ToArray();
        }

        internal string[] GetTargetAssignmentMethodsStrings()
        {
            return _targetAssignmentMethods.Values.ToArray();
        }

        internal string[] GetToPointsCreationMethodsString()
        {
            return _toPointsCreationMethods.Values.ToArray();
        }

        internal AssignmentMethodsEnum GetMethodByString(string methodString)
        {
            return _assignmentMethods.FirstOrDefault(method => method.Value.Equals(methodString)).Key;
        }

        internal AssignmentMethodsEnum GetTargetAssignmentMethodByString(string methodString)
        {
            return _targetAssignmentMethods.FirstOrDefault(method => method.Value.Equals(methodString)).Key;
        }

        internal ToPointsCreationMethodsEnum GetCreationMethodByString(string methodString)
        {
            return _toPointsCreationMethods.FirstOrDefault(method => method.Value.Equals(methodString)).Key;
        }

        internal void CalcFunToPoints(AssignmentMethodsEnum assignmentMethod, ToPointsCreationMethodsEnum creationMethod, bool isNewTarget, double length = -1)
        {
            if(pointsToShow[ProfileSettingsPointButtonEnum.CenterFun] == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCenterPointNotChosenText", "Будь ласка, оберіть центральну точку для подальших розрахунків"),
                                    LocalizationContext.Instance.MessageBoxTitle);

                return;
            }

            var rl = _mapLayersManager.RasterLayers.FirstOrDefault(l => l.Name == View.DemLayerName);

            if(rl == null || String.IsNullOrEmpty(View.DemLayerName))
            {
                MessageBox.Show(LocalizationContext.Instance.DemLayerNotChosenText, LocalizationContext.Instance.MessageBoxTitle);
                return;
            }

            if(isNewTarget || _funGeometries == null)
            {
                var geometries = FunCreationManager.GetGeometriesByMethod(assignmentMethod);
                if(geometries != null && geometries.Count() != 0)
                {
                    _funGeometries = geometries;
                }
                else if(assignmentMethod != AssignmentMethodsEnum.Sector)
                {
                    return;
                }
            }
            
            var polylines = new List<IPolyline>();

            double minAzimuth = -1;
            double maxAzimuth = -1;
            double maxLength = length;
            var centerPoint = pointsToShow[ProfileSettingsPointButtonEnum.CenterFun];

            if(assignmentMethod == AssignmentMethodsEnum.Sector)
            {
                polylines = EsriTools.CreatePolylinesFromPointAndAzimuths(centerPoint, View.FunLength, View.FunLinesCount, View.FunAzimuth1, View.FunAzimuth2).ToList();

                minAzimuth = View.FunAzimuth1;
                maxAzimuth = View.FunAzimuth2;
                maxLength = View.FunLength;
            }
            else
            {
                bool isCircle = false;

                var points = EsriTools.GetPointsFromGeometries(_funGeometries, centerPoint.SpatialReference, out isCircle);

                bool isPointInside = (_funGeometries.First().GeometryType == esriGeometryType.esriGeometryPoint) ? false : EsriTools.IsPointOnExtent(EsriTools.GetEnvelopeOfGeometriesList(new List<IGeometry>(_funGeometries)), pointsToShow[ProfileSettingsPointButtonEnum.CenterFun]);

                if(points.Count() == 1 && points[0].Points.Count == 1 && !(isCircle && isPointInside))
                {
                    creationMethod = ToPointsCreationMethodsEnum.ToVertices;
                }

                try
                {
                    switch(creationMethod)
                    {
                        case ToPointsCreationMethodsEnum.Default:

                            polylines = EsriTools.CreateDefaultPolylinesForFun(centerPoint, points.ToArray(),
                                                                                  isCircle, isPointInside, length, out minAzimuth, out maxAzimuth, out maxLength).ToList();

                            break;

                        case ToPointsCreationMethodsEnum.AzimuthsCenter:

                            var geomCenterPoint = FunCreationManager.GetCenterPoint(_funGeometries.ToList());
                            geomCenterPoint.Project(centerPoint.SpatialReference);
                            var lineToCenter = new Line { FromPoint = centerPoint, ToPoint = geomCenterPoint, SpatialReference = centerPoint.SpatialReference };
                            polylines = EsriTools.CreateDefaultPolylinesForFun(centerPoint, points.ToArray(),
                                                                                  isCircle, isPointInside, length, out minAzimuth, out maxAzimuth, out maxLength, lineToCenter.PosAzimuth()).ToList();

                            break;


                        case ToPointsCreationMethodsEnum.AzimuthsLines:

                            polylines = EsriTools.CreatePolylinesFromPointAndAzimuths(centerPoint, View.FunLength, View.FunLinesCount, View.FunAzimuth1, View.FunAzimuth2).ToList();

                            minAzimuth = View.FunAzimuth1;
                            maxAzimuth = View.FunAzimuth2;
                            maxLength = View.FunLength;

                            break;

                        case ToPointsCreationMethodsEnum.ToVertices:

                            polylines = EsriTools.CreateToVerticesPolylinesForFun(points, centerPoint, length, out minAzimuth, out maxAzimuth, out maxLength).ToList();

                            break;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCalcFunErrorText", "Під час розрахунку набору профілів сталася помилка. Більш детальна інформація знаходиться у журналі"),
                                        LocalizationContext.Instance.MessageBoxTitle);
                    logger.ErrorEx($"> CalcFunToPoints Exception: {ex.Message}");
                }
            }

            if(polylines == null || polylines.Count == 0)
            {
                return;
            }

            SetFunProperties(polylines, minAzimuth, maxAzimuth, maxLength);
            View.SetFunTxtValues(maxLength, maxAzimuth, minAzimuth, polylines.Count);
        }


        internal void SetPointBySelectedMethod(AssignmentMethodsEnum method, ProfileSettingsPointButtonEnum pointType)
        {
            var rl = _mapLayersManager.RasterLayers.FirstOrDefault(l => l.Name == View.DemLayerName);

            if(rl == null || String.IsNullOrEmpty(View.DemLayerName))
            {
                MessageBox.Show(LocalizationContext.Instance.DemLayerNotChosenText, LocalizationContext.Instance.MessageBoxTitle);
                return;
            }

            IPoint point = null;

            switch(method)
            {
                case AssignmentMethodsEnum.GeoCalculator:

                    point = GetPointFromGeoCalculator(pointType);

                    break;

                case AssignmentMethodsEnum.ObservationPoints:

                    point = GetPointFromObservationPoints(pointType);

                    break;


                case AssignmentMethodsEnum.FeatureLayers:

                    point = GetPointFromPointLayers(pointType);

                    break;

            }

            if(point == null)
            {
                return;
            }

            point.AddZCoordinate(rl.Raster);

            var pointToMapSpatial = point.Clone();
            pointToMapSpatial.Project(ArcMap.Document.ActivatedView.FocusMap.SpatialReference);

            if(pointType == ProfileSettingsPointButtonEnum.PointsFist)
            {
                SetFirsPointForLineProfile(point, pointToMapSpatial);
            }
            else if(pointType == ProfileSettingsPointButtonEnum.PointsSecond)
            {
                SetSecondfPointForLineProfile(point, pointToMapSpatial);
            }
            else
            {
                SetCenterPointForFunProfile(point, pointToMapSpatial);
            }

            startPoints[pointType] = point;
            View.SetReturnButtonEnable(pointType, true);
        }

        internal IPoint GetStartPointValue(ProfileSettingsPointButtonEnum pointType)
        {
            return startPoints[pointType];
        }

        internal void FlipPoints()
        {
            var secondPoint = pointsToShow[ProfileSettingsPointButtonEnum.PointsSecond].Clone();

            if(pointsToShow[ProfileSettingsPointButtonEnum.PointsFist] == null || secondPoint == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFlipEmptyPoint", "Будь ласка, оберіть початкову і кінцеву точку"),
                                    LocalizationContext.Instance.MessageBoxTitle);

                logger.WarnEx("> FlipPoints. One of the points is empty");
                return;
            }

            var firstPoint = pointsToShow[ProfileSettingsPointButtonEnum.PointsFist].CloneWithProjecting();
            SetSecondfPointForLineProfile(firstPoint, pointsToShow[ProfileSettingsPointButtonEnum.PointsFist]);

            var secondPointToWgs = secondPoint.CloneWithProjecting();
            SetFirsPointForLineProfile(secondPointToWgs, secondPoint);

        }

        private void SetFunProperties(IEnumerable<IPolyline> polylines, double minAzimuth, double maxAzimuth, double maxLength)
        {
            var profileSetting = profileSettings[ProfileSettingsTypeEnum.Fun];

            if(profileSetting == null)
            {
                profileSetting = new ProfileSettings();
                profileSetting.Type = ProfileSettingsTypeEnum.Fun;
            }

            profileSetting.DemLayerName = View.DemLayerName;

            profileSetting.Azimuth1 = EsriTools.GetFormattedAzimuth(minAzimuth);
            profileSetting.Azimuth2 = EsriTools.GetFormattedAzimuth(maxAzimuth);

            profileSetting.ProfileLines = polylines.ToArray();

            profileSettings[ProfileSettingsTypeEnum.Fun] = profileSetting;

            try
            {
                InvokeOnProfileSettingsChanged();
                logger.WarnEx("");
                GraphicsLayerManager.UpdateCalculatingGraphic(profileSetting.ProfileLines, profileId, (int)ProfileSettingsTypeEnum.Fun);

                SetFunParams(maxAzimuth, minAzimuth, polylines, maxLength);
            }
            catch(Exception ex)
            {
                logger.ErrorEx($"> SetFunProperties Exception: {ex.Message}");
            }
        }
        
        private void SetFunParams(double maxAzimuth, double minAzimuth, IEnumerable<IPolyline> lines, double length = -1)
        {
            double azimuthsSum = 0;
            double allLength = 0;

            foreach(var line in lines)
            {
                if(length == -1)
                {
                    allLength += line.Length;
                }
                var lineWithAngle = new Line { FromPoint = line.FromPoint, ToPoint = line.ToPoint, SpatialReference = line.SpatialReference };

                var azimuth = lineWithAngle.Azimuth();
                if(azimuth < 0) azimuth += 360;
                azimuthsSum += azimuth;
            }

            var linesCount = lines.Count();

            if(length == -1)
            {
                length = allLength / linesCount;
            }

            double angle;
            if(maxAzimuth > minAzimuth)
            {
                angle = maxAzimuth - minAzimuth;
            }
            else
            {
                angle = (360 - maxAzimuth) + minAzimuth;
            }

            var avgAngle = (lines.Count() == 1)? 0 : angle / (linesCount - 1);
            var avgAzimuth = azimuthsSum / linesCount;

            View.SetFunToPointsParams(avgAzimuth, avgAngle, length, linesCount);
        }

        
        private IPoint GetPointFromGeoCalculator(ProfileSettingsPointButtonEnum pointType)
        {
            Dictionary<int, IPoint> points;

            var geoModule = ModuleInteraction.Instance.GetModuleInteraction<IGeocalculatorInteraction>(out bool changes);

            if(!changes && geoModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgGeoCalcModuleDoesnotExistText", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetPointFromGeoCalculator Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgGeoCalcModuleDoesnotExistText", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                points = geoModule.GetPoints();
            }
            catch(Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorHappendText, LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetPointFromGeoCalculator Exception: {ex.Message}");
                return null;
            }

            var pointsWindow = new PointsListModalWindow(points);
            var result = pointsWindow.ShowDialog();

            if(result == DialogResult.OK)
            {
                View.SetPointInfo(pointType, $"{LocalizationContext.Instance.FindLocalizedElement("CmbAssignmentMethodGeoCalcTypeText", "Геокалькулятор")};" +
                                                $" {pointsWindow.SelectedPoint.ObjId}");

                return pointsWindow.SelectedPoint.Point;
            }

            return null;
        }

        private IPoint GetPointFromObservationPoints(ProfileSettingsPointButtonEnum pointType)
        {
            var visibilityModule = ModuleInteraction.Instance.GetModuleInteraction<IVisibilityInteraction>(out bool changes);
            var points = new List<FromLayerPointModel>();

            if(!changes && visibilityModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetPointFromObservationPoints Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                points = visibilityModule.GetObservationPoints();

                if(points == null)
                {
                    MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgObservPointsLayerDoesnotExistText", "У проекті відсутній шар точок спостереження \nБудь ласка додайте шар, щоб мати можливість отримати точки"),
                                LocalizationContext.Instance.MessageBoxTitle);
                    return null;
                }
            }
            catch(MissingFieldException)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле OBJECTID"),
                                   LocalizationContext.Instance.MessageBoxTitle);
                return null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorHappendText, LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetPointFromObservationPoints Exception: {ex.Message}");
                return null;
            }
            
            ObservationPointsListModalWindow observPointsModal = new ObservationPointsListModalWindow(points);
            var result = observPointsModal.ShowDialog();

            if(result == DialogResult.OK)
            {
                View.SetPointInfo(pointType, $"{LocalizationContext.Instance.FindLocalizedElement("ObservPointsTypeText", "Пункти спостереження")}; {observPointsModal.SelectedPoint.ObjId}");
                return observPointsModal.SelectedPoint.Point;
            }

            return null;
        }

        private IPoint GetPointFromPointLayers(ProfileSettingsPointButtonEnum pointType)
        {
            PointsFromLayerModalWindow pointsFromLayerModal = new PointsFromLayerModalWindow();
            var result = pointsFromLayerModal.ShowDialog();

            if(result == DialogResult.OK)
            {
                View.SetPointInfo(pointType, $"{pointsFromLayerModal.LayerName}; {pointsFromLayerModal.SelectedPoint.ObjId}");
                return pointsFromLayerModal.SelectedPoint.Point;
            }

            return null;
        }
        
        private void OnMapSelectionChangedLocal()
        {
            if (View.SelectedProfileSettingsType != ProfileSettingsTypeEnum.Primitives)
            {
                return;
            }
            GetSelectedGraphics();
            SetProfileSettings(ProfileSettingsTypeEnum.Primitives);
        }

        private void GetSelectedGraphics()
        {
            selectedOnMapLines = GraphicsLayerManager.GetSelectedGraphics();
            var cnt = selectedOnMapLines == null ? 0 : selectedOnMapLines.Count();
            var len = selectedOnMapLines == null ? 0 : selectedOnMapLines.Sum(l => l.Length);

            if (OnMapSelectionChanged != null)
            {
                OnMapSelectionChanged?.Invoke(new SelectedGraphicsArgs(cnt, len));
            }
        }
    }
}
