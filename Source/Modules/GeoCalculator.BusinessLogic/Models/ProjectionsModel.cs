using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Models
{
    public class ProjectionsModel
    {
        private ProjectionsModel() { }

        public ProjectionsModel(CoordinateSystemModel wgs84projectionModel, CoordinateSystemModel pulkovo1942projectionModel, CoordinateSystemModel ukraine2000projectionModel)
        {
            WGS84Projection = wgs84projectionModel ?? throw new ArgumentNullException(nameof(wgs84projectionModel));
            Pulkovo1942Projection = pulkovo1942projectionModel ?? throw new ArgumentNullException(nameof(pulkovo1942projectionModel));
            Ukraine2000Projection = ukraine2000projectionModel ?? throw new ArgumentNullException(nameof(ukraine2000projectionModel));
        }
        public CoordinateSystemModel WGS84Projection { get; private set; }
        public CoordinateSystemModel Pulkovo1942Projection { get; private set; }
        public CoordinateSystemModel Ukraine2000Projection { get; private set; }
    }
}
