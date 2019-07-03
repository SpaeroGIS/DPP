using MilSpace.Core.MilSpaceResourceManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile.Localization
{
    internal class LocalizationConstants
    {
        private static LocalizationConstants instance;
        private static MilSpaceResourceManager mngr;

        private static string RefreshButtonToolTipKey = "btnRefreshLayersToolTip";
        private static string CommonLengthTextKey = "lblCommonLengthText";
        private static string SelectedPrimitivesTextKey = "lblSelectedPrimitivesText";

        private static string ProfileTabPageTextKey = "tabProfileTabPageText";
        private static string PofileTreeTabPageTextKey = "tabPofileTreeTabPageText";

        private static string LayersForCalcTextKey = "lblLayersForCalcText";
        private static string DEMTextKey = "lblDEMText";
        private static string VegetationLayerTextKey = "lblVegetationLayerText";
        private static string BuildingsLayerTextKey = "lblBuildingsLayerText";
        private static string RoadsLayerTextKey = "lblRoadsLayerText";
        private static string HydrographyLayerTextKey = "lblHydrographyLayerText";
        private static string PointOfViewLayerTextKey = "lblPointOfViewLayerText";
        private static string SetProfilePropertiesTextKey = "lblSetProfilePropertiesText";
        private static string ProfileNameTextKey = "lblProfileNameText";
        private static string SectionTabTextKey = "tabSectionTabText";
        private static string LoadTabTextKey = "tabLoadTabText";
        private static string PrimitiveTabTextKey = "tabPrimitiveTabText";
        private static string FunTabTextKey = "tabFunTabText";
        private static string СalcProfileTextKey = "btnСalcProfileText";
        private static string LineFirstPointTextKey = "lblLineFirstPointText";
        private static string LinewSecondPointTextKey = "lblLineSecondPointText";
        private static string HeightOfViewFirstTextKey = "lblHeightOfViewFirstText";
        private static string HeightOfViewSecondTextKey = "lblHeightOfViewSecondText";
        private static string DimensionFirstTextKey = "lblDimensionFirstText";
        private static string DimensionSecondTextKey = "lblDimensionSecondText";
        private static string FunBasePointTextKey = "lblFunBasePointText";
        private static string HeightOfViewFunBaseTextKey = "lblHeightOfViewFunBaseText";
        private static string FunParametersTextKey = "lblFunParametersText";
        private static string FunCountTextKey = "lblFunCountText";
        private static string FunDistanceTextKey = "lblFunDistanceText";
        private static string FunAzimuth1TextKey = "lblFunAzimuth1Text";
        private static string FunAzimuth2TextKey = "lblFunAzimuth2Text";
        private static string HeightOfViewGraphicsTextKey = "lblHeightOfViewGraphicsText";
        private static string PrimitivesLayerToSelectTextKey = "lblPrimitivesLayerToSelectText";

        //AddAvailableProfilesSetsModalWindow
        private static string FiltersTitleKey = "gbFiltersTitle";
        private static string NamePlaceholderKey = "txtNamePlaceholder";
        private static string CreatorPlaceholderKey = "txtCreatorPlaceholder";
        private static string GraphTypeTextKey = "cmbGraphTypeText";
        private static string CreationDateTextKey = "lblCreationDateText";
        private static string FromTextKey = "lblFromText";
        private static string ToTextKey = "lblToText";
        private static string PointsTypeTextKey = "pointsTypeText";
        private static string FunTypeTextKey = "funTypeText";
        private static string PrimitiveTypeTextKey = "primitiveTypeText";
        private static string FilterTextKey = "btnFilterText";
        private static string OkTextKey = "btnOkText";
        private static string CancelTextKey = "btnCancelText";
        private static string ProfilesSetsNameColHeaderKey = "lvProfilesSetsNameColHeader";
        private static string ProfilesSetsCreatorColHeaderKey = "lvProfilesSetsCreatorColHeader";
        private static string ProfilesSetsDateColHeaderKey = "lvProfilesSetsDateColHeader";
        private static string ProfilesSetsTypeColHeaderKey = "lvProfilesSetsTypeColHeader";
        private static string ProfilesSetsSharedColHeaderKey = "lvProfilesSetsSharedColHeader";
        private static string ProfilesSetsTitleKey = "modalProfilesSetsTitle";

        private LocalizationConstants()
        {
            //TODO: Define a method to change a culture info to localization
            mngr = new MilSpaceResourceManager("MilSpace.Profile.Calc", CultureInfo.GetCultureInfo("uk-UA"));
        }

        public static string RefreshButtonToolTip => Instance.GetLocalization(RefreshButtonToolTipKey);
        public static string CommonLengthText => Instance.GetLocalization(CommonLengthTextKey);
        public static string SelectedPrimitivesText => Instance.GetLocalization(SelectedPrimitivesTextKey);
        public static string ProfileTabPageText => Instance.GetLocalization(ProfileTabPageTextKey);
        public static string PofileTreeTabPageText => Instance.GetLocalization(PofileTreeTabPageTextKey);
        public static string LayersForCalcText => Instance.GetLocalization(LayersForCalcTextKey);
        public static string DEMText => Instance.GetLocalization(DEMTextKey);
        public static string VegetationLayerText => Instance.GetLocalization(VegetationLayerTextKey);
        public static string BuildingsLayerText => Instance.GetLocalization(BuildingsLayerTextKey);
        public static string RoadsLayerText => Instance.GetLocalization(RoadsLayerTextKey);
        public static string HydrographyLayerText => Instance.GetLocalization(HydrographyLayerTextKey);
        public static string PointOfViewLayerText => Instance.GetLocalization(PointOfViewLayerTextKey);
        public static string SetProfilePropertiesText => Instance.GetLocalization(SetProfilePropertiesTextKey);
        public static string ProfileNameText => Instance.GetLocalization(ProfileNameTextKey);
        public static string SectionTabText => Instance.GetLocalization(SectionTabTextKey);
        public static string LoadTabText => Instance.GetLocalization(LoadTabTextKey);
        public static string PrimitiveTabText => Instance.GetLocalization(PrimitiveTabTextKey);
        public static string FunTabText => Instance.GetLocalization(FunTabTextKey);
        public static string СalcProfileText => Instance.GetLocalization(СalcProfileTextKey);
        public static string LineFirstPointText => Instance.GetLocalization(LineFirstPointTextKey);
        public static string LinewSecondPointText => Instance.GetLocalization(LinewSecondPointTextKey);
        public static string HeightOfViewFirstText => Instance.GetLocalization(HeightOfViewFirstTextKey);
        public static string HeightOfViewSecondText => Instance.GetLocalization(HeightOfViewSecondTextKey);
        public static string DimensionFirstText => Instance.GetLocalization(DimensionFirstTextKey);
        public static string DimensionSecondText => Instance.GetLocalization(DimensionSecondTextKey);
        public static string FunBasePointText => Instance.GetLocalization(FunBasePointTextKey);
        public static string HeightOfViewFunBaseText => Instance.GetLocalization(HeightOfViewFunBaseTextKey);
        public static string FunParametersText => Instance.GetLocalization(FunParametersTextKey);
        public static string FunDistanceText => Instance.GetLocalization(FunDistanceTextKey);
        public static string FunCountText => Instance.GetLocalization(FunCountTextKey);
        public static string FunAzimuth1Text => Instance.GetLocalization(FunAzimuth1TextKey);
        public static string FunAzimuth2Text => Instance.GetLocalization(FunAzimuth2TextKey);
        public static string HeightOfViewGraphicsText => Instance.GetLocalization(HeightOfViewGraphicsTextKey);
        public static string PrimitivesLayerToSelectText => Instance.GetLocalization(PrimitivesLayerToSelectTextKey);

        //AddAvailableProfilesSetsModalWindow
        public static string FiltersTitle => Instance.GetLocalization(FiltersTitleKey);
        public static string NamePlaceholder => Instance.GetLocalization(NamePlaceholderKey);
        public static string CreatorPlaceholder => Instance.GetLocalization(CreatorPlaceholderKey);
        public static string GraphTypeText => Instance.GetLocalization(GraphTypeTextKey);
        public static string CreationDateText => Instance.GetLocalization(CreationDateTextKey);
        public static string FromText => Instance.GetLocalization(FromTextKey);
        public static string ToText => Instance.GetLocalization(ToTextKey);
        public static string PointsTypeText => Instance.GetLocalization(PointsTypeTextKey);
        public static string FunTypeText => Instance.GetLocalization(FunTypeTextKey);
        public static string PrimitiveTypeText => Instance.GetLocalization(PrimitiveTypeTextKey);
        public static string FilterText => Instance.GetLocalization(FilterTextKey);
        public static string OkText => Instance.GetLocalization(OkTextKey);
        public static string CancelText => Instance.GetLocalization(CancelTextKey);
        public static string ProfilesSetsNameColHeader => Instance.GetLocalization(ProfilesSetsNameColHeaderKey);
        public static string ProfilesSetsCreatorColHeader => Instance.GetLocalization(ProfilesSetsCreatorColHeaderKey);
        public static string ProfilesSetsDateColHeader => Instance.GetLocalization(ProfilesSetsDateColHeaderKey);
        public static string ProfilesSetsTypeColHeader => Instance.GetLocalization(ProfilesSetsTypeColHeaderKey);
        public static string ProfilesSetsSharedColHeader => Instance.GetLocalization(ProfilesSetsSharedColHeaderKey);
        public static string ProfilesSetsTitle => Instance.GetLocalization(ProfilesSetsTitleKey);


        private string GetLocalization(string key, string defaultValue = null)
        {
            return mngr.GetLocalization(key, defaultValue);
        }


        internal static LocalizationConstants Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LocalizationConstants();
                }

                return instance;
            }
        }

    }
}
