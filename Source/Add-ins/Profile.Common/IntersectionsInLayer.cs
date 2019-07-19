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

                    LineColor = Color.FromArgb(76, 230, 0);

                    break;

                case LayersEnum.Hydrography:

                    LineColor = Color.FromArgb(0, 196, 255); 

                    break;

                case LayersEnum.Buildings:

                    LineColor = Color.FromArgb(255, 170, 0);

                    break;

                case LayersEnum.Roads:

                    LineColor = Color.FromArgb(156, 156, 156);

                    break;

                case LayersEnum.NotIntersect:

                    LineColor = Color.White;

                    break;
            }
        }
    }
}
