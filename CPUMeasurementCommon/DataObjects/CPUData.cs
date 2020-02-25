using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class CPUData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("received")]
        public DateTime Received { get; set; }

        [JsonProperty("temperature")]
        public Temperature Temperature { get; set; }

        [JsonProperty("averageLoad")]
        public double? AverageLoad { get; set; }

        [JsonProperty("upAddress")]
        public string IPAddress { get; set; }
    }
}
