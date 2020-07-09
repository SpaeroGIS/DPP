using MilSpace.Core.SolutionSettings;

namespace MilSpace.Settings
{
    public class OpenSettingWindow : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public OpenSettingWindow()
        {
        }

        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            ArcMap.Application.CurrentTool = null;

            SolutionSettingsForm form = new SolutionSettingsForm();
            var result = form.ShowDialog();
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
