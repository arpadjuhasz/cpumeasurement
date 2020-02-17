using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Service
{
    public class CPUMeasurementPacket
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public double AverageLoad { get; set; }

        public double Temperature { get; set; }
    }
}
