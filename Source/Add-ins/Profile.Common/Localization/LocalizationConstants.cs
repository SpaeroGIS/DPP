using MilSpace.Core.MilSpaceResourceManager;
using System.Globalization;
using System.IO;

namespace MilSpace.Profile.Localization
{
    internal class LocalizationConstants
    {
        private static LocalizationConstants instance;
        private static MilSpaceResourceManager mngr;


        private static string ProfileCalcDocableWinCationKey = "lblProfileDocableWinCaption";
        private static string RefreshButtonToolTipKey = "btnRefreshLayersToolTip";
        private static string CommonLengthTextKey = "lblCommonLengthText";
        private static string SelectedPrimitivesTextKey = "lblSelectedPrimitivesText";

        private static string ProfileTabPageTextKey = "tabProfileTabPageText";
        private static string PofileTreeTabPageTextKey = "tabPofileTreeTabPageText";

        private static string LayersForCalcTextKey = "lblLayersForCalcText";
        private static string lblDEM_TextKey = "lblDEMTextKey";

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
        private static string NewProfileNameValueKey = "txtNewProfileNameValue";
        private static string GraphicsLayerValueKey = "txtGraphicsLayerValue";
        private static string PickCoordinatesToolMessageKey = "txtPickCoordinatesToolMessage";
        private static string AddAvailableProfilesSetsToolTipKey = "btnAddAvailableProfilesSetsToolTip";
        private static string TakeCoordToolTipKey = "btnTakeCoordToolTip";
        private static string ShowCoordToolTipKey = "btnShowCoordToolTip";
        private static string CopyCoordToolTipKey = "btnCopyCoordToolTip";
        private static string PasteCoordToolTipKey = "btnPasteCoordToolTip";


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
        private static string AddToSessionTextKey = "btnAddToSessionText";
        private static string CloseTextKey = "btnCloseText";
        private static string ProfilesSetsNameColHeaderKey = "lvProfilesSetsNameColHeader";
        private static string ProfilesSetsCreatorColHeaderKey = "lvProfilesSetsCreatorColHeader";
        private static string ProfilesSetsDateColHeaderKey = "lvProfilesSetsDateColHeader";
        private static string ProfilesSetsTypeColHeaderKey = "lvProfilesSetsTypeColHeader";
        private static string ProfilesSetsSharedColHeaderKey = "lvProfilesSetsSharedColHeader";
        private static string ProfilesSetsTitleKey = "modalProfilesSetsTitle";
        private static string AboutSelectedTextKey = "lblAboutSelectedText";
        private static string ResetTextKey = "btnReset";
        private static string SavedProfilesModalWindowTextKey = "frmSavedProfilesModalWindowText";

        private static string ProfileListTextKey = "lblProfileListText";

        private static string ToolBtnPanOnMapTextKey = "hintToolBtnPanOnMapText";
        private static string ToolBtnShowOnMapTextKey = "hintToolBtnShowOnMapText";
        private static string ToolSetProfileSettingsToCalcTextKey = "hintToolSetProfileSettingsToCalcText";
        private static string ToolAddProfileToExistingGraphTextKey = "hintAddProfileToExistingGraphText";
        private static string ToolAddProfileToGraphTextKey = "hintAddProfileToGraphText";
        private static string ToolOpenGraphWindowTextKey = "hintOpenGraphWindowText";
        private static string ToolRemoveProfileTextKey = "hintRemoveProfileText";
        private static string ToolSaveProfileAsSharedTextKey = "hintSaveProfileAsSharedText";
        private static string ToolEraseProfileTextKey = "hintEraseProfileText";
        private static string ToolClearExtraGraphicTextKey = "hintClearExtraGraphicText";

