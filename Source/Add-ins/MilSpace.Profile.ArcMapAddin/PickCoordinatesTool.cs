using System.Globalization;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;

namespace MilSpace.Profile
{
    public class PickCoordinates : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        public PickCoordinates()
        {

            Point = new PointClass();
            Point.PutCoords(0, 0);
        }

        public IPoint Point { get; private set; }

        protected override void OnUpdate()
        {

        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            UID toolId = new UIDClass();
            toolId.Value = ThisAddIn.IDs.PickCoordinates;
            var mxdDoc = ArcMap.Document;
            var screenDisplay = mxdDoc.ActiveView.ScreenDisplay;
            var point = screenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);

            point.SpatialReference = mxdDoc.FocusMap.SpatialReference;

            EsriTools.ProjectToWgs84(point);

            if (ProfileForms.ProfileCalcUI.ToolbarButtonClicked == ProfileForms.ProfileCalcUI.btnPickFirstPoint)
            {
                ProfileForms.ProfileCalcUI.txtFirstPointX.Text = point.X.ToString("F4");//(CultureInfo.InvariantCulture);
                ProfileForms.ProfileCalcUI.txtFirstPointY.Text = point.Y.ToString("F4");
            }

            if (ProfileForms.ProfileCalcUI.ToolbarButtonClicked == ProfileForms.ProfileCalcUI.btnPickSecondPoint)
            {
                ProfileForms.ProfileCalcUI.txtSecondPointX.Text = point.X.ToString("F4");
                ProfileForms.ProfileCalcUI.txtSecondPointY.Text = point.Y.ToString("F4");
            }

            if (ProfileForms.ProfileCalcUI.ToolbarButtonClicked == ProfileForms.ProfileCalcUI.btnPickBasePoint)
            {
                ProfileForms.ProfileCalcUI.txtBasePointX.Text = point.X.ToString("F4");
                ProfileForms.ProfileCalcUI.txtBasePointY.Text = point.Y.ToString("F4");
            }

        }
    }

}
