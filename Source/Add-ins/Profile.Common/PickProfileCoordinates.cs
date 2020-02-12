using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.DTO;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Profile
{
    public class PickProfileCoordinates : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        public PickProfileCoordinates()
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
            toolId.Value = ThisAddIn.IDs.PickProfileCoordinates;
            var mxdDoc = ArcMap.Document;
            var screenDisplay = mxdDoc.ActiveView.ScreenDisplay;
            var point = screenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);
            var pointToSave = screenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);

            point.SpatialReference = pointToSave.SpatialReference = mxdDoc.FocusMap.SpatialReference;

            EsriTools.ProjectToWgs84(point);

            var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileCalc.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileCalc);

            var dem = winImpl.MilSpaceProfileCalsController.View.DemLayerName;

            MapLayersManager mngr = new MapLayersManager(mxdDoc.ActiveView);

            //Set Z value using selected DEM
            var rl = mngr.RasterLayers.FirstOrDefault(l => l.Name == dem);

            if (rl != null)
            {
                point.AddZCoordinate(rl.Raster);
                pointToSave.AddZCoordinate(rl.Raster);
            }
            else
            {
                //TODO: Localize the message box
                MessageBox.Show("Please, choose a DEM layer to take Z valur from there", "Unlocalized text", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (winImpl.MilSpaceProfileCalsController.View.ActiveButton == ProfileSettingsPointButtonEnum.PointsFist)
            {
                winImpl.MilSpaceProfileCalsController.SetFirsPointForLineProfile(point, pointToSave);
            }

            if (winImpl.MilSpaceProfileCalsController.View.ActiveButton == ProfileSettingsPointButtonEnum.PointsSecond)
            {
                winImpl.MilSpaceProfileCalsController.SetSecondfPointForLineProfile(point, pointToSave);
            }

            if (winImpl.MilSpaceProfileCalsController.View.ActiveButton == ProfileSettingsPointButtonEnum.CenterFun)
            {

                winImpl.MilSpaceProfileCalsController.SetCenterPointForFunProfile(point, pointToSave);
            }

            var settings = winImpl.MilSpaceProfileCalsController.ProfileSettings[ProfileSettingsTypeEnum.Points];
        }
    }

}
