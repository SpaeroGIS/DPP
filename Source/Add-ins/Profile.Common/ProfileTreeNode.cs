using MilSpace.Profile.Localization;
using System.Data;
using System.Windows.Forms;

namespace MilSpace.Profile
{
    internal class ProfileTreeNode : TreeNode
    {
        private const string KeyColumnName = "Key";
        private const string AttributeColumnName = "Attribute";
        private const string ValueColumnName = "Value";

        #region AttributeNames
        //DS TODO: Change localized text
        private static readonly string profileName = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileNameText", "Назва:");
        private static readonly string profileId = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileIdText", "Ідентифікатор:");
        private static readonly string profileType = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileTypeText", "Тип:");
        private static readonly string firstPointX = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileStartPointXText", "Початкова точка (довгота):");
        private static readonly string secondPointX = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileEndPointText", "Кінець в точці:");
        private static readonly string firstPointY = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileStartPointYText", "Початкова точка (широта):");
        private static readonly string secondPointY = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileEndPointText", "Кінець в точці:");
        private static readonly string firstPointHeight = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfilePointOfViewText", "Висота, м:");
        private static readonly string lineDistance = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileDistanceText", "Відстань:");
        private static readonly string linesCount = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileCountText", "Кількість ліній:");
        private static readonly string basePointX = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileBasePointXText", "Базова точка (довгота):");
        private static readonly string basePointY = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileBasePointYText", "Базова точка (широта):");
        private static readonly string azimuth = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileAzimuthText", "Азимут:");
        private static readonly string azimuth1 = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileAzimuth1Text", "Початковий азимут:");
        private static readonly string azimuth2 = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileAzimuth2Text", "Кінцевий азимут:");
        private static readonly string creatorName = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileCreatorText", "Автор:");
        private static readonly string date = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileDateText", "Дата:");
        private static readonly string toPointX = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileEndPointXText", "Кінцева точка (довгота):");
        private static readonly string toPointY = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileEndPointYText", "Кінцева точка (широта):");
        private static readonly string surface = LocalizationContext.Instance.FindLocalizedElement("TxtTreeViewProfileSurfaceText", "Поверхня:");

        #endregion


        //internal string ProfileName { get; set; }
        //internal int ProfileId { get; set; }
        //internal double ProfileDistance { get; set; }

        //internal double LineDistance { get; set; }

        //internal int LinesCount { get; set; }

        //internal IPoint BasePoint { get; set; }

        //internal IPoint ToPoint { get; set; }

        //internal string ProfileType { get; set; }

        //internal double AzimuthFirst { get; set; }

        //internal double AzimuthSecond { get; set; }

        //internal string MapName { get; set; }

        //internal string CreatorName { get; set; }

        //internal DateTime Date { get; set; }

        internal DataTable Attributes { get; private set; }

        internal bool IsProfileNode { get; set; }

        public ProfileTreeNode(string text, int imageIndex, int selectedImageIndex) : base(text, imageIndex, selectedImageIndex)
        {
            Attributes = GenerateDataTable();
        }

        internal void SetProfileName(string profileNameValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.ProfileName, profileNameValue);
        }

