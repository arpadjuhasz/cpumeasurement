using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class AccountPutPassword
    {
        [Required]
        [MaxLength(256)]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [MaxLength(256)]
        [MinLength(8)]
        public string PasswordAgain { get; set; }
    }
}
