using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon
{
    public class CPUMeasurementPacket
    {
        public double? AverageLoad { get; set; }

        public double? Temperature { get; set; }

        public MeasurementUnit TemperatureMeasurementUnit { get; set; } = MeasurementUnit.CELSIUS;
    }
}
