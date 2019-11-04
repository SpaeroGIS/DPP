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
                    ProfileName = session.SessionName.SafeTrim(),
                    ProfileData = session.Serialized,
                    Creator = session.CreatedBy.SafeTrim(),
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
                    Id = dbObservationPoint.idOP.SafeTrim(),
                    Objectid = dbObservationPoint.OBJECTID,
                    Operator = dbObservationPoint.soper.SafeTrim(),
                    RelativeHeight = dbObservationPoint.HRel,
                    Share = dbObservationPoint.ishare,
                    Title = dbObservationPoint.TitleOP.SafeTrim(),
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

        internal static MilSp_VisibilitySession Get(this VisibilitySession visibilitySessionModel)
        {
            try
            {
                var visibilitySession = new MilSp_VisibilitySession
                {
                    IdRow = visibilitySessionModel.IdRow,
                    Id = visibilitySessionModel.Id.SafeTrim(),
                    Name = visibilitySessionModel.Name.SafeTrim(),
                    UserName = visibilitySessionModel.UserName.SafeTrim(),
                    Created = visibilitySessionModel.Created.HasValue ? visibilitySessionModel.Created.Value : DateTime.Now,
                    Started = visibilitySessionModel.Started,
                    Finished = visibilitySessionModel.Finished,
                    CalculatedResults = visibilitySessionModel.CalculatedResults,
                    ReferencedGDB = visibilitySessionModel.ReferencedGDB
                };

                return visibilitySession;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("VisibilitySession", DataOperationsEnum.Convert, ex);
            }
        }

        internal static VisibilitySession Get(this MilSp_VisibilitySession visibilitySessionEntity)
        {
            try
            {
                var visibilitySession = new VisibilitySession
                {
                    IdRow = visibilitySessionEntity.IdRow,
                    Id = visibilitySessionEntity.Id.SafeTrim(),
                    Name = visibilitySessionEntity.Name.SafeTrim(),
                    UserName = visibilitySessionEntity.UserName.SafeTrim(),
                    Created = visibilitySessionEntity.Created,
                    Started = visibilitySessionEntity.Started,
                    Finished = visibilitySessionEntity.Finished,
                    CalculatedResults = visibilitySessionEntity.CalculatedResults,
                    ReferencedGDB = visibilitySessionEntity.ReferencedGDB
                };

                return visibilitySession;
            }
            catch (Exception ex)
            {
                throw new MilSpaceDataException("VisibilitySession", DataOperationsEnum.Convert, ex);
            }
        }

        internal static void Update(this MilSp_VisibilitySession visibilitySessionEntity, VisibilitySession visibilitySession)
        {
            visibilitySessionEntity.Name = visibilitySession.Name.SafeTrim();
            visibilitySessionEntity.UserName = visibilitySession.UserName.SafeTrim();
            visibilitySessionEntity.Created = visibilitySession.Created.Value;
            visibilitySessionEntity.Started = visibilitySession.Started;
            visibilitySessionEntity.Finished = visibilitySession.Finished;
            visibilitySessionEntity.CalculatedResults = visibilitySession.CalculatedResults;
            visibilitySessionEntity.ReferencedGDB = visibilitySession.ReferencedGDB;
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
            dbObservationPoint.idOP = observationPoint.Id.SafeTrim();
            dbObservationPoint.OBJECTID = observationPoint.Objectid;
            dbObservationPoint.soper = observationPoint.Operator.SafeTrim();
            dbObservationPoint.HRel = observationPoint.RelativeHeight;
            dbObservationPoint.ishare = observationPoint.Share;
            dbObservationPoint.TitleOP = observationPoint.Title.SafeTrim();
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
                idOP = observationPoint.Id.SafeTrim(),
                OBJECTID = observationPoint.Objectid,
                soper = observationPoint.Operator.SafeTrim(),
                HRel = observationPoint.RelativeHeight,
                ishare = observationPoint.Share,
                TitleOP = observationPoint.Title.SafeTrim(),
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
                Creator = observObject.soper.SafeTrim(),
                DTO = observObject.DTO.HasValue ? observObject.DTO.Value : DateTime.Now,
                Group = observObject.sGroupOO,
                Id = observObject.idOO.SafeTrim(),
                ObjectId = observObject.OBJECTID,
                Shared = observObject.ifShare.HasValue ? observObject.ifShare.Value != 0 : false,
                Title = observObject.sTitleOO.SafeTrim(),
                ObjectType = objectType
            };
        }

        internal static VisiblilityObservationObjects Get(this ObservationObject observObject)
        {
            return new VisiblilityObservationObjects
            {
                soper = observObject.Creator.SafeTrim(),
                DTO = observObject.DTO,
                sGroupOO = observObject.Group,
                idOO = observObject.Id.SafeTrim(),
                saffiliation = observObject.ObjectType.ToString(),
                OBJECTID = observObject.ObjectId,
                ifShare = observObject.Shared ? 1 : 0,
                sTitleOO = observObject.Title.SafeTrim()
            };
        }

        private static string SafeTrim(this string text)
        {
            return (!string.IsNullOrEmpty(text)) ? text.Trim() : text;
        }
    }
}
