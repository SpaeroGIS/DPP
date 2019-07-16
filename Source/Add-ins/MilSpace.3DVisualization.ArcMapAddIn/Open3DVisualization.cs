using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using MilSpace.DataAccess.Facade;
using MilSpace.Profile;

namespace MilSpace.Visualization3D
{
    public class Open3DVisualization : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Open3DVisualization()
        {
        }

        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            ArcMap.Application.CurrentTool = null;

            //todo: Temp remove ref?

            Test3D.Instance.SetCurrentProfile();
            Test3D.Instance.SetSelectedProfileTo3D();
            //Visualization3DHandler.OpenProfilesSetIn3D();
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
