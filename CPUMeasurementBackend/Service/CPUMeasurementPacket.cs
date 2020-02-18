using CPUMeasurementCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Service
{
    public class CPUMeasurementPacket
    {
        public double AverageLoad { get; set; }

        public MeasurementUnit AverageLoadMeasurementUnit { get; set; }

        public double Temperature { get; set; }

        public MeasurementUnit TemperatureMeasurementUnit { get; set; }
    }
}
