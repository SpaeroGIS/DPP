using MilSpace.Core.DataAccess;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer.Sentinel;

namespace MilSpace.DataAccess.DataTransfer
{
    public static class ConvertCenter
    {
        public static Dictionary<ObservationObjectTypesEnum, string> ObservationObjectTypes = Enum.GetValues(typeof(ObservationObjectTypesEnum)).Cast<ObservationObjectTypesEnum>().ToDictionary(k => k, v => v.ToString());

        internal static ProfileSession Get(this MilSp_Profile profileData)
        {

            ProfileSession session = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ProfileSession));

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream))
                {
                    // Various for loops etc as necessary that will ultimately do this:
                    writer.Write(profileData.ProfileData);
                    writer.Flush();

                    try
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        if (serializer.Deserialize(memoryStream) is ProfileSession result)
                        {
                            session = result;
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: log the error
                        throw new MilSpaceDataException("MilSp_Profile", DataOperationsEnum.Convert, ex);
                    }
                }
            }

            session.CreatedBy = profileData.Creator;
            session.CreatedOn = profileData.Created;
            session.Shared = profileData.Shared;

            return session;
        }

        internal static MilSp_Profile Get(this ProfileSession session)
        {
            try
            {
                MilSp_Profile res = new MilSp_Profile
                {
                    idRow = session.SessionId,
                    ProfileName = session.SessionName?.Trim(),
                    ProfileData = session.Serialized,
                    Creator = session.CreatedBy?.Trim(),
                    Created = session.CreatedOn,
                    Shared = session.Shared
                };

                return res;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("ProfileSession", DataOperationsEnum.Convert, ex);
            }

        }

        internal static ObservationPoint Get(this VisiblilityObservPoint dbObservationPoint)
        {
            try
            {
                ObservationPoint observationPoint = new ObservationPoint
                {
                    Affiliation = dbObservationPoint.saffiliation,
                    AngelCameraRotationH = dbObservationPoint.AnglCameraRotationH,
                    AngelCameraRotationV = dbObservationPoint.AnglCameraRotationV,
                    AngelFrameH = dbObservationPoint.AngFrameH,
                    AngelFrameV = dbObservationPoint.AnglFrameV,
                    AngelMaxH = dbObservationPoint.AnglMaxH,
                    AngelMinH = dbObservationPoint.AnglMinH,
                    AzimuthStart = dbObservationPoint.AzimuthB,
                    AzimuthEnd = dbObservationPoint.AzimuthE,
                    AzimuthMainAxis = dbObservationPoint.AzimuthMainAxis,
                    Dto = dbObservationPoint.dto,
                    Group = dbObservationPoint.sGroup,
                    Id = dbObservationPoint.idOP?.Trim(),
                    Objectid = dbObservationPoint.OBJECTID,
                    Operator = dbObservationPoint.soper?.Trim(),
                    RelativeHeight = dbObservationPoint.HRel,
                    Share = dbObservationPoint.ishare,
                    Title = dbObservationPoint.TitleOP?.Trim(),
                    Type = dbObservationPoint.TypeOP,
                    InnerRadius = dbObservationPoint.InnerRadius,
                    OuterRadius = dbObservationPoint.OuterRadius,
                    X = dbObservationPoint.XWGS,
                    Y = dbObservationPoint.YWGS
                };

                return observationPoint;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("ProfileSession", DataOperationsEnum.Convert, ex);
            }
        }

        internal static MilSp_VisibilityTask Get(this VisibilityTask visibilitySessionModel)
        {
            try
            {
                var visibilitySession = new MilSp_VisibilityTask
                {
                    IdRow = visibilitySessionModel.IdRow,
                    Id = visibilitySessionModel.Id?.Trim(),
                    Name = visibilitySessionModel.Name?.Trim(),
                    UserName = visibilitySessionModel.UserName?.Trim(),
                    Created = visibilitySessionModel.Created ?? DateTime.Now,
                    Started = visibilitySessionModel.Started,
                    Finished = visibilitySessionModel.Finished,
                    CalculatedResults = visibilitySessionModel.CalculatedResults,
                    ReferencedGDB = visibilitySessionModel.ReferencedGDB,
                    Surface = visibilitySessionModel.Surface,
                    CalculationType = (int)visibilitySessionModel.CalculationType,
                    TaskLog = visibilitySessionModel.TaskLog
                };

                return visibilitySession;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("VisibilitySession", DataOperationsEnum.Convert, ex);
            }
        }

        internal static VisibilityTask Get(this MilSp_VisibilityTask visibilitySessionEntity)
        {
            try
            {
                var visibilitySession = new VisibilityTask
                {
                    IdRow = visibilitySessionEntity.IdRow,
                    Id = visibilitySessionEntity.Id?.Trim(),
                    Name = visibilitySessionEntity.Name?.Trim(),
                    UserName = visibilitySessionEntity.UserName?.Trim(),
                    Created = visibilitySessionEntity.Created,
                    Started = visibilitySessionEntity.Started,
                    Finished = visibilitySessionEntity.Finished,
                    CalculatedResults = visibilitySessionEntity.CalculatedResults,
                    ReferencedGDB = visibilitySessionEntity.ReferencedGDB,
                    Surface = visibilitySessionEntity.Surface,
                    //TODO: Check value gatting form DB if ti is inside Enum's values
                    CalculationType = (VisibilityCalcTypeEnum)visibilitySessionEntity.CalculationType,
                    TaskLog = visibilitySessionEntity.TaskLog
                };

                return visibilitySession;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("VisibilitySession", DataOperationsEnum.Convert, ex);
            }
        }

        internal static void Update(this MilSp_VisibilityTask visibilitySessionEntity, VisibilityTask visibilitySession)
        {
            visibilitySessionEntity.Name = visibilitySession.Name?.Trim();
            visibilitySessionEntity.UserName = visibilitySession.UserName?.Trim();
            visibilitySessionEntity.Created = visibilitySession.Created.Value;
            visibilitySessionEntity.Started = visibilitySession.Started;
            visibilitySessionEntity.Finished = visibilitySession.Finished;
            visibilitySessionEntity.CalculatedResults = visibilitySession.CalculatedResults;
            visibilitySessionEntity.ReferencedGDB = visibilitySession.ReferencedGDB;
            visibilitySessionEntity.TaskLog = visibilitySession.TaskLog;
        }

        internal static MilSp_VisiblityResults Get(this VisibilityCalcResults visibilityResultsModel)
        {
            try
            {
                var visibilityResults = new MilSp_VisiblityResults
                {
                    IdRow = visibilityResultsModel.IdRow,
                    Id = visibilityResultsModel.Id?.Trim(),
                    Name = visibilityResultsModel.Name?.Trim(),
                    UserName = visibilityResultsModel.UserName?.Trim(),
                    Created = visibilityResultsModel.Created ?? DateTime.Now,
                    CalculatedResults = visibilityResultsModel.CalculatedResults,
                    ReferencedGDB = visibilityResultsModel.ReferencedGDB,
                    CalculationType = (int)visibilityResultsModel.CalculationType,
                    shared = visibilityResultsModel.Shared,
                    surface = visibilityResultsModel.Surface
                };

                return visibilityResults;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("VisibilityResults", DataOperationsEnum.Convert, ex);
            }
        }

        internal static VisibilityCalcResults Get(this MilSp_VisiblityResults visibilityResultsEntity)
        {
            try
            {
                var visibility = new VisibilityCalcResults
                {
                    IdRow = visibilityResultsEntity.IdRow,
                    Id = visibilityResultsEntity.Id?.Trim(),
                    Name = visibilityResultsEntity.Name?.Trim(),
                    UserName = visibilityResultsEntity.UserName?.Trim(),
                    Created = visibilityResultsEntity.Created,
                    CalculatedResults = visibilityResultsEntity.CalculatedResults,
                    ReferencedGDB = visibilityResultsEntity.ReferencedGDB,
                    Surface = visibilityResultsEntity.surface,
                    Shared = visibilityResultsEntity.shared,
                    CalculationType = (VisibilityCalcTypeEnum)visibilityResultsEntity.CalculationType
                };

                return visibility;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("VisibilityResults", DataOperationsEnum.Convert, ex);
            }
        }

        internal static VisibilityCalcResults ToVisibilityResults(this VisibilityTask session, bool shared)
        {
            try
            {
                var visibility = new VisibilityCalcResults
                {
                    IdRow = session.IdRow,
                    Id = session.Id?.Trim(),
                    Name = session.Name?.Trim(),
                    UserName = session.UserName?.Trim(),
                    Created = session.Created,
                    CalculatedResults = session.CalculatedResults,
                    ReferencedGDB = session.ReferencedGDB,
                    Surface = session.Surface,
                    Shared = shared,
                    CalculationType = session.CalculationType
                };

                return visibility;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("VisibilityResults", DataOperationsEnum.Convert, ex);
            }
        }

        internal static void Update(this MilSp_VisiblityResults visibilityResultsEntity, VisibilityCalcResults visibility)
        {
            visibilityResultsEntity.Name = visibility.Name?.Trim();
            visibilityResultsEntity.UserName = visibility.UserName?.Trim();
            visibilityResultsEntity.Created = visibility.Created.Value;
            visibilityResultsEntity.shared = visibility.Shared;
            visibilityResultsEntity.surface = visibility.Surface;
            visibilityResultsEntity.CalculatedResults = visibility.CalculatedResults;
            visibilityResultsEntity.ReferencedGDB = visibility.ReferencedGDB;
            visibilityResultsEntity.CalculationType = (int)visibility.CalculationType;
        }

        internal static void Update(this VisiblilityObservationObjects dbObservationPoint, ObservationObject observationObject)
        {
            dbObservationPoint.sGroupOO = observationObject.Group;
            dbObservationPoint.sTitleOO = observationObject.Title;
            dbObservationPoint.saffiliation = observationObject.ObjectType.ToString();
            dbObservationPoint.idOO = observationObject.Id;
        }

        internal static void Update(this VisiblilityObservPoint dbObservationPoint, ObservationPoint observationPoint)
        {
            dbObservationPoint.saffiliation = observationPoint.Affiliation;
            dbObservationPoint.AnglCameraRotationH = observationPoint.AngelCameraRotationH;
            dbObservationPoint.AnglCameraRotationV = observationPoint.AngelCameraRotationV;
            dbObservationPoint.AngFrameH = observationPoint.AngelFrameH;
            dbObservationPoint.AnglFrameV = observationPoint.AngelFrameV;
            dbObservationPoint.AnglMaxH = observationPoint.AngelMaxH;
            dbObservationPoint.AnglMinH = observationPoint.AngelMinH;
            dbObservationPoint.AzimuthB = observationPoint.AzimuthStart;
            dbObservationPoint.AzimuthE = observationPoint.AzimuthEnd;
            dbObservationPoint.AzimuthMainAxis = observationPoint.AzimuthMainAxis;
            dbObservationPoint.dto = DateTime.Now;
            dbObservationPoint.sGroup = observationPoint.Group;
            dbObservationPoint.idOP = observationPoint.Id?.Trim();
            dbObservationPoint.OBJECTID = observationPoint.Objectid;
            dbObservationPoint.soper = observationPoint.Operator?.Trim();
            dbObservationPoint.HRel = observationPoint.RelativeHeight;
            dbObservationPoint.ishare = observationPoint.Share;
            dbObservationPoint.TitleOP = observationPoint.Title?.Trim();
            dbObservationPoint.TypeOP = observationPoint.Type;
            dbObservationPoint.XWGS = observationPoint.X;
            dbObservationPoint.YWGS = observationPoint.Y;
        }

        internal static VisiblilityObservPoint Get(this ObservationPoint observationPoint)
        {
            return new VisiblilityObservPoint
            {
                saffiliation = observationPoint.Affiliation,
                AnglCameraRotationH = observationPoint.AngelCameraRotationH,
                AnglCameraRotationV = observationPoint.AngelCameraRotationV,
                AngFrameH = observationPoint.AngelFrameH,
                AnglFrameV = observationPoint.AngelFrameV,
                AnglMaxH = observationPoint.AngelMaxH,
                AnglMinH = observationPoint.AngelMinH,
                AzimuthB = observationPoint.AzimuthStart,
                AzimuthE = observationPoint.AzimuthEnd,
                AzimuthMainAxis = observationPoint.AzimuthMainAxis,
                dto = observationPoint.Dto,
                sGroup = observationPoint.Group,
                idOP = observationPoint.Id?.Trim(),
                OBJECTID = observationPoint.Objectid,
                soper = observationPoint.Operator?.Trim(),
                HRel = observationPoint.RelativeHeight,
                ishare = observationPoint.Share,
                TitleOP = observationPoint.Title?.Trim(),
                TypeOP = observationPoint.Type,
                XWGS = observationPoint.X,
                YWGS = observationPoint.Y
            };
        }

        internal static ObservationObject Get(this VisiblilityObservationObjects observObject)
        {
            var objectType = ObservationObjectTypesEnum.Undefined;

            if (ObservationObjectTypes.Values.Any(t => t.Equals(observObject.saffiliation, StringComparison.InvariantCultureIgnoreCase)))
            {
                objectType = ObservationObjectTypes.First(t => t.Value.Equals(observObject.saffiliation, StringComparison.InvariantCultureIgnoreCase)).Key;
            }

            return new ObservationObject
            {
                Creator = observObject.soper?.Trim(),
                DTO = observObject.DTO ?? DateTime.Now,
                Group = observObject.sGroupOO,
                Id = observObject.idOO?.Trim(),
                ObjectId = observObject.OBJECTID,
                Shared = observObject.ifShare.HasValue ? observObject.ifShare.Value != 0 : false,
                Title = observObject.sTitleOO?.Trim(),
                ObjectType = objectType
            };
        }

        internal static VisiblilityObservationObjects Get(this ObservationObject observObject)
        {
            return new VisiblilityObservationObjects
            {
                soper = observObject.Creator?.Trim(),
                DTO = observObject.DTO,
                sGroupOO = observObject.Group,
                idOO = observObject.Id?.Trim(),
                saffiliation = observObject.ObjectType.ToString(),
                OBJECTID = observObject.ObjectId,
                ifShare = observObject.Shared ? 1 : 0,
                sTitleOO = observObject.Title?.Trim()
            };
        }

        internal static GeoCalcPoint Get(this GeoCalcSessionPoint sessionPoint)
        {
            return new GeoCalcPoint
            {
                PointNumber = sessionPoint.PointNumber,
                X = sessionPoint.X,
                Y = sessionPoint.Y,
                UserName = sessionPoint.userName,
                GuidId = sessionPoint.id,
                AngFrameH = sessionPoint.AngFrameH ?? double.NaN,
                AngelMaxH = sessionPoint.AnglMaxH,
                AngelMinH = sessionPoint.AnglMinH,
                AzimuthStart = sessionPoint.AzimuthB,
                AzimuthEnd = sessionPoint.AzimuthE,
                RelativeHeight = sessionPoint.HRel,
                InnerRadius = sessionPoint.InnerRadius ?? double.NaN,
                OuterRadius = sessionPoint.OuterRadius ?? double.NaN

            };
        }

        internal static GeoCalcSessionPoint Get(this GeoCalcPoint sessionPoint)
        {
            double? GetNulableDouble(double val) { if (double.IsNaN(val)) return null; return val; }

            return new GeoCalcSessionPoint
            {
                PointNumber = sessionPoint.PointNumber,
                X = sessionPoint.X.Value,
                Y = sessionPoint.Y.Value,
                userName = sessionPoint.UserName,
                id = sessionPoint.GuidId,
                AngFrameH = GetNulableDouble(sessionPoint.AngFrameH),
                AnglMaxH = sessionPoint.AngelMaxH,
                AnglMinH = sessionPoint.AngelMinH,
                AzimuthB = sessionPoint.AzimuthStart,
                AzimuthE = sessionPoint.AzimuthEnd,
                HRel = sessionPoint.RelativeHeight,
                InnerRadius = sessionPoint.InnerRadius,
                OuterRadius = sessionPoint.OuterRadius
            };

        }

        internal static void Update(this GeoCalcSessionPoint pointEntity, GeoCalcPoint point)
        {
            pointEntity.PointNumber = point.PointNumber;
            pointEntity.X = point.X.Value;
            pointEntity.Y = point.Y.Value;
            pointEntity.AnglMinH = point.AngelMinH;
            pointEntity.AnglMaxH = point.AngelMaxH;
            pointEntity.AzimuthB = point.AzimuthStart;
            pointEntity.AzimuthE = point.AzimuthEnd;
            pointEntity.HRel = point.RelativeHeight;
            pointEntity.InnerRadius = point.InnerRadius;
            pointEntity.OuterRadius = point.OuterRadius;
        }


        internal static SrtmGrid Get(this MilSp_SrtmGrid dbGrid)
        {
            return new SrtmGrid
            {
                Boundary = dbGrid.Boundary,
                FileName = dbGrid.FileName,
                Id = dbGrid.Id,
                Loaded = dbGrid.Loaded == 1,
                OBJECTID = dbGrid.OBJECTID,
                POINT_X = dbGrid.POINT_X,
                POINT_Y = dbGrid.POINT_Y,
                SRTM = dbGrid.SRTM,
                Zone_UTM = dbGrid.Zone_UTM
            };
        }
        internal static MilSp_SrtmGrid Get(this SrtmGrid grid)
        {
            return new MilSp_SrtmGrid
            {
                Boundary = grid.Boundary,
                FileName = grid.FileName,
                Id = grid.Id,
                Loaded = (short)(grid.Loaded ? 1 : 0),
                OBJECTID = grid.OBJECTID,
                POINT_X = grid.POINT_X,
                POINT_Y = grid.POINT_Y,
                SRTM = grid.SRTM,
                Zone_UTM = grid.Zone_UTM
            };

        }


        internal static SentinelSource Get(this S1Sources source)
        {
            return new SentinelSource
            {
                IdRow = source.idrow,
                BurstNumber = source.nburst.HasValue ? source.nburst.Value : -1,
                DateTime = source.dttime.HasValue ? source.dttime.Value : DateTime.MinValue,
                Dto = source.dto.HasValue ? source.dto.Value : DateTime.MinValue,
                Extend = source.extend,
                Operator = source.soper,
                OrbitNumber = source.norbit.HasValue ? source.norbit.Value : -1,
                SceneId = source.idscene,
            };
        }
        internal static S1Sources Get(this SentinelSource source)
        {
            return new S1Sources
            {
                idrow = source.IdRow,
                nburst = source.BurstNumber,
                dttime = source.DateTime,
                dto = source.Dto,
                extend = source.Extend,
                soper = source.Operator,
                norbit = source.OrbitNumber,
                idscene = source.SceneId,
            };
        }
        internal static S1TilesCoverage Get(this Sentinel1TilesCoverage source)
        {
            return new S1TilesCoverage
            {
                DegreeTileName = source.DegreeTileName,
                DTBaseSurvey = source.DTBaseSurvey,
                idPair1 = source.idPair1,
                idPair2 = source.idPair2,
                idrow = source.IdRow,
                Pair1IW1 = source.Pair1IW1,
                Pair1IW1B1 = source.Pair1IW1B1,
                Pair1IW1B2 = source.Pair1IW1B2,
                Pair1IW2 = source.Pair1IW2,
                Pair1IW2B1 = source.Pair1IW2B1,
                Pair1IW2B2 = source.Pair1IW2B2,
                Pair1IW3 = source.Pair1IW3,
                Pair1IW3B1 = source.Pair1IW3B1,
                Pair1IW3B2 = source.Pair1IW3B2,
                Pair2IW1 = source.Pair2IW1,
                Pair2IW1B1 = source.Pair2IW1B1,
                Pair2IW1B2 = source.Pair2IW1B2,
                Pair2IW2 = source.Pair2IW2,
                Pair2IW2B1 = source.Pair2IW2B1,
                Pair2IW2B2 = source.Pair2IW2B2,
                Pair2IW3 = source.Pair2IW3,
                Pair2IW3B1 = source.Pair2IW3B1,
                Pair2IW3B2 = source.Pair2IW3B2
            };
        }

        internal static Sentinel1TilesCoverage Get(this S1TilesCoverage source)
        {
            return new Sentinel1TilesCoverage
            {
                DegreeTileName = source.DegreeTileName,
                DTBaseSurvey = source.DTBaseSurvey.HasValue ? source.DTBaseSurvey.Value : DateTime.MaxValue,
                idPair1 = source.idPair1.HasValue ? source.idPair1.Value : -1,
                idPair2 = source.idPair2.HasValue ? source.idPair2.Value : -1,
                IdRow = source.idrow,
                Pair1IW1 = source.Pair1IW1.HasValue ? source.Pair1IW1.Value : -1,
                Pair1IW1B1 = source.Pair1IW1B1.HasValue ? source.Pair1IW1B1.Value : -1,
                Pair1IW1B2 = source.Pair1IW1B2.HasValue ? source.Pair1IW1B2.Value : -1,
                Pair1IW2 = source.Pair1IW2.HasValue ? source.Pair1IW2.Value : -1,
                Pair1IW2B1 = source.Pair1IW2B1.HasValue ? source.Pair1IW2B1.Value : -1,
                Pair1IW2B2 = source.Pair1IW2B2.HasValue ? source.Pair1IW2B2.Value : -1,
                Pair1IW3 = source.Pair1IW3.HasValue ? source.Pair1IW3.Value : -1,
                Pair1IW3B1 = source.Pair1IW3B1.HasValue ? source.Pair1IW3B1.Value : -1,
                Pair1IW3B2 = source.Pair1IW3B2.HasValue ? source.Pair1IW3B2.Value : -1,
                Pair2IW1 = source.Pair2IW1.HasValue ? source.Pair2IW1.Value : -1,
                Pair2IW1B1 = source.Pair2IW1B1.HasValue ? source.Pair2IW1B1.Value : -1,
                Pair2IW1B2 = source.Pair2IW1B2.HasValue ? source.Pair2IW1B2.Value : -1,
                Pair2IW2 = source.Pair2IW2.HasValue ? source.Pair2IW2.Value : -1,
                Pair2IW2B1 = source.Pair2IW2B1.HasValue ? source.Pair2IW2B1.Value : -1,
                Pair2IW2B2 = source.Pair2IW2B2.HasValue ? source.Pair2IW2B2.Value : -1,
                Pair2IW3 = source.Pair2IW3.HasValue ? source.Pair2IW3.Value : -1,
                Pair2IW3B1 = source.Pair2IW3B1.HasValue ? source.Pair2IW3B1.Value : -1,
                Pair2IW3B2 = source.Pair2IW3B2.HasValue ? source.Pair2IW3B2.Value : -1
            };
        }
        internal static SentinelPairCoherence Get(this S1PairCoherence source)
        {
            return new SentinelPairCoherence
            {
                Deviation = source.fdeviation.HasValue ? source.fdeviation.Value : -1,
                Dto = source.dto.HasValue ? source.dto.Value : DateTime.MinValue,
                IdRow = source.idrow,
                IdSceneBase = source.idSceneBase,
                IdScentSlave = source.idScentSlave,
                Max = source.fmax.HasValue ? source.fmax.Value : -1,
                Min = source.fmin.HasValue ? source.fmin.Value : -1,
                Mean = source.fmean.HasValue ? source.fmean.Value : -1,
                Operator = source.soper

            };
        }

        internal static S1PairCoherence Get(this SentinelPairCoherence source)
        {
            return new S1PairCoherence
            {
                fdeviation = source.Deviation,
                dto = source.Dto,
                idrow = source.IdRow,
                idSceneBase = source.IdSceneBase,
                idScentSlave = source.IdScentSlave,
                fmax = source.Max,
                fmin = source.Min,
                fmean = source.Mean,
                soper = source.Operator,
            };
        }

        internal static S1SentinelProduct Get(this SentinelProduct product)
        {
            return new S1SentinelProduct
            {
                DateTime = product.DateTime,
                Footprint = product.Footprint,
                Id = product.Id,
                Identifier = product.Identifier,
                Instrument = product.Instrument,
                JTSfootprint = product.JTSfootprint,
                OrbitNumber = product.OrbitNumber,
                PassDirection = product.PassDirection,
                RelativeOrbit = product.RelativeOrbit,
                SliceNumber = product.SliceNumber,
                Uuid = product.Uuid,
                Wkt = product.Wkt,
                Dto = product.Dto,
                sOper = product.Operator,
                TileName = product.RelatedTile.Name
            };
        }

        internal static SentinelProduct Get(this S1SentinelProduct product)
        {
            return new SentinelProduct
            {
                DateTime = product.DateTime,
                Footprint = product.Footprint,
                Id = product.Id,
                Identifier = product.Identifier,
                Instrument = product.Instrument,
                JTSfootprint = product.JTSfootprint,
                OrbitNumber = product.OrbitNumber,
                PassDirection = product.PassDirection,
                RelativeOrbit = product.RelativeOrbit,
                SliceNumber = product.SliceNumber,
                Uuid = product.Uuid,
                Wkt = product.Wkt,
                Dto = product.Dto,
                Operator = product.sOper,
                RelatedTile = new Tile(product.TileName)
            };
        }
    }
}
