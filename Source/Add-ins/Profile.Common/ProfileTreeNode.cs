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
        private static readonly string profileName = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileNameText", "Назва:");
        private static readonly string profileId = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileIdText", "Ідентифікатор:");
        private static readonly string profileType = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileTypeText", "Тип:");
        private static readonly string firstPoint = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileStartPointText", "Початок в точці:");
        private static readonly string secondPoint = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileEndPointText", "Кінець в точці:");
        private static readonly string firstPointHeight = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfilePointOfViewText", "Точка спостереження:");
        private static readonly string lineDistance = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileDistanceText", "Відстань:");
        private static readonly string linesCount = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileCountText", "Кількість ліній:");
        private static readonly string basePoint = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileBasePointText", "Базова точка (довгота / широта):");
        private static readonly string azimuth = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileAzimuthText", "Азимут:");
        private static readonly string azimuth1 = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileAzimuth1Text", "Азимут 1:");
        private static readonly string azimuth2 = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileAzimuth2Text", "Азимут 2:");
        private static readonly string creatorName = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileCreatorText", "Автор:");
        private static readonly string date = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileDateText", "Дата:");
        private static readonly string toPoint = LocalizationContext.Instance.FindLocalizedElement("TxtAttrProfileEndPointText", "Кінець в точці:");


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

        internal void SetStartPoint(string startPointValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.FromPoint, startPointValue);
        }

        internal void SetBasePoint(string basePointValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.BasePoint, basePointValue);
        }

        internal void SetToPoint(string toPointValue)
        {
            SetAttributeValue(Attributes, AttributeKeys.ToPoint, toPointValue);
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


            var firstPointRow = table.NewRow();
            firstPointRow[KeyColumnName] = AttributeKeys.SectionFirstPoint;
            firstPointRow[AttributeColumnName] = firstPoint;
            table.Rows.Add(firstPointRow);

            var firstPointHeightRow = table.NewRow();
            firstPointHeightRow[KeyColumnName] = AttributeKeys.SectionFirstPointHeight;
            firstPointHeightRow[AttributeColumnName] = firstPointHeight;
            table.Rows.Add(firstPointHeightRow);

            var secondPointRow = table.NewRow();
            secondPointRow[KeyColumnName] = AttributeKeys.SectionSecondPoint;
            secondPointRow[AttributeColumnName] = secondPoint;
            table.Rows.Add(secondPointRow);

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

            var basePointRow = table.NewRow();
            basePointRow[KeyColumnName] = AttributeKeys.BasePoint;
            basePointRow[AttributeColumnName] = basePoint;
            table.Rows.Add(basePointRow);

            var toPointRow = table.NewRow();
            toPointRow[KeyColumnName] = AttributeKeys.ToPoint;
            toPointRow[AttributeColumnName] = toPoint;
            table.Rows.Add(toPointRow);

            toPointRow = table.NewRow();
            toPointRow[KeyColumnName] = AttributeKeys.FromPoint;
            toPointRow[AttributeColumnName] = firstPoint;
            table.Rows.Add(toPointRow);

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
