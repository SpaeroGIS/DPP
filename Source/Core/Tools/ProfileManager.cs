using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Actions;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Exceptions;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Exceptions;
using MilSpace.Tools.SurfaceProfile.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.Tools
{
    public class ProfileManager
    {
        private static readonly string FIRST_DIST_Field = "FIRST_DIST";
        private static readonly string FIRST_Z_Field = "FIRST_Z";
        private static readonly string LINE_ID_Field = "LINE_ID";

        private static readonly string WhereAllRecords = "OBJECTID > 0";
        private Logger logger = Logger.GetLoggerEx("ProfileManager");


        public ProfileManager()
        { }


        public ProfileSession GenerateProfile(
            string profileSource,
            IEnumerable<IPolyline> profileLines,
            ProfileSettingsTypeEnum profileSettingsTypeEnum,
            int sessionId, 
            string sessionName, 
            double observHeight, 
            string azimuthes)
        {
            logger.InfoEx("> GenerateProfile START. Adding {0}".InvariantFormat(profileLines.Count()));
            try
            {
                string profileSourceName = GdbAccess.Instance.AddProfileLinesToCalculation(profileLines);

                logger.InfoEx("GenerateProfile. Spatial source:{0}".InvariantFormat(profileSourceName));

                var action = new ActionParam<string>()
                {
                    ParamName = ActionParamNamesCore.Action,
                    Value = ActionsEnum.bsp.ToString()
                };

                string sdtnow = MilSpace.DataAccess.Helper.GetTemporaryNameSuffix();
                var resuTable = $"{MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection}\\StackProfile{sdtnow}";

                logger.InfoEx("GenerateProfile. Temporary profile source:{0}".InvariantFormat(resuTable));

                var profileLineFeatureClass = GdbAccess.Instance.GetProfileLinesFeatureClass(profileSourceName);
                logger.InfoEx("GenerateProfile. Temporary spatial source {0} was created".InvariantFormat(profileLineFeatureClass));

                var prm = new List<IActionParam>
                {
                    action,
                    new ActionParam<string>() { ParamName = ActionParameters.FeatureClass, Value = profileLineFeatureClass },
                    new ActionParam<string>() { ParamName = ActionParameters.ProfileSource, Value = profileSource },
                    new ActionParam<string>() { ParamName = ActionParameters.DataWorkSpace, Value = resuTable},
                    new ActionParam<string>() { ParamName = ActionParameters.OutputSourceName, Value = ""}
                };

                var procc = new ActionProcessor(prm);
                var res = procc.Process<StringCollectionResult>();

                if (res.Exception != null)
                {
                    logger.ErrorEx("GenerateProfile. There was an error on calculation!");
                    throw res.Exception;
                }

                //Take the table and import the data
                ISpatialReference currentSpatialreference = profileLines.First().SpatialReference;
                try
                {
                    string tempTableName = $"StackProfile{sdtnow}";
                    ITable profiletable = GdbAccess.Instance.GetProfileTable(tempTableName);
                    IFeatureClass lines = GdbAccess.Instance.GetCalcProfileFeatureClass(profileSourceName);

                    IQueryFilter queryFilter = new QueryFilter()
                    {
                        WhereClause = WhereAllRecords
                    };

                    ICursor featureCursor = profiletable.Search(queryFilter, true);
                    IRow profileRow;

                    int distanceFld = profiletable.FindField(FIRST_DIST_Field);
                    int zFld = profiletable.FindField(FIRST_Z_Field);
                    int idFld = profiletable.FindField(LINE_ID_Field);

                    List<ProfileSurface> profileSurfaces = new List<ProfileSurface>();
                    ProfileSession session = new ProfileSession()
                    {
                        ProfileSurfaces = profileSurfaces.ToArray(),
                        ProfileLines = GetProfileLines(lines).ToArray(),
                        SessionId = sessionId,
                        SessionName = sessionName,
                        DefinitionType = profileSettingsTypeEnum,
                        ObserverHeight = observHeight,
                        SurfaceLayerName = profileSource,
                        CreatedBy = Environment.UserName,
                        CreatedOn = DateTime.Now,
                        Shared = false,
                        Azimuth = azimuthes
                    };

                    Dictionary<int, List<ProfileSurfacePoint>> surface = new Dictionary<int, List<ProfileSurfacePoint>>();

                    int curLine = -1;
                    IPolyline line = null;
                    IEnumerable<IPoint> verticesCache;
                    Dictionary<IPoint, ProfileSurfacePoint> mapProfilePointToVertex = new Dictionary<IPoint, ProfileSurfacePoint>();
                    Dictionary<IPoint, double> mapProfilePointToDistance = new Dictionary<IPoint, double>();
                    ProfileLine profileLine = null;
                    verticesCache = new IPoint[0];

                    int pointsCount = 0;

                    while ((profileRow = featureCursor.NextRow()) != null)
                    {
                        int lineId = Convert.ToInt32(profileRow.Value[idFld]);
                        pointsCount++;
                        if (!session.ProfileLines.Any(l => l.Id == lineId))
                        {
                            throw new MilSpaceProfileLineNotFound(lineId, profileLineFeatureClass);
                        }

                        List<ProfileSurfacePoint> points;
                        if (!surface.ContainsKey(lineId))
                        {
                            points = new List<ProfileSurfacePoint>();
                            surface.Add(lineId, points);
                        }
                        else
                        {
                            points = surface[lineId];
                        }

                        if (curLine != lineId) // data for new line
                        {
                            curLine = lineId;
                            profileLine = session.ProfileLines.FirstOrDefault(l => l.Id == lineId);
                            line = lines.GetFeature(profileLine.Id).Shape as IPolyline;
                            verticesCache = line.Vertices();
                            mapProfilePointToVertex.ToList().ForEach(v =>
                               {
                                   if (!v.Value.IsEmpty)
                                   {
                                       v.Value.isVertex = true;
                                   }
                               });

                            mapProfilePointToVertex = verticesCache.ToDictionary(k => k, t => new ProfileSurfacePoint());
                            mapProfilePointToDistance = verticesCache.ToDictionary(k => k, t => -1.0);
                        }

                        //Returns the point with Origin (Taken from firstPoint) Spatial reference
                        //var profilePointSource = EsriTools.GetPointFromAngelAndDistance(firstPoint, profileLine.Angel, (double)profileRow.Value[distanceFld]);
                        //var profilePoint = profilePointSource.CloneWithProjecting();

                        // Try to define if this point is close to a vertex

                        double distance = (double)profileRow.Value[distanceFld];

                        ProfileSurfacePoint newSurface = new ProfileSurfacePoint
                        {
                            Distance = distance,
                            Z = (double)profileRow.Value[zFld],
                            //X = profilePoint.X,
                            //Y = profilePoint.Y
                        };

                        IPoint point = new Point();
                        line.QueryPoint(esriSegmentExtension.esriNoExtension, newSurface.Distance, false, point);
                        IProximityOperator proximity = point as IProximityOperator;

                        foreach (var vertx in verticesCache)
                        {
                            var profilePoint = mapProfilePointToVertex[vertx];
                            if (mapProfilePointToDistance[vertx] == 0)// profilePoint.isVertex)
                                continue;

                            double localDistance = proximity.ReturnDistance(vertx);
                            if (mapProfilePointToDistance[vertx] == -1 || mapProfilePointToDistance[vertx] > localDistance)
                            {
                                mapProfilePointToDistance[vertx] = localDistance;
                                mapProfilePointToVertex[vertx] = newSurface;
                                if (localDistance == 0)
                                {
                                    newSurface.isVertex = true;
                                }
                            }
                        }

                        var projected = point.CloneWithProjecting();
                        newSurface.X = projected.X;
                        newSurface.Y = projected.Y;
                        points.Add(newSurface);
                    }

                    mapProfilePointToVertex.ToList().ForEach(v =>
                    {
                        if (!v.Value.IsEmpty)
                        {
                            v.Value.isVertex = true;
                        }
                    });

                    //Delete temp table form the GDB
                    GdbAccess.Instance.DeleteTemporarSource(tempTableName, profileSourceName);

                    Marshal.ReleaseComObject(featureCursor);

                    //TODO: Clean memo using Marhsaling IRow

                    session.ProfileSurfaces = surface.Select(r => new ProfileSurface
                    {
                        LineId = r.Key,
                        ProfileSurfacePoints = r.Value.OrderBy(point => point.Distance).ToArray()
                    }
                    ).ToArray();

                    return session;

                }
                catch (MilSpaceCanotDeletePrifileCalcTable ex)
                {
                    logger.DebugEx("GenerateProfile MilSpaceCanotDeletePrifileCalcTable. ex.Message: {0}", ex.Message);
                    throw ex;
                }
                catch (MilSpaceDataException ex)
                {
                    logger.DebugEx("GenerateProfile MilSpaceDataException. ex.Message: {0}", ex.Message);
                    throw ex;
                }
                catch (Exception ex)
                {
                    logger.DebugEx("GenerateProfile Exception. ex.Message: {0}", ex.Message);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                logger.DebugEx("> GenerateProfile Exception. ex.Message: {0}", ex.Message);
                return null;
            }

        }


        private static IEnumerable<ProfileLine> GetProfileLines(IFeatureClass profileLines)
        {
            var result = new List<ProfileLine>();

            IQueryFilter queryFilter = new QueryFilter()
            {
                WhereClause = WhereAllRecords
            };

            var allrecords = profileLines.Search(queryFilter, true);

            IFeature line = null;
            while ((line = allrecords.NextFeature()) != null)
            {

                if (line.Shape is IPointCollection points)
                {
                    var from = points.Point[0];
                    var to = points.Point[points.PointCount - 1];


                    ILine ln = new Line()
                    {
                        FromPoint = from,
                        ToPoint = to,
                        SpatialReference = line.Shape.SpatialReference
                    };


                    var transformedFrom = from.CloneWithProjecting();
                    var transformedTo = to.CloneWithProjecting();
                    IPolyline polyline = line.ShapeCopy as IPolyline;
                    polyline.Project(EsriTools.Wgs84Spatialreference);

                    var profileLine = new ProfileLine
                    {
                        PointFrom = new ProfilePoint { X = transformedFrom.X, Y = transformedFrom.Y },
                        PointTo = new ProfilePoint { X = transformedTo.X, Y = transformedTo.Y },
                        Id = line.OID,
                        Length = polyline.Length,
                        Line = polyline,
                        SpatialReference = EsriTools.Wgs84Spatialreference,
                        Azimuth = double.MinValue
                    };

                    var vertices = profileLine.Vertices;
                    if (vertices.Count() == 2)
                    {
                        profileLine.Azimuth = ln.Azimuth();
                    }
                    else
                    {
                        profileLine.PointCollection = vertices.Select(p =>
                        {
                            var pnt = p.CloneWithProjecting();
                            return new ProfilePoint { X = pnt.X, Y = pnt.Y };
                        }).ToArray();
                    }

                    result.Add(profileLine);
                }
            }

            Marshal.ReleaseComObject(allrecords);

            return result;
        }
    }
}
