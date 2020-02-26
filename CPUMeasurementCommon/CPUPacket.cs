using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon
{
    public class CPUDataPacket
    {
        public double? AverageLoad { get; set; }

        public Temperature Temperature { get; set; }
    }
}