        private static string RemoveProfaileMessageKey = "msgRemoveProfaileText";
        private static string TreeViewProfileTextKey = "txtTreeViewProfileText";
        private static string NotAllowedToShareMessageKey = "msgNotAllowedToShareText";
        private static string ErrorHappendTextMessageKey = "msgErrorHappendText";
        private static string DeleteProfaileMessageKey = "msgDeleteProfaileText";

        //ProfilesTreeModalWindow
        private static string PointsNodeTextKey = "tvProfilesPointsNodeText";
        private static string FunNodeTextKey = "tvProfilesFunNodeText";
        private static string PrimitiveNodeTextKey = "tvProfilesPritiveNodeText";

        private static string CancelTextKey = "btnCancelText";
        private static string OkTextKey = "btnOkText";
        private static string ProfilesTreeModalTitleKey = "modalProfilesTreeModalTitle";


        //GraphWindow
        private static string ObserverHeightTextKey = "lblObserverHeightText";
        private static string VisibleLineColorTextKey = "lblVisibleLineColorText";
        private static string InvisibleLineColorTextKey = "lblInvisibleLineColorText";
        private static string SelectedProfileNameTextKey = "lblSelectedProfileNameText";
        private static string CopyStripMenuItemTextKey = "copyStripMenuItemText";

        private static string DisplayProfileSignatureToolTipKey = "graphTbBtnDisplayProfileSignatureToolTip";
        private static string DeleteSelectedProfileToolTipKey = "graphTbBtnDeleteSelectedProfileToolTip";
        private static string AddProfileGraphToolTipKey = "graphTbBtnAddProfileGraphToolTip";
        private static string PanToSelectedProfileToolTipKey = "graphTbBtnPanToSelectedProfileToolTip";
        private static string PanToProfilesSetToolTipKey = "graphTbBtnPanToProfilesSetToolTip";
        private static string ShowAllProfilesToolTipKey = "graphTbBtnShowAllProfilesToolTip";
        private static string ObserverHeightIgnoreToolTipKey = "graphTbBtnObserverHeightIgnoreToolTip";
        private static string ZoomInToolTipKey = "graphTbBtnZoomInToolTip";
        private static string ZoomOutToolTipKey = "graphTbBtnZoomOutToolTip";
        private static string AddPageToolTipKey = "graphTbBtnAddPageToolTip";
        private static string DeletePageToolTipKey = "graphTbBtnDeletePageToolTip";
        private static string SaveToolTipKey = "graphTbBtnSaveToolTip";
        private static string UpdateIntersectionsLinesToolTipKey = "graphTbBtnUpdateIntersectionsLinesToolTip";
        private static string ProfileDetailsToolTipKey = "lvProfileDetailsToolTip";
        private static string ChangeAllObserversHeightsToolTipKey = "btnChangeAllObserversHeightsToolTip";
        private static string ProfilePropertiesIsVisibleColToolTipKey = "dgvProfilePropertiesIsVisibleColToolTip";
        private static string ProfilePropertiesProfileNumberColToolTipKey = "dgvProfilePropertiesProfileNumberColToolTip";
        private static string ProfilePropertiesAzimuthColToolTipKey = "dgvProfilePropertiesAzimuthColToolTip";
        private static string ProfilePropertiesObserverHeightColToolTipKey = "dgvProfilePropertiesObserverHeightColToolTip";
        private static string ProfilePropertiesProfileLengthColToolTipKey = "dgvProfilePropertiesProfileLengthColToolTip";
        private static string ProfilePropertiesMinHeightColToolTipKey = "dgvProfilePropertiesMinHeightColToolTip";
        private static string ProfilePropertiesMaxHeightColToolTipKey = "dgvProfilePropertiesMaxHeightColToolTip";
        private static string ProfilePropertiesHeightDifferenceColToolTipKey = "dgvProfilePropertiesHeightDifferenceColToolTip";
        private static string ProfilePropertiesDescendingAngleColToolTipKey = "dgvProfilePropertiesDescendingAngleColToolTip";
        private static string ProfilePropertiesAscendingAngleColToolTipKey = "dgvProfilePropertiesAscendingAngleColToolTip";
        private static string ProfilePropertiesVisiblePercentColToolTipKey = "dgvProfilePropertiesVisiblePercentColToolTip";

