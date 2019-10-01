using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public class GdbAccess
    {
        private static GdbAccess instance = null;
        private static IApplication application = null;

        private static readonly string profileCalcFeatureClass = "CalcProfile_L";

        //Visibility dataset template 
        private static readonly string visibilityCalcFeatureClass = "VDSR";
        private static readonly string visibilityCalcFeatureDataset = $"{visibilityCalcFeatureClass}DS";
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

        //public string AddObservationPointsToCalculation(IWorkspace sourceWorkspace1, IFeatureClass sourceFeatureClass,
        //    IEnumerable<IFeature> observationPoints)
        //{
        //    //Target dataset name
        //    string temporarySuffix = MilSpace.DataAccess.Helper.GetTemporaryNameSuffix();
        //    string datasetName = $"{visibilityCalcFeatureDataset}{temporarySuffix}";
        //    string stringtargetFeatureClassName = $"{visibilityCalcFeatureClass}{temporarySuffix}";

        //    // Create the objects and references necessary for field validation.
        //    IFieldChecker fieldChecker = new FieldCheckerClass();
        //    IFields sourceFields = sourceFeatureClass.Fields;
        //    IFields targetFields = null;
        //    IEnumFieldError enumFieldError = null;

        //    // Set the required properties for the IFieldChecker interface.


        //    try
        //    {
        //        //Source Dataset name
        //        //IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
        //        //sourceFeatureClassName.FeatureDatasetName = ((IDataset)sourceFeatureClass).FullName as IDatasetName;

        //        // Create a name object for the source (shapefile) workspace and open it.
        //        IWorkspaceName sourceWorkspaceName = new WorkspaceNameClass
        //        {
        //            WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory",
        //            PathName = @"E:\Data\MilSpace3D\MilSpaceCalc_40a.gdb"
        //        };
        //        IName sourceWorkspaceIName = (IName)sourceWorkspaceName;
        //        IWorkspace sourceWorkspace = (IWorkspace)sourceWorkspaceIName.Open();

        //        // Create a name object for the source dataset.
        //        IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
        //        IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
        //        sourceDatasetName.Name = "MilSp_Visible_ObservPoints";
        //        sourceDatasetName.WorkspaceName = sourceWorkspaceName;



        //        //IWorkspaceName targetWorkspaceName = new WorkspaceNameClass
        //        //{
        //        //    WorkspaceFactoryProgID = "esriDataSourcesGDB.FileGDBWorkspaceFactory",
        //        //    PathName = @"E:\Data\MilSpace3D\MilSpaceCalc_40a.gdb"
        //        //};



        //        // Create a name object for the target dataset.
        //        IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
        //        IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
        //        targetDatasetName.Name = stringtargetFeatureClassName;
        //        targetDatasetName.WorkspaceName = (((IDataset)targetDataset).FullName as IDatasetName).WorkspaceName;


        //        IWorkspaceName targetWorkspaceName = new WorkspaceNameClass
        //        {
        //            WorkspaceFactoryProgID = "esriDataSourcesGDB.FileGDBWorkspaceFactory",
        //            PathName = @"E:\Data\MilSpace3D\MilSpaceCalc_40a.gdb"
        //        };

        //        IName targetWorkspaceIName = (IName)targetWorkspaceName;
        //        IWorkspace targetWorkspace = (IWorkspace)targetWorkspaceIName.Open();
        //        // Create a name object for the target dataset.
        //        //IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
        //        //IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
        //        targetDatasetName.Name = "Cities";
        //        targetDatasetName.WorkspaceName = targetWorkspaceName;


        //        // Validate the fields and check for errors.
        //        fieldChecker.InputWorkspace = sourceWorkspace;
        //        fieldChecker.ValidateWorkspace = targetWorkspace;

        //        fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);
        //        if (enumFieldError != null)
        //        {
        //            // Handle the errors in a way appropriate to your application.
        //            logger.ErrorEx("Errors were encountered during field validation.");
        //            throw new InvalidDataException("Errors were encountered during field validation.");
        //        }

        //        // Find the shape field.
        //        String shapeFieldName = sourceFeatureClass.ShapeFieldName;
        //        int shapeFieldIndex = sourceFeatureClass.FindField(shapeFieldName);
        //        IField shapeField = sourceFields.get_Field(shapeFieldIndex);

        //        // Get the geometry definition from the shape field and clone it.
        //        IGeometryDef geometryDef = shapeField.GeometryDef;
        //        IClone geometryDefClone = (IClone)geometryDef;
        //        IClone targetGeometryDefClone = geometryDefClone.Clone();
        //        IGeometryDef targetGeometryDef = (IGeometryDef)targetGeometryDefClone;

        //        //Define filter for importing
        //        IQueryFilter queryFilter = new QueryFilterClass();
        //        queryFilter.WhereClause = "ObjectId > 0";

        //        // Create the converter and run the conversion.
        //        IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
        //        IEnumInvalidObject enumInvalidObject = featureDataConverter.ConvertFeatureClass
        //            (sourceFeatureClassName, null, null, targetFeatureClassName,
        //            targetGeometryDef, targetFields, "", 1000, 0);

        //        // Check for errors.
        //        IInvalidObjectInfo invalidObjectInfo = null;
        //        enumInvalidObject.Reset();
        //        while ((invalidObjectInfo = enumInvalidObject.Next()) != null)
        //        {
        //            // Handle the errors in a way appropriate to the application.
        //            Console.WriteLine("Errors occurred for the following feature: {0}",
        //                invalidObjectInfo.InvalidObjectID);
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        logger.ErrorEx(ex.Message);
        //        return null;
        //    }

        //    return datasetName;
        //}

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


        public string ExportObservationPoints(IDataset sourceDataset, int[] filter)
        {
            IWorkspace targetWorkspace = calcWorkspace;

            //Target dataset name
            string temporarySuffix = MilSpace.DataAccess.Helper.GetTemporaryNameSuffix();
            string datasetName = $"{visibilityCalcFeatureDataset}{temporarySuffix}";
            string nameOfTargetDataset = $"{visibilityCalcFeatureClass}{temporarySuffix}";

            try
            {
                //create source workspace name
                ITable pSourceTab = (ITable)sourceDataset;
                IFeatureClass sourceFeatureClass = (IFeatureClass)sourceDataset;

                IDataset sourceWorkspaceDataset = (IDataset)sourceDataset.Workspace;
                IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;

                IFeatureClassName sourceDatasetName = (IFeatureClassName)sourceDataset.FullName;

                //create target workspace name
                IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
                IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
                //create target dataset name
                IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();


                //IFeatureWorkspace featureworkspace = (IFeatureWorkspace)calcWorkspace;
                //IFeatureDataset targetDataset = featureworkspace.CreateFeatureDataset(datasetName, EsriTools.Wgs84Spatialreference);

                IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
                targetDatasetName.WorkspaceName = targetWorkspaceName;
                targetDatasetName.Name = nameOfTargetDataset;
                ////Open input Featureclass to get field definitions.
                //ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceDatasetName;
                //ITable sourceTable = (ITable)sourceName.Open();

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

                if (enumFieldError == null)
                {
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();

                    //(IFeatureDatasetName)targetDataset.FullName
                    IEnumInvalidObject enumErrors =
                        fctofc.ConvertFeatureClass(sourceDatasetName, queryFilter, null, targetFeatureClassName, targetGeometryDef,
                        pSourceTab.Fields, "", 1000, 0);
                }

                return ((IDatasetName)targetFeatureClassName).Name;
            }
            catch (Exception exp)
            {
                logger.ErrorEx(exp.ToString());
                return null;
            }
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
            pointFeature.Shape = point;

            SetObservPointValues(featureClass, pointFeature, pointArgs);
        }

        public void UpdateObservPoint(IFeatureClass featureClass, ObservationPoint observPoint, int objectId)
        {
            IFeature pointFeature = featureClass.GetFeature(objectId);
            SetObservPointValues(featureClass, pointFeature, observPoint);
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

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if (!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, currentFeatureClass))
            {
                //TODO: Create the feature class
                throw new MilSpaceDataException(currentFeatureClass, Core.DataAccess.DataOperationsEnum.Access);
            }

            return featureWorkspace.OpenFeatureClass(currentFeatureClass);
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

        private void SetObservPointValues(IFeatureClass featureClass, IFeature pointFeature, ObservationPoint pointArgs)
        {
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
            pointFeature.set_Value(featureClass.FindField("dto"), pointArgs.Dto);
            pointFeature.set_Value(featureClass.FindField("soper"), pointArgs.Operator);

            pointFeature.Store();
        }
    }
}
