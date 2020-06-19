using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Core.SolutionSettings
{
    public partial class SolutionSettingsForm : Form
    {
        public SolutionSettingsForm()
        {
            InitializeComponent();
            LocalizeStrings();
        }

        private void LocalizeStrings()
        {
            mainTabControl.TabPages["tbGraphics"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabSurfaceCaption", "поверхня");
            mainTabControl.TabPages["tbConfiguration"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabGraphicsCaption", "графіка");
            mainTabControl.TabPages["tbSurface"].Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_tabConfigurationCaption", "конфігурація (сеанс)");
            
            lblDEM.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblDEMText", "Вибір поверхні (DEM) для розрахунків");
            lblSurfaceInfo.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblSurfaceInfoText", "Інформація про поверхню");
            lblShowGraphics.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblShowGraphicsText", "(2) відобразити графіку");
            lblClearGraphics.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblClearGraphicsText", "(1) очистити графіку");
            lblSeanseInfo.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_lblSeanseInfoText", "Відомості про сеанс роботи");

            btnApply.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_btnApplyText", "Застосувати");
            btnExit.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_btnExitText", "Вийти");
            btnConnectToMap.Text = LocalizationContext.Instance.FindLocalizedElement("SolutionSettingsWindow_btnConnectToMapText", "Приєднати до карти");

            PopulateCheckListBox();
        }

        private void PopulateCheckListBox()
        {
            chckListBoxClearGraphics.Items.Clear();
            chckListBoxClearGraphics.Items.AddRange(LocalizationContext.Instance.ClearGraphicsLocalisation.Values.ToArray());

            chckListBoxShowGraphics.Items.Clear();
            chckListBoxShowGraphics.Items.AddRange(LocalizationContext.Instance.ShowGraphicsLocalisation.Values.ToArray());
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
