using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CPUMeasurementService
{
    public class ClientConfiguration
    {
        public string ServerIPAddress { get; set; } = "127.0.0.1";
        public int ServerManagementPort { get; set; } = 1400;
        public int ServerMeasurementPort { get; set; } = 1401;
        public int MeasurementIntervalInSeconds { get; set; } = 300;

        
    }
}
