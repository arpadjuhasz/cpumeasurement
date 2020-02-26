using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class ClientData
    {
        //public IPAddress IPAddress { get; set; }

        //public string IP { get { return IPAddress.ToString(); } }

        public string MAC { get; set; }

        public string ComputerName { get; set; }

        public string CPUName { get; set; }

        public int IntervalInSeconds { get; set; }
    }
}
