﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class ClientData
    {
        public string ComputerName { get; set; }

        public string CPUName { get; set; }

        public int MeasurementInterval { get; set; }

        public DateTime LastUpdate { get; set; }

        public string IPv4Address { get; set; }

        public bool UpdateRequested { get; set; } = false;

        public int? MeasurementIntervalRequested { get; set; }
    }
}