        private static string ProfilePropertiesIsVisibleColHeaderKey = "dgvProfilePropertiesIsVisibleColHeader";
        private static string ProfilePropertiesProfileNumberColHeaderKey = "dgvProfilePropertiesProfileNumberColHeader";
        private static string ProfilePropertiesAzimuthColHeaderKey = "dgvProfilePropertiesAzimuthColHeader";
        private static string ProfilePropertiesObserverHeightColHeaderKey = "dgvProfilePropertiesObserverHeightColHeader";
        private static string ProfilePropertiesProfileLengthColHeaderKey = "dgvProfilePropertiesProfileLengthColHeader";
        private static string ProfilePropertiesMinHeightColHeaderKey = "dgvProfilePropertiesMinHeightColHeader";
        private static string ProfilePropertiesMaxHeightColHeaderKey = "dgvProfilePropertiesMaxHeightColHeader";
        private static string ProfilePropertiesHeightDifferenceColHeaderKey = "dgvProfilePropertiesHeightDifferenceColHeader";
        private static string ProfilePropertiesDescendingAngleColHeaderKey = "dgvProfilePropertiesDescendingAngleColHeader";
        private static string ProfilePropertiesAscendingAngleColHeaderKey = "dgvProfilePropertiesAscendingAngleColHeader";
        private static string ProfilePropertiesVisiblePercentColHeaderKey = "dgvProfilePropertiesVisiblePercentColHeader";

        private static string ProfileDetailsStateTextKey = "lvProfileDetailsStateText";
        private static string ProfileDetailsEndPointsTextKey = "lvProfileDetailsEndPointsText";
        private static string ProfileDetailsAzimuthTextKey = "lvProfileDetailsAzimuthText";
        private static string ProfileDetailsLengthTextKey = "lvProfileDetailsLengthText";
        private static string ProfileDetailsHeightTextKey = "lvProfileDetailsHeightText";
        private static string ProfileDetailsAnglesTextKey = "lvProfileDetailsAnglesText";
        private static string ProfileDetailsVisibilityTextKey = "lvProfileDetailsVisibilityText";

        private static string ProfilesSetSharedTextKey = "profilesSetSharedText";
        private static string ProfilesSetNotSharedTextKey = "profilesSetNotSharedText";
        private static string RemovingProfileMessageKey = "mbRemovingProfileMessage";
        private static string RemovingTabMessageKey = "mbRemovingTabMessage";
        private static string MessageBoxTitleKey = "messageBoxTitle";
        private static string SelectFolderDescriptionKey = "fbdSelectFolderDescription";
        private static string FolderAlreadyExistMessageKey = "mbFolderAlreadyExistMessage";
        private static string DataExportProfilePropertiesHeaderKey = "dataExportProfilePropertiesHeader";
        private static string DataExportPointsPropertiesHeaderKey = "dataExportPointsPropertiesHeader";
        private static string LineNotFoundErrorMessageKey = "lineNotFoundErrorMessage";
        private static string GraphTitleKey = "tabGraphTitle";

        private static string AttrProfileNameTextKey = "txtAttrProfileNameText";
        private static string AttrProfileIdTextKey = "txtAttrProfileIdText";
        private static string AttrProfileTypeTextKey = "txtAttrProfileTypeText";
        private static string AttrProfileEndPointTextKey = "txtAttrProfileEndPointText";
        private static string AttrProfileStartPointTextKey = "txtAttrProfileStartPointText";
        private static string AttrProfilePointOfViewTextKey = "txtAttrProfilePointOfViewText";
        private static string AttrProfileDistanceTextKey = "txtAttrProfileDistanceText";
        private static string AttrProfileAzimuthTextKey = "txtAttrProfileAzimuthText";
        private static string AttrProfileAzimuth1TextKey = "txtAttrProfileAzimuth1Text";
        private static string AttrProfileAzimuth2TextKey = "txtAttrProfileAzimuth2Text";
        private static string AttrProfileBasePointTextKey = "txtAttrProfileBasePointText";
        private static string AttrProfileCountTextKey = "txtAttrProfileCountText";
        private static string AttrProfileCreatorTextKey = "txtAttrProfileCreatorText";
        private static string AttrProfileDateTextKey = "txtAttrProfileDateText";


