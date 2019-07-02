using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.Exceptions;
using MilSpace.Core.Tools;
using MilSpace.DataAccess;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Exceptions;
using MilSpace.DataAccess.Facade;
using MilSpace.Profile.DTO;
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
    public class MilSpaceProfileCalsController
    {
        //TODO: Localize
        private static string graphiclayerTitle = "Graphics Layer";

        private int profileId;
        GraphicsLayerManager graphicsLayerManager;
        private Logger logger = Logger.GetLoggerEx("MilSpaceProfileCalsController");

        public delegate void ProfileSettingsChangedDelegate(ProfileSettingsEventArgs e);

        public delegate void OnMapSelectionChangedDelegate(SelectedGraphicsArgs selectedLines);

        public event ProfileSettingsChangedDelegate OnProfileSettingsChanged;

        public event OnMapSelectionChangedDelegate OnMapSelectionChanged;

        private static ProfileSettingsTypeEnum[] profileSettingsType = Enum.GetValues(typeof(ProfileSettingsTypeEnum)).Cast<ProfileSettingsTypeEnum>().ToArray();


        List<ProfileSession> _workingProfiles = new List<ProfileSession>();
        private const string NewProfilePrefix = "Новый профиль";

        private MilSpaceProfileGraphsController graphsController;

        private Dictionary<ProfileSettingsPointButtonEnum, IPoint> pointsToShow = new Dictionary<ProfileSettingsPointButtonEnum, IPoint>()

        {
            {ProfileSettingsPointButtonEnum.CenterFun, null},
            {ProfileSettingsPointButtonEnum.PointsFist, null},
            {ProfileSettingsPointButtonEnum.PointsSecond, null}
        };

        private IEnumerable<IPolyline> selectedOnMapLines;


        private Dictionary<ProfileSettingsTypeEnum, ProfileSettings> profileSettings = new Dictionary<ProfileSettingsTypeEnum, ProfileSettings>()
        {
            {ProfileSettingsTypeEnum.Points, null},
            {ProfileSettingsTypeEnum.Fun, null},
            {ProfileSettingsTypeEnum.Primitives, null},
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

        internal void OnDocumentsLoad()
        {
            logger.InfoEx("Document loaded.");

            IActiveViewEvents_Event activeViewEvents = (IActiveViewEvents_Event)View.ActiveView.FocusMap;
            IActiveViewEvents_SelectionChangedEventHandler handler = new IActiveViewEvents_SelectionChangedEventHandler(OnMapSelectionChangedLocal);
            activeViewEvents.SelectionChanged += handler;
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
                }

                return graphsController;
            }
        }

        internal void FlashPoint(ProfileSettingsPointButtonEnum pointType)
        {
            EsriTools.PanToGeometry(View.ActiveView, pointsToShow[pointType]);

            EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, new IGeometry[] { pointsToShow[pointType] });
          //  View.ActiveView.Refresh();
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
                    logger.ErrorEx(ex.Message);
                    MessageBox.Show(ex.Message, "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (Exception ex)
                {
                    logger.ErrorEx(ex.Message);
                }
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

            GraphicsLayerManager.UpdateCalculatingGraphic(profileSetting.ProfileLines, profileIdValue, (int)profileType);
        }


        /// <summary>
        /// Do Actions to generate profile(s), save them and set properties to default values
        /// </summary>
        /// <returns>Profile Session data</returns>
        internal ProfileSession GenerateProfile()
        {
            string errorMessage;
            try
            {

                ProfileManager manager = new ProfileManager();
                var profileSetting = profileSettings[View.SelectedProfileSettingsType];
                var newProfileId = GenerateProfileId();
                logger.InfoEx($"Profile {newProfileId}. Generation started");
                var newProfileName = GenerateProfileName(newProfileId);

                var session = manager.GenerateProfile(View.DemLayerName, profileSetting.ProfileLines, View.SelectedProfileSettingsType, newProfileId, newProfileName, View.ObserveHeight);
                logger.InfoEx($"Profile {newProfileId}. Generated");

                if (session.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    session.Segments = ProfileLinesConverter.GetSegmentsFromProfileLine(session.ProfileSurfaces, ArcMap.Document.FocusMap.SpatialReference);
                }
                else
                {
                    session.SetSegments(ArcMap.Document.FocusMap.SpatialReference);
                }

                SetPeofileId();
                SetProfileName();
                return session;

            }
            catch (MilSpaceCanotDeletePrifileCalcTable ex)
            {
                //TODO Localize error message
                errorMessage = ex.Message;
            }
            catch (MilSpaceDataException ex)
            {
                //TODO Localize error message
                errorMessage = ex.Message;

            }
            catch (Exception ex)
            {
                //TODO log error
                //TODO Localize error message
                errorMessage = ex.Message;
            }

            MessageBox.Show(errorMessage);
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

        internal void ShowProfileOnMap()
        {
            var mapScale = View.ActiveView.FocusMap.MapScale;
            var profile = GetProfileSessionFromSelectedNode();
            if (profile == null)
            {
                logger.ErrorEx("Cannot find Selected Profiles set");
                return;
            }

            var profileLines = profile.ProfileLines.Select(line => line.Line as IGeometry);
            IEnvelope env = new EnvelopeClass();

            foreach (var line in profileLines)
            {
                env.Union(line.Envelope);
            }

            EsriTools.PanToGeometry(View.ActiveView, env);

            if (profile.DefinitionType == ProfileSettingsTypeEnum.Primitives)
            {
                EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, profile.Segments.First().Polylines);
            }
            else
            {
                EsriTools.FlashGeometry(View.ActiveView.ScreenDisplay, profileLines);
            }
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
            MilSpaceProfileFacade.GetUserProfileSessions().ToList().ForEach(p =>
               {
                   p.ConvertLinesToEsriPolypile(View.ActiveView.FocusMap.SpatialReference);
                   AddProfileToList(p);
               }
            );

            OnDocumentsLoad();
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
                    graphsController.AddProfileToTab(profileLine, profileSurface);
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
                CallGraphsHandle(profileSession);
            }
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
                return MilSpaceProfileFacade.SaveProfileSession(profile);
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
                MessageBox.Show(ex.Message, "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            View.ProfileName = $"{NewProfilePrefix} {profileId}";
        }

        private ProfileSession GetProfileSessionById(int profileId)
        {
            var profile = MilSpaceProfileFacade.GetProfileSessionById(profileId);
            profile.ConvertLinesToEsriPolypile(View.ActiveView.FocusMap.SpatialReference);
            return profile;
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

        private ProfileSession GetEmptyProfileSession()
        {
            var newProfileId = GenerateProfileId();

            var session = new ProfileSession()
            {
                DefinitionType = ProfileSettingsTypeEnum.Composed,
                SessionId = newProfileId,
                SessionName = GenerateProfileName(newProfileId),
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
