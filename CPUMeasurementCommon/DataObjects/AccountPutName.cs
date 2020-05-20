using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class AccountPutName
    {
        [MaxLength(256, ErrorMessage = "Name is too long! Maximum 256 letters.")]
        public string Name { get; set; }
    }
}
