using MilSpace.Core.DataAccess;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.IO;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public static class ConvertCenter
    {
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
                    ProfileName = session.SessionName.Trim(),
                    ProfileData = session.Serialized,
                    Creator = session.CreatedBy.Trim(),
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
                Id = dbObservationPoint.idOP,
                Objectid = dbObservationPoint.OBJECTID,
                Operator = dbObservationPoint.soper,
                RelativeHeihgt = dbObservationPoint.HRel,
                Share = dbObservationPoint.ishare,
                Title = dbObservationPoint.TitleOP,
                Type = dbObservationPoint.TypeOP,
                X = dbObservationPoint.XWGS,
                Y = dbObservationPoint.YWGS
            };

            return observationPoint;

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
            dbObservationPoint.idOP = observationPoint.Id;
            dbObservationPoint.OBJECTID = observationPoint.Objectid;
            dbObservationPoint.soper = observationPoint.Operator;
            dbObservationPoint.HRel = observationPoint.RelativeHeihgt;
            dbObservationPoint.ishare = observationPoint.Share;
            dbObservationPoint.TitleOP = observationPoint.Title;
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
                idOP = observationPoint.Id,
                OBJECTID = observationPoint.Objectid,
                soper = observationPoint.Operator,
                HRel = observationPoint.RelativeHeihgt,
                ishare = observationPoint.Share,
                TitleOP = observationPoint.Title,
                TypeOP = observationPoint.Type,
                XWGS = observationPoint.X,
                YWGS = observationPoint.Y
            };
        }
    }
}