        private LocalizationConstants()
        {
            //TODO: Define a method to change a culture info to localization
            string s = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mngr = new MilSpaceResourceManager(s, "SPPRD.Profile.Calc", CultureInfo.GetCultureInfo("uk-UA"));
            //mngr = new MilSpaceResourceManager("SPPRD.Profile.Calc", CultureInfo.GetCultureInfo("uk-UA"));
        }

        public static string ProfileCalcDocableWinCationText => instance.GetLocalization(ProfileCalcDocableWinCationKey, "Спостереження. Розрахунок профілю");
        public static string AttrProfileDateText => instance.GetLocalization(AttrProfileDateTextKey, "Дата");
        public static string AttrProfileCreatorText => instance.GetLocalization(AttrProfileCreatorTextKey, "Автор");
        public static string AttrProfileCountText => instance.GetLocalization(AttrProfileCountTextKey, "Кількість ліній");
        public static string AttrProfileBasePointText => instance.GetLocalization(AttrProfileBasePointTextKey, "Базова точка(довгота / широта)");
        public static string AttrProfileAzimuthText => instance.GetLocalization(AttrProfileAzimuthTextKey, "Азимут");
        public static string AttrProfileAzimuth2Text => instance.GetLocalization(AttrProfileAzimuth2TextKey, "Азимут 2");
        public static string AttrProfileAzimuth1Text => instance.GetLocalization(AttrProfileAzimuth1TextKey, "Азимут 1");
        public static string AttrProfileDistanceText => instance.GetLocalization(AttrProfileDistanceTextKey, "Відстань");
        public static string AttrProfilePointOfViewText => instance.GetLocalization(AttrProfilePointOfViewTextKey, "Точка спостереження");
        public static string AttrProfileStartPointText => instance.GetLocalization(AttrProfileStartPointTextKey, "Початок в точці");
        public static string AttrProfileEndPointText => instance.GetLocalization(AttrProfileEndPointTextKey, "Кінець в точці");
        public static string AttrProfileTypeText => instance.GetLocalization(AttrProfileTypeTextKey);
        public static string AttrProfileIdText => instance.GetLocalization(AttrProfileIdTextKey);
        public static string AttrProfileNameText => instance.GetLocalization(AttrProfileNameTextKey);
        public static string ErrorOnShareProfileTextMessage => instance.GetLocalization(ErrorHappendTextMessageKey);
        public static string ErrorOnDelitingProfileTextMessage => instance.GetLocalization(ErrorHappendTextMessageKey);
        public static string ErrorOnDataAccessTextMessage => instance.GetLocalization(ErrorHappendTextMessageKey);
        public static string NotAllowedToShareMessage => instance.GetLocalization(NotAllowedToShareMessageKey);
        public static string TreeViewProfileText => instance.GetLocalization(TreeViewProfileTextKey);
        public static string RemoveProfaileMessage => instance.GetLocalization(RemoveProfaileMessageKey);
        public static string DeleteProfaileMessage => instance.GetLocalization(DeleteProfaileMessageKey);
        public static string ToolClearExtraGraphicToolTip => instance.GetLocalization(ToolClearExtraGraphicTextKey);
        public static string ToolEraseProfileToolTip => instance.GetLocalization(ToolEraseProfileTextKey);
        public static string ToolSaveProfileAsSharedToolTip => instance.GetLocalization(ToolSaveProfileAsSharedTextKey);
        public static string ToolRemoveProfileToolTip => instance.GetLocalization(ToolRemoveProfileTextKey);
        public static string ToolOpenGraphWindowToolTip => instance.GetLocalization(ToolOpenGraphWindowTextKey);
        public static string ToolAddProfileToGraphToolTip => instance.GetLocalization(ToolAddProfileToGraphTextKey);
        public static string ToolAddProfileToExistingGraphToolTip => instance.GetLocalization(ToolAddProfileToExistingGraphTextKey);
        public static string ToolSetProfileSettingsToCalcToolTip => instance.GetLocalization(ToolSetProfileSettingsToCalcTextKey);
        public static string ToolPanOnMapToolTip => instance.GetLocalization(ToolBtnPanOnMapTextKey);
        public static string ToolBtnShowOnMapToolTip => instance.GetLocalization(ToolBtnShowOnMapTextKey);
        public static string ProfileListText => instance.GetLocalization(ProfileListTextKey);

