using System.Globalization;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using static MilSpace.Profile.DockableWindowMilSpaceProfileCalc;

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
            var pointToSave = screenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);

            point.SpatialReference = mxdDoc.FocusMap.SpatialReference;

            pointToSave.SpatialReference = mxdDoc.FocusMap.SpatialReference;

            EsriTools.ProjectToWgs84(point);

            var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileCalc.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileCalc);

            if (winImpl.MilSpaceProfileCalsController.View.ActiveButton == ProfileSettingsPointButton.PointsFist)
            {

                var points = ProfileForms.ProfileGeometries[ProfileSettingsTypeEnum.Points] as IPoint[];
                points[0] = point;


                winImpl.MilSpaceProfileCalsController.SetFirsPointForLineProfile(point);

                //ProfileForms.ProfileCalcUI.txtFirstPointX.Text = point.X.ToString("F4");//(CultureInfo.InvariantCulture);
                //ProfileForms.ProfileCalcUI.txtFirstPointY.Text = point.Y.ToString("F4");
            }

            if (winImpl.MilSpaceProfileCalsController.View.ActiveButton == ProfileSettingsPointButton.PointsSecond)
            {
                var points = ProfileForms.ProfileGeometries[ProfileSettingsTypeEnum.Points] as IPoint[];
                points[1] = point;

                winImpl.MilSpaceProfileCalsController.SetSecondfPointForLineProfile(point);

                //ProfileForms.ProfileCalcUI.txtSecondPointX.Text = point.X.ToString("F4");
                //ProfileForms.ProfileCalcUI.txtSecondPointY.Text = point.Y.ToString("F4");
            }

            if (winImpl.MilSpaceProfileCalsController.View.ActiveButton == ProfileSettingsPointButton.CenterFun)
            {

                winImpl.MilSpaceProfileCalsController.SetCenterPointForFunProfile(point);

                //ProfileForms.ProfileCalcUI.txtBasePointX.Text = point.X.ToString("F4");
                //ProfileForms.ProfileCalcUI.txtBasePointY.Text = point.Y.ToString("F4");
            }

        }
    }

}
