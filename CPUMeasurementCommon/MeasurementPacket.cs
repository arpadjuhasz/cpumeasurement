using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon
{
    public class MeasurementPacket
    {
        public double? AverageLoad { get; set; }

        public Temperature Temperature { get; set; }

        public int MeasurementIntervalInSeconds { get; set; }

        public DateTime MeasurementDate { get; set; }

        public MeasurementPacket GetMemberwiseClone()
        {
            return new MeasurementPacket
            {
                AverageLoad = AverageLoad,
                MeasurementDate = MeasurementDate,
                MeasurementIntervalInSeconds = MeasurementIntervalInSeconds,
                Temperature = Temperature,
            };
        }
    }
}