        public static string RefreshButtonToolTip => Instance.GetLocalization(RefreshButtonToolTipKey);
        public static string CommonLengthText => Instance.GetLocalization(CommonLengthTextKey);
        public static string SelectedPrimitivesText => Instance.GetLocalization(SelectedPrimitivesTextKey);
        public static string ProfileTabPageText => Instance.GetLocalization(ProfileTabPageTextKey);
        public static string PofileTreeTabPageText => Instance.GetLocalization(PofileTreeTabPageTextKey);
        public static string LayersForCalcText => Instance.GetLocalization(LayersForCalcTextKey);

        public static string lblDEM_Text => Instance.GetLocalization(lblDEM_TextKey, "Шар ЦМР/ЦММ");

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
        public static string AboutSelectedText => Instance.GetLocalization(AboutSelectedTextKey);
        public static string NewProfileNameValue => Instance.GetLocalization(NewProfileNameValueKey);
        public static string GraphicsLayerValue => Instance.GetLocalization(GraphicsLayerValueKey);
        public static string PickCoordinatesToolMessage => Instance.GetLocalization(PickCoordinatesToolMessageKey);
        public static string AddAvailableProfilesSetsToolTip => Instance.GetLocalization(AddAvailableProfilesSetsToolTipKey);
        public static string TakeCoordToolTip => Instance.GetLocalization(TakeCoordToolTipKey, "Взяти координати з карти");
        public static string ShowCoordToolTip => Instance.GetLocalization(ShowCoordToolTipKey, "Показати координати на карті");
        public static string CopyCoordToolTip => Instance.GetLocalization(CopyCoordToolTipKey, "Копіювати координату");
        public static string PasteCoordToolTip => Instance.GetLocalization(PasteCoordToolTipKey, "Вставити координату");

