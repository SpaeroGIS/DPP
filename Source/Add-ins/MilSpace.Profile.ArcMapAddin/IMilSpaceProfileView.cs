using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile
{
    public interface IMilSpaceProfileView
    {

        void SetController(MilSpaceProfileCalsController controller);

        ProfileSettingsTypeEnum SelectedProfileSettingsType { get; }

        ProfileSettingsPointButton ActiveButton { get; }

        IActiveView ActiveView { get; }

        /// <summary>
        /// First point for Line  Profile setting 
        /// </summary>
        IPoint LinePropertiesFirstPoint { set; }

        /// <summary>
        /// Second point for Line  Profile setting 
        /// </summary>
        IPoint LinePropertiesSecondPoint { set; }

        /// <summary>
        /// Center point for Fun Profile setting 
        /// </summary>
        IPoint FunPropertiesCenterPoint { set; }

        int FunLinesCount { get; }

        double FunAzimuth1 { get; }
        double FunAzimuth2 { get; }

        double FunLength { get; }

        MilSpaceProfileCalsController Controller { get; }

        bool AllowToProfileCalc { get; }

        string DemLayerName { get; }

        int ProfileId { set; }

    }
}
