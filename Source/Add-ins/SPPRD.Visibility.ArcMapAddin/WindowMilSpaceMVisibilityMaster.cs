using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Visibility
{
    public partial class WindowMilSpaceMVisibilityMaster : Form
    {
        public WindowMilSpaceMVisibilityMaster()
        {
            InitializeComponent();
        }

        private void NextStepButton_Click(object sender, EventArgs e)
        {
            if(StepsTabControl.SelectedIndex < StepsTabControl.TabCount - 1) StepsTabControl.SelectedIndex++; 
        }

        private void PreviousStepButton_Click(object sender, EventArgs e)
        {
            if (StepsTabControl.SelectedIndex != 0) StepsTabControl.SelectedIndex--;
        }
    }
}