        //AddAvailableProfilesSetsModalWindow
        public static string FiltersTitle => Instance.GetLocalization(FiltersTitleKey, "Фільтри");
        public static string NamePlaceholder => Instance.GetLocalization(NamePlaceholderKey, "Назва профілю");
        public static string CreatorPlaceholder => Instance.GetLocalization(CreatorPlaceholderKey, "Автор");
        public static string GraphTypeText => Instance.GetLocalization(GraphTypeTextKey, "Усі типи");
        public static string CreationDateText => Instance.GetLocalization(CreationDateTextKey, "Дата створення:");
        public static string FromText => Instance.GetLocalization(FromTextKey, "з");
        public static string ToText => Instance.GetLocalization(ToTextKey, "до");
        public static string PointsTypeText => Instance.GetLocalization(PointsTypeTextKey, "Лінія");
        public static string FunTypeText => Instance.GetLocalization(FunTypeTextKey, "\"Віяло\"");
        public static string PrimitiveTypeText => Instance.GetLocalization(PrimitiveTypeTextKey, "Примітив");
        public static string FilterText => Instance.GetLocalization(FilterTextKey, "Фільтрувати");
        public static string AddToSessionText => Instance.GetLocalization(AddToSessionTextKey, "Додати до сесії");
        public static string CloseText => Instance.GetLocalization(CloseTextKey, "Закрити");
        public static string ProfilesSetsNameColHeader => Instance.GetLocalization(ProfilesSetsNameColHeaderKey, "Назва профілю");
        public static string ProfilesSetsCreatorColHeader => Instance.GetLocalization(ProfilesSetsCreatorColHeaderKey, "Автор");
        public static string ProfilesSetsDateColHeader => Instance.GetLocalization(ProfilesSetsDateColHeaderKey, "Дата");
        public static string ProfilesSetsTypeColHeader => Instance.GetLocalization(ProfilesSetsTypeColHeaderKey, "Тип");
        public static string ProfilesSetsSharedColHeader => Instance.GetLocalization(ProfilesSetsSharedColHeaderKey, "Спільний");
        public static string ProfilesSetsTitle => Instance.GetLocalization(ProfilesSetsTitleKey, "Додати доступні набори профілів");
        public static string ResetText => Instance.GetLocalization(ResetTextKey, "Скинути");

        //ProfilesTreeModalWindow
        public static string PointsNodeText => Instance.GetLocalization(PointsNodeTextKey, "Відрізки");
        public static string FunNodeText => Instance.GetLocalization(FunNodeTextKey, "\"Віяло\"");
        public static string PrimitiveNodeText => Instance.GetLocalization(PrimitiveNodeTextKey, "Графіка");
        public static string CancelText => Instance.GetLocalization(CancelTextKey, "Відмінити");
        public static string OkText => Instance.GetLocalization(OkTextKey, "OK");
        public static string ProfilesTreeModalTitle => Instance.GetLocalization(ProfilesTreeModalTitleKey, "Дерево профілів");

