using System.ComponentModel.DataAnnotations;

namespace CPUMeasurementCommon.DataObjects
{
    public class ManagementPut
    {
        [Required]
        public string IPv4Address { get; set; }

        [Required()]
        [Range(1, int.MaxValue)]
        public int MeasurementIntervalInSeconds { get; set; }
    }
}
