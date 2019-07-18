using MilSpace.DataAccess.DataTransfer;
using System;

namespace MilSpace.Profile.SurfaceProfileChartControl
{
    internal class GraphProfileClickedArgs : EventArgs
    {
        public GraphProfileClickedArgs (double X, double Y)
        {
            ProfilePoint = new ProfilePoint { X = X, Y = Y };
        }

        public GraphProfileClickedArgs(ProfilePoint point)
        {
            ProfilePoint = point;
        }

        public ProfilePoint ProfilePoint { get; }
    }
}