        //GraphWindow
        public static string ObserverHeightText => Instance.GetLocalization(ObserverHeightTextKey, "Пункт спостереження (м)");
        public static string VisibleLineColorText => Instance.GetLocalization(VisibleLineColorTextKey, "видимі");
        public static string InvisibleLineColorText => Instance.GetLocalization(InvisibleLineColorTextKey, "невидимі");
        public static string SelectedProfileNameText => Instance.GetLocalization(SelectedProfileNameTextKey, "Профіль:");
        public static string CopyStripMenuItemText => Instance.GetLocalization(CopyStripMenuItemTextKey, "Скопіювати");
        public static string DisplayProfileSignatureToolTip => Instance.GetLocalization(DisplayProfileSignatureToolTipKey, "Відобразити підписи профілів на графіку");
        public static string DeleteSelectedProfileToolTip => Instance.GetLocalization(DeleteSelectedProfileToolTipKey, "Видалити вибраний профіль з графіка");
        public static string AddProfileGraphToolTip => Instance.GetLocalization(AddProfileGraphToolTipKey, "Додати профіль на графік");
        public static string PanToSelectedProfileToolTip => Instance.GetLocalization(PanToSelectedProfileToolTipKey, "Позиціонування на вибраний профіль");
        public static string PanToProfilesSetToolTip => Instance.GetLocalization(PanToProfilesSetToolTipKey, "Позиціонування на набір профілів");
        public static string ShowAllProfilesToolTip => Instance.GetLocalization(ShowAllProfilesToolTipKey, "Показати всі профілі");
        public static string ObserverHeightIgnoreToolTip => Instance.GetLocalization(ObserverHeightIgnoreToolTipKey, "Враховувати/ігнорувати висоту пункту спостереження при масштабуванні");
        public static string ZoomInToolTip => Instance.GetLocalization(ZoomInToolTipKey, "Збільшити масштаб");
        public static string ZoomOutToolTip => Instance.GetLocalization(ZoomOutToolTipKey, "Зменшити масштаб");
        public static string AddPageToolTip => Instance.GetLocalization(AddPageToolTipKey, "Додати нову вкладку для графіка");
        public static string DeletePageToolTip => Instance.GetLocalization(DeletePageToolTipKey, "Видалити вкладку");
        public static string SaveToolTip = Instance.GetLocalization(SaveToolTipKey, "Зберегти дані");
        public static string UpdateIntersectionsLinesToolTip => Instance.GetLocalization(UpdateIntersectionsLinesToolTipKey, "Розрахувати лінії перетину з шарами");
        public static string ProfileDetailsToolTip => Instance.GetLocalization(ProfileDetailsToolTipKey, "Щоб скопіювати віділені рядки натисніть праву клавішу миші");
        public static string ChangeAllObserversHeightsToolTip => Instance.GetLocalization(ChangeAllObserversHeightsToolTipKey, "Змінити висоту всіх пунктів спостереження");
        public static string ProfilePropertiesIsVisibleColToolTip => Instance.GetLocalization(ProfilePropertiesIsVisibleColToolTipKey, "Показати/сховати профіль");
        public static string ProfilePropertiesProfileNumberColToolTip => Instance.GetLocalization(ProfilePropertiesProfileNumberColToolTipKey, "Номер профіля");
        public static string ProfilePropertiesAzimuthColToolTip => Instance.GetLocalization(ProfilePropertiesAzimuthColToolTipKey, "Азимут");
        public static string ProfilePropertiesObserverHeightColToolTip => Instance.GetLocalization(ProfilePropertiesObserverHeightColToolTipKey, "Висота пункту спостереження (м)");
        public static string ProfilePropertiesProfileLengthColToolTip => Instance.GetLocalization(ProfilePropertiesProfileLengthColToolTipKey, "Довжина профіля (м)");
        public static string ProfilePropertiesMinHeightColToolTip => Instance.GetLocalization(ProfilePropertiesMinHeightColToolTipKey, "Мінімальна висота (м)");
        public static string ProfilePropertiesMaxHeightColToolTip => Instance.GetLocalization(ProfilePropertiesMaxHeightColToolTipKey, "Максимальна висота (м)");
        public static string ProfilePropertiesHeightDifferenceColToolTip => Instance.GetLocalization(ProfilePropertiesHeightDifferenceColToolTipKey, "Різниця висот (м)");
        public static string ProfilePropertiesDescendingAngleColToolTip => Instance.GetLocalization(ProfilePropertiesDescendingAngleColToolTipKey, "Максимальний кут спуску (градуси)");
        public static string ProfilePropertiesAscendingAngleColToolTip => Instance.GetLocalization(ProfilePropertiesAscendingAngleColToolTipKey, "Максимальний кут підйому (градуси)");
        public static string ProfilePropertiesVisiblePercentColToolTip => Instance.GetLocalization(ProfilePropertiesVisiblePercentColToolTipKey, "Процент видимих зон");
        public static string ProfilePropertiesIsVisibleColHeader => Instance.GetLocalization(ProfilePropertiesIsVisibleColHeaderKey, "ВД");
        public static string ProfilePropertiesProfileNumberColHeader => Instance.GetLocalization(ProfilePropertiesProfileNumberColHeaderKey, "Н");
        public static string ProfilePropertiesAzimuthColHeader => Instance.GetLocalization(ProfilePropertiesAzimuthColHeaderKey, "АЗ");
        public static string ProfilePropertiesObserverHeightColHeader => Instance.GetLocalization(ProfilePropertiesObserverHeightColHeaderKey, "В");
        public static string ProfilePropertiesProfileLengthColHeader => Instance.GetLocalization(ProfilePropertiesProfileLengthColHeaderKey, "Д");
        public static string ProfilePropertiesMinHeightColHeader => Instance.GetLocalization(ProfilePropertiesMinHeightColHeaderKey, "МнВ");
        public static string ProfilePropertiesMaxHeightColHeader => Instance.GetLocalization(ProfilePropertiesMaxHeightColHeaderKey, "МкВ");
        public static string ProfilePropertiesHeightDifferenceColHeader => Instance.GetLocalization(ProfilePropertiesHeightDifferenceColHeaderKey, "РВ");
        public static string ProfilePropertiesDescendingAngleColHeader => Instance.GetLocalization(ProfilePropertiesDescendingAngleColHeaderKey, "КС");
        public static string ProfilePropertiesAscendingAngleColHeader => Instance.GetLocalization(ProfilePropertiesAscendingAngleColHeaderKey, "КП");
        public static string ProfilePropertiesVisiblePercentColHeader => Instance.GetLocalization(ProfilePropertiesVisiblePercentColHeaderKey, "ВП");
        public static string ProfileDetailsStateText => Instance.GetLocalization(ProfileDetailsStateTextKey, "Стан:");
        public static string ProfileDetailsEndPointsText => Instance.GetLocalization(ProfileDetailsEndPointsTextKey, "Початок - кінець");
        public static string ProfileDetailsAzimuthText => Instance.GetLocalization(ProfileDetailsAzimuthTextKey, "Азимут:");
        public static string ProfileDetailsLengthText => Instance.GetLocalization(ProfileDetailsLengthTextKey, "Довжина:");
        public static string ProfileDetailsHeightText => Instance.GetLocalization(ProfileDetailsHeightTextKey, "Висота (м):");
        public static string ProfileDetailsAnglesText => Instance.GetLocalization(ProfileDetailsAnglesTextKey, "Кути:");
        public static string ProfileDetailsVisibilityText => Instance.GetLocalization(ProfileDetailsVisibilityTextKey, "Видимі зони (%):");
        public static string ProfilesSetSharedText => Instance.GetLocalization(ProfilesSetSharedTextKey, "Спільний");
        public static string ProfilesSetNotSharedText => Instance.GetLocalization(ProfilesSetNotSharedTextKey, "Приватний");
        public static string RemovingProfileMessage => Instance.GetLocalization(RemovingProfileMessageKey, "Ви дійсно хочете видалити профіль?");
        public static string RemovingTabMessage => Instance.GetLocalization(RemovingTabMessageKey, "Ви дійсно хочете видалити вкладку?");
        public static string MessageBoxTitle => Instance.GetLocalization(MessageBoxTitleKey, "MilSpace");
        public static string SelectFolderDescription => Instance.GetLocalization(SelectFolderDescriptionKey, "Виберіть папку для збереження даних");
        public static string FolderAlreadyExistMessage => Instance.GetLocalization(FolderAlreadyExistMessageKey, "Каталог з іменем {1} вже існує \n Ви дійсно хочете оновити дані?");
        public static string DataExportProfilePropertiesHeader => Instance.GetLocalization(DataExportProfilePropertiesHeaderKey, $"Номер; Назва профілю; Зовнішній; Стан; Точка початку; Точка кінцю;" +
                                                                                                                                 $"Азимут; Пункт спостереження; Довжина; Макс. висота; Різниця висот;" +
                                                                                                                                 $"Мін. висота; Макс. кут підйому; Макс. кут спуску; Процент видимих зон");
        public static string DataExportPointsPropertiesHeader => Instance.GetLocalization(DataExportPointsPropertiesHeaderKey, "Номер; X; Y; Z; Відстань; Точка перегину; Видима; Перетини");
        public static string LineNotFoundErrorMessage => Instance.GetLocalization(LineNotFoundErrorMessageKey, "Лінію не знайдено");
        public static string GraphTitle => Instance.GetLocalization(GraphTitleKey, "Графік");
        public static string SavedProfilesModalWindowText => Instance.GetLocalization(SavedProfilesModalWindowTextKey);


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
