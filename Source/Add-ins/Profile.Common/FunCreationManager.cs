using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Core.Tools;
using MilSpace.Profile.Helpers;
using MilSpace.Profile.Localization;
using MilSpace.Profile.ModalWindows;
using MilSpace.Tools.GraphicsLayer;
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
        private static Logger _logger = Logger.GetLoggerEx("MilSpace.Profile.FunCreationManager");

        public static IEnumerable<IGeometry> GetGeometriesByMethod(AssignmentMethodsEnum method)
        {
            var geometries = new List<IGeometry>();

            try
            {
                switch (method)
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

                        var geomFromSelectedGraphic = GetTargetGeometriesFromSelectedGraphic();

                        if (geomFromSelectedGraphic == null)
                        {
                            geometries = null;
                            MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgSelectedGeomNotFoundText", "Будь ласка, оберіть графіку для розрахунку набору профілів"),
                                                LocalizationContext.Instance.MessageBoxTitle);
                            _logger.WarnEx("> GetGeometriesByMethod There are no selected graphics on the map");
                        }
                        else
                        {
                            geometries = geomFromSelectedGraphic.ToList();
                        }

                        break;
                }
            }
            catch(Exception ex)
            {

                _logger.ErrorEx($"> GetGeometriesByMethod Exception: {ex.Message}");
            }

            return geometries;
        }

        public static IPoint GetCenterPoint(List<IGeometry> geometries)
        {
            var envelope = EsriTools.GetEnvelopeOfGeometriesList(geometries);
            var x = envelope.XMin + (envelope.XMax - envelope.XMin) / 2;
            var y = envelope.YMin + (envelope.YMax - envelope.YMin) / 2;

            return new Point { X = x, Y = y, SpatialReference = envelope.SpatialReference };
        }

        private static List<IGeometry> GetTargetObservPoints()
        {
            var visibilityModule = ModuleInteraction.Instance.GetModuleInteraction<IVisibilityInteraction>(out bool changes);
            var points = new List<FromLayerPointModel>();

            if(!changes && visibilityModule == null)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним"), LocalizationContext.Instance.MessageBoxTitle);
                _logger.ErrorEx($"> GetTargetObservPoints Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
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
                _logger.ErrorEx($"> GetTargetObservPoints Exception: {ex.Message}");
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
                _logger.ErrorEx($"> GetTargetObservObjects Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
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
                _logger.ErrorEx($"> GetTargetObservObjects Exception: {ex.Message}");
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
                _logger.ErrorEx($"> GetTargetPointsFromGeoCalculator Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgGeoCalcModuleDoesnotExistText", "Модуль Геокалькулятор не було підключено \nБудь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }

            try
            {
                points = geoModule.GetPoints();
            }
            catch(Exception ex)
            {
                MessageBox.Show(LocalizationContext.Instance.ErrorHappendText, LocalizationContext.Instance.MessageBoxTitle);
                _logger.ErrorEx($"> GetTargetPointsFromGeoCalculator Exception: {ex.Message}");
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
            try
            {
                var geomFromLayerModal = new GeometriesFromLayerForFunToPointsModalWindow();
                var result = geomFromLayerModal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return geomFromLayerModal.SelectedGeometries;
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.WarnEx($"> GetTargetGeometriesFromFeatureLayer Exception: {ex.Message}");
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgRequiredLayersDoesNotExists", "У проекті відсутні необхідні шари"),
                                LocalizationContext.Instance.MessageBoxTitle);
            }

            return null;
        }

        private static IEnumerable<IGeometry> GetTargetGeometriesFromSelectedGraphic()
        {
            return GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).GetAllSelectedGraphics();
        }

    }
}
