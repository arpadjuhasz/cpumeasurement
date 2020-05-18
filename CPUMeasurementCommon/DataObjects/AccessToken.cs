using System;
using System.Collections.Generic;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class AccessToken
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }

        public string Token { get; set; }
    }
}