        internal void SetProfileType(string profileTypeValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.ProfileType, profileTypeValue);
        }

        internal void SetLineDistance(string lineDistanceValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.LineDistance, lineDistanceValue);
        }

        internal void SetLineCount(string lineCountValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.LinesCount, lineCountValue);
        }

        internal void SetStartPointX(string startPointXValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.FromPointX, startPointXValue);
        }

        internal void SetStartPointY(string startPointYValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.FromPointX, startPointYValue);
        }

        internal void SetBasePointX(string basePointValueX)
        {
            SetAttributeValue(Attributes, AttributeKeys.BasePointX, basePointValueX);
        }

        internal void SetBasePointY(string basePointValueY)
        {
            SetAttributeValue(Attributes, AttributeKeys.BasePointY, basePointValueY);
        }

        internal void SetToPointX(string toPointValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.ToPointX, toPointValue);
        }

        internal void SetToPointY(string toPointValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.ToPointY, toPointValue);
        }

        internal void SetBasePointHeight(string height)
        {
            SetAttributeValue(Attributes, AttributeKeys.SectionFirstPointHeight, height);
        }

        internal void SetToPointHeight(string height)
        {
            SetAttributeValue(Attributes, AttributeKeys.SectionSecondPointHeight, height);
        }

        internal void SetAzimuth(string azimuth1Value)
        {

            SetAttributeValue(Attributes, AttributeKeys.Azimuth, azimuth1Value);
        }

        internal void SetAzimuth1(double azimuth1Value)
        {

            SetAttributeValue(Attributes, AttributeKeys.Azimuth1, azimuth1Value == double.MinValue ? "" : azimuth1Value.ToString("F0"));
        }

        internal void SetAzimuth2(double azimuth2Value)
        {
            SetAttributeValue(Attributes, AttributeKeys.Azimuth2, azimuth2Value == double.MinValue ? "" : azimuth2Value.ToString("F0"));
        }


        internal void SetCreatorName(string creatorNameValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.CreatorName, creatorNameValue);
        }

        internal void SetDate(string dateValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.Date, dateValue);
        }

        internal void SetSurface(string surfaceValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.Surface, surfaceValue);
        }

        private DataTable GenerateDataTable()
        {
            var table = new DataTable();

            table.Columns.Add(KeyColumnName, typeof(string));
            table.Columns.Add(AttributeColumnName, typeof(string));
            table.Columns.Add(ValueColumnName, typeof(string));
            table.PrimaryKey = new DataColumn[] { table.Columns[KeyColumnName] };

            var profileNameRow = table.NewRow();
            profileNameRow[KeyColumnName] = AttributeKeys.ProfileName;
            profileNameRow[AttributeColumnName] = profileName;
            table.Rows.Add(profileNameRow);


            var profileTypeRow = table.NewRow();
            profileTypeRow[KeyColumnName] = AttributeKeys.ProfileType;
            profileTypeRow[AttributeColumnName] = profileType;
            table.Rows.Add(profileTypeRow);

            //var firstPointXRow = table.NewRow();
            //firstPointXRow[KeyColumnName] = AttributeKeys.SectionFirstPointX;
            //firstPointXRow[AttributeColumnName] = firstPointX;
            //table.Rows.Add(firstPointXRow);

            //var firstPointYRow = table.NewRow();
            //firstPointYRow[KeyColumnName] = AttributeKeys.SectionFirstPointY;
            //firstPointYRow[AttributeColumnName] = firstPointY;
            //table.Rows.Add(firstPointYRow);

            //var firstPointHeightRow = table.NewRow();
            //firstPointHeightRow[KeyColumnName] = AttributeKeys.SectionFirstPointHeight;
            //firstPointHeightRow[AttributeColumnName] = firstPointHeight;
            //table.Rows.Add(firstPointHeightRow);

            //var secondPointXRow = table.NewRow();
            //secondPointXRow[KeyColumnName] = AttributeKeys.SectionSecondPointX;
            //secondPointXRow[AttributeColumnName] = secondPointX;
            //table.Rows.Add(secondPointXRow);

            //var secondPointYRow = table.NewRow();
            //secondPointYRow[KeyColumnName] = AttributeKeys.SectionSecondPointY;
            //secondPointYRow[AttributeColumnName] = secondPointY;
            //table.Rows.Add(secondPointYRow);


            //var secondPointHeightRow = table.NewRow();
            //secondPointHeightRow[KeyColumnName] = AttributeKeys.SectionSecondPointHeight;
            //secondPointHeightRow[AttributeColumnName] = secondPointHeight;
            //table.Rows.Add(secondPointHeightRow);

            var lineDistanceRow = table.NewRow();
            lineDistanceRow[KeyColumnName] = AttributeKeys.LineDistance;
            lineDistanceRow[AttributeColumnName] = lineDistance;
            table.Rows.Add(lineDistanceRow);

            var linesCountRow = table.NewRow();
            linesCountRow[KeyColumnName] = AttributeKeys.LinesCount;
            linesCountRow[AttributeColumnName] = linesCount;
            table.Rows.Add(linesCountRow);

            var basePointXRow = table.NewRow();
            basePointXRow[KeyColumnName] = AttributeKeys.BasePointX;
            basePointXRow[AttributeColumnName] = basePointX;
            table.Rows.Add(basePointXRow);

            var basePointYRow = table.NewRow();
            basePointYRow[KeyColumnName] = AttributeKeys.BasePointY;
            basePointYRow[AttributeColumnName] = basePointY;
            table.Rows.Add(basePointYRow);

            var toPointXRow = table.NewRow();
            toPointXRow[KeyColumnName] = AttributeKeys.ToPointX;
            toPointXRow[AttributeColumnName] = toPointX;
            table.Rows.Add(toPointXRow);

            var firstPointXRow = table.NewRow();
            firstPointXRow[KeyColumnName] = AttributeKeys.FromPointX;
            firstPointXRow[AttributeColumnName] = firstPointX;
            table.Rows.Add(firstPointXRow);

            var firstPointYRow = table.NewRow();
            firstPointYRow[KeyColumnName] = AttributeKeys.FromPointY;
            firstPointYRow[AttributeColumnName] = firstPointY;
            table.Rows.Add(firstPointYRow);

            var azimuthFirstRow = table.NewRow();
            azimuthFirstRow[KeyColumnName] = AttributeKeys.Azimuth;
            azimuthFirstRow[AttributeColumnName] = azimuth;
            table.Rows.Add(azimuthFirstRow);

            azimuthFirstRow = table.NewRow();
            azimuthFirstRow[KeyColumnName] = AttributeKeys.Azimuth1;
            azimuthFirstRow[AttributeColumnName] = azimuth1;
            table.Rows.Add(azimuthFirstRow);

            var azimuthSecondRow = table.NewRow();
            azimuthSecondRow[KeyColumnName] = AttributeKeys.Azimuth2;
            azimuthSecondRow[AttributeColumnName] = azimuth2;
            table.Rows.Add(azimuthSecondRow);

            var creatorNameRow = table.NewRow();
            creatorNameRow[KeyColumnName] = AttributeKeys.CreatorName;
            creatorNameRow[AttributeColumnName] = creatorName;
            table.Rows.Add(creatorNameRow);

            var dateNameRow = table.NewRow();
            dateNameRow[KeyColumnName] = AttributeKeys.Date;
            dateNameRow[AttributeColumnName] = date;
            table.Rows.Add(dateNameRow);

            var surfaceRow = table.NewRow();
            surfaceRow[KeyColumnName] = AttributeKeys.Surface;
            surfaceRow[AttributeColumnName] = surface;
            table.Rows.Add(surfaceRow);

            return table;
        }

        private DataRow GetRowByKey(DataTable table, string attributeKey)
        {
            var row = table.Rows.Find(attributeKey);// ($"{KeyColumnName}={attributeKey}").First();
            return row;
        }

        private void SetAttributeValue(DataTable table, string attributeKey, string attributeValue)
        {
            var row = GetRowByKey(table, attributeKey);
            row[ValueColumnName] = attributeValue;
        }
    }
}
