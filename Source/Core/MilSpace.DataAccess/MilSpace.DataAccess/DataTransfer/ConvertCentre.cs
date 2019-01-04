using MilSpace.DataAccess.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public static class ConvertCenter
    {
        internal static ProfileSession Get(this MilSp_Profile profileData)
        {

            ProfileSession result = new ProfileSession
            {
                SessionId = profileData.idRow,
                SessionName = profileData.ProfileName
                
            };

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
                        if (serializer.Deserialize(memoryStream) is ProfileSurface[] surface)
                        {
                            result.ProfileSurface = surface;
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: log the error
                    }
                }
            }

            return result;
        }

        internal static MilSp_Profile Get(this ProfileSession session)
        {
            MilSp_Profile res = new MilSp_Profile
            {
                idRow = session.SessionId,
                ProfileName = session.SessionName
            };

            XmlSerializer serializer = new XmlSerializer(typeof(ProfileSurface));


            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, session.ProfileSurface);
                try
                {
                    res.ProfileData = Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
                }
                catch (Exception ex)
                {
                    //TODO: log the error
                }
            }
            return res;
        }
    }
}
