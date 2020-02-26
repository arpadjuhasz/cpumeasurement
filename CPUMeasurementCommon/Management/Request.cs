using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon.Management
{
    public class Request
    {
        public RequestType RequestType { get; set; }
    }

    public enum RequestType
    { 
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3
    }
}
