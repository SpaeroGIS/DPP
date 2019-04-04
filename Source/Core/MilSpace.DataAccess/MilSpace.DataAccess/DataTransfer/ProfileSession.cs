using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfileSession 
    {
        public ProfileLine[] ProfileLines;
        public ProfileSurface[] ProfileSurfaces;
        public int SessionId;
        public string SessionName;

        [XmlIgnore]
        public  string Serialized
        {
            get {

                return Serialize(this);
            }
        }

        private static string Serialize(ProfileSession session)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProfileSession));


            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, session);
                try
                {
                    return Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
                }
                catch (Exception ex)
                {
                    //TODO: log the error
                }

                return null;
            }
        }
    }
}
