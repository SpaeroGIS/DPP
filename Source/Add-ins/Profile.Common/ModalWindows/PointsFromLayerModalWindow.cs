using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class PointsFromLayerModalWindow : Form
    {
        private MapLayersManager _mapLayersManager = new MapLayersManager(ArcMap.Document.ActiveView);
        private static Logger _log = Logger.GetLoggerEx("MilSpace.Profile.ModalWindows.PointsFromLayerModalWindow");
        private List<FromLayerPointModel> _points; 
        public IPoint SelectedPoint;

        public PointsFromLayerModalWindow()
        {
            InitializeComponent();
            LocalizeStrings();
            PopulateLayerComboBox();
        }

        private void LocalizeStrings()
        {
            lblChooseLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblLayersText", "Шар");
            lblField.Text = LocalizationContext.Instance.FindLocalizedElement("LblFieldsText", "Поле");
            btnChoosePoint.Text = LocalizationContext.Instance.FindLocalizedElement("BtnChooseText", "Обрати");

            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalPointsFromLayerTitle", "Вибір точки з точкового шару");
        }

        private void PopulateLayerComboBox()
        {
            var layers = _mapLayersManager.PointLayers.Where(layer => !layer.Name.Equals("MilSp_Visible_ObservPoints"));

            cmbLayers.Items.Clear();
            cmbLayers.Items.AddRange(layers.Select(l => l.Name).ToArray());
        }

        private void PopulateFieldsComboBox(string layerName)
        {
            var fields = new List<string>();

            try
            {
                var layer = _mapLayersManager.GetLayer(layerName);
                fields = _mapLayersManager.GetFeatureLayerFields(layer as IFeatureLayer);
            }
            catch(Exception ex)
            {
                _log.ErrorEx($"> PopulateFieldsComboBox. Exception: {ex}");
                return;
            }

            if(fields.Count == 0)
            {
                cmbFields.Enabled = false;
                FillPointsGrid();
                return;
            }

            cmbFields.Items.Clear();
            cmbFields.Enabled = true;
            cmbFields.Items.AddRange(fields.ToArray());
        }

        private void FillPointsGrid(string selectedField = null)
        {
            var layer = _mapLayersManager.GetLayer(cmbLayers.SelectedItem.ToString()) as IFeatureLayer;
            _points = new List<FromLayerPointModel>();

            var featureClass = layer.FeatureClass;
            var idFieldIndex = featureClass.FindField("OBJECTID");
            var selectedFieldIndex = (selectedField != null) ? featureClass.FindField(selectedField) : -2;

            if(idFieldIndex == -1)
            {
                _log.WarnEx($"> FillPointsGrid. Warning: Cannot find fild \"OBJECTID\" in featureClass {featureClass.AliasName}");
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле OBJECTID"),
                                    LocalizationContext.Instance.MessageBoxTitle);

                return;
            }

            if(selectedFieldIndex == -1)
            {
                _log.WarnEx($"> FillPointsGrid. Warning: Cannot find fild {selectedField} in featureClass {featureClass.AliasName}");
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

                    if(featureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        var point = shape as IPoint;
                        var pointCopy = point.Clone();
                        pointCopy.Project(EsriTools.Wgs84Spatialreference);

                        int id = -1;
                        string displayedField = string.Empty;

                        if(idFieldIndex >= 0)
                        {
                            id = (int)feature.Value[idFieldIndex];
                        }

                        if(selectedFieldIndex >= 0)
                        {
                            displayedField = feature.Value[selectedFieldIndex].ToString();
                        }

                        _points.Add(new FromLayerPointModel { Point = point, ObjId = id, DisplayedField = displayedField });
                    }
                    else
                    {
                        _log.ErrorEx($"> FillPointsGrid. Exception: Layer {cmbLayers.SelectedItem.ToString()} doesn`t have a feature class with type esriGeometryPoint");
                    }

                    feature = featureCursor.NextFeature();
                }

                dgvPoints.Rows.Clear();
                dgvPoints.Columns["DisplayFieldCol"].Visible = selectedField != null;

                foreach(var point in _points)
                {
                    dgvPoints.Rows.Add(point.ObjId, point.DisplayedField, Math.Round(point.Point.X, 5), Math.Round(point.Point.Y, 5));
                }

            }
            catch(Exception ex)
            {
                _log.ErrorEx($"> ImportFromLayer Exception. ex.Message:{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }
        }

        private void CmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbFields.SelectedItem = null;
            PopulateFieldsComboBox(cmbLayers.SelectedItem.ToString());
            dgvPoints.Rows.Clear();
            lblLayer.Text = cmbLayers.SelectedItem.ToString();
        }

        private void CmbFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbFields.SelectedItem == null)
            {
                return;
            }

            dgvPoints.Columns["DisplayFieldCol"].HeaderText = cmbFields.SelectedItem.ToString();
            FillPointsGrid(cmbFields.SelectedItem.ToString());
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            if(dgvPoints.SelectedRows.Count > 0)
            {
                SelectedPoint = _points.First(point => point.ObjId == (int)dgvPoints.SelectedRows[0].Cells["IdCol"].Value).Point;
            }
        }
    }
}
