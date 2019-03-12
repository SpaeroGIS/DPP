using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ProfileSession 
    {
        public ProfioleLine[] ProfileLines;
        public ProfileSurface[] ProfileSurface;
        public int SessionId;
        public string SessionName;
    }
}
