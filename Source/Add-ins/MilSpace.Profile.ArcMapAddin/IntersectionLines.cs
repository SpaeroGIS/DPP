using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile
{
    public class IntersectionLines
    {
        public List<ProfileLine> Lines { get; set; }
        public LayersEnum Type { get; set; }
        public Color LineColor { get; set; }
        public int LineId { get; set; }

        public void SetDefaultColor()
        {
            switch (Type)
            {
                case LayersEnum.Vegetation:

                    LineColor = Color.Green;

                    break;

                case LayersEnum.Hydrography:

                    LineColor = Color.Blue;

                    break;

                case LayersEnum.Buildings:

                    LineColor = Color.DimGray;

                    break;

                case LayersEnum.Roads:

                    LineColor = Color.Black;

                    break;

                case LayersEnum.NotIntersect:

                    LineColor = Color.White;

                    break;
            }
        }
    }
}
