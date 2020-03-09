using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class LoginPost
    {
        [Required]
        [Range(8, 256)]
        public string Username { get; set; }

        [Required]
        [Range(0,256)]
        public string Password { get; set; }
    }
}
