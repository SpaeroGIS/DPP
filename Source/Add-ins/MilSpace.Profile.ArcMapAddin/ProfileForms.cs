using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Desktop.AddIns;

namespace MilSpace.Profile
{
    internal static class ProfileForms
    {
        internal static DockableWindowMilSpaceProfileCalc ProfileCalcUI
        {
            get
            {
                var winImpl = AddIn.FromID<DockableWindowMilSpaceProfileCalc.AddinImpl>(ThisAddIn.IDs.DockableWindowMilSpaceProfileCalc);
                DockableWindowMilSpaceProfileCalc form = winImpl.DockableWindowUI;
                return form;
            }
        }
    }
}
