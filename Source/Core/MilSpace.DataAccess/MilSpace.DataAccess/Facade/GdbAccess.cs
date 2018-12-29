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

namespace MilSpace.DataAccess.Facade
{
    public class GdbAccess
    {
        private static GdbAccess instance = null;

        private static string calcFeatureClass = "CalcProfile_L";
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
        public void AddPrifileLinesToCalculation(double xFrom, double yFrom, double xTo, double yTo)
        {
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)calcWorkspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();

            IFeatureClass calc = GetCalcProfileFeatureClass();

            var newLine = calc.CreateFeature();
            IPolyline polyline = new PolylineClass();
            WKSPoint[] segmentWksPoints = new WKSPoint[2];
            ISubtypes subtypes = (ISubtypes)calc;
            IRowSubtypes rowSubtypes = (IRowSubtypes)newLine;
            if (subtypes.HasSubtype)                                // does the feature class have subtypes?        
            {
                rowSubtypes.SubtypeCode = 1;                        //
            }
            segmentWksPoints[0].X = xFrom;
            segmentWksPoints[0].Y = yFrom;
            segmentWksPoints[1].X = xTo;
            segmentWksPoints[1].Y = yTo;

            //TODO: create Id value
            //newLine.set_Value(newLine.Fields.FindField("ID"), newId);

            IPointCollection4 trackLine = new PolylineClass();
            IGeometryBridge2 m_geometryBridge = new GeometryEnvironmentClass();
            m_geometryBridge.AddWKSPoints(trackLine, ref segmentWksPoints);
            IGeometry trackGeometry = trackLine as IGeometry;
            newLine.Shape = trackGeometry;

            newLine.Store();
            workspaceEdit.StopEditOperation();
            workspaceEdit.StopEditing(true);
        }

        public void EraseProfileLines()
        {
            IFeatureClass calc = GetCalcProfileFeatureClass();
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

        public string ProfileLinesFeatureClass
        {
            get { return $"{ MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection}\\{calcFeatureClass}"; }
        }

        private IFeatureClass GetCalcProfileFeatureClass()
        {

            IWorkspace2 wsp2 = (IWorkspace2)calcWorkspace;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)calcWorkspace;

            if (!wsp2.get_NameExists(esriDatasetType.esriDTFeatureClass, calcFeatureClass))
            {
                //TODO: Create the feature class
                throw new FileNotFoundException(calcFeatureClass);
            }

            return featureWorkspace.OpenFeatureClass(calcFeatureClass);
        }


    }
}
