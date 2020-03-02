using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class MeasurementIntervalUpdate
    {
        [Required]
        public string IPv4Address { get; set; }

        [Required()]
        [Range(1, int.MaxValue)]
        public int MeasurementIntervalInSeconds { get; set; }
    }
}
