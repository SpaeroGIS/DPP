using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Profile.Helpers;
using MilSpace.Profile.Localization;
using MilSpace.Profile.ModalWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Profile
{
    internal static class FunCreationManager
    {
        private static Logger logger = Logger.GetLoggerEx("MilSpace.Profile.FunCreationManager");

        public static IEnumerable<IGeometry> GetGeometriesByMethod(AssignmentMethodsEnum method)
        {
            var geometries = new List<IGeometry>();

            try
            {
                switch(method)
                {
                    case AssignmentMethodsEnum.ObservationPoints:

                        geometries = GetTargetObservPoints();

                        break;

                    case AssignmentMethodsEnum.ObservationObjects:

                        geometries = GetTargetObservObjects();

                        break;

                    case AssignmentMethodsEnum.GeoCalculator:

                        geometries = GetTargetPointsFromGeoCalculator();

                        break;

                    case AssignmentMethodsEnum.FeatureLayers:

                        geometries = GetTargetGeometriesFromFeatureLayer();

                        break;

                    case AssignmentMethodsEnum.SelectedGraphic:


                        break;
                }
            }
            catch(Exception ex)
            {

                //TODO: Log
            }

            return geometries;
        }

        private static List<IGeometry> GetTargetObservPoints()
        {
            var visibilityModule = ModuleInteraction.Instance.GetModuleInteraction<IVisibilityInteraction>(out bool changes);
            var points = new List<FromLayerPointModel>();

            if(!changes && visibilityModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetTargetObservPoints Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                points = visibilityModule.GetObservationPoints();

                if(points == null)
                {
                    MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgObservPointsLayerDoesnotExistText", "У проекті відсутній шар точок спостереження \nБудь ласка додайте шар, щоб мати можливість отримати точки"),
                                LocalizationContext.Instance.MessageBoxTitle);
                    return null;
                }
            }
            catch(MissingFieldException)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле OBJECTID"),
                                   LocalizationContext.Instance.MessageBoxTitle);
                return null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorHappendText, LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetTargetObservPoints Exception: {ex.Message}");
                return null;
            }

            ObservPointsForFunToPointsModalWindow observPointsForFunToPointsModal = new ObservPointsForFunToPointsModalWindow(points);

            var result = observPointsForFunToPointsModal.ShowDialog();

            if(result == DialogResult.OK)
            {
                return observPointsForFunToPointsModal.SelectedPoints;
            }

            return null;
        }

        private static List<IGeometry> GetTargetObservObjects()
        {
            var visibilityModule = ModuleInteraction.Instance.GetModuleInteraction<IVisibilityInteraction>(out bool changes);
            List<FromLayerGeometry> observObjects;

            if(!changes && visibilityModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetTargetObservObjects Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                observObjects = visibilityModule.GetObservationObjects();

                if(observObjects == null)
                {
                    MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgObservObjectsLayerDoesnotExistText", "У проекті відсутній шар об'єктів спостереження. Будь ласка додайте шар, щоб мати можливість отримати точки"),
                                LocalizationContext.Instance.MessageBoxTitle);
                    return null;
                }
            }
            catch(MissingFieldException)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgCannotFindObjIdText", "У шарі відсутнє поле OBJECTID"),
                                   LocalizationContext.Instance.MessageBoxTitle);
                return null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorHappendText, LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetTargetObservObjects Exception: {ex.Message}");
                return null;
            }

            ObservObjForFunModalWindow observObjForFunModal = new ObservObjForFunModalWindow(observObjects);

            var result = observObjForFunModal.ShowDialog();

            if(result == DialogResult.OK)
            {
                return observObjForFunModal.SelectedPoints;
            }

            return null;
        }

        private static List<IGeometry> GetTargetPointsFromGeoCalculator()
        {
            Dictionary<int, IPoint> points;

            var geoModule = ModuleInteraction.Instance.GetModuleInteraction<IGeocalculatorInteraction>(out bool changes);

            if(!changes && geoModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgGeoCalcModuleDoesnotExistText", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetTargetPointsFromGeoCalculator Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgGeoCalcModuleDoesnotExistText", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                points = geoModule.GetPoints();
            }
            catch(Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorHappendText, LocalizationContext.Instance.MessageBoxTitle);
                logger.ErrorEx($"> GetTargetPointsFromGeoCalculator Exception: {ex.Message}");
                return null;
            }

            var pointsWindow = new CalcPointsForFunToPointsModalWindow(points);
            var result = pointsWindow.ShowDialog();

            if(result == DialogResult.OK)
            {
                return pointsWindow.SelectedPoints;
            }

            return null;
        }

        private static List<IGeometry> GetTargetGeometriesFromFeatureLayer()
        {
            var geomFromLayerModal = new GeometriesFromLayerForFunToPointsModalWindow();
            var result = geomFromLayerModal.ShowDialog();

            if(result == DialogResult.OK)
            {
                return geomFromLayerModal.SelectedGeometries;
            }

            return null;
        }

    }
}
