using ESRI.ArcGIS.Geodatabase;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;

namespace MilSpace.DataAccess.Facade
{
    public class SentinelFacade
    {
        public static void AddFootprint(SentinelFootprint footprint)
        {
            IFeatureClass sentinelFeatureClass = GdbAccess.Instance.GetCalcWorkspaceFeatureClass("Sentinel");



            var fldUuid = sentinelFeatureClass.Fields.FindField("UUid");
            var fldPass = sentinelFeatureClass.Fields.FindField("PassDirection");
            var fldRel = sentinelFeatureClass.Fields.FindField("RelativeOrbit");
            var fldCenterPointX = sentinelFeatureClass.Fields.FindField("CenterPointX");
            var fldCenterPointY = sentinelFeatureClass.Fields.FindField("CenterPointY");

            var fldId = sentinelFeatureClass.Fields.FindField("Id");
            var fldIdentifier = sentinelFeatureClass.Fields.FindField("Identifier");


            var centroid = footprint.Footprint.Envelope.GetCentroid();

            


            IQueryFilter queryFilter = new QueryFilter();
            queryFilter.WhereClause = $"0.0001 >= ((CenterPointX - {centroid.X.ToFormattedString(6)}) * (CenterPointX - {centroid.X.ToFormattedString(6)})) + ((CenterPointY - {centroid.Y.ToFormattedString(6)}) * (CenterPointY - {centroid.Y.ToFormattedString(6)}))";

            //spatialFilter.GeometryField = sentinelFeatureClass.ShapeFieldName;
            //spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            // specify the geometry to query with. apply a buffer if desired

            // Use the ITopologicalOperator interface to create a buffer.
            //ITopologicalOperator topoOperator = (ITopologicalOperator)centroid;
            //IGeometry buffer = topoOperator.Buffer(0.01);
            //spatialFilter.Geometry = buffer;

            var featureCursor = sentinelFeatureClass.Search(queryFilter, false);
            var row = featureCursor.NextFeature();
            if (row != null)
            {
                //Found
            }
            else
            {
                var newFeature = sentinelFeatureClass.CreateFeature();
                newFeature.Shape = footprint.Footprint;
                newFeature.set_Value(fldUuid, footprint.Uuid);
                newFeature.set_Value(fldId, footprint.Id);
                newFeature.set_Value(fldIdentifier, footprint.Identifier);
                newFeature.set_Value(fldPass, footprint.PassDirection);
                newFeature.set_Value(fldRel, footprint.RelativeOrbit);

                newFeature.set_Value(fldCenterPointX, centroid.X);
                newFeature.set_Value(fldCenterPointY, centroid.Y);
                newFeature.Store();
            }

        }
    }
}
