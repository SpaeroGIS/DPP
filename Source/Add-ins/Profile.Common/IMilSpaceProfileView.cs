using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.DTO;
using MilSpace.Profile.Helpers;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MilSpace.Profile
{
    public interface IMilSpaceProfileView
    {

        void SetController(MilSpaceProfileCalcsController controller);

        ProfileSettingsTypeEnum SelectedProfileSettingsType { get; }

        ProfileSettingsPointButtonEnum ActiveButton { get; }
       
        IActiveView ActiveView { get; }

        /// <summary>
        /// First point for Line  Profile setting 
        /// </summary>
        IPoint LinePropertiesFirstPoint { set; }

        /// <summary>
        /// Second point for Line  Profile setting 
        /// </summary>
        IPoint LinePropertiesSecondPoint { set; }

        /// <summary>
        /// Center point for Fun Profile setting 
        /// </summary>
        IPoint FunPropertiesCenterPoint { set; }
        AssignmentMethodsEnum PrimitiveAssignmentMethod { get; }

        int FunLinesCount { get; }

        double FunAzimuth1 { get; }
        double FunAzimuth2 { get; }

        int SectionHeightFirst { get; }
        int SectionHeightSecond { get; }
        int FanHeight { get; }

        double FunLength { get; }

        double ObserveHeight { get; }

        MilSpaceProfileCalcsController Controller { get; }

        bool AllowToProfileCalc { get; }

        string DemLayerName { get; }

        int ProfileId { set; }

        string ProfileName { get; set; }

        TreeViewSelectedProfileIds SelectedProfileSessionIds { get; }

        IEnumerable<string> GetLayersForLineSelection { get; }

        bool RemoveTreeViewItem();

        /// <summary>
        /// Adds the <paramref name="profile"/> into the tree view 
        /// </summary>
        /// <param name="profile">MilSpace Profile </param>
        /// <returns>Returns true if the parent node is chacked. Used to add thr Profile to the Graphics layer </returns>

        void AddProfileToList(ProfileSession profile);

        bool AddNodeToTreeView(ProfileSession profile);

        ProfileSession GetProfileFromList(string profileName, ProfileSettingsTypeEnum profileSettingsType);
        void SetProifileLineInfo(double length, double azimuth);
        void SetPointInfo(ProfileSettingsPointButtonEnum pointType, string text);
        void SetReturnButtonEnable(ProfileSettingsPointButtonEnum pointType, bool enabled);
        void SetFunToPointsParams(double averageAzimuth, double averageAngle, double avgLength, int count);
        void SetFunTxtValues(double length, double maxAzimuth, double minAzimuth, int linesCount);
        void SetPrimitiveInfo(double length, double azimuth, double projectionLength, int segmentsCount);
        void RecalculateFun();
        void RecalculateFunWithParams();

        List<string> GetLayers();

        ProfileSettingsTypeEnum GetProfileTypeFromNode();

        int GetProfileNameFromNode();
        TreeView GetTreeView();
        void ChangeSessionHeightInNode(int sessionId, double height, ProfileSettingsTypeEnum type);
    }
}
