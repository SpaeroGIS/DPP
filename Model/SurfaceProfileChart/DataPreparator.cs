using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SurfaceProfileChart
{
    internal class DataPreparator
    {

        public static ProfileSession Get()
        {

            ProfileSession session = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ProfileSession));

            using (var fileStream = File.OpenRead("TestData.xml"))
            {
                //using (var writer = new StreamWriter(fileStream))
                {
                    // Various for loops etc as necessary that will ultimately do this:


                    try
                    {
                        if (serializer.Deserialize(fileStream) is ProfileSession result)
                        {
                            session = result;
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: log the error
                    }
                }
            }

            return session;
        }

    }
}
