using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class ObservationPointsListModalWindow : Form
    {
        private List<FromLayerPointModel> _points = new List<FromLayerPointModel>();
        private Logger _log = Logger.GetLoggerEx("MilSpace.Profile.ModalWindows.ObservationPointsListModalWindow");
        internal FromLayerPointModel SelectedPoint;

        public ObservationPointsListModalWindow(IFeatureLayer observPointsLayer)
        {
            InitializeComponent();
            LocalizeStrings();
            FillPointsGrid(observPointsLayer);
        }

        private void LocalizeStrings()
        {
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalObservPointsTitle", "Вибір точки з шару точок спостереження");
            btnChoosePoint.Text = LocalizationContext.Instance.FindLocalizedElement("BtnChooseText", "Обрати");
            dgvPoints.Columns["IdCol"].HeaderText = LocalizationContext.Instance.FindLocalizedElement("DgvObservPointsIdHeader", "Ідентифікатор");
            dgvPoints.Columns["TitleCol"].HeaderText = LocalizationContext.Instance.FindLocalizedElement("DgvObservPointsTitleHeader", "Назва");
        }

        private void FillPointsGrid(IFeatureLayer layer)
        {
            var featureClass = layer.FeatureClass;
            var idFieldIndex = featureClass.FindField("OBJECTID");
            var titleFieldIndex = featureClass.FindField("TitleOp");

            if(idFieldIndex == -1)
            {
                _log.WarnEx($"> FillPointsGrid. Warning: Cannot find fild \"OBJECTID\" in featureClass {featureClass.AliasName}");
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле OBJECTID"),
                                    LocalizationContext.Instance.MessageBoxTitle);

                return;
            }

            if(titleFieldIndex == -1)
            {
                _log.WarnEx($"> FillPointsGrid. Warning: Cannot find fildTitleOp in featureClass {featureClass.AliasName}");
            }

            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = "OBJECTID > 0";

            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            try
            {
                while(feature != null)
                {
                    var shape = feature.Shape;

                    var point = shape as IPoint;
                    var pointCopy = point.Clone();
                    pointCopy.Project(EsriTools.Wgs84Spatialreference);

                    int id = -1;
                    string titleField = string.Empty;

                    if(idFieldIndex >= 0)
                    {
                        id = (int)feature.Value[idFieldIndex];
                    }

                    if(titleFieldIndex >= 0)
                    {
                        titleField = feature.Value[titleFieldIndex].ToString();
                    }

                    _points.Add(new FromLayerPointModel { Point = pointCopy, ObjId = id, DisplayedField = titleField });

                    feature = featureCursor.NextFeature();
                }

                dgvPoints.Rows.Clear();

                foreach(var point in _points)
                {
                    dgvPoints.Rows.Add(point.ObjId, point.DisplayedField, Math.Round(point.Point.X, 5), Math.Round(point.Point.Y, 5));
                }

            }
            catch(Exception ex)
            {
                _log.ErrorEx($"> FillPointsGrid Exception. ex.Message:{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            if(dgvPoints.SelectedRows.Count > 0)
            {
                SelectedPoint = _points.First(p => p.ObjId == (int)dgvPoints.SelectedRows[0].Cells["IdCol"].Value);
            }
        }
    }
}
