using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilSpace.ProjectionsConverter;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using MilSpace.ProjectionsConverter.Models;
using MilSpace.ProjectionsConverter.ReferenceData;
using ESRI.ArcGIS.esriSystem;

namespace MilSpace.ProjectionsConverter.UI
{
    public class CoordinatesConverterCommand : IDockableWindow, ICommand, ITool
    {
        private DockableWindowCoordinatesConverter mainForm;

        public void OnCreate(object Hook)
        {
            if (mainForm == null && Hook is IApplication arcMap)
            {
                var businessLogic = new BusinessLogic(arcMap, new DataExport());
                mainForm = new DockableWindowCoordinatesConverter(arcMap, businessLogic, CreateProjecstionsModelFromSettings());                
            }            
        }

        public void OnClick()
        {
            mainForm.Show();
        }

        public bool Enabled => true;

        public bool Checked => false;

        public string Name => "Coordinates Converter";

        public string Caption => "Coordinates Converter";

        public string Tooltip => "Converts point coordinates to a different coordinate systems";

        public string Message => "";

        public string HelpFile => "";

        public int HelpContextID => 0;

        public int Bitmap => 0;

        public string Category => "MilSpaceCoordConverter";

        public void OnMouseDown(int button, int shift, int x, int y)
        {
            if (button == 1 && shift == 0)
                mainForm.ArcMap_MouseDown(x, y);
        }

        public void OnMouseMove(int button, int shift, int x, int y)
        {
            
        }

        public void OnMouseUp(int button, int shift, int x, int y)
        {
            
        }

        public void OnDblClick()
        {
            
        }

        public void OnKeyDown(int keyCode, int shift)
        {
            
        }

        public void OnKeyUp(int keyCode, int shift)
        {
            
        }

        public bool OnContextMenu(int x, int y)
        {
            return true;
        }

        public void Refresh(int hdc)
        {
            
        }

        public bool Deactivate()
        {
            return true;
        }

        public int Cursor => 1;

        private ProjectionsModel CreateProjecstionsModelFromSettings()
        {
            return new ProjectionsModel(new SingleProjectionModel((int)esriSRProjCSType.esriSRProjCS_WGS1984UTM_36N, 30.000, 0.000),
                                        new SingleProjectionModel((int)esriSRProjCSType.esriSRProjCS_Pulkovo1942GK_6N, 30.000, 44.330),
                                        new SingleProjectionModel(Constants.Ukraine2000ID[2], 30.000, 43.190));
        }

        public void Show(bool Show)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible()
        {
            throw new NotImplementedException();
        }

        public void Dock(esriDockFlags dockFlags)
        {
            throw new NotImplementedException();
        }

        string IDockableWindow.Caption { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public UID ID => throw new NotImplementedException();

        public dynamic UserData => throw new NotImplementedException();
    }
}
