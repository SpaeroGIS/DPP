using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MilSpace.Visibility
{
    public class ButtonMilSpaceMVis : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ButtonMilSpaceMVis()
        {
        }

        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            ArcMap.Application.CurrentTool = null;
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
