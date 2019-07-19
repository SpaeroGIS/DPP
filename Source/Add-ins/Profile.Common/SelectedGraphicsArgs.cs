using System;

namespace MilSpace.Profile
{
    public class SelectedGraphicsArgs : EventArgs
    {
        private readonly int linesCount;
        private readonly double fullLength;
        public SelectedGraphicsArgs(int linesCount, double commonLength)
        {
            this.fullLength = commonLength;
            this.linesCount = linesCount;
        }

        public int LinesCount => linesCount;

        public double FullLength => fullLength;
    }
}
