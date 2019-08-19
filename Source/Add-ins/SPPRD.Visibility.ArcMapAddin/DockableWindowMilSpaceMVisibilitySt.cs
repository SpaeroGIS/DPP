using ESRI.ArcGIS.Editor;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visibility.DTO;
using MilSpace.Visibility.ViewController;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Visibility
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindowMilSpaceMVisibilitySt : UserControl, IObservationPointsView
    {
        private ObservationPointsController controller;
        public DockableWindowMilSpaceMVisibilitySt(object hook, ObservationPointsController controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.controller.SetView(this);
            this.Hook = hook;
          
        }


        public DockableWindowMilSpaceMVisibilitySt(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            controller.UpdateObservationPointsList();
            SubscribeForEvents();
            InitilizeData();
        }

        private void SubscribeForEvents()
        {

            IEditEvents_Event editEvent = (IEditEvents_Event)ArcMap.Editor;
            editEvent.OnCreateFeature += controller.OnCreateFeature;

        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        #region
        public VeluableObservPointFieldsEnum GetFilter
        {
            get
            {
                var result = VeluableObservPointFieldsEnum.All;

                if (chckFilterAffiliation.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Affiliation;
                }
                if (chckFilterDate.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Date;
                }

                if (chckFilterId.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Id;
                }
                if (chckFilterType.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Type;
                }

                return result;
            }
        }

       public IEnumerable<string> GetTypes
        {
            get
            {

                return controller.GetObservationPointMobilityTypes();
            }
        }
       public  IEnumerable<string> GetAffiliation
        {
            get
            {

                return controller.GetObservationPointTypes();
            }
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter)
        {
            lstObservationPoinst.Items.Clear();

            if (observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(i => new ObservPointGui

                {
                    Text = i.GetItemValue(filter),
                    Id = i.Id
                });

                lstObservationPoinst.DataSource = ItemsToShow;
                lstObservationPoinst.DisplayMember = "Text";
                lstObservationPoinst.Update();
            }

        }


        private void InitilizeData()
        {
            cmbObservPointType.Items.Clear();
            cmbObservPointType.Items.AddRange(GetTypes.ToArray());

            cmbAffiliation.Items.Clear();
            cmbAffiliation.Items.AddRange(GetAffiliation.ToArray());
        }
        #endregion

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowMilSpaceMVisibilitySt m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                var controller = new ObservationPointsController();

                m_windowUI = new DockableWindowMilSpaceMVisibilitySt(this.Hook, controller);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void toolBar9_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            (new WindowMilSpaceMVisibilityMaster()).ShowDialog();
        }

        private void toolBar7_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {


        }
    }
}
