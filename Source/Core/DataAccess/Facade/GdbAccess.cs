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
        private static string divider = "\\";


        private static readonly string profileCalcFeatureClass = "CalcProfile_L";

        private Logger logger = Logger.GetLoggerEx("GdbAccess");

        private GdbAccess()
        {
            string calcGdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;
            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
            logger.InfoEx($"Opening access to {calcGdb}.");
            calcWorkspace = workspaceFactory.OpenFromFile(calcGdb, 0);
            if (calcWorkspace == null)
            {
                logger.ErrorEx($"Cannot access to {calcGdb}.");
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
                        //TODO: Log error
                    }
                }

                return instance;
            }
        }
        private static IWorkspace calcWorkspace = null;
        private static IWorkspace workingWorkspace = null;

        public string AddProfileLinesToCalculation(IEnumerable<IPolyline> profileLines)
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
                }
                );


            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return featureClassName;
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

            IWorkspace targetWorkspace = calcWorkspace;

            IWorkspaceEdit workspaceEdit = null;
            bool isBeingEdited = false;
            try
            {
                //create source workspace name
                ITable pSourceTab = (ITable)sourceDataset;
                IFeatureClass sourceFeatureClass = (IFeatureClass)sourceDataset;

                IDataset sourceWorkspaceDataset = (IDataset)sourceDataset.Workspace;
                IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;
                IFeatureClassName sourceDatasetName = (IFeatureClassName)sourceDataset.FullName;

                workspaceEdit = (IWorkspaceEdit)sourceDataset.Workspace;
                isBeingEdited = workspaceEdit.IsBeingEdited();
                if (!isBeingEdited)
                {
                    workspaceEdit.StartEditing(true);
                    workspaceEdit.StartEditOperation();
                }

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
                        logger.WarnEx($"Export error in the filed {targetFields.Field[error.FieldIndex].AliasName}");
                        if (!targetFields.Field[error.FieldIndex].AliasName.StartsWith("shape_ST", StringComparison.InvariantCultureIgnoreCase))
                        {
                            exportWithError = false;
                            logger.ErrorEx("Export is not acceptable");
                        }

                        error = enumFieldError.Next();
                    }
                }

                if (enumFieldError == null || exportWithError)
                {
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();

                    //(IFeatureDatasetName)targetDataset.FullName
                    IEnumInvalidObject enumErrors =
                        fctofc.ConvertFeatureClass(sourceDatasetName, queryFilter, null, targetFeatureClassName, targetGeometryDef,
                        pSourceTab.Fields, "", 1000, 0);
                    return ((IDatasetName)targetFeatureClassName).Name;
                }
            }
            catch (Exception exp)
            {
                logger.ErrorEx(exp.ToString());
            }
            finally
            {
                if (workspaceEdit != null && !isBeingEdited)
                {
                    workspaceEdit.StopEditOperation();
                    workspaceEdit.StopEditing(false);
                }
            }

            return null;
        }

        public string GenerateTempProfileLinesStorage()
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            string newFeatureClassName = $"{profileCalcFeatureClass}{MilSpace.DataAccess.Helper.GetTemporaryNameSuffix()}";

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
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = "OBJECTID > 0";

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
                //TODO: add logging
                throw;
            }
            catch (Exception ex)
            {
                //TODO: add logging

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
            IWorkspace2 wsp2 = (IWorkspace2)workspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

            //if(!wsp2.NameExists[esriDatasetType.esriDTFeatureClass, featureClass])
            //{
            var datasetNames = workspace.DatasetNames[esriDatasetType.esriDTFeatureClass];
            var featureName = datasetNames.Next().Name;

            while (!featureName.EndsWith(featureClass))
            {
                featureName = datasetNames.Next().Name;
            }

            featureClass = featureName;
            // }

            return featureWorkspace.OpenFeatureClass(featureClass);
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

            if (IsDatasetExist(calcWorkspace, visibilityResult.ResultName, 
                VisibilityTask.GetEsriDataTypeByVisibilityresyltType(visibilityResult.RessutType)))
            {
                return GetDataset(calcWorkspace, visibilityResult.ResultName, 
                    VisibilityTask.GetEsriDataTypeByVisibilityresyltType(visibilityResult.RessutType));
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
            var featurelayer = new FeatureLayer();
            featurelayer.Name = featureClassName;
            featurelayer.FeatureClass = OpenFeatureClass(WorkingWorkspace, featureClassName);
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
                        throw new MilSpaceDataException(MilSpaceConfiguration.ConnectionProperty.WorkingGDBConnection, Core.DataAccess.DataOperationsEnum.Access);
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


        private void GenerateTempStorage(string featureClassName, IFields fields, esriGeometryType type)
        {
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
                    geometryDefEdit.HasZ_2 = true;
                    geometryDefEdit.GeometryType_2 = type;
                    geometryDefEdit.SpatialReference_2 = ArcMapInstance.Document.FocusMap.SpatialReference;
                }

                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(featureClassName, fields,
                    ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "shape", "");
            }

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
        }

        private void SetObservPointValues(IFeatureClass featureClass, IFeature pointFeature, IPoint point, ObservationPoint pointArgs)
        {
            if (point != null)
            {
                pointFeature.Shape = point;
            }

            pointFeature.set_Value(featureClass.FindField("TitleOP"), pointArgs.Title);
            pointFeature.set_Value(featureClass.FindField("TypeOP"), pointArgs.Type.ToString());
            pointFeature.set_Value(featureClass.FindField("saffiliation"), pointArgs.Affiliation.ToString());
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
            pointFeature.set_Value(featureClass.FindField("dto"), pointArgs.Dto);
            pointFeature.set_Value(featureClass.FindField("soper"), pointArgs.Operator);

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
            fieldObjName.AliasName_2 = "Назва ОС";
            fieldObjName.IsNullable_2 = true;
            fieldsEdit.AddField(fieldObjName);

            IFieldEdit2 fieldObjId = new FieldClass() as IFieldEdit2;
            fieldObjId.Name_2 = "IdOO";
            fieldObjId.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldObjId.AliasName_2 = "Id ОС";
            fieldObjId.IsNullable_2 = true;
            fieldsEdit.AddField(fieldObjId);

            IFieldEdit2 fieldObjArea = new FieldClass() as IFieldEdit2;
            fieldObjArea.Name_2 = "AreaOO";
            fieldObjArea.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldObjArea.IsNullable_2 = false;
            fieldObjArea.AliasName_2 = "Площа ОС";
            fieldsEdit.AddField(fieldObjArea);

            IFieldEdit2 fieldVA = new FieldClass() as IFieldEdit2;
            fieldVA.Name_2 = "VisibilityArea";
            fieldVA.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldVA.AliasName_2 = "Площа видимості ОС";
            fieldVA.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVA);

            IFieldEdit2 fieldVAPercent = new FieldClass() as IFieldEdit2;
            fieldVAPercent.Name_2 = "VisibilityPercentage";
            fieldVAPercent.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldVAPercent.AliasName_2 = "Процент видимості";
            fieldVAPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVAPercent);

            IFieldEdit2 fieldPointsSee = new FieldClass() as IFieldEdit2;
            fieldPointsSee.Name_2 = "OPSee";
            fieldPointsSee.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldPointsSee.AliasName_2 = "К-ть спостережень";
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
            fieldVAPercent.AliasName_2 = "Процент видимості";
            fieldVAPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVAPercent);

            IFieldEdit2 fieldExpectedVAToAllPercent = new FieldClass() as IFieldEdit2;
            fieldExpectedVAToAllPercent.Name_2 = "CurrentToAllExpectedVAPercentage";
            fieldExpectedVAToAllPercent.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldExpectedVAToAllPercent.AliasName_2 = "Процент зони покриття ПС від загальної";
            fieldExpectedVAToAllPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldExpectedVAToAllPercent);

            IFieldEdit2 fieldVAToAllPercent = new FieldClass() as IFieldEdit2;
            fieldVAToAllPercent.Name_2 = "CurrentVAToAllExpectedVAPercentage";
            fieldVAToAllPercent.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldVAToAllPercent.AliasName_2 = "Процент відношення видимої зони до загальної зони покриття";
            fieldVAToAllPercent.IsNullable_2 = false;
            fieldsEdit.AddField(fieldVAToAllPercent);

            IFieldEdit2 fieldPointsSee = new FieldClass() as IFieldEdit2;
            fieldPointsSee.Name_2 = "OPSee";
            fieldPointsSee.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldPointsSee.AliasName_2 = "К-ть спостережень";
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
                logger.ErrorEx($"An error occured during table {tableName} filling/n{ex.Message}");

            }
        }


        public void FillVSCoverageTable(List<CoverageTableRowModel> tableModel, string tableName, string gdb)
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
                return;
            }

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            var table = GenerateVSCoverageTable(tableName, workspace);

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

                    newRow.Value[table.FindField("ExpectedVisibilityArea")] = row.ExpectedArea;
                    newRow.Value[table.FindField("VisibilityArea")] = row.VisibilityArea;
                    newRow.Value[table.FindField("VisibilityPercentage")] = row.VisibilityPercent;
                    newRow.Value[table.FindField("CurrentToAllExpectedVAPercentage")] = row.ToAllExpectedAreaPercent;
                    newRow.Value[table.FindField("CurrentVAToAllExpectedVAPercentage")] = row.ToAllVisibilityAreaPercent;

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
                logger.ErrorEx($"An error occured during table {tableName} filling/n{ex.Message}");
            }
}

        private void GenerateCoverageAreasTempStorage(string newFeatureClassName, string gdb, IWorkspace workspace)
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
            geometryDefEdit.SpatialReference_2 = ArcMapInstance.Document.FocusMap.SpatialReference;

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

            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(newFeatureClassName, fields,
                    ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "shape", "");

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
        }

        public IFeatureClass AddCoverageAreaFeature(IPolygon polygon, int pointId, string featureClassName, string gdb)
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

            IWorkspace2 wsp2 = workspace as IWorkspace2;

            if (!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, featureClassName))
            {
                GenerateCoverageAreasTempStorage(featureClassName, gdb, workspace);
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

                return featureClass;
            }
            catch(Exception ex)
            {
                logger.ErrorEx($"An error occured during table {featureClassName} filling/n{ex.Message}");
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

        public void RemoveCoverageAreaTemporStorage(string name)
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
