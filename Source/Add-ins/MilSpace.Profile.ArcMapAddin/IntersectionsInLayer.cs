using MilSpace.DataAccess;
using System.Collections.Generic;
using System.Drawing;

namespace MilSpace.Profile
{
    public class IntersectionsInLayer
    {
        public List<IntersectionLine> Lines { get; set; }
        public LayersEnum Type { get; set; }
        public Color LineColor { get; set; }

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

                    LineColor = Color.Gray;

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
