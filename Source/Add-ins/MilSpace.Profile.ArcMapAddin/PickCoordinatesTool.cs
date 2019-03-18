using System.Globalization;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

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
            Point = screenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);
            if (ProfileForms.ProfileCalcUI.ToolbarButtonClicked == ProfileForms.ProfileCalcUI.btnPickFirstPoint)
            {
                ProfileForms.ProfileCalcUI.txtFirstPointX.Text = Point.X.ToString(CultureInfo.InvariantCulture);
                ProfileForms.ProfileCalcUI.txtFirstPointY.Text = Point.Y.ToString(CultureInfo.InvariantCulture);
            }

            if (ProfileForms.ProfileCalcUI.ToolbarButtonClicked == ProfileForms.ProfileCalcUI.btnPickSecondPoint)
            {
                ProfileForms.ProfileCalcUI.txtSecondPointX.Text = Point.X.ToString(CultureInfo.InvariantCulture);
                ProfileForms.ProfileCalcUI.txtSecondPointY.Text = Point.Y.ToString(CultureInfo.InvariantCulture);
            }

            if (ProfileForms.ProfileCalcUI.ToolbarButtonClicked == ProfileForms.ProfileCalcUI.btnPickBasePoint)
            {
                ProfileForms.ProfileCalcUI.txtBasePointX.Text = Point.X.ToString(CultureInfo.InvariantCulture);
                ProfileForms.ProfileCalcUI.txtBasePointY.Text = Point.Y.ToString(CultureInfo.InvariantCulture);
            }

        }
    }

}
