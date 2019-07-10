using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MilSpace.Visualization3D
{
    public class Button3DVisualization : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button3DVisualization()
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
