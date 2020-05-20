using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class AccountPutName
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }
    }
}
