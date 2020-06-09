using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.DTO
{
    internal class Azimuths
    {
        public double StartAzimuth;
        public double EndAzimuth;

        public void FormatAzimuth()
        {
            SetAzimuthIn360Range(ref StartAzimuth);
            SetAzimuthIn360Range(ref EndAzimuth);

            if(StartAzimuth == 360)
            {
                StartAzimuth = 0;
            }

            if (EndAzimuth == 0)
            {
                EndAzimuth = 360;
            }
        }

        private void SetAzimuthIn360Range(ref double azimuth)
        {
            if (azimuth < 0)
            {
                azimuth += 360;
            }
            else if (azimuth > 360)
            {
                azimuth -= 360;
            }
        }
    }
}
