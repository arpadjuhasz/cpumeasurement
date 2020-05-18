using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class CPUMeasurement
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("received")]
        public DateTime Received { get; set; }

        [JsonProperty("temperature")]
        public Temperature Temperature { get; set; } = new Temperature(null, MeasurementUnit.CELSIUS);

        [JsonProperty("averageLoad")]
        public double? AverageLoad { get; set; }

        [JsonProperty("IPAddress")]
        public string IPAddress { get; set; }

        public DateTime MeasurementDate { get; set; }

        public int MeasurementIntervalInSeconds { get; set; }

        public static CPUMeasurement Create(MeasurementPacket packet, IPAddress ipAddress)
        {
            return new CPUMeasurement
            {
                Id = Guid.NewGuid(),
                AverageLoad = packet.AverageLoad,
                IPAddress = ipAddress.ToString(),
                Received = DateTime.UtcNow,
                Temperature = packet.Temperature,
                MeasurementDate = packet.MeasurementDate,
                MeasurementIntervalInSeconds = packet.MeasurementIntervalInSeconds
            };
        }
    }
}
