using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPUMeasurementBackend.Repository.Poco
{
    public class CPUDataGet
    {
        public int id { get; set; }
        public DateTime received { get; set; }
        public double? temperature { get; set; }
        public int temperature_unit_id { get; set; }
        public double? average_load { get; set; }
        public string ip_address { get; set; }
    }
}
