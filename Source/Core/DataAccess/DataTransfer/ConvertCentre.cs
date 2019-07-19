using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MilSpace.Core.DataAccess;
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
                    Creator =  session.CreatedBy.Trim(),
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
    }
}
