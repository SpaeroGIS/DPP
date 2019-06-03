using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;

namespace MilSpace.Profile
{
    internal class ProfileTreeNode: TreeNode
    {
        private const string KeyColumnName = "Key";
        private const string AttributeColumnName = "Attribute";
        private const string ValueColumnName = "Value";

        #region AttributeNames
        private const string profileName = "Имя профиля:";
        private const string profileId = "Идентификатор профиля:";
        private const string profileType = "Тип профиля:";
        private const string profileDistance = "Дистанция профиля:";
        private const string firstPoint = "Стартовая точка:";
        private const string secondPoint = "Конечная точка";
        private const string firstPointHeight = "Высота стартовой точки:";
        private const string secondPointHeight = "Высота конечной точки:";
        private const string lineDistance = "Длинна линии:";
        private const string linesCount = "Количечство линий";
        private const string basePoint = "Базовая точка:";
        private const string toPoint = "Конечная точка:";
        private const string azimuth1 = "Азимут 1:";
        private const string azimuth2 = "Азимут 2:";
        private const string mapName = "Имя карты:";
        private const string creatorName = "Автор:";
        private const string date = "Дата:";
        #endregion


        internal string ProfileName { get; set; }
        internal int ProfileId { get; set; }
        internal double ProfileDistance { get; set; }

        internal double LineDistance { get; set; }

        internal int LinesCount { get; set; }

        internal IPoint BasePoint { get; set; }

        internal IPoint ToPoint { get; set; }

        internal string ProfileType { get; set; }



        internal double AzimuthFirst { get; set; }

        internal double AzimuthSecond { get; set; }

        internal string MapName { get; set; }

        internal string CreatorName { get; set; }

        internal DateTime Date { get; set; }

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
            SetAttributeValue(Attributes,AttributeKeys.SectionFirstPointHeight, height);
        }

        internal void SetToPointHeight(string height)
        {
            SetAttributeValue(Attributes, AttributeKeys.SectionSecondPointHeight, height);
        }

        internal void SetAzimuth1(string azimuth1Value)
        {
            SetAttributeValue(Attributes, AttributeKeys.Azimuth1, azimuth1Value);
        }

        internal void SetAzimuth2(string azimuth2Value)
        {
            SetAttributeValue(Attributes, AttributeKeys.Azimuth2, azimuth2Value);
        }

        //internal void SetMapName(string mapNameValue)
        //{
        //    SetAttributeValue(Attributes, AttributeKeys.MapName, mapNameValue);
        //}

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

            var secondPointHeightRow = table.NewRow();
            secondPointHeightRow[KeyColumnName] = AttributeKeys.SectionSecondPointHeight;
            secondPointHeightRow[AttributeColumnName] = secondPointHeight;
            table.Rows.Add(secondPointHeightRow);

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

            var azimuthFirstRow = table.NewRow();
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
