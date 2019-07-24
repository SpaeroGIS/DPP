using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
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

        private static readonly string calcFeatureClass = "CalcProfile_L";

        private GdbAccess()
        {
            string calcGdb = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;
            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactory();
            calcWorkspace = workspaceFactory.OpenFromFile(calcGdb, 0);
        }

        public static GdbAccess Instance
        {
            get
            {
                if(instance == null)
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

        public IApplication Application
        {
            set
            {
                if(application == null)
                {
                    ArcMapInstance.Application = application = value;
                }
            }
        }

        public string GenerateTempProfileLinesStorage()
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            string newFeatureClassName = $"{calcFeatureClass}{MilSpace.DataAccess.Helper.GetTemporaryNameSuffix()}";

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if(!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, newFeatureClassName))
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

        public IFeatureClass AddProfileLinesTo3D(Dictionary<IPolyline, bool> profileLines)
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
                    newLine.set_Value(isVisibleFieldIndex, l.Value? 1:0);

                    newLine.Shape = l.Key;
                    newLine.Store();

                    i++;
                }
                );

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return calc;
        }

        public IFeatureClass AddProfilePointsTo3D(IEnumerable<IPoint> points)
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

            return calc;
        }

        public IFeatureClass AddPolygonTo3D(IEnumerable<IPolygon> polygons)
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
                pointFeature.Shape = polygon;
                pointFeature.Store();
            });

            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);

            return calc;
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

            while((feature = featureCursor.NextFeature()) != null)
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

            if(!wsp2.get_NameExists(esriDatasetType.esriDTTable, resultTable))
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
                if(tabledataset != null)
                {

                    while(tabledataset != null)
                    {
                        if(!tabledataset.Name.Equals(resultTable))
                        {
                            tabledataset = datasets.Next();
                            continue;
                        }

                        if(!tabledataset.CanDelete())
                        {
                            throw new MilSpaceCanotDeletePrifileCalcTable(resultTable, MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection);
                        }

                        tabledataset.Delete();
                        break;
                    }
                }

                //Delete temprorary Feature class (Profile lites)
                datasets = calcWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);

                if(tabledataset != null)
                {
                    while(tabledataset != null)
                    {
                        if(!tabledataset.Name.Equals(lineFeatureClass))
                        {
                            tabledataset = datasets.Next();
                            continue;
                        }

                        if(!tabledataset.CanDelete())
                        {
                            throw new MilSpaceCanotDeletePrifileCalcTable(lineFeatureClass, MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection);
                        }

                        tabledataset.Delete();
                        break;
                    }
                }

                return true;

            }
            catch(MilSpaceCanotDeletePrifileCalcTable ex)
            {
                //TODO: add logging
                throw;
            }
            catch(Exception ex)
            {
                //TODO: add logging

            }
            return false;

        }


        public IFeatureClass GetCalcProfileFeatureClass(string currentFeatureClass)
        {

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if(!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, currentFeatureClass))
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
            GenerateTempStorage(newFeatureClassName, null, esriGeometryType.esriGeometryPolygon);
            return newFeatureClassName;
        }


        private void GenerateTempStorage(string featureClassName, IFields fields, esriGeometryType type)
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if(!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, featureClassName))
            {
                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;

                if(fields == null)
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
    }
}
