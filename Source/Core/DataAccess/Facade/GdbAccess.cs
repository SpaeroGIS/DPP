using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.DataAccess.Facade
{
    public class GdbAccess
    {
        private static GdbAccess instance = null;
        private static IApplication application = null;

        private const string divider = "\\";
        private const string profileCalcFeatureClass = "CalcProfile_L";

        private static Logger logger = Logger.GetLoggerEx("MilSpace.DataAccess.Facade.GdbAccess");

        private GdbAccess()
        {
            logger.DebugEx($"> GdbAccess. START");
            try
            {
                //Nikol 20191202
                logger.DebugEx("GdbAccess. MilSpaceConfiguration.ConfigurationFileName: {0}",
                    MilSpaceConfiguration.ConfigurationFileName);
                //logger.DebugEx("GdbAccess. MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection: {0}",
                //    MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection);
                //logger.DebugEx("GdbAccess. MilSpaceConfiguration.ConnectionProperty.WorkingGDBConnection: {0}",
                //    MilSpaceConfiguration.ConnectionProperty.WorkingGDBConnection);
                //Nikol 20191202

                logger.DebugEx("GdbAccess. MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection: {0}", 
                    MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection);

                string calcGdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;
                logger.DebugEx($"GdbAccess. calcGdb: {0}", calcGdb);
                IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
                logger.DebugEx($"GdbAccess. Opening access to {calcGdb}");
                calcWorkspace = workspaceFactory.OpenFromFile(calcGdb, 0);
                if (calcWorkspace == null)
                {
                    logger.ErrorEx($"> GdbAccess END. Cannot access to {calcGdb}");
                }
                else
                {
                    logger.DebugEx($"> GdbAccess END. Access to {calcGdb}");
                }
            }
            catch (Exception ex)
            {
                logger.ErrorEx("> GdbAccess Exception. ex.Message: {0}", ex.Message);
            }
        }

        public static GdbAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    try
                    {
                        instance = new GdbAccess();
                    }
                    catch
                    {
                        //
                    }
                }
                return instance;
            }
        }

        private static IWorkspace calcWorkspace = null;
        private static IWorkspace workingWorkspace = null;

        public string AddProfileLinesToCalculation(IEnumerable<IPolyline> profileLines)
        {
            logger.DebugEx($"> AddProfileLinesToCalculation START");
            try
            {
                string featureClassName = GenerateTempProfileLinesStorage();

                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();

                IFeatureClass calc = GetCalcProfileFeatureClass(featureClassName);
                var GCS_WGS = Helper.GetBasePointSpatialReference();

                profileLines.ToList().ForEach(
                    l =>
                    {
                        var newLine = calc.CreateFeature();
                        newLine.Shape = l;
                        newLine.Store();
                    });

                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);

                logger.DebugEx($"> AddProfileLinesToCalculation END. featureClassName:{0}", featureClassName);
                return featureClassName;
            }
            catch (Exception ex)
            {
                logger.DebugEx($"> AddProfileLinesToCalculation Exception. {0}", ex.Message);
                return "";
            }
        }

        public VisibilityCalculationResultsEnum CheckVisibilityResult(string sessionName)
        {
            //Check Points and Objects\Stations

            VisibilityCalculationResultsEnum results = VisibilityCalculationResultsEnum.None;

            foreach (var map in VisibilityTask.EsriDatatypeToResultMapping)
            {
                var datasets = calcWorkspace.get_DatasetNames(map.Key);
                var dataSet = datasets.Next();

                while (dataSet != null)
                {
                    foreach (var result in map.Value)
                    {
                        string comparitionName = VisibilityTask.GetResultName(result, sessionName);
                        //it might be to check feature class type for FeatureClass dataset like Point for Observponts and Polygon for ObservObjects
                        if (dataSet.Name.Equals(comparitionName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            results = results | result;
                        }
                    }
                    dataSet = datasets.Next();
                }
            }

            return results;
        }

        public IApplication Application
        {
            set
            {
                if (application == null)
                {
                    ArcMapInstance.Application = application = value;
                }
            }
        }

        public string ExportObservationFeatureClass(IDataset sourceDataset, string nameOfTargetDataset, int[] filter)
        {
            logger.DebugEx("> ExportObservationFeatureClass START. nameOfTargetDataset:{0}", nameOfTargetDataset);

            IWorkspace targetWorkspace = calcWorkspace;
            IWorkspaceEdit workspaceEdit = null;
            try
            {
                //create source workspace name
                ITable pSourceTab = (ITable)sourceDataset;
                IFeatureClass sourceFeatureClass = (IFeatureClass)sourceDataset;

                IDataset sourceWorkspaceDataset = (IDataset)sourceDataset.Workspace;
                IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;
                IFeatureClassName sourceDatasetName = (IFeatureClassName)sourceDataset.FullName;

                workspaceEdit = (IWorkspaceEdit)sourceDataset.Workspace;

                //create target workspace name
                IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
                IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
                //create target dataset name
                IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();

                IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
                targetDatasetName.WorkspaceName = targetWorkspaceName;
                targetDatasetName.Name = nameOfTargetDataset;

                // we want to convert all of the features
                IQueryFilter queryFilter = new QueryFilterClass();
                var whereClause = string.Join(" OR ", filter.Select(id => $"OBJECTID={id}").ToArray());
                //string whereClause = "OBJECTID=10061 OR OBJECTID=10465";

                queryFilter.WhereClause = whereClause;
                //Validate the field names because you are converting between different workspace types.
                IFieldChecker fieldChecker = new FieldCheckerClass();
                IFields targetFields;
                IFields sourceFields = pSourceTab.Fields;
                IEnumFieldError enumFieldError;

                // Most importantly set the input and validate workspaces!
                fieldChecker.InputWorkspace = sourceDataset.Workspace;
                fieldChecker.ValidateWorkspace = targetWorkspace;
                fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);

                // Get the geometry definition from the shape field and clone it.
                string shapeFieldName = sourceFeatureClass.ShapeFieldName;
                int shapeFieldIndex = sourceFeatureClass.FindField(shapeFieldName);
                IField shapeField = sourceFields.get_Field(shapeFieldIndex);

                IGeometryDef geometryDef = shapeField.GeometryDef;
                IClone geometryDefClone = (IClone)geometryDef;
                IClone targetGeometryDefClone = geometryDefClone.Clone();
                IGeometryDef targetGeometryDef = (IGeometryDef)targetGeometryDefClone;
                bool exportWithError = true;
                //crutch CHeck if the errof in the AreField
                if (enumFieldError != null)
                {
                    var error = enumFieldError.Next();
                    while (error != null)
                    {
                        logger.WarnEx($"ExportObservationFeatureClass. Export error in the filed {targetFields.Field[error.FieldIndex].AliasName}");
                        if (!targetFields.Field[error.FieldIndex].AliasName.StartsWith("shape_ST", StringComparison.InvariantCultureIgnoreCase))
                        {
                            exportWithError = false;
                            logger.ErrorEx("ExportObservationFeatureClass ERROR. Export is not acceptable");
                        }
                        error = enumFieldError.Next();
                    }
                }

                if (enumFieldError == null || exportWithError)
                {
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();

                    //(IFeatureDatasetName)targetDataset.FullName
                    IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(
                        sourceDatasetName, 
                        queryFilter, 
                        null, 
                        targetFeatureClassName, 
                        targetGeometryDef,
                        pSourceTab.Fields, 
                        "", 
                        1000, 
                        0);

                    if (enumErrors != null)
                    {
                        var error = enumErrors.Next();
                        while (error != null)
                        {
                            logger.WarnEx("ExportObservationFeatureClass. Export error in the featureID:{0} ErrorDescription{1}", 
                                error.InvalidObjectID, error.ErrorDescription);
                            error = enumErrors.Next();
                        }
                    }

                    logger.DebugEx("> ExportObservationFeatureClass END. nameOfTargetDataset:{0}", ((IDatasetName)targetFeatureClassName).Name);
                    return ((IDatasetName)targetFeatureClassName).Name;
                }
            }
            catch (Exception exp)
            {
                logger.DebugEx("ExportObservationFeatureClass Exception:{0}", exp.Message);
            }
            finally
            {
                //if (workspaceEdit != null && !isBeingEdited)
                //{
                //    workspaceEdit.StopEditOperation();
                //    workspaceEdit.StopEditing(false);
                //}
            }

            logger.DebugEx("> ExportObservationFeatureClass END. targetFeatureClassName NULL");
            return null;
        }

        public IFeatureClass GenerateTemporaryObservationPointFeatureClass(IFields observPointsFCFields, string name)
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IFeatureClass featureClass = null;

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if (!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, name))
            {
                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;

                 featureClass = featureWorkspace.CreateFeatureClass(name, observPointsFCFields,
                        ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID,
                        esriFeatureType.esriFTSimple, "shape", "");
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return featureClass;
        }

        public ITable GenerateVOTable(IFields observPointsFCFields, string tableName)
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            ITable table = null;

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            var fieldsEdit = (IFieldsEdit)observPointsFCFields;

            IField visibilityPercentField = new FieldClass();
            IFieldEdit pointIdFieldEdit = (IFieldEdit)visibilityPercentField;
            pointIdFieldEdit.Name_2 = "VisibilityPercent";
            pointIdFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldsEdit.AddField(visibilityPercentField);

            try
            {
                if (!wsp2.get_NameExists(esriDatasetType.esriDTTable, tableName))
                {
                    table = featureWorkspace.CreateTable(tableName, observPointsFCFields, null, null, "");
                }
                else
                {
                    table = featureWorkspace.OpenTable(tableName);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"Cannot create table {tableName} /n{ex.Message}");
                return null;
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return table;
        }

        public string GenerateTempProfileLinesStorage()
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            string newFeatureClassName = $"{profileCalcFeatureClass}{Helper.GetTemporaryNameSuffix()}";

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if (!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, newFeatureClassName))
            {

                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                IFields fields = ocDescription.RequiredFields;

                // Find the shape field in the required fields and modify its GeometryDef to
                // use point geometry and to set the spatial reference.

                int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);

                IField field = fields.get_Field(shapeFieldIndex);
                IGeometryDef geometryDef = field.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
                geometryDefEdit.SpatialReference_2 = ArcMapInstance.Document.FocusMap.SpatialReference; ;

                IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
                IField nameField = new FieldClass();
                IFieldEdit nameFieldEdit = (IFieldEdit)nameField;
                nameFieldEdit.Name_2 = "ID";
                nameFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.AddField(nameField);

                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(newFeatureClassName, fields,
                    ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "shape", "");

            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return newFeatureClassName;

        }

        private IWorkspace ConnectToSqlGDB()
        {

            SqlConnectionStringBuilder connectionBldr = new SqlConnectionStringBuilder(MilSpaceConfiguration.ConnectionProperty.WorkingGDBConnection);

            //var props = pWrkspcFact.ReadConnectionPropertiesFromFile(@"C:\Users\copoka\AppData\Roaming\ESRI\Desktop10.4\ArcCatalog\Dno.sde");
            //object names, values;
            //props.GetAllProperties(out names, out values);

            string dataSource = connectionBldr.DataSource;
            int dividerIndex = dataSource.IndexOf(divider);
            string server = dividerIndex > 0 ? dataSource.Substring(0, dividerIndex) : dataSource;

            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("SERVER", server);
            propertySet.SetProperty("INSTANCE", $"sde:sqlserver:{connectionBldr.DataSource}");
            propertySet.SetProperty("DBCLIENT", "sqlserver");
            propertySet.SetProperty("DATABASE", connectionBldr.InitialCatalog); // Only if it is needed
            propertySet.SetProperty("AUTHENTICATION_MODE", "OSA");

            try
            {
                IWorkspaceFactory pWrkspcFact = new SdeWorkspaceFactory();
                return pWrkspcFact.Open(propertySet, 0);
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"Cannot open GDB from {MilSpaceConfiguration.ConnectionProperty.WorkingGDBConnection} /n{ex.Message}");
                return null;
            }

        }

        public string AddProfileLinesTo3D(Dictionary<IPolyline, bool> profileLines)
        {
            string featureClassName = GenerateTemp3DLineStorage();

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IFeatureClass calc = GetCalcProfileFeatureClass(featureClassName);
            var GCS_WGS = Helper.GetBasePointSpatialReference();

            int i = 0;

            profileLines.ToList().ForEach(
                l =>
                {
                    var newLine = calc.CreateFeature();

                    int idFieldIndex = calc.FindField("ID");
                    newLine.set_Value(idFieldIndex, i);

                    int isVisibleFieldIndex = calc.FindField("IS_VISIBLE");
                    newLine.set_Value(isVisibleFieldIndex, l.Value ? 1 : 0);

                    newLine.Shape = l.Key;
                    newLine.Store();

                    i++;
                }
                );

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return featureClassName;
        }

        public string AddProfilePointsTo3D(IEnumerable<IPoint> points)
        {
            string featureClassName = GenerateTemp3DPointStorage();

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IFeatureClass calc = GetCalcProfileFeatureClass(featureClassName);
            var GCS_WGS = Helper.GetBasePointSpatialReference();

            int i = 0;

            points.ToList().ForEach(point =>
            {
                var pointFeature = calc.CreateFeature();

                int idFieldIndex = calc.FindField("ID");
                pointFeature.set_Value(idFieldIndex, i);

                pointFeature.Shape = point;
                pointFeature.Store();
            });

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return featureClassName;
        }

        public string AddPolygonTo3D(Dictionary<IPolygon, bool> polygons)
        {
            string featureClassName = GenerateTemp3DPolygonStorage();

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IFeatureClass calc = GetCalcProfileFeatureClass(featureClassName);
            var GCS_WGS = Helper.GetBasePointSpatialReference();

            polygons.ToList().ForEach(polygon =>
            {
                var pointFeature = calc.CreateFeature();
                pointFeature.Shape = polygon.Key;

                int isVisibleFieldIndex = calc.FindField("IS_VISIBLE");
                pointFeature.set_Value(isVisibleFieldIndex, polygon.Value ? 1 : 0);

                pointFeature.Store();
            });

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return featureClassName;
        }

        public void AddGeometryToFeatureClass(IGeometry geometry, IFeatureClass featureClass)
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            var feature = featureClass.CreateFeature();
            feature.Shape = geometry;

            feature.Store();

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
        }

        public void AddObservPoint(IPoint point, ObservationPoint pointArgs, IFeatureClass featureClass)
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            var pointFeature = featureClass.CreateFeature();

            SetObservPointValues(featureClass, pointFeature, point, pointArgs);
        }

        public void UpdateObservPoint(IPoint point, IFeatureClass featureClass, ObservationPoint observPoint, int objectId)
        {
            IFeature pointFeature = featureClass.GetFeature(objectId);
            SetObservPointValues(featureClass, pointFeature, point, observPoint);
        }

        public void RemoveObservPoint(IFeatureClass featureClass, int objectId)
        {
            IFeature pointFeature = featureClass.GetFeature(objectId);
            pointFeature.Delete();
        }

        public void EraseProfileLines()
        {
            IFeatureClass calc = GetCalcProfileFeatureClass("CalcProfile_L");
            IQueryFilter queryFilter = new QueryFilterClass
            {
                WhereClause = $"{calc.OIDFieldName} >= 0"
            };

            IFeatureCursor featureCursor = calc.Search(queryFilter, true);

            IFeature feature = null;
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            while ((feature = featureCursor.NextFeature()) != null)
            {
                feature.Delete();
            }
            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

        }

        public string GetProfileLinesFeatureClass(string profileSourceName)
        {
            { return $"{ MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection}\\{profileSourceName}"; }
        }

        public ITable GetProfileTable(string resultTable)
        {
            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if (!wsp2.get_NameExists(esriDatasetType.esriDTTable, resultTable))
            {
                //TODO: Create the feature class
                throw new FileNotFoundException(resultTable);
            }

            return featureWorkspace.OpenTable(resultTable);
        }

        public bool DeleteTemporarSource(string resultTable, string lineFeatureClass)
        {
            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            try
            {
                var datasets = calcWorkspace.get_Datasets(esriDatasetType.esriDTTable);
                IDataset tabledataset = datasets.Next();
                if (tabledataset != null)
                {

                    while (tabledataset != null)
                    {
                        if (!tabledataset.Name.Equals(resultTable))
                        {
                            tabledataset = datasets.Next();
                            continue;
                        }

                        if (!tabledataset.CanDelete())
                        {
                            throw new MilSpaceCanotDeletePrifileCalcTable(resultTable, MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection);
                        }

                        tabledataset.Delete();
                        break;
                    }
                }

                //Delete temprorary Feature class (Profile lites)
                datasets = calcWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);

                if (tabledataset != null)
                {
                    while (tabledataset != null)
                    {
                        if (!tabledataset.Name.Equals(lineFeatureClass))
                        {
                            tabledataset = datasets.Next();
                            continue;
                        }

                        if (!tabledataset.CanDelete())
                        {
                            throw new MilSpaceCanotDeletePrifileCalcTable(lineFeatureClass, MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection);
                        }

                        tabledataset.Delete();
                        break;
                    }
                }

                return true;

            }
            catch (MilSpaceCanotDeletePrifileCalcTable ex)
            {
                logger.ErrorEx(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.ErrorEx(ex.Message);

            }
            return false;

        }

        public IFeatureClass GetCalcProfileFeatureClass(string currentFeatureClass)
        {
            return OpenFeatureClass(calcWorkspace, currentFeatureClass);
        }

        public IFeatureClass GetFeatureFromWorkingWorkspace(string featureClassName)
        {
            return OpenFeatureClass(WorkingWorkspace, featureClassName);
        }

        private static IRasterDataset OpenRasterDataset(IWorkspace workspace, string rasterDatasetName)
        {
            IRasterWorkspaceEx rasterWorkspace = workspace as IRasterWorkspaceEx;

            if (IsDatasetExist(workspace, rasterDatasetName, esriDatasetType.esriDTRasterDataset))
            {
                return rasterWorkspace.OpenRasterDataset(rasterDatasetName);
            }

            return null;
        }

        private static IFeatureClass OpenFeatureClass(IWorkspace workspace, string featureClass)
        {
            logger.InfoEx("> OpenFeatureClass START featureClass:{0} ", featureClass);

            try
            {
                IWorkspace2 wsp2 = (IWorkspace2)workspace;
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                //if(!wsp2.NameExists[esriDatasetType.esriDTFeatureClass, featureClass])
                //{
                var datasetNames = workspace.DatasetNames[esriDatasetType.esriDTFeatureClass];
                var featureName = datasetNames.Next().Name;
                while (!featureName.EndsWith(featureClass))
                {
                    var dataset = datasetNames.Next();

                    if(dataset == null)
                    {
                        return null;
                    }

                    featureName = dataset.Name;
                }

                featureClass = featureName;
                // }
                IFeatureClass ifw = featureWorkspace.OpenFeatureClass(featureClass);

                logger.InfoEx("> OpenFeatureClass END");
                return ifw;
            }
            catch (Exception ex)
            {
                logger.InfoEx("> OpenFeatureClass EXCEPTION:{0}", ex.Message);
                return null;
            }
        }

        internal bool CheckDatasetExistanceInCalcWorkspace(string dataSetName, esriDatasetType datasetType)
        {
            return IsDatasetExist(calcWorkspace, dataSetName, datasetType);
        }

        internal IEnumerable<string> GetCalculatedObserPointsNames(string observPoinsFeatureClassName)
        {
            if (GdbAccess.Instance.CheckDatasetExistanceInCalcWorkspace(observPoinsFeatureClassName, esriDatasetType.esriDTFeatureClass))
            {
                var fx = OpenFeatureClass(calcWorkspace, observPoinsFeatureClassName);

                var titleFld = fx.FindField("TitleOP");
                var observPoints = fx.GetFeatures(titleFld, false);
                var observPoint = observPoints.NextFeature();
                var result = new List<string>();
                while (observPoint != null)
                {
                    result.Add(observPoint.Value[titleFld].ToString());
                    observPoint = observPoints.NextFeature();
                }
                observPoint = null;
                observPoints = null;
                Marshal.ReleaseComObject(observPoint);
                Marshal.ReleaseComObject(observPoints);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Returns Calculation entity title 
        /// </summary>
        /// <param name="observPoinsFeatureClassName"></param>
        /// <param name="titleField"></param>
        /// <returns></returns>
        internal IEnumerable<string> GetCalcEntityNamesFromFeatureClass(string entityFeatureClassName, string titleField)
        {
            if (GdbAccess.Instance.CheckDatasetExistanceInCalcWorkspace(entityFeatureClassName, esriDatasetType.esriDTFeatureClass))
            {
                var fx = OpenFeatureClass(calcWorkspace, entityFeatureClassName);

                var titleFld = fx.FindField(titleField);
                var observPoints = fx.Search(null, false);
                var observPoint = observPoints.NextFeature();
                var result = new List<string>();
                while (observPoint != null)
                {
                    result.Add(observPoint.Value[titleFld].ToString());
                    observPoint = observPoints.NextFeature();
                }
                Marshal.ReleaseComObject(observPoints);
                return result;
            }

            return null;
        }

        private static IDataset GetDataset(IWorkspace workspace, string featureClass, esriDatasetType type)
        {
            if (type == esriDatasetType.esriDTFeatureClass)
            {
                return OpenFeatureClass(workspace, featureClass) as IDataset;
            }
            if (type == esriDatasetType.esriDTRasterDataset)
            {
                return OpenRasterDataset(workspace, featureClass) as IDataset;
            }

            throw new NotImplementedException(type.ToString());
        }

        public IDataset GetDatasetFromCalcWorkspace(VisibilityResultInfo visibilityResult)
        {
            if (visibilityResult.RessutType == VisibilityCalculationResultsEnum.CoverageTable)
            {
                return null;
            }

            return GetDatasetFromCalcWorkspace(visibilityResult.ResultName, visibilityResult.RessutType);
        }

        public IDataset GetDatasetFromCalcWorkspace(string dataSetName, VisibilityCalculationResultsEnum datasetType)
        {

            if (IsDatasetExist(calcWorkspace, dataSetName,
                VisibilityTask.GetEsriDataTypeByVisibilityresyltType(datasetType)))
            {
                return GetDataset(calcWorkspace, dataSetName,
                    VisibilityTask.GetEsriDataTypeByVisibilityresyltType(datasetType));
            }

            return null;
        }
        public IEnumerable<IDataset> GetDatasetsFromCalcWorkspace(IEnumerable<VisibilityResultInfo> visibilityResults)
        {
            var mapping = VisibilityTask.EsriDatatypeToResultMapping;
            return visibilityResults.Select(v =>
                GetDatasetFromCalcWorkspace(v)).
                Where(i => i != null).ToArray();
        }

        private static bool IsDatasetExist(IWorkspace workspace, string datasetName, esriDatasetType datasetType)
        {
            IWorkspace2 wsp2 = (IWorkspace2)workspace;
            return wsp2.NameExists[datasetType, datasetName];
        }

        public ILayer GetLayerFromWorkingWorkspace(string featureClassName)
        {
            var featurelayer = new FeatureLayer
            {
                Name = featureClassName,
                FeatureClass = OpenFeatureClass(WorkingWorkspace, featureClassName)
            };

            return featurelayer;
        }

        private IWorkspace WorkingWorkspace
        {
            get
            {
                if (workingWorkspace == null)
                {
                    workingWorkspace = ConnectToSqlGDB();
                    if (workingWorkspace == null)
                    {
                        throw new MilSpaceDataException(
                            MilSpaceConfiguration.ConnectionProperty.WorkingGDBConnection, 
                            Core.DataAccess.DataOperationsEnum.Access);
                        //throw new MilSpaceVisibilityDataException("Cannot open working GDB");
                    }
                }
                return workingWorkspace;
            }
        }

        private string GenerateTemp3DLineStorage()
        {
            string newFeatureClassName = $"Line3D_L{Helper.GetTemporaryNameSuffix()}";

            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
            IFields fields = ocDescription.RequiredFields;

            int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);

            IField field = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = field.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.HasZ_2 = true;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
            geometryDefEdit.SpatialReference_2 = ArcMapInstance.Document.FocusMap.SpatialReference;

            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            IField nameField = new FieldClass();
            IFieldEdit nameFieldEdit = (IFieldEdit)nameField;
            nameFieldEdit.Name_2 = "ID";
            nameFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldsEdit.AddField(nameField);

            IField isVisibleField = new FieldClass();
            IFieldEdit isVisibleFieldEdit = (IFieldEdit)isVisibleField;
            isVisibleFieldEdit.Name_2 = "IS_VISIBLE";
            isVisibleFieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldsEdit.AddField(isVisibleFieldEdit);

            GenerateTempStorage(newFeatureClassName, fields, esriGeometryType.esriGeometryPolyline);

            return newFeatureClassName;
        }

        public IFeatureClass GenerateTempStorage(string featureClassName, IFields fields,
                                                    esriGeometryType type, IActiveView activeView = null,
                                                    bool isZAware = true, bool addSuffix = false)
        {
            IFeatureClass featureClass = null;
            
            if(addSuffix)
            {
                featureClassName += Helper.GetTemporaryNameSuffix();
            }

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if (!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, featureClassName))
            {
                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;

                if (fields == null)
                {
                    fields = ocDescription.RequiredFields;

                    int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);

                    IField field = fields.get_Field(shapeFieldIndex);
                    IGeometryDef geometryDef = field.GeometryDef;
                    IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                    geometryDefEdit.HasZ_2 = isZAware;
                    geometryDefEdit.GeometryType_2 = type;
                    geometryDefEdit.SpatialReference_2 = (activeView == null)? ArcMapInstance.Document.FocusMap.SpatialReference : activeView.FocusMap.SpatialReference;
                }

                featureClass = featureWorkspace.CreateFeatureClass(featureClassName, fields,
                   ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "shape", "");
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return featureClass;
        }

        private string GenerateTemp3DPointStorage()
        {
            string newFeatureClassName = $"Point3D_L{Helper.GetTemporaryNameSuffix()}";

            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
            IFields fields = ocDescription.RequiredFields;

            int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);

            IField field = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = field.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.HasZ_2 = true;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            geometryDefEdit.SpatialReference_2 = ArcMapInstance.Document.FocusMap.SpatialReference;

            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            IField nameField = new FieldClass();
            IFieldEdit nameFieldEdit = (IFieldEdit)nameField;
            nameFieldEdit.Name_2 = "ID";
            nameFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldsEdit.AddField(nameField);

            GenerateTempStorage(newFeatureClassName, fields, esriGeometryType.esriGeometryPoint);
            return newFeatureClassName;
        }

        private string GenerateTemp3DPolygonStorage()
        {
            string newFeatureClassName = $"Polygon3D_L{Helper.GetTemporaryNameSuffix()}";

            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
            IFields fields = ocDescription.RequiredFields;

            int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);

            IField field = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = field.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.HasZ_2 = true;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            geometryDefEdit.SpatialReference_2 = ArcMapInstance.Document.FocusMap.SpatialReference;

            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            IField isVisibleField = new FieldClass();
            IFieldEdit isVisibleFieldEdit = (IFieldEdit)isVisibleField;
            isVisibleFieldEdit.Name_2 = "IS_VISIBLE";
            isVisibleFieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldsEdit.AddField(isVisibleFieldEdit);

            GenerateTempStorage(newFeatureClassName, fields, esriGeometryType.esriGeometryPolygon);
            return newFeatureClassName;
        }

        public void AddObserverPointFields(IFeatureClass featureClass)
        {
            if (featureClass.FindField("AzimuthB") == -1)
            {
                IField azimuthBField = new FieldClass();
                IFieldEdit azimuthBFieldEdit = (IFieldEdit)azimuthBField;
                azimuthBFieldEdit.Name_2 = "AzimuthB";
                azimuthBFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                featureClass.AddField(azimuthBField);
            }

            if (featureClass.FindField("AzimuthE") == -1)
            {
                IField azimuthEField = new FieldClass();
                IFieldEdit azimuthEFieldEdit = (IFieldEdit)azimuthEField;
                azimuthEFieldEdit.Name_2 = "AzimuthE";
                azimuthEFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                featureClass.AddField(azimuthEField);
            }

            if (featureClass.FindField("AnglMinH") == -1)
            {
                IField anglMinHField = new FieldClass();
                IFieldEdit anglMinHFieldEdit = (IFieldEdit)anglMinHField;
                anglMinHFieldEdit.Name_2 = "AnglMinH";
                anglMinHFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                featureClass.AddField(anglMinHField);
            }

            if (featureClass.FindField("AnglMaxH") == -1)
            {
                IField anglMaxHField = new FieldClass();
                IFieldEdit anglMaxHFieldEdit = (IFieldEdit)anglMaxHField;
                anglMaxHFieldEdit.Name_2 = "AnglMaxH";
                anglMaxHFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                featureClass.AddField(anglMaxHField);
            }

            if (featureClass.FindField("HRel") == -1)
            {
                IField heightField = new FieldClass();
                IFieldEdit heightFieldEdit = (IFieldEdit)heightField;
                heightFieldEdit.Name_2 = "HRel";
                heightFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                featureClass.AddField(heightField);
            }

            SetObservPointDefaultValues(featureClass);
        }

        private void SetObservPointDefaultValues(IFeatureClass featureClass)
        {
            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor searchCursor = featureClass.Search(null, false);
                comReleaser.ManageLifetime(searchCursor);
                IFeature feature = null;

                var azimuthBIndex = featureClass.FindField("AzimuthB");
                var azimuthEIndex = featureClass.FindField("AzimuthE");
                var anglMinIndex = featureClass.FindField("AnglMinH");
                var anglMaxIndex = featureClass.FindField("AnglMaxH");
                var hRelIndex = featureClass.FindField("HRel");

                while ((feature = searchCursor.NextFeature()) != null)
                {

                    if (azimuthBIndex != -1 && feature.Value[azimuthBIndex] == DBNull.Value)
                    {
                        feature.set_Value(azimuthBIndex, 0);
                    }

                    if (azimuthEIndex != -1 && feature.Value[azimuthEIndex] == DBNull.Value)
                    {
                        feature.set_Value(azimuthEIndex, 360);
                    }

                    if (anglMinIndex != -1 && feature.Value[anglMinIndex] == DBNull.Value)
                    {
                        feature.set_Value(anglMinIndex, -90);
                    }

                    if (anglMaxIndex != -1 && feature.Value[anglMaxIndex] == DBNull.Value)
                    {
                        feature.set_Value(anglMaxIndex, 90);
                    }

                    if (hRelIndex != -1 && feature.Value[hRelIndex] == DBNull.Value)
                    {
                        feature.set_Value(hRelIndex, 0);
                    }

                    feature.Store();
                }
            }
        }

            private void SetObservPointValues(IFeatureClass featureClass, IFeature pointFeature, IPoint point, ObservationPoint pointArgs)
        {
            if (point != null)
            {
                pointFeature.Shape = point;
            }

            pointFeature.set_Value(featureClass.FindField("TitleOP"), pointArgs.Title);
            pointFeature.set_Value(featureClass.FindField("TypeOP"), pointArgs.Type);
            pointFeature.set_Value(featureClass.FindField("saffiliation"), pointArgs.Affiliation);
            pointFeature.set_Value(featureClass.FindField("XWGS"), pointArgs.X);
            pointFeature.set_Value(featureClass.FindField("YWGS"), pointArgs.Y);
            pointFeature.set_Value(featureClass.FindField("HRel"), pointArgs.RelativeHeight);
            pointFeature.set_Value(featureClass.FindField("AvailableHeightLover"), pointArgs.AvailableHeightLover);
            pointFeature.set_Value(featureClass.FindField("AvailableHeightUpper"), pointArgs.AvailableHeightUpper);
            pointFeature.set_Value(featureClass.FindField("AzimuthB"), pointArgs.AzimuthStart);
            pointFeature.set_Value(featureClass.FindField("AzimuthE"), pointArgs.AzimuthEnd);
            pointFeature.set_Value(featureClass.FindField("AzimuthMainAxis"), pointArgs.AzimuthMainAxis);
            pointFeature.set_Value(featureClass.FindField("AnglCameraRotationH"), pointArgs.AngelCameraRotationH);
            pointFeature.set_Value(featureClass.FindField("AnglCameraRotationV"), pointArgs.AngelCameraRotationV);
            pointFeature.set_Value(featureClass.FindField("AnglMinH"), pointArgs.AngelMinH);
            pointFeature.set_Value(featureClass.FindField("AnglMaxH"), pointArgs.AngelMaxH);
            pointFeature.set_Value(featureClass.FindField("InnerRadius"), pointArgs.InnerRadius);
            pointFeature.set_Value(featureClass.FindField("OuterRadius"), pointArgs.OuterRadius);

            pointFeature.Store();
        }

        private ITable GenerateVACoverageTable(string tableName, IWorkspace workspace)
        {
            ITable table;
            IFieldsEdit fieldsEdit = new FieldsClass();

            IFieldEdit2 fieldId = new FieldClass() as IFieldEdit2;
            fieldId.Name_2 = "OBJECTID";
            fieldId.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(fieldId);

            IFieldEdit2 fieldPointName = new FieldClass() as IFieldEdit2;
            fieldPointName.Name_2 = "TitleOP";
            fieldPointName.Type_2 = esriFieldType.esriFieldTypeString;
            fieldPointName.AliasName_2 = "Назва ПС";
            fieldPointName.IsNullable_2 = true;
            fieldsEdit.AddField(fieldPointName);

            IFieldEdit2 fieldPointId = new FieldClass() as IFieldEdit2;
            fieldPointId.Name_2 = "IdOP";
            fieldPointId.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldPointId.IsNullable_2 = true;
            fieldPointId.AliasName_2 = "Id ПС";
            fieldsEdit.AddField(fieldPointId);

            IFieldEdit2 fieldObjName = new FieldClass() as IFieldEdit2;
            fieldObjName.Name_2 = "TitleOO";
            fieldObjName.Type_2 = esriFieldType.esriFieldTypeString;
            fieldObjName.AliasName_2 = "Назва ОН";
            fieldObjName.IsNullable_2 = true;
            fieldsEdit.AddField(fieldObjName);

            IFieldEdit2 fieldObjId = new FieldClass() as IFieldEdit2;
            fieldObjId.Name_2 = "IdOO";
            fieldObjId.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldObjId.AliasName_2 = "Id ОН";
            fieldObjId.IsNullable_2 = true;
            fieldsEdit.AddField(fieldObjId);

            IFieldEdit2 fieldObjArea = new FieldClass() as IFieldEdit2;
            fieldObjArea.Name_2 = "AreaOO";
            fieldObjArea.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldObjArea.IsNullable_2 = false;
            fieldObjArea.AliasName_2 = "Площа ОН";
            fieldsEdit.AddField(fieldObjArea);

            IFieldEdit2 fieldVA = new FieldClass() as IFieldEdit2;
            fieldVA.Name_2 = "VisibilityArea";
            fieldVA.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldVA.AliasName_2 = "Площа видимості ОН";
            fieldVA.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVA);

            IFieldEdit2 fieldVAPercent = new FieldClass() as IFieldEdit2;
            fieldVAPercent.Name_2 = "VisibilityPercentage";
            fieldVAPercent.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldVAPercent.AliasName_2 = "Відсоток видимості";
            fieldVAPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVAPercent);

            IFieldEdit2 fieldPointsSee = new FieldClass() as IFieldEdit2;
            fieldPointsSee.Name_2 = "OPSee";
            fieldPointsSee.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldPointsSee.AliasName_2 = "Спостережень";
            fieldPointsSee.IsNullable_2 = true;
            fieldsEdit.AddField(fieldPointsSee);

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IWorkspace2 wsp2 = workspace as IWorkspace2;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

            try
            {
                if(!wsp2.get_NameExists(esriDatasetType.esriDTTable, tableName))
                {
                    table = featureWorkspace.CreateTable(tableName, fieldsEdit, null, null, "");
                }
                else
                {
                    table = featureWorkspace.OpenTable(tableName);
                }
            }
            catch(Exception ex)
            {
                logger.ErrorEx($"Cannot create table {tableName} /n{ex.Message}");
                return null;
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return table;
        }

        private ITable GenerateVSCoverageTable(string tableName, IWorkspace workspace)
        {
            ITable table;
            IFieldsEdit fieldsEdit = new FieldsClass();
            
            IFieldEdit2 fieldId = new FieldClass() as IFieldEdit2;
            fieldId.Name_2 = "OBJECTID";
            fieldId.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(fieldId);

            IFieldEdit2 fieldPointName = new FieldClass() as IFieldEdit2;
            fieldPointName.Name_2 = "TitleOP";
            fieldPointName.Type_2 = esriFieldType.esriFieldTypeString;
            fieldPointName.AliasName_2 = "Назва ПС";
            fieldPointName.IsNullable_2 = true;
            fieldsEdit.AddField(fieldPointName);

            IFieldEdit2 fieldPointId = new FieldClass() as IFieldEdit2;
            fieldPointId.Name_2 = "IdOP";
            fieldPointId.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldPointId.IsNullable_2 = true;
            fieldPointId.AliasName_2 = "Id ПС";
            fieldsEdit.AddField(fieldPointId);

            IFieldEdit2 fieldExpectedVA = new FieldClass() as IFieldEdit2;
            fieldExpectedVA.Name_2 = "ExpectedVisibilityArea";
            fieldExpectedVA.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldExpectedVA.AliasName_2 = "Площа зони покриття";
            fieldExpectedVA.IsNullable_2 = true;
            fieldsEdit.AddField(fieldExpectedVA);

            IFieldEdit2 fieldVA = new FieldClass() as IFieldEdit2;
            fieldVA.Name_2 = "VisibilityArea";
            fieldVA.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldVA.AliasName_2 = "Площа видимої зони";
            fieldVA.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVA);

            IFieldEdit2 fieldVAPercent = new FieldClass() as IFieldEdit2;
            fieldVAPercent.Name_2 = "VisibilityPercentage";
            fieldVAPercent.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldVAPercent.AliasName_2 = "Відсоток видимості";
            fieldVAPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVAPercent);

            IFieldEdit2 fieldExpectedVAToAllPercent = new FieldClass() as IFieldEdit2;
            fieldExpectedVAToAllPercent.Name_2 = "CurrentToAllExpectedVAPercentage";
            fieldExpectedVAToAllPercent.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldExpectedVAToAllPercent.AliasName_2 = "Відсоток зони покриття ПС від загальної";
            fieldExpectedVAToAllPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldExpectedVAToAllPercent);

            IFieldEdit2 fieldVAToAllPercent = new FieldClass() as IFieldEdit2;
            fieldVAToAllPercent.Name_2 = "CurrentVAToAllExpectedVAPercentage";
            fieldVAToAllPercent.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldVAToAllPercent.AliasName_2 = "Відсоток зони видимості до загальної зони покриття";
            fieldVAToAllPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVAToAllPercent);

            IFieldEdit2 fieldPointsSee = new FieldClass() as IFieldEdit2;
            fieldPointsSee.Name_2 = "OPSee";
            fieldPointsSee.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldPointsSee.AliasName_2 = "спостережень";
            fieldPointsSee.IsNullable_2 = true;
            fieldsEdit.AddField(fieldPointsSee);

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IWorkspace2 wsp2 = workspace as IWorkspace2;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

            try
            {
                if(!wsp2.get_NameExists(esriDatasetType.esriDTTable, tableName))
                {
                    table = featureWorkspace.CreateTable(tableName, fieldsEdit, null, null, "");
                }
                else
                {
                    table = featureWorkspace.OpenTable(tableName);
                }
            }
            catch(Exception ex)
            {
                logger.ErrorEx($"Cannot create table {tableName} /n{ex.Message}");
                return null;
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return table;
        }

        public void FillVACoverageTable(List<CoverageTableRowModel> tableModel, string tableName, string gdb)
        {
            logger.DebugEx("> FillVACoverageTable START. gdb:{0} tableName:{1}", gdb, tableName);

            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
            IWorkspace workspace;

            try
            {
                workspace = workspaceFactory.OpenFromFile(gdb, 0);
            }
            catch(Exception ex)
            {
                logger.ErrorEx($"FillVACoverageTable. Cannot open GDB from {gdb} /n{ex.Message}");
                Marshal.ReleaseComObject(workspaceFactory);
                return;
            }

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();
            var table = GenerateVACoverageTable(tableName, workspace);

            try
            {
                foreach(var row in tableModel)
                {
                    var newRow = table.CreateRow();
                    newRow.Value[table.FindField("TitleOp")] = row.ObservPointName;
                    if(row.ObservPointId != -1)
                    {
                        newRow.Value[table.FindField("IdOP")] = row.ObservPointId;
                    }
                    newRow.Value[table.FindField("TitleOO")] = row.ObservObjName;

                    if(row.ObservObjId != -1)
                    {
                        newRow.Value[table.FindField("IdOO")] = row.ObservObjId;
                    }
                    newRow.Value[table.FindField("AreaOO")] = row.ObservObjArea;
                    newRow.Value[table.FindField("VisibilityArea")] = row.VisibilityArea;
                    newRow.Value[table.FindField("VisibilityPercentage")] = row.VisibilityPercent;

                    if(row.ObservPointsSeeCount != -1)
                    {
                        newRow.Value[table.FindField("OPSee")] = row.ObservPointsSeeCount;
                    }
                    newRow.Store();
                }
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);

                Marshal.ReleaseComObject(workspaceFactory);
            }
            catch(Exception ex)
            {
                logger.ErrorEx($"> FillVACoverageTable Exception {ex.Message}");
                return;
            }

            logger.DebugEx("> FillVACoverageTable END");
        }


        public void FillVSCoverageTable(List<CoverageTableRowModel> tableModel, string tableName, string gdb)
        {
            logger.InfoEx("> FillVSCoverageTable START tableName:{0} gdb:{1}", tableName, gdb);

            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
            IWorkspace workspace;

            try
            {
                workspace = workspaceFactory.OpenFromFile(gdb, 0);
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"> FillVSCoverageTable ERROR. Cannot open GDB from {gdb} /n{ex.Message}");
                Marshal.ReleaseComObject(workspaceFactory);
                return;
            }

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            var table = GenerateVSCoverageTable(tableName, workspace);

            try
            {
                foreach (var row in tableModel)
                {
                    var newRow = table.CreateRow();

                    newRow.Value[table.FindField("TitleOp")] = row.ObservPointName;

                    if (row.ObservPointId != -1)
                    {
                        newRow.Value[table.FindField("IdOP")] = row.ObservPointId;
                    }

                    newRow.Value[table.FindField("ExpectedVisibilityArea")] = row.ExpectedArea;
                    newRow.Value[table.FindField("VisibilityArea")] = row.VisibilityArea;
                    newRow.Value[table.FindField("VisibilityPercentage")] = row.VisibilityPercent;
                    newRow.Value[table.FindField("CurrentToAllExpectedVAPercentage")] = row.ToAllExpectedAreaPercent;
                    newRow.Value[table.FindField("CurrentVAToAllExpectedVAPercentage")] = row.ToAllVisibilityAreaPercent;

                    if (row.ObservPointsSeeCount != -1)
                    {
                        newRow.Value[table.FindField("OPSee")] = row.ObservPointsSeeCount;
                    }

                    newRow.Store();
                }

                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);

                Marshal.ReleaseComObject(workspaceFactory);
                logger.InfoEx("> FillVSCoverageTable END");
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"> FillVSCoverageTable EXCEPTION. An error occured during table {tableName} filling: {ex.Message}");
            }
        }

        public void FillBestParametersTable(Dictionary<IFeature, short> observPointBestParams, ITable table, string tableName)
        {
            logger.InfoEx("> FillBestParametersTable START tableName:{0}", tableName);

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            try
            {
                foreach (var feature in observPointBestParams)
                {
                    var newRow = table.CreateRow();
                    var featureFields = feature.Key.Fields;
                    
                    for(int i = 0; i < featureFields.FieldCount; i++)
                    {
                        if (featureFields.Field[i].Name != table.OIDFieldName)
                        {
                            var fieldIndex = table.FindField(featureFields.Field[i].Name);
                            if (fieldIndex >= 0)
                            {
                                newRow.Value[fieldIndex] = feature.Key.Value[i];
                            }
                        }
                    }
                    
                    newRow.Value[table.FindField("VisibilityPercent")] = feature.Value;

                    newRow.Store();
                }
               
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);

                logger.InfoEx("> FillBestParametersTable END");
            }
            catch (Exception ex)
            {
                logger.ErrorEx($"> FillBestParametersTable EXCEPTION. An error occured during table {tableName} filling: {ex.Message}");
            }
        }

        private void GenerateCoverageAreasTempStorage(string newFeatureClassName, IWorkspace workspace, ISpatialReference sr)
        {
            logger.InfoEx("> GenerateCoverageAreasTempStorage START. newFeatureClassName:{0}", newFeatureClassName);
            try
            {
                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                IFields fields = ocDescription.RequiredFields;

                int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);

                IField field = fields.get_Field(shapeFieldIndex);
                IGeometryDef geometryDef = field.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.HasZ_2 = false;
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;

                //geometryDefEdit.SpatialReference_2 = VisibilityManager.CurrentMap.SpatialReference;
                geometryDefEdit.SpatialReference_2 = sr;

                IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

                IField pointIdField = new FieldClass();
                IFieldEdit pointIdFieldEdit = (IFieldEdit)pointIdField;
                pointIdFieldEdit.Name_2 = "ObservPointId";
                pointIdFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.AddField(pointIdField);

                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;

                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();

                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(
                    newFeatureClassName,
                    fields,
                    ocDescription.InstanceCLSID,
                    ocDescription.ClassExtensionCLSID,
                    esriFeatureType.esriFTSimple,
                    "shape",
                    "");

                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);

                logger.InfoEx("> GenerateCoverageAreasTempStorage END");
            }
            catch (Exception ex)
            {
                logger.ErrorEx("> GenerateCoverageAreasTempStorage EXCEPTION. ex.Message:{0}", ex.Message);
            }
        }


        public IFeatureClass AddCalcPointsFeature(IEnumerable<IPoint> points, string featureClassName, ISpatialReference sr)
        {
            logger.InfoEx("> AddCalcPointsFeature START. featureClassName:{0}", featureClassName);

            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
            IWorkspace workspace;
            try
            {
                workspace = calcWorkspace;
            }
            catch(Exception ex)
            {
                Marshal.ReleaseComObject(workspaceFactory);

                logger.ErrorEx($"> AddCalcPointsFeature ERROR. Cannot open GDB: {ex.Message}");
                return null;
            }

            IWorkspace2 wsp2 = workspace as IWorkspace2;
            if(!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, featureClassName))
            {
                GenerateCalcPointsTempStorage(featureClassName, workspace, sr); 
            }

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            try
            {
                IFeatureClass featureClass = OpenFeatureClass(workspace, featureClassName);
                int i = 1;

                foreach(var point in points)
                {
                    var areaFeature = featureClass.CreateFeature();
                    areaFeature.Shape = point;
                    areaFeature.set_Value(featureClass.FindField("PointNumber"), i);

                    areaFeature.Store();
                    i++;
                }
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
                Marshal.ReleaseComObject(workspaceFactory);

                logger.InfoEx("> AddCalcPointsFeature END");
                return featureClass;
            }
            catch(Exception ex)
            {
                logger.ErrorEx("> AddCalcPointsFeature Exception. ex.Message:{0}", ex.Message);
                return null;
            }
        }

        private void GenerateCalcPointsTempStorage(string newFeatureClassName, IWorkspace workspace, ISpatialReference sr)
        {
            logger.InfoEx("> GenerateCalcPointsTempStorage START. newFeatureClassName:{0}", newFeatureClassName);
            try
            {
                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                IFields fields = ocDescription.RequiredFields;

                int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);

                IField field = fields.get_Field(shapeFieldIndex);
                IGeometryDef geometryDef = field.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.HasZ_2 = false;
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;

                geometryDefEdit.SpatialReference_2 = sr;

                IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

                IField pointNumField = new FieldClass();
                IFieldEdit pointNumFieldEdit = (IFieldEdit)pointNumField;
                pointNumFieldEdit.Name_2 = "PointNumber";
                pointNumFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.AddField(pointNumField);

                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;

                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();

                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(
                    newFeatureClassName,
                    fields,
                    ocDescription.InstanceCLSID,
                    ocDescription.ClassExtensionCLSID,
                    esriFeatureType.esriFTSimple,
                    "shape",
                    "");

                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);

                logger.InfoEx("> GenerateCalcPointsTempStorage END");
            }
            catch(Exception ex)
            {
                logger.ErrorEx("> GenerateCalcPointsTempStorage EXCEPTION. ex.Message:{0}", ex.Message);
            }
        }

        public IFeatureClass AddCoverageAreaFeature(IPolygon polygon, int pointId, string featureClassName, ISpatialReference sr)
        {
            logger.InfoEx("> AddCoverageAreaFeature START. featureClassName:{0} pointId:{1}", featureClassName, pointId);

            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
            IWorkspace workspace;
            try
            {
                //workspace = workspaceFactory.OpenFromFile(gdb, 0);
                workspace = calcWorkspace;
            }
            catch (Exception ex)
            {
                Marshal.ReleaseComObject(workspaceFactory);

                logger.ErrorEx($"> AddCoverageAreaFeature ERROR. Cannot open GDB: {ex.Message}");
                return null;
            }

            IWorkspace2 wsp2 = workspace as IWorkspace2;
            if (!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, featureClassName))
            {
                GenerateCoverageAreasTempStorage(featureClassName, workspace, sr);  //IPolygon polygon
            }

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            try
            {
                IFeatureClass featureClass = OpenFeatureClass(workspace, featureClassName);
                var areaFeature = featureClass.CreateFeature();
                areaFeature.Shape = polygon;
                if(pointId != -1)
                {
                    areaFeature.set_Value(featureClass.FindField("ObservPointId"), pointId);
                }

                areaFeature.Store();
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
                Marshal.ReleaseComObject(workspaceFactory);

                logger.InfoEx("> AddCoverageAreaFeature END");
                return featureClass;
            }
            catch(Exception ex)
            {
                logger.ErrorEx("> AddCoverageAreaFeature Exception. ex.Message:{0}", ex.Message);
                return null;
            }
        }

        public IFeatureClass GetFeatureClass(string gdb, string featureClassName)
        {
            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
            IWorkspace workspace;

            try
            {
                workspace = workspaceFactory.OpenFromFile(gdb, 0);
            }
            catch(Exception ex)
            {
                logger.ErrorEx($"Cannot open GDB from {gdb} /n{ex.Message}");
                Marshal.ReleaseComObject(workspaceFactory);
                return null;
            }

            var featureClass = OpenFeatureClass(workspace, featureClassName);

            Marshal.ReleaseComObject(workspaceFactory);

            return featureClass;
        }

        public void RemoveFeatureClass(string name)
        {
            IFeatureWorkspaceManage wspManage = (IFeatureWorkspaceManage)calcWorkspace;
            var datasets = calcWorkspace.Datasets[esriDatasetType.esriDTFeatureClass];
            var currentDataset = datasets.Next();

            while (currentDataset != null && !currentDataset.Name.EndsWith(name))
            {
                currentDataset = datasets.Next();
            }

            if (currentDataset != null)
            {
                if (wspManage.CanDelete(currentDataset.FullName))
                {
                    try
                    {
                        currentDataset.Delete();
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorEx(ex.Message);
                    }
                }
            }
        }
    }
}
