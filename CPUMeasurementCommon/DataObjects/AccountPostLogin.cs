using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class AccountPostLogin
    {
        [Required]
        [MaxLength(256)]
        public string Username { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }
    }
}
