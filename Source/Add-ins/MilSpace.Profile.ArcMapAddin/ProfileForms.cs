using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geometry;

namespace MilSpace.Profile
{
    internal static class ProfileForms
    {

        private static MilSpaceProfileCalsController controller = null;

        internal static DockableWindowMilSpaceProfileCalc ProfileCalcUI
        {
            get
            {
                if (controller == null)
                {
                    var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileCalc.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileCalc);

                    controller = winImpl.MilSpaceProfileCalsController;
                }
                return controller.View as DockableWindowMilSpaceProfileCalc;

            }
        }


        //TODO: Add other items
        internal static Dictionary<ProfileSettingsTypeEnum, IEnumerable<IGeometry>> ProfileGeometries = new Dictionary<ProfileSettingsTypeEnum, IEnumerable<IGeometry>>()
        {
            { ProfileSettingsTypeEnum.Points, new IPoint[] { null, null } }
        };

    }

    public enum ProfileSettingsPointButton
    {
        None,
        PointsFist,
        PointsSecond,
        CenterFun,
    }


    public enum ProfileSettingsTypeEnum
    {
        Points,
        Fun,
        SelectedFeatures,
        Load
    }
}
