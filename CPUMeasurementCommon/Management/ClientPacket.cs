using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon.Management
{
    public class ClientPacket
    {
        public string ComputerName { get; set; }

        public string CPUName { get; set; }

        public int MeasurementIntervalInSeconds { get; set; }
    }
}
