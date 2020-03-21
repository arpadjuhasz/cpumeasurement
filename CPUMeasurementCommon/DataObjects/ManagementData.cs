using CPUMeasurementCommon.Management;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class ManagementData
    {
        public string ComputerName { get; set; }

        public string CPUName { get; set; }

        public int MeasurementInterval { get; set; }

        public DateTime LastUpdate { get; set; }

        public string IPv4Address { get; set; }

        public bool UpdateRequested { get; set; } = false;

        public int? MeasurementIntervalRequested { get; set; }

        public static ManagementData Create(ManagementPacket packet, IPAddress ipAddress)
        {
            return new ManagementData
            {
                IPv4Address = ipAddress.ToString(),
                CPUName = packet.CPUName,
                MeasurementInterval = packet.MeasurementIntervalInSeconds,
                ComputerName = packet.ComputerName,
                LastUpdate = DateTime.UtcNow,
                UpdateRequested = false
            };
        }
    }
}
