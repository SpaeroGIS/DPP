using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using MilSpace.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Framework;
using MilSpace.DataAccess.Exceptions;
using MilSpace.Core.Tools;

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
                    
                    //ISubtypes subtypes = (ISubtypes)calc;
                    //IRowSubtypes rowSubtypes = (IRowSubtypes)newLine;
                    //if (subtypes.HasSubtype)                                // does the feature class have subtypes?        
                    //{
                    //    rowSubtypes.SubtypeCode = 1;                        //
                    //}

                    //IGeometry trackGeometry = EsriTools.CreatePolylineFromPoints(l.FromPoint, l.ToPoint);
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
                if (application == null)
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
    }
}
