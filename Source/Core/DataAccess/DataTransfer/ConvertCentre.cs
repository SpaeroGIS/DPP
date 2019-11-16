using MilSpace.Core.DataAccess;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using MilSpace.Core;

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
                    Created = visibilitySessionModel.Created.HasValue ? visibilitySessionModel.Created.Value : DateTime.Now,
                    Started = visibilitySessionModel.Started,
                    Finished = visibilitySessionModel.Finished,
                    CalculatedResults = visibilitySessionModel.CalculatedResults,
                    ReferencedGDB = visibilitySessionModel.ReferencedGDB,
                    Surface = visibilitySessionModel.Surface,
                    CalculationType = (int)visibilitySessionModel.CalculationType
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
                    CalculationType = (VisibilityCalcTypeEnum)visibilitySessionEntity.CalculationType
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
                    Created = visibilityResultsModel.Created.HasValue ? visibilityResultsModel.Created.Value : DateTime.Now,
                    CalculatedResults = visibilityResultsModel.CalculatedResults,
                    ReferencedGDB = visibilityResultsModel.ReferencedGDB,
                    CalculationType = (int)visibilityResultsModel.CalculationType,
                    shared = visibilityResultsModel.Shared,
                    surface = visibilityResultsModel.Surface
                };

                return visibilityResults;
            }
            catch(Exception ex)
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
            catch(Exception ex)
            {
                throw new MilSpaceDataException("VisibilityResults", DataOperationsEnum.Convert, ex);
            }
        }

        internal static VisibilityCalcResults ToVisibilityResults(this VisibilitySession session, bool shared)
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
                    CalculationType = (VisibilityCalcTypeEnum)session.CalculationType
                };

                return visibility;
            }
            catch(Exception ex)
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
                DTO = observObject.DTO.HasValue ? observObject.DTO.Value : DateTime.Now,
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
    }
}
