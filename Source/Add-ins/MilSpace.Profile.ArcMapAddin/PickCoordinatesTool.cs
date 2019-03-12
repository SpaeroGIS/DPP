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
            ProfileForms.ProfileCalcUI.txtFirstPointX.Text = Point.X.ToString(CultureInfo.InvariantCulture);
            ProfileForms.ProfileCalcUI.txtFirstPointY.Text = Point.Y.ToString(CultureInfo.InvariantCulture);
        }
    }

}
