using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilSpace.ProjectionsConverter;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;

namespace MilSpace.ProjectionsConverter.UI
{
    public class CoordinatesConverterCommand : ICommand
    {
        private Form mainForm;
        public void OnCreate(object Hook)
        {
            if (mainForm == null && Hook is IApplication arcMap)
            {
                var businessLogic = new BusinessLogic(arcMap, new DataExport());
                mainForm = new CoordinatesConverter(businessLogic);
            }
        }

        public void OnClick()
        {
            mainForm.Show();
        }

        public bool Enabled => throw new NotImplementedException();

        public bool Checked => throw new NotImplementedException();

        public string Name => "Coordinates Converter";

        public string Caption => "Coordinates Converter";

        public string Tooltip => "Converts point coordinates to a different coordinate systems";

        public string Message => throw new NotImplementedException();

        public string HelpFile => throw new NotImplementedException();

        public int HelpContextID => throw new NotImplementedException();

        public int Bitmap => throw new NotImplementedException();

        public string Category => throw new NotImplementedException();
    }
}
