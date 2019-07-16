//using MilSpace.DataAccess.DataTransfer;
//using MilSpace.DataAccess.Facade;
using MilSpace.Visualization3D.ReferenceData;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Visualization3D
{
    public partial class ProfilesVisualizationForm : Form
    {
        public ProfilesVisualizationForm()
        {
            InitializeComponent();
            LocalizeComponent();
            InitializeTreeView();
        }

        private void InitializeTreeView()
        {
            //var userProfileSessions = MilSpaceProfileFacade.GetUserProfileSessions();
        }

        private void LocalizeComponent()
        {
            try
            {
                var context = new LocalizationContext();

                //Captions
                this.Text = context.WindowCaption;

                //Labels
                this.SurfaceLabel.Text = context.SurfaceLabel;
                this.BuildingsHightLabel.Text = context.HightLablel;
                this.HydroHightLabel.Text = context.HightLablel;
                this.PlantsHightLablel.Text = context.HightLablel;
                this.TransportHightLabel.Text = context.HightLablel;
                this.BuildingsLayerLabel.Text = context.BuildingsLayerLabel;
                this.HydroLayerLabel.Text = context.HydroLayerLabel;
                this.PlantsLayerLabel.Text = context.PlantsLayerLabel;
                this.TransportLayerLabel.Text = context.TransportLayerLabel;
                this.ProfilesTabPage.Text = context.ProfilesLabel;

                //Buttons
                this.GenerateButton.Text = context.GenerateButton;
            }
            catch { MessageBox.Show("No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."); }
        }

        //public bool AddNodeToTreeView(ProfileSession profile)
        //{
        //        if (profile.DefinitionType == ProfileSettingsTypeEnum.Points)
        //        {
        //            var firstX = profile.ProfileLines.First().Line.FromPoint.X;
        //            var firstY = profile.ProfileLines.First().Line.FromPoint.Y;
        //            var secondX = profile.ProfileLines.First().Line.ToPoint.X;
        //            var secondY = profile.ProfileLines.First().Line.ToPoint.Y;                    
        //        }

        //        if (profile.DefinitionType == ProfileSettingsTypeEnum.Fun)
        //        {
        //            var basePointX = profile.ProfileLines.FirstOrDefault().Line.FromPoint.X;
        //            var basePointY = profile.ProfileLines.FirstOrDefault().Line.FromPoint.Y;
        //            var lineDistance = profile.ProfileLines.FirstOrDefault().Line.Length;
        //            var linesCount = profile.ProfileLines.ToList().Count;                
        //        }
                
        //        foreach (var line in profile.ProfileLines)
        //        {
        //            var azimuth = line.Azimuth.ToString("F0");
        //            var nodeName = profile.DefinitionType == ProfileSettingsTypeEnum.Points
        //                ? $"{azimuth}" :
        //                (line.Azimuth == double.MinValue ? $"lineDefinition ({System.Array.IndexOf(profile.ProfileLines, line) + 1})" :
        //                $"{azimuth} ({System.Array.IndexOf(profile.ProfileLines, line) + 1})"); 
        //        }
        //    return true;
        //}

        //private string ConvertProfileTypeToString(ProfileSettingsTypeEnum profileType)
        //{
        //    switch (profileType)
        //    {
        //        case ProfileSettingsTypeEnum.Points:
        //            return LocalizationConstants.SectionTabText;

        //        case ProfileSettingsTypeEnum.Fun:
        //            return LocalizationConstants.FunTabText;

        //        case ProfileSettingsTypeEnum.Primitives:
        //            return LocalizationConstants.PrimitiveTabText;

        //        case ProfileSettingsTypeEnum.Load:
        //            return LocalizationConstants.LoadTabText;

        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(profileType), profileType, null);
        //    }
        //}

        private void ProfilesVisualizationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }
    }
}
