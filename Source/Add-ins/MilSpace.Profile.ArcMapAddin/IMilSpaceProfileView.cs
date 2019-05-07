using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.DTO;

namespace MilSpace.Profile
{
    public interface IMilSpaceProfileView
    {

        void SetController(MilSpaceProfileCalsController controller);

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

        int FunLinesCount { get; }

        double FunAzimuth1 { get; }
        double FunAzimuth2 { get; }

        int SectionHeightFirst { get; }
        int SectionHeightSecond { get; }
        int FanHeight { get; }

        double FunLength { get; }

        MilSpaceProfileCalsController Controller { get; }

        bool AllowToProfileCalc { get; }

        string DemLayerName { get; }

        int ProfileId { set; }

        string ProfileName { get; set; }

        TreeViewSelectedProfileIds SelectedProfileSessionIds { get; }

        bool RemoveTreeViewItem();

        /// <summary>
        /// Adds the <paramref name="profile"/> into the tree view 
        /// </summary>
        /// <param name="profile">MilSpace Profile </param>
        /// <returns>Returns true if the parent node is chacked. Used to add thr Profile to the Graphics layer </returns>
        bool AddSectionProfileNodes(ProfileSession profile);

        bool AddFanProfileNode(ProfileSession profile);

        void AddSectionProfileToList(ProfileSession profile);
        void AddFanProfileToList(ProfileSession profile);

        ProfileSession GetSectionProfile(string profileName);

        ProfileSession GetFanProfile(string profileName);

        ProfileSettingsTypeEnum GetProfileTypeFromNode();

        string GetProfileNameFromNode();

    }
}
